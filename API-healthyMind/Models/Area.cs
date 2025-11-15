using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class Area
{
    /// <summary>
    /// Identificador de la facultad o area
    /// </summary>
    public int AreaCodigo { get; set; }

    /// <summary>
    /// Nombre de la facultad o area
    /// </summary>
    public string? AreaNombre { get; set; }

    [JsonIgnore]
    public string? AreaEstadoRegistro { get; set; }
    [JsonIgnore]
    public int? AreaPsicCodFk { get; set; }
    public virtual Psicologo? AreaPsicologo { get; set; }

    [JsonIgnore]
    public virtual ICollection<Programaformacion> Programaformacions { get; set; } = new List<Programaformacion>();
}
