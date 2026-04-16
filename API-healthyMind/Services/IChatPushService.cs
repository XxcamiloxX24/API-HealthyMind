namespace API_healthyMind.Services;

/// <summary>
/// Envía notificaciones en tiempo real al servicio de chat (Socket.IO → sala Psicologo_{id}).
/// </summary>
public interface IChatPushService
{
    /// <summary>
    /// No lanza si la URL o el secreto no están configurados; registra advertencias ante fallos HTTP.
    /// </summary>
    Task NotifyPsychologistAsync(
        int psychologistId,
        string notificationType,
        string title,
        string message,
        int? appointmentId = null,
        int? seguimientoId = null,
        string? deepLink = null,
        CancellationToken cancellationToken = default);
}
