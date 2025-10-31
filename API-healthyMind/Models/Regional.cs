using System;
using System.Collections.Generic;

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

    public string? RegEstadoRegistro { get; set; }

    public virtual ICollection<Centro> Centros { get; set; } = new List<Centro>();

    public virtual ICollection<Ciudad> Ciudads { get; set; } = new List<Ciudad>();
}
