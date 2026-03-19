namespace API_healthyMind.Models.DTO
{
    public class DiarioDTO
    {
        public string? DiaTitulo { get; set; }
        /// <summary>URL del enlace a la imagen del diario.</summary>
        public string? DiaImagenUrl { get; set; }
        public int DiaAprendizFk { get; set; }
    }
}
