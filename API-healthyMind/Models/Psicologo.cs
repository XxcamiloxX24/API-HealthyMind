using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class Psicologo
{

    public int PsiCodigo { get; set; }


    public int? PsiDocumento { get; set; }


    public string? PsiNombre { get; set; }


    public string? PsiApellido { get; set; }

    public string? PsiEspecialidad { get; set; }

    public string? PsiTelefono { get; set; }


    public DateTime? PsiFechaRegistro { get; set; }

    public DateOnly? PsiFechaNac { get; set; }

    public string? PsiDireccion { get; set; }

    public string? PsiCorreoInstitucional { get; set; }

    public string? PsiCorreoPersonal { get; set; }

    [JsonIgnore]
    public string? PsiPassword { get; set; }

    [JsonIgnore]
    public byte[]? PsiFirma { get; set; }

    /// <summary>
    /// estado del psicologo
    /// </summary>
    [JsonIgnore]
    public string? PsiEstadoRegistro { get; set; }
    [JsonIgnore]
    public virtual ICollection<Area> Area { get; set; } = new List<Area>();

    [JsonIgnore]
    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    [JsonIgnore]
    public virtual ICollection<SeguimientoAprendiz> SeguimientoAprendizs { get; set; } = new List<SeguimientoAprendiz>();

    [JsonIgnore]
    public virtual ICollection<TestGeneral> TestGenerals { get; set; } = new List<TestGeneral>();
}
