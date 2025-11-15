using API_healthyMind.Repositorios.Interfaces;
using System.Net;
using System.Net.Mail;
using SmtpClient = System.Net.Mail.SmtpClient;


namespace API_healthyMind.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var smtp = new SmtpClient()
            {
                Host = _config["Email:Host"],
                Port = int.Parse(_config["Email:Port"]),
                Credentials = new NetworkCredential(
                    _config["Email:User"],
                    _config["Email:Pass"]
                ),
                EnableSsl = true
            };

            var mail = new MailMessage(_config["Email:User"], to, subject, body);
            await smtp.SendMailAsync(mail);
        }
    }
}
