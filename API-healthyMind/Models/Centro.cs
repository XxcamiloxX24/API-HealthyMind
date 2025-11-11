using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class Centro
{
    /// <summary>
    /// identificador del centro
    /// </summary>
    public int CenCodigo { get; set; }

    /// <summary>
    /// nombre del centro
    /// </summary>
    public string? CenNombre { get; set; }

    /// <summary>
    /// direccion del centro
    /// </summary>
    public string? CenDireccion { get; set; }

    public string? CenEstadoRegistro { get; set; }

    public int? CenRegCodFk { get; set; }

    public int? CenCodFk { get; set; }

    public virtual Centro? CenCodFkNavigation { get; set; }

    public virtual Regional? Regional { get; set; }
    [JsonIgnore]
    public virtual ICollection<Centro> InverseCenCodFkNavigation { get; set; } = new List<Centro>();

    [JsonIgnore]
    public virtual ICollection<Programaformacion> Programaformacions { get; set; } = new List<Programaformacion>();
}
