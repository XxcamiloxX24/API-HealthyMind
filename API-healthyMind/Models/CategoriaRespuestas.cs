using System;
using System.Collections.Generic;

namespace API_healthyMind.Models;

public partial class CategoriaRespuestas
{
    public int CatResCodigo { get; set; }

    public string? CatResNombre { get; set; }

    public string? CatResDescripcion { get; set; }

    public virtual ICollection<Respuestas> Respuesta { get; set; } = new List<Respuestas>();
}
