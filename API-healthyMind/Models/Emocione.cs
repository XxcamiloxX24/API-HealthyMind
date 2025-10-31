using System;
using System.Collections.Generic;

namespace API_healthyMind.Models;

public partial class Emocione
{
    /// <summary>
    /// identificador de la emocion
    /// </summary>
    public int EmoCodigo { get; set; }

    /// <summary>
    /// nombre de la emocion
    /// </summary>
    public string? EmoNombre { get; set; }

    /// <summary>
    /// descripcion de la emocion
    /// </summary>
    public string? EmoDescripcion { get; set; }

    /// <summary>
    /// url de la imagen de la emocion
    /// </summary>
    public string? EmoImage { get; set; }

    public virtual ICollection<PaginaDiario> PaginaDiarios { get; set; } = new List<PaginaDiario>();
}
