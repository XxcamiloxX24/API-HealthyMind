using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class Preguntas
{
    public int PregCodigo { get; set; }

    public string? PregDescripcion { get; set; }

    [JsonIgnore]
    public int? PregCategoriaFk { get; set; }
    public virtual CategoriaPreguntas? CategoriaPregunta { get; set; }
    [JsonIgnore]
    public string? PregEstadoRegistro { get; set; }

    [JsonIgnore]
    public virtual ICollection<TestPreguntas> TestPregunta { get; set; } = new List<TestPreguntas>();
}
