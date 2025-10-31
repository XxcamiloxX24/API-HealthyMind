using System;
using System.Collections.Generic;

namespace API_healthyMind.Models;

public partial class Ciudad
{
    public int CiuCodigo { get; set; }

    public string? CiuNombre { get; set; }

    public int? CiuRegionalFk { get; set; }

    public virtual ICollection<Aprendiz> Aprendizs { get; set; } = new List<Aprendiz>();

    public virtual Regional? CiuRegionalFkNavigation { get; set; }
}
