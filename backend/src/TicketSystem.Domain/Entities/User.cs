using TicketSystem.Domain.Common;

namespace TicketSystem.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;

    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    public ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();
    public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
    public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
}
