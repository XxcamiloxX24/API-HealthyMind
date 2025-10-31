using System;
using System.Collections.Generic;

namespace API_healthyMind.Models;

public partial class TestGeneral
{
    public int TestGenCodigo { get; set; }

    public int? TestGenApreFk { get; set; }

    public int? TestGenPsicoFk { get; set; }

    public DateTime? TestGenFechaRealiz { get; set; }

    public string? TestGenResultados { get; set; }

    public string? TestGenRecomendacion { get; set; }

    public string? TestGenEstado { get; set; }

    public virtual AprendizFicha? TestGenApreFkNavigation { get; set; }

    public virtual Psicologo? TestGenPsicoFkNavigation { get; set; }

    public virtual ICollection<TestPreguntas> TestPregunta { get; set; } = new List<TestPreguntas>();
}
