namespace TicketSystem.Application.Common.Models;

public class LookupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}

public class UserSummaryDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
}
