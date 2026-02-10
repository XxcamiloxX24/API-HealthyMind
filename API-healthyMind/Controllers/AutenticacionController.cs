using API_healthyMind.Models;
using API_healthyMind.Repositorios.Implementacion;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_healthyMind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        
        private readonly IAuthService _authService;

        public AutenticacionController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("ValidarAdmin")]
        public IActionResult ValidarAdmin([FromBody] Usuario request)
        {
            var token = _authService.LoginAdmin(request.CorreoPersonal, request.Password);

            if (token == null)
                return Unauthorized("Credenciales inválidas");

            return Ok(new { token });
        }

        [HttpPost("ValidarPsicologo")]
        public async Task<IActionResult> ValidarPsicologo([FromBody] Usuario request)
        {
            var token = await _authService.LoginPsicologo(request.CorreoPersonal, request.Password);

            if (token == null)
                return Unauthorized("Credenciales inválidas");

            return Ok(new { token });
        }

        [HttpPost("ValidarAprendiz")]
        public async Task<IActionResult> ValidarAprendiz([FromBody] Usuario request)
        {
            var token = await _authService.LoginAprendiz(request.CorreoPersonal, request.Password);

            if (token == null)
                return Unauthorized("Credenciales inválidas");

            return Ok(new { token });
        }
        [Authorize]
        [HttpGet("debug-auth")]
        public IActionResult DebugAuth()
        {
            return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
        }
    }
}
