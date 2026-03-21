using System;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class ReporteHistorial
{
    public int RephCodigo { get; set; }

    [JsonIgnore]
    public int RephReporteFk { get; set; }

    public string RephAccion { get; set; } = null!;

    public string? RephDescripcion { get; set; }

    public DateTime RephFecha { get; set; }

    [JsonIgnore]
    public virtual Reporte? RephReporteFkNavigation { get; set; }
}
