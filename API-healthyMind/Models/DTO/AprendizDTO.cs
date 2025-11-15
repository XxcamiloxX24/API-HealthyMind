namespace API_healthyMind.Models.DTO
{
    public class AprendizDTO
    {
        
        public string? AprTipoDocumento { get; set; }

        public int? AprNroDocumento { get; set; }

        public DateOnly? AprFechaNac { get; set; }

        public string? AprNombre { get; set; }

        public string? AprSegundoNombre { get; set; }

        public string? AprApellido { get; set; }

        public string? AprSegundoApellido { get; set; }

        public string? AprCorreoInstitucional { get; set; }

        public string? AprCorreoPersonal { get; set; }

        public string? AprPassword { get; set; }

        public string? AprDireccion { get; set; }

        public int? AprCiudadFk { get; set; }

        public int? AprTelefono { get; set; }

        public string? AprEps { get; set; }

        public string? AprPatologia { get; set; }

        public int? AprEstadoAprFk { get; set; }

        public string? AprTipoPoblacion { get; set; }

        public int? AprTelefonoAcudiente { get; set; }

        public string? AprAcudNombre { get; set; }

        public string? AprAcudApellido { get; set; }
    }
}
