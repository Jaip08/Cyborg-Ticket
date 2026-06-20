using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Tickets = new TicketRepository(context);
        Users = new UserRepository(context);
        Categories = new Repository<Category>(context);
        Comments = new Repository<TicketComment>(context);
        Attachments = new Repository<TicketAttachment>(context);
        Activities = new Repository<ActivityLog>(context);
        Roles = new Repository<Role>(context);
        PasswordResetTokens = new Repository<PasswordResetToken>(context);
    }

    public ITicketRepository Tickets { get; }
    public IUserRepository Users { get; }
    public IRepository<Category> Categories { get; }
    public IRepository<TicketComment> Comments { get; }
    public IRepository<TicketAttachment> Attachments { get; }
    public IRepository<ActivityLog> Activities { get; }
    public IRepository<Role> Roles { get; }
    public IRepository<PasswordResetToken> PasswordResetTokens { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
