using TicketSystem.Domain.Common;

namespace TicketSystem.Domain.Entities;

public class TicketComment : BaseEntity
{
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = default!;

    public Guid AuthorId { get; set; }
    public User Author { get; set; } = default!;

    public string Content { get; set; } = default!;

    // Internal notes are only visible to staff working the ticket, not the requester.
    public bool IsInternal { get; set; }
}
