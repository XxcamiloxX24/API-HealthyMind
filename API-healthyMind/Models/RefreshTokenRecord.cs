using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_healthyMind.Models;

[Table("refresh_tokens")]
public class RefreshTokenRecord
{
    [Key]
    [Column("rt_id")]
    public int Id { get; set; }

    [Column("rt_user_id")]
    [MaxLength(64)]
    public string UserId { get; set; } = "";

    [Column("rt_role")]
    [MaxLength(32)]
    public string Role { get; set; } = "";

    [Column("rt_token_hash")]
    [MaxLength(64)]
    public string TokenHash { get; set; } = "";

    [Column("rt_expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("rt_created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("rt_revoked_at")]
    public DateTime? RevokedAt { get; set; }

    [Column("rt_replaced_by")]
    public int? ReplacedBy { get; set; }
}
