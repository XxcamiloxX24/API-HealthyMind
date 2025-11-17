using System.Text;
using System.Security.Cryptography;

namespace API_healthyMind.Services
{
    public static class PasswordHelper
    {
        public static string GenerarHash(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }
    }
}
