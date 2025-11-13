using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class Ficha
{
    /// <summary>
    /// Numero de ficha
    /// </summary>
    public int FicCodigo { get; set; }

    /// <summary>
    /// Tipo de jornada de la ficha
    /// </summary>
    public string? FicJornada { get; set; }

    public DateOnly? FicFechaInicio { get; set; }

    public DateOnly? FicFechaFin { get; set; }

    /// <summary>
    /// Estado de formacion de la ficha
    /// </summary>
    public string? FicEstadoFormacion { get; set; }

    [JsonIgnore]
    public int? FicProgramaFk { get; set; }
    public virtual Programaformacion? programaFormacion { get; set; }
    [JsonIgnore]
    public string? FicEstadoRegistro { get; set; }

    [JsonIgnore]
    public virtual ICollection<AprendizFicha> AprendizFichas { get; set; } = new List<AprendizFicha>();

}
