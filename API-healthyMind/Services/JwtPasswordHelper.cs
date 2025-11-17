using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;


namespace API_healthyMind.Services
{
    public static class JwtPasswordHelper
    {
        private static readonly string Key = "f7ad9b48f344387a515b2d4279f80407";

        public static string GenerarToken(int documento, string tipoUsuario)
        {
            var claims = new[]
            {
            new Claim("documento", documento.ToString()),
            new Claim("tipo", tipoUsuario)
        };

            var keyBytes = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var creds = new SigningCredentials(keyBytes, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public class TokenData
        {
            public int Documento { get; set; }
            public string TipoUsuario { get; set; }
        }

        public static TokenData ValidarToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(Key);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;

            return new TokenData
            {
                Documento = int.Parse(jwt.Claims.First(c => c.Type == "documento").Value),
                TipoUsuario = jwt.Claims.First(c => c.Type == "tipo").Value
            };
        }

    }
}
