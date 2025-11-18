namespace API_healthyMind.Models.DTO.Filtros
{
    public class FiltroCitasDTO
    {
        public int? DocumentoAprendiz { get; set; }
        public int? FichaCodigo { get; set; }
        public string? AreaNombre { get; set; }
        public string? ProgramaNombre { get; set; }
        public int? PsicologoDocumento { get; set; }
        public string? TipoPoblacion { get; set; }
        public int? EstadoAprendizID { get; set; }
        public string? CentroNombre { get; set; }
        public string? Jornada { get; set; }

        public string? TipoCita {  get; set; }
        public DateOnly? fechaProgramada { get; set; }

    }
}
