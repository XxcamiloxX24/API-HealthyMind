namespace API_healthyMind.Models.DTO;

/// <summary>
/// Formato de respuesta para el frontend según el boceto.
/// </summary>
public class ReporteResponseDTO
{
    public int Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public string Fecha { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public List<string> Historial { get; set; } = new();
    public string Usuario { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string Prioridad { get; set; } = null!;
    public string Categoria { get; set; } = null!;
    public string AsignadoA { get; set; } = null!;
    public string? FechaActualizacion { get; set; }
}
