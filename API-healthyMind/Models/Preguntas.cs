using System;
using System.Collections.Generic;

namespace API_healthyMind.Models;

public partial class Preguntas
{
    public int PregCodigo { get; set; }

    public string? PregDescripcion { get; set; }

    public int? PregCategoriaFk { get; set; }

    public string? PregEstadoRegistro { get; set; }

    public virtual CategoriaPreguntas? PregCategoriaFkNavigation { get; set; }

    public virtual ICollection<TestPreguntas> TestPregunta { get; set; } = new List<TestPreguntas>();
}
