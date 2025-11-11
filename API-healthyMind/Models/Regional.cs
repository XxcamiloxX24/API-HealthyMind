using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class Regional
{
    /// <summary>
    /// identificador del regional
    /// </summary>
    public int RegCodigo { get; set; }

    /// <summary>
    /// nombre del regional
    /// </summary>
    public string? RegNombre { get; set; }
    [JsonIgnore]
    public string? RegEstadoRegistro { get; set; }

    [JsonIgnore]
    public virtual ICollection<Centro> Centros { get; set; } = new List<Centro>();

    [JsonIgnore]
    public virtual ICollection<Ciudad> Ciudads { get; set; } = new List<Ciudad>();
}
