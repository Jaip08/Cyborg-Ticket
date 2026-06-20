using TicketSystem.Application.Common.Models;

namespace TicketSystem.Application.Features.Activity;

public class ActivityDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public UserSummaryDto User { get; set; } = default!;
}
