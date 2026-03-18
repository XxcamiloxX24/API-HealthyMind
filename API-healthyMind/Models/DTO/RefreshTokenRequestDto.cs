using System.Text.Json.Serialization;

namespace API_healthyMind.Models.DTO;

public class RefreshTokenRequestDto
{
    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; set; }
}
