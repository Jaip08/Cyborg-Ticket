using TicketSystem.Domain.Common;

namespace TicketSystem.Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public string TokenHash { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }

    public bool IsValid => UsedAt is null && ExpiresAt > DateTime.UtcNow;
}
