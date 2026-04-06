using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace API_healthyMind.Services;

public class ChatPushService : IChatPushService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<ChatPushService> _logger;

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
        var secret = (_config["Chat:InternalNotifySecret"] ?? Environment.GetEnvironmentVariable("CHAT_INTERNAL_NOTIFY_SECRET") ?? "").Trim();
        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(secret))
        {
            _logger.LogWarning(
                "Chat push omitido: configura Chat:NotifyBaseUrl y Chat:InternalNotifySecret (o variable CHAT_INTERNAL_NOTIFY_SECRET).");
            return;
        }

        var url = $"{baseUrl}/api/chat/internal/notify";
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.TryAddWithoutValidation("X-Internal-Secret", secret);
            req.Content = JsonContent.Create(new NotifyBody
            {
                PsychologistId = psychologistId,
                Type = notificationType,
                Title = title,
                Message = message,
                AppointmentId = appointmentId
            });

            var resp = await _http.SendAsync(req, cancellationToken);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Chat push falló: POST {Url} → {Status} {Body}. Revisa CHAT_INTERNAL_NOTIFY_SECRET en Render y Chat:InternalNotifySecret en la API.",
                    url,
                    (int)resp.StatusCode,
                    body);
            }
            else
            {
                _logger.LogInformation(
                    "Chat push OK: psicólogo {PsiId} notificado vía {Url}",
                    psychologistId,
                    url);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Chat push excepción al notificar psicólogo {PsiId}", psychologistId);
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
