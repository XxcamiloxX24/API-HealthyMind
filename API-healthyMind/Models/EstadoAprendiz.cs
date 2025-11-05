using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class EstadoAprendiz
{
    /// <summary>
    /// Identificador del estado
    /// </summary>
    public int EstAprCodigo { get; set; }

    /// <summary>
    /// Nombre del estado
    /// </summary>
    public string? EstAprNombre { get; set; }

    /// <summary>
    /// Descripcion del estado
    /// </summary>
    public string? EstAprDescrip { get; set; }

    [JsonIgnore]
    public virtual ICollection<Aprendiz> Aprendizs { get; set; } = new List<Aprendiz>();
}
