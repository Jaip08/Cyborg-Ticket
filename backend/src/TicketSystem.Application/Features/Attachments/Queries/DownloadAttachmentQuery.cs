using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;

namespace TicketSystem.Application.Features.Attachments.Queries;

public record DownloadAttachmentQuery(Guid AttachmentId) : IRequest<AttachmentFile>;

public record AttachmentFile(string FileName, string ContentType, Stream Content);

public class DownloadAttachmentQueryHandler : IRequestHandler<DownloadAttachmentQuery, AttachmentFile>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorage _storage;
    private readonly ICurrentUser _currentUser;

    public DownloadAttachmentQueryHandler(IApplicationDbContext db, IFileStorage storage, ICurrentUser currentUser)
    {
        _db = db;
        _storage = storage;
        _currentUser = currentUser;
    }

    public async Task<AttachmentFile> Handle(DownloadAttachmentQuery request, CancellationToken cancellationToken)
    {
        var attachment = await _db.TicketAttachments.AsNoTracking()
            .Include(a => a.Ticket)
            .FirstOrDefaultAsync(a => a.Id == request.AttachmentId, cancellationToken)
            ?? throw new NotFoundException("Attachment", request.AttachmentId);

        if (!TicketPolicy.CanView(_currentUser, attachment.Ticket))
            throw new ForbiddenAccessException("You don't have access to this attachment.");

        var stream = await _storage.OpenAsync(attachment.StoredName, cancellationToken)
            ?? throw new NotFoundException("The file is no longer available.");

        return new AttachmentFile(attachment.FileName, attachment.ContentType, stream);
    }
}
