using TicketSystem.Domain.Common;

namespace TicketSystem.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
