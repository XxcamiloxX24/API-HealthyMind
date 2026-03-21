namespace API_healthyMind.Models.DTO;

public class RecomendacionDTO
{
    public int? RecSeguimientoFk { get; set; }
    public string RecTitulo { get; set; } = null!;
    public string? RecDescripcion { get; set; }
    /// <summary>Pendiente, En Progreso, Completada</summary>
    public string? RecEstado { get; set; }
}
