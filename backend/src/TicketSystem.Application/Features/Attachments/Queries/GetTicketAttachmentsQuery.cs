using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;

namespace TicketSystem.Application.Features.Attachments.Queries;

public record GetTicketAttachmentsQuery(Guid TicketId) : IRequest<List<AttachmentDto>>;

public class GetTicketAttachmentsQueryHandler : IRequestHandler<GetTicketAttachmentsQuery, List<AttachmentDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetTicketAttachmentsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<List<AttachmentDto>> Handle(GetTicketAttachmentsQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _db.Tickets.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken)
            ?? throw new NotFoundException("Ticket", request.TicketId);

        if (!TicketPolicy.CanView(_currentUser, ticket))
            throw new ForbiddenAccessException("You don't have access to this ticket.");

        return await _db.TicketAttachments.AsNoTracking()
            .Where(a => a.TicketId == request.TicketId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                ContentType = a.ContentType,
                FileSize = a.FileSize,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
