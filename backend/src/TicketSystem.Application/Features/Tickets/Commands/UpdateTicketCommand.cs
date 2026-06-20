using FluentValidation;
using MediatR;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Features.Tickets.Commands;

public record UpdateTicketCommand(
    Guid Id,
    string Title,
    string Description,
    TicketPriority Priority,
    Guid CategoryId,
    DateTime? DueDate) : IRequest<TicketDto>;

public class UpdateTicketCommandValidator : AbstractValidator<UpdateTicketCommand>
{
    public UpdateTicketCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, TicketDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public UpdateTicketCommandHandler(IUnitOfWork uow, IApplicationDbContext db, ICurrentUser currentUser)
    {
        _uow = uow;
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TicketDto> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id ?? throw new UnauthorizedException();

        var ticket = await _uow.Tickets.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Ticket", request.Id);

        if (!TicketPolicy.CanEdit(_currentUser, ticket))
            throw new ForbiddenAccessException("You can only edit tickets you raised or that are assigned to you.");

        if (ticket.CategoryId != request.CategoryId)
        {
            _ = await _uow.Categories.GetByIdAsync(request.CategoryId, cancellationToken)
                ?? throw new NotFoundException("Category", request.CategoryId);
        }

        var priorityChanged = ticket.Priority != request.Priority;

        ticket.Title = request.Title.Trim();
        ticket.Description = request.Description.Trim();
        ticket.Priority = request.Priority;
        ticket.CategoryId = request.CategoryId;
        ticket.DueDate = request.DueDate;

        await _uow.Activities.AddAsync(new ActivityLog
        {
            TicketId = ticket.Id,
            UserId = userId,
            Action = "Updated",
            Description = priorityChanged
                ? $"Updated ticket details (priority set to {request.Priority})."
                : "Updated ticket details."
        }, cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);

        return (await TicketQueries.GetDtoAsync(_db, ticket.Id, cancellationToken))!;
    }
}
