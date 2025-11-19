using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class CategoriaRespuestas
{
    public int CatResCodigo { get; set; }

    public string? CatResNombre { get; set; }

    public string? CatResDescripcion { get; set; }
    [JsonIgnore]
    public virtual ICollection<Respuestas> Respuesta { get; set; } = new List<Respuestas>();
}
