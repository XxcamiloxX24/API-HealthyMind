using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Repositorios.Interfaces;
using API_healthyMind.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class AuthService : IAuthService
    {
        private readonly IUnidadDeTrabajo _uow;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly string _secretKey;
        private readonly string? _adminEmail;
        private readonly string? _adminPassword;
        private readonly string? _adminPasswordHash;
        private readonly int _accessTokenMinutes;

        public AuthService(
            IUnidadDeTrabajo uow,
            IRefreshTokenService refreshTokenService,
            IConfiguration configuration)
        {
            _uow = uow;
            _refreshTokenService = refreshTokenService;
            var settings = configuration.GetSection("settings");
            _secretKey = settings["secretkey"] ?? configuration["JWT_SecretKey"] ?? string.Empty;
            _adminEmail = (settings["adminEmail"] ?? configuration["ADMIN_EMAIL"])?.Trim();
            _adminPassword = (settings["adminPassword"] ?? configuration["ADMIN_PASSWORD"])?.Trim();
            _adminPasswordHash = (settings["adminPasswordHash"] ?? configuration["ADMIN_PASSWORD_HASH"])?.Trim();
            _accessTokenMinutes = configuration.GetValue("Jwt:AccessTokenMinutes", 30);
            if (_accessTokenMinutes < 1) _accessTokenMinutes = 30;
        }

        public async Task<AuthTokenResult?> LoginAdmin(string correo, string password)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(password))
                return null;

            correo = correo.Trim();
            password = password.Trim();

            if (string.IsNullOrWhiteSpace(_adminEmail))
                return null;

            if (!string.Equals(correo, _adminEmail, StringComparison.OrdinalIgnoreCase))
                return null;

            if (!string.IsNullOrWhiteSpace(_adminPasswordHash))
            {
                if (BCrypt.Net.BCrypt.Verify(password, _adminPasswordHash))
                    return await IssueTokensAsync(correo, Roles.Administrador);
                return null;
            }

            if (!string.IsNullOrWhiteSpace(_adminPassword) && password == _adminPassword)
                return await IssueTokensAsync(correo, Roles.Administrador);

            return null;
        }

        public Task<AuthTokenResult?> LoginPsicologo(string correo, string password)
        {
            return LoginGenerico(
                correo,
                password,
                _uow.Psicologo,
                p => p.PsiCorreoPersonal,
                p => p.PsiPassword,
                p => p.PsiEstadoRegistro == "activo",
                Roles.Psicologo,
                p => p.PsiCodigo.ToString()
            );
        }

        public Task<AuthTokenResult?> LoginAprendiz(string correo, string password)
        {
            return LoginGenerico(
                correo,
                password,
                _uow.Aprendiz,
                a => a.AprCorreoPersonal,
                a => a.AprPassword,
                a => a.AprEstadoRegistro == "activo",
                Roles.Aprendiz,
                a => a.AprCodigo.ToString()
            );
        }

        public async Task<AuthTokenResult?> RefrescarAsync(string refreshToken)
        {
            var rotated = await _refreshTokenService.ValidateAndRotateAsync(refreshToken);
            if (rotated == null)
                return null;

            var access = GenerarToken(rotated.UserId, rotated.Role);
            return new AuthTokenResult
            {
                AccessToken = access,
                RefreshToken = rotated.NewRefreshTokenPlain,
                ExpiresIn = _accessTokenMinutes * 60,
                TokenType = "Bearer"
            };
        }

        private async Task<AuthTokenResult> IssueTokensAsync(string userId, string role)
        {
            var access = GenerarToken(userId, role);
            var refresh = await _refreshTokenService.CreateAsync(userId, role);
            return new AuthTokenResult
            {
                AccessToken = access,
                RefreshToken = refresh,
                ExpiresIn = _accessTokenMinutes * 60,
                TokenType = "Bearer"
            };
        }

        private async Task<AuthTokenResult?> LoginGenerico<T>(
            string correo,
            string password,
            InterfazGenerica<T> repositorio,
            Expression<Func<T, string>> correoSelector,
            Expression<Func<T, string>> passwordSelector,
            Expression<Func<T, bool>> estadoActivo,
            string rol,
            Func<T, string> idSelector
        ) where T : class
        {
            var parameter = correoSelector.Parameters[0];

            var visitor = new ParameterReplaceVisitor(estadoActivo.Parameters[0], parameter);
            var estadoActivoBodyNormalizado = visitor.Visit(estadoActivo.Body);

            var lambdaFinal = Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    Expression.Equal(correoSelector.Body, Expression.Constant(correo)),
                    estadoActivoBodyNormalizado
                ),
                parameter
            );

            var usuario = await repositorio.ObtenerPrimero(lambdaFinal);

            if (usuario == null)
                return null;

            var passwordHash = passwordSelector.Compile()(usuario);

            if (!BCrypt.Net.BCrypt.Verify(password, passwordHash))
                return null;

            return await IssueTokensAsync(idSelector(usuario), rol);
        }

        private string GenerarToken(string id, string rol)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Role, rol)
            };

            var keyBytes = Encoding.UTF8.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.UtcNow.AddMinutes(-1),
                Expires = DateTime.UtcNow.AddMinutes(_accessTokenMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private class ParameterReplaceVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _from, _to;

            public ParameterReplaceVisitor(ParameterExpression from, ParameterExpression to)
            {
                _from = from;
                _to = to;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _from ? _to : base.VisitParameter(node);
            }
        }
    }
}
