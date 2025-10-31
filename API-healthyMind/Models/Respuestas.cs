using System;
using System.Collections.Generic;

namespace API_healthyMind.Models;

public partial class Respuestas
{
    public int ResCodigo { get; set; }

    public int? ResCategoriaFk { get; set; }

    public string? ResDescripcion { get; set; }

    public string? ResEstadoRegistro { get; set; }

    public virtual CategoriaRespuestas? ResCategoriaFkNavigation { get; set; }

    public virtual ICollection<TestPreguntas> TestPregunta { get; set; } = new List<TestPreguntas>();
}
