namespace API_healthyMind.Models.DTO;

public class ReporteCreateDTO
{
    public string Titulo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public string Prioridad { get; set; } = "media";
    public string Categoria { get; set; } = null!;
}
