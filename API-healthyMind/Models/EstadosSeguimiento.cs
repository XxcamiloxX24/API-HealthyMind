namespace API_healthyMind.Models;

/// <summary>
/// Estados para clasificar un seguimiento del aprendiz.
/// Campo: seg_estado_seguimiento en tabla seguimiento_aprendiz
/// </summary>
public static class EstadosSeguimiento
{
    public const string Critico = "Criticos";
    public const string EnObservacion = "En Observacion";
    public const string Estable = "Estables";

    public static readonly string[] Todos = { Critico, EnObservacion, Estable };

    public static bool EsValido(string? estado)
    {
        if (string.IsNullOrWhiteSpace(estado)) return false;
        return Todos.Contains(estado.Trim(), StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Normaliza al valor exacto usado en BD.</summary>
    public static string? Normalizar(string? estado)
    {
        if (string.IsNullOrWhiteSpace(estado)) return null;
        var t = estado.Trim();
        return Todos.FirstOrDefault(x => x.Equals(t, StringComparison.OrdinalIgnoreCase));
    }
}
