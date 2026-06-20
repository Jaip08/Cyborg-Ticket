using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;

namespace TicketSystem.Application.Features.Tickets.Queries;

public record GetTicketByIdQuery(Guid Id) : IRequest<TicketDto>;

public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetTicketByIdQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TicketDto> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id ?? throw new UnauthorizedException();

        var row = await TicketQueries
            .Project(_db.Tickets.AsNoTracking().Where(t => t.Id == request.Id))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Ticket", request.Id);

        if (!TicketPolicy.IsStaff(_currentUser) && row.CreatedById != userId && row.AssignedToId != userId)
            throw new ForbiddenAccessException("You don't have access to this ticket.");

        return TicketQueries.Map(row);
    }
}
