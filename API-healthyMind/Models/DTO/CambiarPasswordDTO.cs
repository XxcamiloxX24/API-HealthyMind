namespace API_healthyMind.Models.DTO
{
    public class CambiarPasswordDTO
    {
        public string UsuarioDocumento { get; set; }
        public string PasswordActual { get; set; }
        public string PasswordNueva { get; set; }
    }
}
