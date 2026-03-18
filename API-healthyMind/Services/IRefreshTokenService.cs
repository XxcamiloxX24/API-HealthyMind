namespace API_healthyMind.Services;

public sealed class RefreshRotationResult
{
    public string UserId { get; init; } = "";
    public string Role { get; init; } = "";
    public string NewRefreshTokenPlain { get; init; } = "";
}

public interface IRefreshTokenService
{
    Task<string> CreateAsync(string userId, string role, CancellationToken cancellationToken = default);
    Task<RefreshRotationResult?> ValidateAndRotateAsync(string refreshTokenPlain, CancellationToken cancellationToken = default);
}
