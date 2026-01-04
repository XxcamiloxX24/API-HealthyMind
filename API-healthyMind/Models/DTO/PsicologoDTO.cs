using System.Text.Json.Serialization;

namespace API_healthyMind.Models.DTO
{
    public class PsicologoDTO
    {
        public string? PsiDocumento { get; set; }


        public string? PsiNombre { get; set; }


        public string? PsiApellido { get; set; }

        public string? PsiEspecialidad { get; set; }

        public string? PsiTelefono { get; set; }

        public DateOnly? PsiFechaNac { get; set; }

        public string? PsiDireccion { get; set; }

        public string? PsiCorreoInstitucional { get; set; }

        public string? PsiCorreoPersonal { get; set; }

        public string? PsiPassword { get; set; }
    }
}
