using TicketSystem.Domain.Common;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Domain.Entities;

public class Ticket : BaseEntity
{
    public string TicketNumber { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;

    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;

    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = default!;

    public Guid? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }

    public DateTime? DueDate { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    public ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();
    public ICollection<ActivityLog> Activities { get; set; } = new List<ActivityLog>();

    public bool IsClosed => Status is TicketStatus.Closed;
    public bool IsOverdue => DueDate.HasValue && DueDate.Value.Date < DateTime.UtcNow.Date && !IsClosed;
}
