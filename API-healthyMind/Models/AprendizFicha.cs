using System;
using System.Collections.Generic;

namespace API_healthyMind.Models;

public partial class AprendizFicha
{
    public int AprFicCodigo { get; set; }

    public int? AprFicAprendizFk { get; set; }

    public int? AprFicFichaFk { get; set; }

    public string? AprFicEstadoRegistro { get; set; }

    public virtual Aprendiz? AprFicAprendizFkNavigation { get; set; }

    public virtual Ficha? AprFicFichaFkNavigation { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual ICollection<SeguimientoAprendiz> SeguimientoAprendizs { get; set; } = new List<SeguimientoAprendiz>();

    public virtual ICollection<TestGeneral> TestGenerals { get; set; } = new List<TestGeneral>();
}
