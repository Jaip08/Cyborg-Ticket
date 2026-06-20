using TicketSystem.Domain.Common;

namespace TicketSystem.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
