using TicketSystem.Domain.Common;

namespace TicketSystem.Domain.Entities;

public class ActivityLog : BaseEntity
{
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = default!;

    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public string Action { get; set; } = default!;
    public string Description { get; set; } = default!;
}
