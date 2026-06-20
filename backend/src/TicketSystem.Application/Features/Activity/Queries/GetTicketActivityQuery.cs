using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Common.Security;

namespace TicketSystem.Application.Features.Activity.Queries;

public record GetTicketActivityQuery(Guid TicketId) : IRequest<List<ActivityDto>>;

public class GetTicketActivityQueryHandler : IRequestHandler<GetTicketActivityQuery, List<ActivityDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetTicketActivityQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<List<ActivityDto>> Handle(GetTicketActivityQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _db.Tickets.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken)
            ?? throw new NotFoundException("Ticket", request.TicketId);

        if (!TicketPolicy.CanView(_currentUser, ticket))
            throw new ForbiddenAccessException("You don't have access to this ticket.");

        return await _db.ActivityLogs.AsNoTracking()
            .Where(a => a.TicketId == request.TicketId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new ActivityDto
            {
                Id = a.Id,
                Action = a.Action,
                Description = a.Description,
                CreatedAt = a.CreatedAt,
                User = new UserSummaryDto { Id = a.UserId, FullName = a.User.FullName }
            })
            .ToListAsync(cancellationToken);
    }
}
