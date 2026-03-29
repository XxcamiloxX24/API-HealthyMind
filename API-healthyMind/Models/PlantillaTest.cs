using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class PlantillaTest
{
    public int PlaTstCodigo { get; set; }

    public string PlaTstNombre { get; set; } = null!;

    public string? PlaTstDescripcion { get; set; }

    public int? PlaTstPsicologoFk { get; set; }

    [JsonIgnore]
    public string? PlaTstEstadoRegistro { get; set; }

    public DateTime? PlaTstFechaCreacion { get; set; }

    [JsonIgnore]
    public virtual Psicologo? PlaTstPsicologoFkNavigation { get; set; }

    [JsonIgnore]
    public virtual ICollection<PlantillaPregunta> Preguntas { get; set; } = new List<PlantillaPregunta>();

    [JsonIgnore]
    public virtual ICollection<TestGeneral> TestGenerales { get; set; } = new List<TestGeneral>();
}
