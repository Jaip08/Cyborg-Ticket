using TicketSystem.Domain.Common;

namespace TicketSystem.Domain.Entities;

public class TicketAttachment : BaseEntity
{
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = default!;

    public string FileName { get; set; } = default!;
    public string StoredName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; }

    public Guid UploadedById { get; set; }
    public User UploadedBy { get; set; } = default!;
}
