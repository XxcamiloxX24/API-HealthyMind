using System;
using System.Collections.Generic;

namespace API_healthyMind.Models;

public partial class Aprendiz
{
    /// <summary>
    /// Codigo unico del aprendiz
    /// </summary>
    public int AprCodigo { get; set; }

    /// <summary>
    /// Tipo de documento de identidad
    /// </summary>
    public string? AprTipoDocumento { get; set; }

    /// <summary>
    /// Numero de documento
    /// </summary>
    public int? AprNroDocumento { get; set; }

    public DateOnly? AprFechaNac { get; set; }

    /// <summary>
    /// Nombre del aprendiz
    /// </summary>
    public string? AprNombre { get; set; }

    /// <summary>
    /// Segundo nombre del aprendiz
    /// </summary>
    public string? AprSegundoNombre { get; set; }

    /// <summary>
    /// Apellido del aprendiz
    /// </summary>
    public string? AprApellido { get; set; }

    /// <summary>
    /// Segundo Apellido
    /// </summary>
    public string? AprSegundoApellido { get; set; }

    public string? AprCorreoInstitucional { get; set; }

    /// <summary>
    /// Correo del aprendiz
    /// </summary>
    public string? AprCorreoPersonal { get; set; }

    public string? AprPassword { get; set; }

    public string? AprDireccion { get; set; }

    public int? AprCiudadFk { get; set; }

    /// <summary>
    /// Numero de celular
    /// </summary>
    public int? AprTelefono { get; set; }

    public string? AprEps { get; set; }

    public string? AprPatologia { get; set; }

    public int? AprEstadoAprFk { get; set; }

    public string? AprTipoPoblacion { get; set; }

    /// <summary>
    /// Numero de celular de un acudiente
    /// </summary>
    public int? AprTelefonoAcudiente { get; set; }

    /// <summary>
    /// Nombre del acudiente
    /// </summary>
    public string? AprAcudNombre { get; set; }

    /// <summary>
    /// apelldio del acudiente
    /// </summary>
    public string? AprAcudApellido { get; set; }

    public byte[]? AprFirma { get; set; }

    /// <summary>
    /// Estado del registro
    /// </summary>
    public string? AprEstadoRegistro { get; set; }

    public DateTime? AprFechaEliminacion { get; set; }

    public string? AprRazonEliminacion { get; set; }

    public virtual Ciudad? AprCiudadFkNavigation { get; set; }

    public virtual EstadoAprendiz? AprEstadoAprFkNavigation { get; set; }

    public virtual ICollection<AprendizFicha> AprendizFichas { get; set; } = new List<AprendizFicha>();

    public virtual ICollection<Diario> Diarios { get; set; } = new List<Diario>();
}
