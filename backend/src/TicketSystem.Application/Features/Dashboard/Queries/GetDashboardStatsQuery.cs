using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Features.Dashboard.Queries;

public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetDashboardStatsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var tickets = _db.Tickets.AsNoTracking().VisibleTo(_currentUser);

        var stats = await tickets
            .GroupBy(_ => 1)
            .Select(g => new DashboardStatsDto
            {
                TotalTickets = g.Count(),
                OpenTickets = g.Count(t => t.Status == TicketStatus.Open),
                InProgressTickets = g.Count(t => t.Status == TicketStatus.InProgress),
                OnHoldTickets = g.Count(t => t.Status == TicketStatus.OnHold),
                ResolvedTickets = g.Count(t => t.Status == TicketStatus.Resolved),
                ClosedTickets = g.Count(t => t.Status == TicketStatus.Closed),
                HighPriorityTickets = g.Count(t => t.Priority == TicketPriority.High || t.Priority == TicketPriority.Critical),
                OverdueTickets = g.Count(t => t.DueDate != null && t.DueDate < today && t.Status != TicketStatus.Closed),
                UnassignedTickets = g.Count(t => t.AssignedToId == null)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return stats ?? new DashboardStatsDto();
    }
}
