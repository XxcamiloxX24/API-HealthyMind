using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class Emociones
{
    public int EmoCodigo { get; set; }

    public string? EmoNombre { get; set; }

    /// <summary>Caracter Unicode del emoji (ej: "😊").</summary>
    public string? EmoEmoji { get; set; }

    /// <summary>Valor de escala 1-10 que determina la categoría.</summary>
    public int EmoEscala { get; set; } = 5;

    /// <summary>Color hex de fondo para la UI (ej: "#fef3c7").</summary>
    public string? EmoColorFondo { get; set; }

    public string? EmoDescripcion { get; set; }

    public string? EmoImage { get; set; }

    public string EmoEstadoRegistro { get; set; } = "activo";

    [JsonIgnore]
    public virtual ICollection<PaginaDiario> PaginaDiarios { get; set; } = new List<PaginaDiario>();

    /// <summary>Categoría derivada de la escala (no se persiste en BD).</summary>
    [JsonIgnore]
    public string Categoria => EmoEscala switch
    {
        <= 2 => "Critica",
        <= 4 => "Negativa",
        <= 6 => "Neutral",
        _    => "Positiva"
    };
}
