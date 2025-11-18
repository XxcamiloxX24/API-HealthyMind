namespace API_healthyMind.Models.DTO
{
    public class CitaDTO
    {
        public string? CitTipoCita { get; set; }

        /// <summary>
        /// Fecha en la que fue programada la cita
        /// </summary>
        public DateOnly? CitFechaProgramada { get; set; }

        /// <summary>
        /// Hora de inicio de la cita
        /// </summary>
        public TimeOnly? CitHoraInicio { get; set; }

        /// <summary>
        /// Hora de fin de la cita
        /// </summary>
        public TimeOnly? CitHoraFin { get; set; }

        /// <summary>
        /// Motivo de la cita
        /// </summary>
        public string? CitMotivo { get; set; }

        /// <summary>
        /// Anotaciones u observaciones antes o despues de la atencio
        /// </summary>
        public string? CitAnotaciones { get; set; }

        /// <summary>
        /// fecha en la que se realizo el registro de la cita
        /// </summary>
        public DateTime? CitFechaCreacion { get; set; }

        /// <summary>
        /// estado de la cita
        /// </summary>
        public string? CitEstadoCita { get; set; }

        /// <summary>
        /// Aprendiz
        /// </summary>
        public int? CitAprCodFk { get; set; }

        /// <summary>
        /// Psicologo
        /// </summary>
        public int? CitPsiCodFk { get; set; }
    }
}
