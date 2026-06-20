using FluentValidation;
using MediatR;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Features.Tickets.Commands;

public record CreateTicketCommand(
    string Title,
    string Description,
    TicketPriority Priority,
    Guid CategoryId,
    Guid? AssignedToId,
    DateTime? DueDate) : IRequest<TicketDto>;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, TicketDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public CreateTicketCommandHandler(IUnitOfWork uow, IApplicationDbContext db, ICurrentUser currentUser)
    {
        _uow = uow;
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id ?? throw new UnauthorizedException();

        var category = await _uow.Categories.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category", request.CategoryId);

        Guid? assigneeId = null;
        if (request.AssignedToId is { } requestedAssignee)
        {
            if (!TicketPolicy.CanAssign(_currentUser))
                throw new ForbiddenAccessException("Only managers and admins can assign tickets.");

            var assignee = await _uow.Users.GetByIdAsync(requestedAssignee, cancellationToken)
                ?? throw new NotFoundException("User", requestedAssignee);
            if (!assignee.IsActive)
                throw new BadRequestException("Cannot assign a ticket to a deactivated user.");

            assigneeId = assignee.Id;
        }

        var ticket = new Ticket
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Priority = request.Priority,
            Status = TicketStatus.Open,
            CategoryId = category.Id,
            CreatedById = userId,
            AssignedToId = assigneeId,
            DueDate = request.DueDate
        };

        ticket.Activities.Add(new ActivityLog
        {
            UserId = userId,
            Action = "Created",
            Description = $"Opened ticket at {request.Priority} priority."
        });

        if (assigneeId is not null)
        {
            ticket.Activities.Add(new ActivityLog
            {
                UserId = userId,
                Action = "Assigned",
                Description = "Assigned on creation."
            });
        }

        await _uow.Tickets.AddAsync(ticket, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return (await TicketQueries.GetDtoAsync(_db, ticket.Id, cancellationToken))!;
    }
}
