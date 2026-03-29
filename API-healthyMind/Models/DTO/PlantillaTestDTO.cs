namespace API_healthyMind.Models.DTO;

public class PlantillaTestDTO
{
    public string PlaTstNombre { get; set; } = null!;
    public string? PlaTstDescripcion { get; set; }
    public List<PlantillaPreguntaDTO>? Preguntas { get; set; }
}

public class PlantillaPreguntaDTO
{
    public string PlaPrgTexto { get; set; } = null!;
    /// <summary>si_no, verdadero_falso, opcion_multiple, escala</summary>
    public string PlaPrgTipo { get; set; } = "opcion_multiple";
    public int PlaPrgOrden { get; set; }
    public List<PlantillaOpcionDTO>? Opciones { get; set; }
}

public class PlantillaOpcionDTO
{
    public string PlaOpcTexto { get; set; } = null!;
    public int PlaOpcOrden { get; set; }
}
