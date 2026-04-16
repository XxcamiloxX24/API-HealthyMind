using System.Text.Json.Serialization;

namespace API_healthyMind.Models;

public partial class AlertaRachaEmocional
{
    public int AreCodigo { get; set; }

    public int AreAprendizFk { get; set; }

    public int ArePsicologoFk { get; set; }

    public int? AreSeguimientoFk { get; set; }

    public DateOnly AreFechaReciente { get; set; }

    public string AreRegla { get; set; } = "AMBAS";

    /// <summary>JSON array con las 3 fechas ISO de la racha.</summary>
    public string AreFechasJson { get; set; } = "[]";

    /// <summary>JSON array con los promedios de escala de cada dia.</summary>
    public string? AreEscalasJson { get; set; }

    public string? AreMensaje { get; set; }

    public string AreEstado { get; set; } = "nueva";

    public DateTime AreFechaCreacion { get; set; }

    public DateTime? AreFechaLectura { get; set; }

    public DateTime? AreFechaResolucion { get; set; }

    public string? AreNotasResolucion { get; set; }

    public string AreEstadoRegistro { get; set; } = "activo";

    [JsonIgnore]
    public virtual Aprendiz? AreAprendizFkNavigation { get; set; }

    [JsonIgnore]
    public virtual Psicologo? ArePsicologoFkNavigation { get; set; }

    [JsonIgnore]
    public virtual SeguimientoAprendiz? AreSeguimientoFkNavigation { get; set; }
}
