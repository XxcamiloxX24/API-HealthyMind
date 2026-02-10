using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;
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
        private readonly string _secretKey;

        public AuthService(IUnidadDeTrabajo uow, IConfiguration configuration)
        {
            _uow = uow;
            _secretKey = configuration["settings:secretkey"];
        }

        public string? LoginAdmin(string correo, string password)
        {
            if (correo == "healthymindsoporte2@gmail.com" &&
                password == "AdminHealthy123")
            {
                return GenerarToken(correo, Roles.Administrador);
            }

            return null;
        }

        public async Task<string?> LoginPsicologo(string correo, string password)
        {
            return await LoginGenerico(
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

        public async Task<string?> LoginAprendiz(string correo, string password)
        {
            return await LoginGenerico(
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

        private async Task<string?> LoginGenerico<T>(
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
            // 1. Obtenemos el parámetro base de la primera expresión (ej: 'p' o 'a')
            var parameter = correoSelector.Parameters[0];

            // 2. Visitamos la expresión 'estadoActivo' y reemplazamos su parámetro 
            //    por el parámetro 'parameter' de arriba.
            var visitor = new ParameterReplaceVisitor(estadoActivo.Parameters[0], parameter);
            var estadoActivoBodyNormalizado = visitor.Visit(estadoActivo.Body);

            // 3. Combinamos las expresiones usando el parámetro unificado
            //    Condición: (Correo == correo) AND (Estado == activo)
            var lambdaFinal = Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    Expression.Equal(correoSelector.Body, Expression.Constant(correo)),
                    estadoActivoBodyNormalizado // Usamos el cuerpo corregido
                ),
                parameter // Pasamos el parámetro único
            );

            // 4. Ejecutamos la consulta con el lambda bien formado
            var usuario = await repositorio.ObtenerPrimero(lambdaFinal);

            if (usuario == null)
                return null;

            var passwordHash = passwordSelector.Compile()(usuario);

            if (!BCrypt.Net.BCrypt.Verify(password, passwordHash))
                return null;

            return GenerarToken(idSelector(usuario), rol);
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

                Expires = DateTime.UtcNow.AddDays(5),
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
                // Si encontramos el parámetro viejo, lo cambiamos por el nuevo
                return node == _from ? _to : base.VisitParameter(node);
            }
        }
    }
}
