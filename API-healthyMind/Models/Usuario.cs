using System.Text.Json.Serialization;

namespace API_healthyMind.Models
{
    public class Usuario
    {
        [JsonPropertyName("correoPersonal")]
        public string CorreoPersonal { get; set; } = "";

        [JsonPropertyName("password")]
        public string Password { get; set; } = "";
    }
}
