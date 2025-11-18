namespace API_healthyMind.Models.DTO
{
    public class SeguimientoAprendizDTO
    {
        public int? SegAprendizFk { get; set; }
        public int? SegPsicologoFk { get; set; }

        /// <summary>
        /// Fecha de inicio del seguimiento
        /// </summary>
        public DateTime? SegFechaSeguimiento { get; set; }

        /// <summary>
        /// Fecha final del seguimiento
        /// </summary>
        public DateTime? SegFechaFin { get; set; }

        public string? SegAreaRemitido { get; set; }

        public int? SegTrimestreActual { get; set; }

        public string? SegMotivo { get; set; }

        /// <summary>
        /// Descripcion u observacion al aprendiz
        /// </summary>
        public string? SegDescripcion { get; set; }

        /// <summary>
        /// recomendaciones para el aprendiz
        /// </summary>
        public string? SegRecomendaciones { get; set; }
        public string? SegFirmaProfesional { get; set; }
        public string? SegFirmaAprendiz { get; set; }
    }
}
