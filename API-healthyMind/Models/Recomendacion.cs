using System;
using System.Collections.Generic;

namespace API_healthyMind.Models;

public partial class Recomendacion
{
    public int RecCodigo { get; set; }
    public int RecSeguimientoFk { get; set; }
    public string RecTitulo { get; set; } = null!;
    public string? RecDescripcion { get; set; }
    public DateTime? RecFechaVencimiento { get; set; }
    public string? RecEstado { get; set; }
    public string? RecEstadoRegistro { get; set; }
    public DateTime? RecFechaCreacion { get; set; }
    public DateTime? RecFechaActualizacion { get; set; }

    public virtual SeguimientoAprendiz RecSeguimientoFkNavigation { get; set; } = null!;
}
