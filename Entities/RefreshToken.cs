namespace HrManagement.Api.Entities;

// Backs AuthSessionDto - lets you revoke sessions instead of trusting a JWT forever.
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty; // store a hash, never the raw token
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}
