using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class AprendizFicha
{
    public int AprFicCodigo { get; set; }

    [JsonIgnore]
    public int? AprFicAprendizFk { get; set; }
    public virtual Aprendiz? Aprendiz { get; set; }
    [JsonIgnore]
    public int? AprFicFichaFk { get; set; }
    public virtual Ficha? Ficha { get; set; }

    [JsonIgnore]
    public string? AprFicEstadoRegistro { get; set; }



    [JsonIgnore]
    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    [JsonIgnore]
    public virtual ICollection<SeguimientoAprendiz> SeguimientoAprendizs { get; set; } = new List<SeguimientoAprendiz>();

    [JsonIgnore]
    public virtual ICollection<TestGeneral> TestGenerals { get; set; } = new List<TestGeneral>();
}
