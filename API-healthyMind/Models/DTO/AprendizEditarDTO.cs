namespace API_healthyMind.Models.DTO
{
    public class AprendizEditarDTO
    {
        public string? AprTipoDocumento { get; set; }
        public string? AprNroDocumento { get; set; }
        public DateOnly? AprFechaNac { get; set; }

        public string? AprNombre { get; set; }

        public string? AprSegundoNombre { get; set; }

        public string? AprApellido { get; set; }

        public string? AprSegundoApellido { get; set; }

        public string? AprCorreoInstitucional { get; set; }
        public string? AprCorreoPersonal { get; set; }

        public string? AprDireccion { get; set; }

        public int? AprCiudadFk { get; set; }

        public string? AprTelefono { get; set; }

        public string? AprEps { get; set; }

        public string? AprPatologia { get; set; }

        public int? AprEstadoAprFk { get; set; }

        public string? AprTipoPoblacion { get; set; }

        public string? AprTelefonoAcudiente { get; set; }

        public string? AprAcudNombre { get; set; }

        public string? AprAcudApellido { get; set; }
    }
}
