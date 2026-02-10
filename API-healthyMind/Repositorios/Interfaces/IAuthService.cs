namespace API_healthyMind.Repositorios.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginPsicologo(string correo, string password);
        Task<string?> LoginAprendiz(string correo, string password);
        string? LoginAdmin(string correo, string password);
    }
}
