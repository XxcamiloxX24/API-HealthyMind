using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class PlantillaOpcion
{
    public int PlaOpcCodigo { get; set; }

    public int PlaOpcPreguntaFk { get; set; }

    public string PlaOpcTexto { get; set; } = null!;

    public int PlaOpcOrden { get; set; }

    [JsonIgnore]
    public string? PlaOpcEstadoRegistro { get; set; }

    [JsonIgnore]
    public virtual PlantillaPregunta PlaOpcPreguntaFkNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<TestRespuesta> TestRespuestas { get; set; } = new List<TestRespuesta>();
}
