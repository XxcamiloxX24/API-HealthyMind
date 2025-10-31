using System;
using System.Collections.Generic;

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

    public string? ProgEstadoRegistro { get; set; }

    public int? ProgNivFormFk { get; set; }

    public int? ProgAreaFk { get; set; }

    public int? ProgCentroFk { get; set; }

    public virtual ICollection<Ficha> Fichas { get; set; } = new List<Ficha>();

    public virtual Area? ProgAreaFkNavigation { get; set; }

    public virtual Centro? ProgCentroFkNavigation { get; set; }

    public virtual NivelFormacion? ProgNivFormFkNavigation { get; set; }
}
