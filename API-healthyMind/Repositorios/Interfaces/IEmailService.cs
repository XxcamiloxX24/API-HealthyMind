namespace API_healthyMind.Repositorios.Interfaces
{
    public interface IEmailService
    {
        /// <param name="isHtml">Si es true, el cuerpo se envía como HTML (texto plano alternativo recomendado en clientes antiguos no aplica aquí).</param>
        Task SendAsync(string to, string subject, string body, bool isHtml = false);
    }
}
