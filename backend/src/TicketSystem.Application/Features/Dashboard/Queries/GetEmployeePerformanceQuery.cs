using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Features.Dashboard.Queries;

public record GetEmployeePerformanceQuery : IRequest<List<EmployeePerformanceDto>>;

public class GetEmployeePerformanceQueryHandler : IRequestHandler<GetEmployeePerformanceQuery, List<EmployeePerformanceDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetEmployeePerformanceQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<List<EmployeePerformanceDto>> Handle(GetEmployeePerformanceQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Tickets.AsNoTracking().Where(t => t.AssignedToId != null);

        // An employee only gets to see their own numbers.
        if (!TicketPolicy.IsStaff(_currentUser))
        {
            var userId = _currentUser.Id;
            query = query.Where(t => t.AssignedToId == userId);
        }

        return await query
            .GroupBy(t => new { t.AssignedToId, t.AssignedTo!.FullName })
            .Select(g => new EmployeePerformanceDto
            {
                UserId = g.Key.AssignedToId!.Value,
                FullName = g.Key.FullName,
                Assigned = g.Count(),
                Resolved = g.Count(t => t.Status == TicketStatus.Resolved || t.Status == TicketStatus.Closed),
                Open = g.Count(t => t.Status != TicketStatus.Resolved && t.Status != TicketStatus.Closed)
            })
            .OrderByDescending(x => x.Assigned)
            .ToListAsync(cancellationToken);
    }
}
