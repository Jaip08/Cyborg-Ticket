using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;

namespace TicketSystem.Application.Features.Dashboard.Queries;

public record GetMonthlyAnalyticsQuery(int Months = 6) : IRequest<List<MonthlyPointDto>>;

public class GetMonthlyAnalyticsQueryHandler : IRequestHandler<GetMonthlyAnalyticsQuery, List<MonthlyPointDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetMonthlyAnalyticsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<List<MonthlyPointDto>> Handle(GetMonthlyAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var months = request.Months is < 1 or > 24 ? 6 : request.Months;
        var now = DateTime.UtcNow;
        var start = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-(months - 1));

        var tickets = _db.Tickets.AsNoTracking().VisibleTo(_currentUser);

        var created = await tickets
            .Where(t => t.CreatedAt >= start)
            .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var resolved = await tickets
            .Where(t => t.ResolvedAt != null && t.ResolvedAt >= start)
            .GroupBy(t => new { t.ResolvedAt!.Value.Year, t.ResolvedAt!.Value.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var points = new List<MonthlyPointDto>();
        for (var i = 0; i < months; i++)
        {
            var bucket = start.AddMonths(i);
            points.Add(new MonthlyPointDto
            {
                Month = bucket.ToString("yyyy-MM"),
                Created = created.FirstOrDefault(c => c.Year == bucket.Year && c.Month == bucket.Month)?.Count ?? 0,
                Resolved = resolved.FirstOrDefault(r => r.Year == bucket.Year && r.Month == bucket.Month)?.Count ?? 0
            });
        }

        return points;
    }
}
