using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class Aprendiz
{
    /// <summary>
    /// Codigo unico del aprendiz
    /// </summary>
    public int AprCodigo { get; set; }

    public DateTime AprFechaCreacion { get; set; }

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
    [JsonIgnore]
    public string? AprPassword { get; set; }

    public string? AprDireccion { get; set; }
    [JsonIgnore]
    public int? AprCiudadFk { get; set; }
    public virtual Ciudad? Municipio { get; set; }

    /// <summary>
    /// Numero de celular
    /// </summary>
    public int? AprTelefono { get; set; }

    public string? AprEps { get; set; }

    public string? AprPatologia { get; set; }
    [JsonIgnore]
    public int? AprEstadoAprFk { get; set; }
    public virtual EstadoAprendiz? EstadoAprendiz { get; set; }

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
    [JsonIgnore]
    public byte[]? AprFirma { get; set; }

    /// <summary>
    /// Estado del registro
    /// </summary>
    [JsonIgnore]
    public string? AprEstadoRegistro { get; set; }
    [JsonIgnore]
    public DateTime? AprFechaEliminacion { get; set; }
    [JsonIgnore]
    public string? AprRazonEliminacion { get; set; }


    [JsonIgnore]
    public virtual ICollection<AprendizFicha> AprendizFichas { get; set; } = new List<AprendizFicha>();
    [JsonIgnore]
    public virtual ICollection<Diario> Diarios { get; set; } = new List<Diario>();
}
