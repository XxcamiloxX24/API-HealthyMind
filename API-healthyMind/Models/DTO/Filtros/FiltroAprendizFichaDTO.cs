namespace API_healthyMind.Models.DTO.Filtros
{
    public class FiltroAprendizFichaDTO
    {
        public int? FichaCodigo { get; set; }
        public string? AreaNombre { get; set; }
        public string? ProgramaNombre { get; set; }
        public string? PsicologoID { get; set; }
        /// <summary>Filtro por código del psicólogo (PsiCodigo). Prioridad sobre PsicologoID cuando ambos se usan.</summary>
        public int? PsicologoCodigo { get; set; }
        public string? TipoPoblacion { get; set; }
        public string? Eps { get; set; }
        public int? EstadoAprendizID { get; set; }
        public string? CentroNombre { get; set; }
        public string? Jornada { get; set; }
        public string? AreaRemitido { get; set; }
        public int? TrimestreActual { get; set; }
        /// <summary>Filtro por estado del seguimiento: Criticos, En Observacion, Estables.</summary>
        public string? EstadoSeguimiento { get; set; }
        public string? AprendizDocumento { get; set; }

        public DateTime? FechaInicioDesde { get; set; }
        public DateTime? FechaInicioHasta { get; set; }

        public DateTime? FechaFinDesde { get; set; }
        public DateTime? FechaFinHasta { get; set; }
    }
}
