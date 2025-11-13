using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class Programaformacion
{
    /// <summary>
    /// Codigo o identificador del programa de formacion
    /// </summary>
    public int ProgCodigo { get; set; }

    /// <summary>
    /// Nombre del programa
    /// </summary>
    public string? ProgNombre { get; set; }

    /// <summary>
    /// Modalidad del programa
    /// </summary>
    public string? ProgModalidad { get; set; }

    /// <summary>
    /// Tipo de modalidad de formacion
    /// </summary>
    public string? ProgFormaModalidad { get; set; }

    [JsonIgnore]
    public int? ProgNivFormFk { get; set; }
    public virtual NivelFormacion? NivelFormacion { get; set; }

    [JsonIgnore]
    public int? ProgAreaFk { get; set; }
    public virtual Area? Area { get; set; }

    [JsonIgnore]
    public int? ProgCentroFk { get; set; }
    public virtual Centro? Centro { get; set; }

    [JsonIgnore]
    public string? ProgEstadoRegistro { get; set; }
    [JsonIgnore]
    public virtual ICollection<Ficha> Fichas { get; set; } = new List<Ficha>();
}
