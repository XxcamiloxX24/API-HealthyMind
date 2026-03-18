using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_healthyMind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public AutenticacionController(IAuthService authService, IWebHostEnvironment env, IConfiguration configuration)
        {
            _authService = authService;
            _env = env;
            _configuration = configuration;
        }

        /// <summary>
        /// Endpoint de diagnóstico temporal (solo Development). Muestra si la configuración de admin se carga correctamente.
        /// </summary>
        [HttpGet("debug-admin-config")]
        public IActionResult DebugAdminConfig()
        {
            if (!_env.IsDevelopment())
                return NotFound();

            var settings = _configuration.GetSection("settings");
            var adminEmail = settings["adminEmail"];
            var adminPassword = settings["adminPassword"];
            var adminPasswordHash = settings["adminPasswordHash"];

            return Ok(new
            {
                environment = _env.EnvironmentName,
                adminEmailDefinido = !string.IsNullOrWhiteSpace(adminEmail),
                adminEmailValor = adminEmail ?? "(null)",
                adminPasswordDefinido = !string.IsNullOrWhiteSpace(adminPassword),
                adminPasswordLongitud = adminPassword?.Length ?? 0,
                adminPasswordHashDefinido = !string.IsNullOrWhiteSpace(adminPasswordHash)
            });
        }

        [HttpPost("ValidarAdmin")]
        public async Task<IActionResult> ValidarAdmin([FromBody] Usuario? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CorreoPersonal) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Se requieren correoPersonal y password");

            var result = await _authService.LoginAdmin(request.CorreoPersonal, request.Password);

            if (result == null)
                return Unauthorized("Credenciales inválidas");

            return Ok(ToLoginResponse(result));
        }

        [HttpPost("ValidarPsicologo")]
        public async Task<IActionResult> ValidarPsicologo([FromBody] Usuario? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CorreoPersonal) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Se requieren correoPersonal y password");

            var result = await _authService.LoginPsicologo(request.CorreoPersonal, request.Password);

            if (result == null)
                return Unauthorized("Credenciales inválidas");

            return Ok(ToLoginResponse(result, includePsicologoId: true));
        }

        [HttpPost("ValidarAprendiz")]
        public async Task<IActionResult> ValidarAprendiz([FromBody] Usuario? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CorreoPersonal) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Se requieren correoPersonal y password");

            var result = await _authService.LoginAprendiz(request.CorreoPersonal, request.Password);

            if (result == null)
                return Unauthorized("Credenciales inválidas");

            return Ok(ToLoginResponse(result, includeAprendizId: true));
        }

        /// <summary>
        /// Renueva accessToken y refreshToken. No requiere JWT; solo el refreshToken del último login o refresco.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("refrescar")]
        public async Task<IActionResult> Refrescar([FromBody] RefreshTokenRequestDto? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new { message = "Se requiere refreshToken" });

            var result = await _authService.RefrescarAsync(request.RefreshToken.Trim());
            if (result == null)
                return Unauthorized(new { message = "refresh_token_invalido_o_expirado" });

            return Ok(ToLoginResponse(result));
        }

        private static object ToLoginResponse(AuthTokenResult r, bool includePsicologoId = false, bool includeAprendizId = false)
        {
            var payload = new Dictionary<string, object?>
            {
                ["accessToken"] = r.AccessToken,
                ["refreshToken"] = r.RefreshToken,
                ["expiresIn"] = r.ExpiresIn,
                ["tokenType"] = r.TokenType,
                ["token"] = r.AccessToken
            };
            var nameId = ParseNameIdFromJwt(r.AccessToken);
            if (includePsicologoId && int.TryParse(nameId, out var pid))
                payload["psicologoId"] = pid;
            if (includeAprendizId && int.TryParse(nameId, out var aid))
                payload["aprendizId"] = aid;
            return payload;
        }

        private static string? ParseNameIdFromJwt(string jwt)
        {
            try
            {
                var parts = jwt.Split('.');
                if (parts.Length < 2) return null;
                var payload = parts[1];
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }
                var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload.Replace('-', '+').Replace('_', '/')));
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    if (prop.Name is "nameid" or "sub")
                        return prop.Value.GetString();
                    if (prop.Name.Contains("nameidentifier", StringComparison.OrdinalIgnoreCase))
                        return prop.Value.GetString();
                }
            }
            catch { /* ignore */ }
            return null;
        }

        [Authorize]
        [HttpGet("debug-auth")]
        public IActionResult DebugAuth()
        {
            return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
        }
    }
}
