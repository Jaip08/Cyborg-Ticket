using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Features.Dashboard.Queries;

public record GetStatusBreakdownQuery : IRequest<List<StatusCountDto>>;

public class GetStatusBreakdownQueryHandler : IRequestHandler<GetStatusBreakdownQuery, List<StatusCountDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetStatusBreakdownQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<List<StatusCountDto>> Handle(GetStatusBreakdownQuery request, CancellationToken cancellationToken)
    {
        var grouped = await _db.Tickets.AsNoTracking()
            .VisibleTo(_currentUser)
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var counts = grouped.ToDictionary(g => g.Status, g => g.Count);

        // Include every status so the chart legend stays stable even at zero.
        return Enum.GetValues<TicketStatus>()
            .Select(s => new StatusCountDto { Status = s.ToString(), Count = counts.GetValueOrDefault(s) })
            .ToList();
    }
}
