using System;
using System.Collections.Generic;

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

    public int? FicProgramaFk { get; set; }

    public string? FicEstadoRegistro { get; set; }

    public virtual ICollection<AprendizFicha> AprendizFichas { get; set; } = new List<AprendizFicha>();

    public virtual Programaformacion? FicProgramaFkNavigation { get; set; }
}
