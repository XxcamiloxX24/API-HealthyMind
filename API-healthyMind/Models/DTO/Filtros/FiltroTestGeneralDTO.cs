namespace API_healthyMind.Models.DTO.Filtros
{
    public class FiltroTestGeneralDTO
    {
        public int? Codigo { get; set; }
        public string? TipoDocumento { get; set; }
        public long? NroDocumento { get; set; }
        public string? PrimerNombre { get; set; }
        public string? PrimerApellido { get; set; }
        public int? MunicipioID { get; set; }
        public string? MunicipioNombre { get; set; }
        public int? DepartamentoID { get; set; }
        public int? EstadoAprendizID { get; set; }
        public string? Eps { get; set; }
        public string? TipoPoblacion { get; set; }
        public int? PsicologoDocumento {  get; set; } 

        public DateTime? FechaRealizacionDesde { get; set; }
        public DateTime? FechaRealizacionHasta { get; set; }
    }
}
