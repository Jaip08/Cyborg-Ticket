using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Features.Dashboard.Queries;

public record GetPriorityBreakdownQuery : IRequest<List<PriorityCountDto>>;

public class GetPriorityBreakdownQueryHandler : IRequestHandler<GetPriorityBreakdownQuery, List<PriorityCountDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetPriorityBreakdownQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<List<PriorityCountDto>> Handle(GetPriorityBreakdownQuery request, CancellationToken cancellationToken)
    {
        var grouped = await _db.Tickets.AsNoTracking()
            .VisibleTo(_currentUser)
            .GroupBy(t => t.Priority)
            .Select(g => new { Priority = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var counts = grouped.ToDictionary(g => g.Priority, g => g.Count);

        return Enum.GetValues<TicketPriority>()
            .Select(p => new PriorityCountDto { Priority = p.ToString(), Count = counts.GetValueOrDefault(p) })
            .ToList();
    }
}
