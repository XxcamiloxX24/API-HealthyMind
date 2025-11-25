namespace API_healthyMind.Models.DTO.Filtros
{
    public class FiltroFichaDTO
    {
        public int? FichaCodigo { get; set; }
        public string? AreaNombre { get; set; }
        public string? ProgramaNombre { get; set; }
        public string? PsicologoID { get; set; }
        public string? CentroNombre { get; set; }
        public string? Jornada { get; set; }

        public DateOnly? FechaInicio { get; set; }
        public DateOnly? FechaFin { get; set; }
    }
}
