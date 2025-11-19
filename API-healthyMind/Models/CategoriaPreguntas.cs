using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class CategoriaPreguntas
{
    public int CatPregCodigo { get; set; }

    public string? CatPregNombre { get; set; }

    public string? CatPregDescripcion { get; set; }
    [JsonIgnore]
    public virtual ICollection<Preguntas> Pregunta { get; set; } = new List<Preguntas>();
}
