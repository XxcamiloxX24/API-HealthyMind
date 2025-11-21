using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class TestPreguntas
{
    [JsonIgnore]
    public int TesRegistroFk { get; set; }
    public virtual TestGeneral TesRegistroFkNavigation { get; set; } = null!;

    [JsonIgnore]
    public int TesPregFk { get; set; }
    public virtual Preguntas TesPregFkNavigation { get; set; } = null!;

    [JsonIgnore]
    public int TesRespFk { get; set; }
    public virtual Respuestas TesRespFkNavigation { get; set; } = null!;
    [JsonIgnore]
    public string? TesEstadoRegistro { get; set; }



}
