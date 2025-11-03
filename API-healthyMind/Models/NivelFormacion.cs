using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class NivelFormacion
{
    /// <summary>
    /// Identificador del tipo de formacion
    /// </summary>
    public int NivForCodigo { get; set; }

    /// <summary>
    /// Nombre del tipo de formacion
    /// </summary>
    public string? NivForNombre { get; set; }

    /// <summary>
    /// Descripcion del tipo de formacion
    /// </summary>
    public string? NivForDescripcion { get; set; }
    [JsonIgnore]
    public virtual ICollection<Programaformacion> Programaformacions { get; set; } = new List<Programaformacion>();
}
