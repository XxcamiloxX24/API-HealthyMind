using System;
using System.Collections.Generic;

namespace API_healthyMind.Models;

public partial class Diario
{
    public int DiaCodigo { get; set; }

    public string? DiaTitulo { get; set; }

    public DateOnly? DiaFechaCreacion { get; set; }

    public string DiaEstadoRegistro { get; set; } = null!;

    public int DiaAprendizFk { get; set; }

    public virtual Aprendiz aprendiz { get; set; } = null!;

    public virtual ICollection<PaginaDiario> PaginaDiarios { get; set; } = new List<PaginaDiario>();
}
