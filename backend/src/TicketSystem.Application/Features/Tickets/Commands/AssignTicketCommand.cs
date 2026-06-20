using MediatR;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Features.Tickets.Commands;

public record AssignTicketCommand(Guid TicketId, Guid AssigneeId) : IRequest<TicketDto>;

public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, TicketDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public AssignTicketCommandHandler(IUnitOfWork uow, IApplicationDbContext db, ICurrentUser currentUser)
    {
        _uow = uow;
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TicketDto> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id ?? throw new UnauthorizedException();

        if (!TicketPolicy.CanAssign(_currentUser))
            throw new ForbiddenAccessException("Only managers and admins can assign tickets.");

        var ticket = await _uow.Tickets.GetByIdAsync(request.TicketId, cancellationToken)
            ?? throw new NotFoundException("Ticket", request.TicketId);

        var assignee = await _uow.Users.GetByIdAsync(request.AssigneeId, cancellationToken)
            ?? throw new NotFoundException("User", request.AssigneeId);
        if (!assignee.IsActive)
            throw new BadRequestException("Cannot assign a ticket to a deactivated user.");

        var isReassignment = ticket.AssignedToId is not null && ticket.AssignedToId != assignee.Id;
        ticket.AssignedToId = assignee.Id;

        await _uow.Activities.AddAsync(new ActivityLog
        {
            TicketId = ticket.Id,
            UserId = userId,
            Action = isReassignment ? "Reassigned" : "Assigned",
            Description = $"{(isReassignment ? "Reassigned" : "Assigned")} to {assignee.FullName}."
        }, cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);

        return (await TicketQueries.GetDtoAsync(_db, ticket.Id, cancellationToken))!;
    }
}
