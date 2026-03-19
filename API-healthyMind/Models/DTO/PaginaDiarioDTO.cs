namespace API_healthyMind.Models.DTO
{
    public class PaginaDiarioDTO
    {
        public string? PagTitulo { get; set; }
        public string? PagContenido { get; set; }
        /// <summary>URL del enlace a la imagen de la página.</summary>
        public string? PagImagenUrl { get; set; }
        public int? PagDiarioFk { get; set; }
        public int? PagEmocionFk { get; set; }
    }
}
