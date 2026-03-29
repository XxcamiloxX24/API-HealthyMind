using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class TestRespuesta
{
    public int TesResCodigo { get; set; }

    public int TesResTestFk { get; set; }

    public int TesResPreguntaFk { get; set; }

    public int TesResOpcionFk { get; set; }

    public DateTime? TesResFechaRespuesta { get; set; }

    [JsonIgnore]
    public string? TesResEstadoRegistro { get; set; }

    [JsonIgnore]
    public virtual TestGeneral TesResTestFkNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual PlantillaPregunta TesResPreguntaFkNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual PlantillaOpcion TesResOpcionFkNavigation { get; set; } = null!;
}
