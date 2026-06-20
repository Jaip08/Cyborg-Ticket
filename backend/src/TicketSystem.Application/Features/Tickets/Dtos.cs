using TicketSystem.Application.Common.Models;

namespace TicketSystem.Application.Features.Tickets;

public class TicketDto
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Priority { get; set; } = default!;

    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public bool IsOverdue { get; set; }
    public int CommentCount { get; set; }
    public int AttachmentCount { get; set; }

    public LookupDto Category { get; set; } = default!;
    public UserSummaryDto CreatedBy { get; set; } = default!;
    public UserSummaryDto? AssignedTo { get; set; }
}
