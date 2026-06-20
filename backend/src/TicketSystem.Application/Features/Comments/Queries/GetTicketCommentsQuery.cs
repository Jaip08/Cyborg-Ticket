using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;

namespace TicketSystem.Application.Features.Comments.Queries;

public record GetTicketCommentsQuery(Guid TicketId) : IRequest<List<CommentDto>>;

public class GetTicketCommentsQueryHandler : IRequestHandler<GetTicketCommentsQuery, List<CommentDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetTicketCommentsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<List<CommentDto>> Handle(GetTicketCommentsQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _db.Tickets.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken)
            ?? throw new NotFoundException("Ticket", request.TicketId);

        if (!TicketPolicy.CanView(_currentUser, ticket))
            throw new ForbiddenAccessException("You don't have access to this ticket.");

        var canSeeInternal = TicketPolicy.CanSeeInternalNotes(_currentUser, ticket);

        var query = _db.TicketComments.AsNoTracking().Where(c => c.TicketId == request.TicketId);
        if (!canSeeInternal)
            query = query.Where(c => !c.IsInternal);

        return await query
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                IsInternal = c.IsInternal,
                CreatedAt = c.CreatedAt,
                Author = new CommentAuthorDto
                {
                    Id = c.AuthorId,
                    FullName = c.Author.FullName,
                    Role = c.Author.Role.Name
                }
            })
            .ToListAsync(cancellationToken);
    }
}
