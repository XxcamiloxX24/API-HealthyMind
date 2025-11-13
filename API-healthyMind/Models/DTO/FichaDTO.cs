namespace API_healthyMind.Models.DTO
{
    public class FichaDTO
    {
        public int FicCodigo { get; set; }

        public string? FicJornada { get; set; }

        public DateOnly? FicFechaInicio { get; set; }

        public DateOnly? FicFechaFin { get; set; }

        public string? FicEstadoFormacion { get; set; }

        public int? FicProgramaFk { get; set; }
    }
}
