using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Common.Interfaces;

public interface IUnitOfWork
{
    ITicketRepository Tickets { get; }
    IUserRepository Users { get; }
    IRepository<Category> Categories { get; }
    IRepository<TicketComment> Comments { get; }
    IRepository<TicketAttachment> Attachments { get; }
    IRepository<ActivityLog> Activities { get; }
    IRepository<Role> Roles { get; }
    IRepository<PasswordResetToken> PasswordResetTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
