using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class PlantillaPregunta
{
    public int PlaPrgCodigo { get; set; }

    public int PlaPrgPlantillaFk { get; set; }

    public string PlaPrgTexto { get; set; } = null!;

    /// <summary>si_no, verdadero_falso, opcion_multiple, escala</summary>
    public string PlaPrgTipo { get; set; } = "opcion_multiple";

    public int PlaPrgOrden { get; set; }

    [JsonIgnore]
    public string? PlaPrgEstadoRegistro { get; set; }

    [JsonIgnore]
    public virtual PlantillaTest PlaPrgPlantillaFkNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<PlantillaOpcion> Opciones { get; set; } = new List<PlantillaOpcion>();

    [JsonIgnore]
    public virtual ICollection<TestRespuesta> TestRespuestas { get; set; } = new List<TestRespuesta>();
}
