using System;
using System.Collections.Generic;

namespace API_healthyMind.Models;

public partial class SeguimientoAprendiz
{
    /// <summary>
    /// Identificador del seguimiento
    /// </summary>
    public int SegCodigo { get; set; }

    /// <summary>
    /// Aprendiz
    /// </summary>
    public int? SegAprendizFk { get; set; }

    /// <summary>
    /// Psicologo
    /// </summary>
    public int? SegPsicologoFk { get; set; }

    /// <summary>
    /// Fecha de inicio del seguimiento
    /// </summary>
    public DateOnly? SegFechaSeguimiento { get; set; }

    /// <summary>
    /// Fecha final del seguimiento
    /// </summary>
    public DateOnly? SegFechaFin { get; set; }

    public string? SegAreaRemitido { get; set; }

    public int? SegTrimestreActual { get; set; }

    public string? SegMotivo { get; set; }

    /// <summary>
    /// Descripcion u observacion al aprendiz
    /// </summary>
    public string? SegDescripcion { get; set; }

    /// <summary>
    /// recomendaciones para el aprendiz
    /// </summary>
    public string? SegRecomendaciones { get; set; }

    public string? SegEstadoRegistro { get; set; }

    public virtual AprendizFicha? SegAprendizFkNavigation { get; set; }

    public virtual Psicologo? SegPsicologoFkNavigation { get; set; }
}
