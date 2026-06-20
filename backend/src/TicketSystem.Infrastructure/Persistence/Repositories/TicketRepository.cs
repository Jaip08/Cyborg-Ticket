using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Infrastructure.Persistence.Repositories;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Ticket?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await Set
            .Include(t => t.Category)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
}
