using System.Security.Cryptography;
using System.Text;
using API_healthyMind.Data;
using API_healthyMind.Models;
using Microsoft.EntityFrameworkCore;

namespace API_healthyMind.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _db;
    private readonly int _refreshTokenDays;

    public RefreshTokenService(AppDbContext db, IConfiguration configuration)
    {
        _db = db;
        _refreshTokenDays = configuration.GetValue("Jwt:RefreshTokenDays", 14);
        if (_refreshTokenDays < 1) _refreshTokenDays = 14;
    }

    public async Task<string> CreateAsync(string userId, string role, CancellationToken cancellationToken = default)
    {
        var plain = GenerateSecureToken();
        var hash = Sha256Hex(plain);
        var now = DateTime.UtcNow;
        var row = new RefreshTokenRecord
        {
            UserId = userId,
            Role = role,
            TokenHash = hash,
            ExpiresAt = now.AddDays(_refreshTokenDays),
            CreatedAt = now
        };
        _db.RefreshTokenRecords.Add(row);
        await _db.SaveChangesAsync(cancellationToken);
        return plain;
    }

    public async Task<RefreshRotationResult?> ValidateAndRotateAsync(string refreshTokenPlain, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenPlain))
            return null;

        var hash = Sha256Hex(refreshTokenPlain.Trim());
        var existing = await _db.RefreshTokenRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.TokenHash == hash && x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow,
                cancellationToken);

        if (existing == null)
            return null;

        // Re-load tracked entity for update
        var tracked = await _db.RefreshTokenRecords.FindAsync(new object[] { existing.Id }, cancellationToken);
        if (tracked == null || tracked.RevokedAt != null)
            return null;

        var newPlain = GenerateSecureToken();
        var newHash = Sha256Hex(newPlain);
        var now = DateTime.UtcNow;

        var newRow = new RefreshTokenRecord
        {
            UserId = tracked.UserId,
            Role = tracked.Role,
            TokenHash = newHash,
            ExpiresAt = now.AddDays(_refreshTokenDays),
            CreatedAt = now
        };
        _db.RefreshTokenRecords.Add(newRow);
        await _db.SaveChangesAsync(cancellationToken);

        tracked.RevokedAt = now;
        tracked.ReplacedBy = newRow.Id;
        await _db.SaveChangesAsync(cancellationToken);

        return new RefreshRotationResult
        {
            UserId = tracked.UserId,
            Role = tracked.Role,
            NewRefreshTokenPlain = newPlain
        };
    }

    private static string GenerateSecureToken()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string Sha256Hex(string value)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
