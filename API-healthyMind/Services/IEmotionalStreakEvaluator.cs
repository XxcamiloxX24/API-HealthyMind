namespace API_healthyMind.Services;

public interface IEmotionalStreakEvaluator
{
    /// <summary>
    /// Evalua si las ultimas 3 fechas con registro emocional del aprendiz cumplen
    /// el umbral de riesgo. Si es asi, crea alerta idempotente y notifica al psicologo.
    /// No lanza excepciones; registra advertencias en el logger.
    /// </summary>
    Task EvaluarYNotificarAsync(int aprendizId, CancellationToken ct = default);
}
