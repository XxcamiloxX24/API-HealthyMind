using System;
using System.Collections.Generic;

namespace API_healthyMind.Models;

public partial class TestPreguntas
{
    public int TesRegistroFk { get; set; }

    public int TesPregFk { get; set; }

    public int TesRespFk { get; set; }

    public string? TesEstadoRegistro { get; set; }

    public virtual Preguntas TesPregFkNavigation { get; set; } = null!;

    public virtual TestGeneral TesRegistroFkNavigation { get; set; } = null!;

    public virtual Respuestas TesRespFkNavigation { get; set; } = null!;
}
