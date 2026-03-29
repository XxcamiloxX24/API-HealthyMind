namespace API_healthyMind.Models.DTO;

public class ResponderTestDTO
{
    public List<RespuestaItemDTO> Respuestas { get; set; } = new();
}

public class RespuestaItemDTO
{
    public int PreguntaId { get; set; }
    public int OpcionId { get; set; }
}
