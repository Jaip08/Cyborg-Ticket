using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Common;

namespace TicketSystem.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<T> Set;

    public Repository(ApplicationDbContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await Set.FindAsync(new object[] { id }, cancellationToken);

    public async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
        => await Set.ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await Set.AddAsync(entity, cancellationToken);

    public void Update(T entity) => Set.Update(entity);

    public void Remove(T entity) => Set.Remove(entity);
}
