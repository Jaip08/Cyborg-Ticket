namespace TicketSystem.Application.Features.Attachments;

public class AttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
}
