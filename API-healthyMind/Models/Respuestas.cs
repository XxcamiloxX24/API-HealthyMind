using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class Respuestas
{
    public int ResCodigo { get; set; }

    [JsonIgnore]
    public int? ResCategoriaFk { get; set; }
    public virtual CategoriaRespuestas? CategoriaRespuesta { get; set; }

    public string? ResDescripcion { get; set; }

    [JsonIgnore]
    public string? ResEstadoRegistro { get; set; }

    [JsonIgnore]
    public virtual ICollection<TestPreguntas> TestPregunta { get; set; } = new List<TestPreguntas>();
}
