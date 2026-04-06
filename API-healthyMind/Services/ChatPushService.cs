using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace API_healthyMind.Services;

public class ChatPushService : IChatPushService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<ChatPushService> _logger;
    private const int MaxRetries = 1;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(3);

    public ChatPushService(HttpClient http, IConfiguration config, ILogger<ChatPushService> logger)
    {
        _http = http;
        _config = config;
        _logger = logger;
    }

    public async Task NotifyPsychologistAsync(
        int psychologistId,
        string notificationType,
        string title,
        string message,
        int? appointmentId = null,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = (_config["Chat:NotifyBaseUrl"] ?? "").Trim().TrimEnd('/');
        var secret = (_config["Chat:InternalNotifySecret"]
                      ?? Environment.GetEnvironmentVariable("CHAT_INTERNAL_NOTIFY_SECRET")
                      ?? "").Trim();

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(secret))
        {
            _logger.LogWarning(
                "Chat push omitido: configura Chat:NotifyBaseUrl y Chat:InternalNotifySecret (o variable CHAT_INTERNAL_NOTIFY_SECRET).");
            return;
        }

        var url = $"{baseUrl}/api/chat/internal/notify";
        var body = new NotifyBody
        {
            PsychologistId = psychologistId,
            Type = notificationType,
            Title = title,
            Message = message,
            AppointmentId = appointmentId
        };

        for (int attempt = 0; attempt <= MaxRetries; attempt++)
        {
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Post, url);
                req.Headers.TryAddWithoutValidation("X-Internal-Secret", secret);
                req.Content = JsonContent.Create(body);

                var resp = await _http.SendAsync(req, cancellationToken);

                if (resp.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        "Chat push OK (intento {Attempt}): psicólogo {PsiId} notificado vía {Url}",
                        attempt + 1, psychologistId, url);
                    return;
                }

                var respBody = await resp.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Chat push falló (intento {Attempt}): POST {Url} → {Status} {Body}",
                    attempt + 1, url, (int)resp.StatusCode, respBody);

                if ((int)resp.StatusCode == 401)
                {
                    _logger.LogError(
                        "Chat push 401: CHAT_INTERNAL_NOTIFY_SECRET en Render NO coincide con Chat:InternalNotifySecret de la API. No se reintentará.");
                    return;
                }
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(
                    "Chat push timeout (intento {Attempt}): {Url} no respondió a tiempo. Posible cold start de Render.",
                    attempt + 1, url);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Chat push excepción (intento {Attempt}) al notificar psicólogo {PsiId}",
                    attempt + 1, psychologistId);
            }

            if (attempt < MaxRetries)
            {
                _logger.LogInformation("Chat push: reintentando en {Delay}s...", RetryDelay.TotalSeconds);
                await Task.Delay(RetryDelay, cancellationToken);
            }
        }
    }

    private sealed class NotifyBody
    {
        [JsonPropertyName("psychologistId")]
        public int PsychologistId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("title")]
        public string Title { get; set; } = "";

        [JsonPropertyName("message")]
        public string Message { get; set; } = "";

        [JsonPropertyName("appointmentId")]
        public int? AppointmentId { get; set; }
    }
}
