using System;
using System.Collections.Generic;

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

    public string? AreaEstadoRegistro { get; set; }

    public int? AreaCenCodFk { get; set; }

    public virtual ICollection<Programaformacion> Programaformacions { get; set; } = new List<Programaformacion>();
}
