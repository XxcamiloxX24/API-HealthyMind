using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class Reporte
{
    public int RepCodigo { get; set; }

    public string RepTitulo { get; set; } = null!;

    public string RepDescripcion { get; set; } = null!;

    public DateTime RepFechaCreacion { get; set; }

    public string RepEstado { get; set; } = "creado";

    public string RepPrioridad { get; set; } = "media";

    public string RepCategoria { get; set; } = null!;

    public string RepAsignadoA { get; set; } = "Administrador";

    public DateTime? RepFechaActualizacion { get; set; }

    public string RepTipoReportador { get; set; } = null!;

    [JsonIgnore]
    public int? RepAprendizFk { get; set; }

    [JsonIgnore]
    public int? RepPsicologoFk { get; set; }

    [JsonIgnore]
    public string RepEstadoRegistro { get; set; } = "activo";

    [JsonIgnore]
    public virtual Aprendiz? RepAprendizFkNavigation { get; set; }

    [JsonIgnore]
    public virtual Psicologo? RepPsicologoFkNavigation { get; set; }

    [JsonIgnore]
    public virtual ICollection<ReporteHistorial> ReporteHistorials { get; set; } = new List<ReporteHistorial>();
}
