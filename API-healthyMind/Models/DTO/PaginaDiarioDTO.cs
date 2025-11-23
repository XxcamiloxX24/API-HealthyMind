namespace API_healthyMind.Models.DTO
{
    public class PaginaDiarioDTO
    {
        public string? PagTitulo { get; set; }
        public string? PagContenido { get; set; }
        public int? PagDiarioFk { get; set; }
        public int? PagEmocionFk { get; set; }
    }
}
