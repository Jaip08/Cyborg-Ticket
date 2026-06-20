using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Common.Security;

public static class TicketVisibility
{
    // Staff get the whole board; everyone else only sees what they raised or own.
    public static IQueryable<Ticket> VisibleTo(this IQueryable<Ticket> query, ICurrentUser user)
    {
        if (TicketPolicy.IsStaff(user))
            return query;

        var userId = user.Id;
        return query.Where(t => t.CreatedById == userId || t.AssignedToId == userId);
    }
}
