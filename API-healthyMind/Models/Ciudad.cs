using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class Ciudad
{
    public int CiuCodigo { get; set; }

    public string? CiuNombre { get; set; }

    [JsonIgnore]
    public int? CiuRegionalFk { get; set; }
    public virtual Regional? Regional { get; set; }
    [JsonIgnore]
    public virtual ICollection<Aprendiz> Aprendizs { get; set; } = new List<Aprendiz>();

}
