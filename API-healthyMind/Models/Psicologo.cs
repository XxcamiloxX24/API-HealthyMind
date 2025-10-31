using System;
using System.Collections.Generic;

namespace API_healthyMind.Models;

public partial class Psicologo
{
    /// <summary>
    /// identificador del psicologo
    /// </summary>
    public int PsiCodigo { get; set; }

    /// <summary>
    /// documento del psicologo
    /// </summary>
    public int? PsiDocumento { get; set; }

    /// <summary>
    /// Nombre del psicologo
    /// </summary>
    public string? PsiNombre { get; set; }

    /// <summary>
    /// Apellido del psicologo
    /// </summary>
    public string? PsiApellido { get; set; }

    /// <summary>
    /// especialidad del psicologo
    /// </summary>
    public string? PsiEspecialidad { get; set; }

    /// <summary>
    /// Numero de telefono del psicologo
    /// </summary>
    public string? PsiTelefono { get; set; }

    /// <summary>
    /// Fecha en la que se le hizo el registro
    /// </summary>
    public DateTime? PsiFechaRegistro { get; set; }

    /// <summary>
    /// Fecha de nacimiento
    /// </summary>
    public DateOnly? PsiFechaNac { get; set; }

    /// <summary>
    /// ubicacion de su oficina de trabajo
    /// </summary>
    public string? PsiDireccion { get; set; }

    /// <summary>
    /// correo institucional
    /// </summary>
    public string? PsiCorreoInstitucional { get; set; }

    /// <summary>
    /// correo del psicologo
    /// </summary>
    public string? PsiCorreoPersonal { get; set; }

    /// <summary>
    /// contraseña del psicologo
    /// </summary>
    public string? PsiPassword { get; set; }

    public byte[]? PsiFirma { get; set; }

    /// <summary>
    /// estado del psicologo
    /// </summary>
    public string? PsiEstadoRegistro { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual ICollection<SeguimientoAprendiz> SeguimientoAprendizs { get; set; } = new List<SeguimientoAprendiz>();

    public virtual ICollection<TestGeneral> TestGenerals { get; set; } = new List<TestGeneral>();
}
