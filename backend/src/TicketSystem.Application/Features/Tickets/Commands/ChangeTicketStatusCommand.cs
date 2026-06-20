using FluentValidation;
using MediatR;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Features.Tickets.Commands;

public record ChangeTicketStatusCommand(Guid TicketId, TicketStatus Status) : IRequest<TicketDto>;

public class ChangeTicketStatusCommandValidator : AbstractValidator<ChangeTicketStatusCommand>
{
    public ChangeTicketStatusCommandValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class ChangeTicketStatusCommandHandler : IRequestHandler<ChangeTicketStatusCommand, TicketDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeProvider _clock;

    public ChangeTicketStatusCommandHandler(
        IUnitOfWork uow, IApplicationDbContext db, ICurrentUser currentUser, IDateTimeProvider clock)
    {
        _uow = uow;
        _db = db;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<TicketDto> Handle(ChangeTicketStatusCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id ?? throw new UnauthorizedException();

        var ticket = await _uow.Tickets.GetByIdAsync(request.TicketId, cancellationToken)
            ?? throw new NotFoundException("Ticket", request.TicketId);

        if (!TicketPolicy.CanChangeStatus(_currentUser, ticket))
            throw new ForbiddenAccessException("You are not allowed to change this ticket's status.");

        var previous = ticket.Status;
        if (previous == request.Status)
            return (await TicketQueries.GetDtoAsync(_db, ticket.Id, cancellationToken))!;

        var wasFinished = previous is TicketStatus.Resolved or TicketStatus.Closed;
        ticket.Status = request.Status;

        switch (request.Status)
        {
            case TicketStatus.Resolved:
                ticket.ResolvedAt = _clock.UtcNow;
                ticket.ClosedAt = null;
                break;
            case TicketStatus.Closed:
                ticket.ResolvedAt ??= _clock.UtcNow;
                ticket.ClosedAt = _clock.UtcNow;
                break;
            default:
                ticket.ResolvedAt = null;
                ticket.ClosedAt = null;
                break;
        }

        var reopened = wasFinished && request.Status is not (TicketStatus.Resolved or TicketStatus.Closed);

        await _uow.Activities.AddAsync(new ActivityLog
        {
            TicketId = ticket.Id,
            UserId = userId,
            Action = reopened ? "Reopened" : "StatusChanged",
            Description = $"Status changed from {previous} to {request.Status}."
        }, cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);

        return (await TicketQueries.GetDtoAsync(_db, ticket.Id, cancellationToken))!;
    }
}
