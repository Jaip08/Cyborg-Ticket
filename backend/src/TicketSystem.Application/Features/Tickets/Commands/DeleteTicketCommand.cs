using MediatR;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;

namespace TicketSystem.Application.Features.Tickets.Commands;

public record DeleteTicketCommand(Guid Id) : IRequest;

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public DeleteTicketCommandHandler(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _uow.Tickets.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Ticket", request.Id);

        if (!TicketPolicy.CanDelete(_currentUser, ticket))
            throw new ForbiddenAccessException("You are not allowed to delete this ticket.");

        _uow.Tickets.Remove(ticket);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
