using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Ticket> Tickets { get; }
    DbSet<TicketComment> TicketComments { get; }
    DbSet<TicketAttachment> TicketAttachments { get; }
    DbSet<Category> Categories { get; }
    DbSet<ActivityLog> ActivityLogs { get; }
    DbSet<PasswordResetToken> PasswordResetTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
