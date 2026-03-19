using System;
using System.Collections.Generic;

namespace API_healthyMind.Models;

public partial class PaginaDiario
{
    public int PagCodigo { get; set; }

    public string? PagTitulo { get; set; }

    public string? PagContenido { get; set; }

    /// <summary>URL del enlace a la imagen asociada a la página.</summary>
    public string? PagImagenUrl { get; set; }

    public DateTime PagFechaRealizacion { get; set; }

    public int? PagDiarioFk { get; set; }

    public int? PagEmocionFk { get; set; }

    public string? PagEstadoRegistro { get; set; }

    public virtual Diario? PagDiarioFkNavigation { get; set; }

    public virtual Emociones? PagEmocionFkNavigation { get; set; }
}
