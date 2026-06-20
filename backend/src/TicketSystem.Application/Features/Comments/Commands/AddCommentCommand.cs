using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Features.Comments.Commands;

public record AddCommentCommand(Guid TicketId, string Content, bool IsInternal) : IRequest<CommentDto>;

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(4000);
    }
}

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, CommentDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public AddCommentCommandHandler(IUnitOfWork uow, IApplicationDbContext db, ICurrentUser currentUser)
    {
        _uow = uow;
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<CommentDto> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id ?? throw new UnauthorizedException();

        var ticket = await _db.Tickets.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken)
            ?? throw new NotFoundException("Ticket", request.TicketId);

        if (!TicketPolicy.CanView(_currentUser, ticket))
            throw new ForbiddenAccessException("You don't have access to this ticket.");

        if (request.IsInternal && !TicketPolicy.CanSeeInternalNotes(_currentUser, ticket))
            throw new ForbiddenAccessException("Only staff working the ticket can post internal notes.");

        var comment = new TicketComment
        {
            TicketId = ticket.Id,
            AuthorId = userId,
            Content = request.Content.Trim(),
            IsInternal = request.IsInternal
        };

        await _uow.Comments.AddAsync(comment, cancellationToken);
        await _uow.Activities.AddAsync(new ActivityLog
        {
            TicketId = ticket.Id,
            UserId = userId,
            Action = "Commented",
            Description = request.IsInternal ? "Added an internal note." : "Added a comment."
        }, cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);

        var author = await _db.Users.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new { u.FullName, Role = u.Role.Name })
            .FirstAsync(cancellationToken);

        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            IsInternal = comment.IsInternal,
            CreatedAt = comment.CreatedAt,
            Author = new CommentAuthorDto { Id = userId, FullName = author.FullName, Role = author.Role }
        };
    }
}
