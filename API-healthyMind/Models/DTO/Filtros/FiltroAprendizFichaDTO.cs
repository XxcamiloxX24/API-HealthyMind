namespace API_healthyMind.Models.DTO.Filtros
{
    public class FiltroAprendizFichaDTO
    {
        public int? FichaCodigo { get; set; }
        public string? AreaNombre { get; set; }
        public string? ProgramaNombre { get; set; }
        public string? PsicologoID { get; set; }
        public string? TipoPoblacion { get; set; }
        public string? Eps { get; set; }
        public int? EstadoAprendizID { get; set; }
        public string? CentroNombre { get; set; }
        public string? Jornada { get; set; }
        public string? AreaRemitido { get; set; }
        public int? TrimestreActual { get; set; }
        public string? AprendizDocumento { get; set; }

        public DateTime? FechaInicioDesde { get; set; }
        public DateTime? FechaInicioHasta { get; set; }

        public DateTime? FechaFinDesde { get; set; }
        public DateTime? FechaFinHasta { get; set; }
    }
}
