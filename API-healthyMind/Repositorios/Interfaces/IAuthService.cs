using API_healthyMind.Models.DTO;

namespace API_healthyMind.Repositorios.Interfaces
{
    public interface IAuthService
    {
        Task<AuthTokenResult?> LoginPsicologo(string correo, string password);
        Task<AuthTokenResult?> LoginAprendiz(string correo, string password);
        Task<AuthTokenResult?> LoginAdmin(string correo, string password);
        Task<AuthTokenResult?> RefrescarAsync(string refreshToken);
    }
}
