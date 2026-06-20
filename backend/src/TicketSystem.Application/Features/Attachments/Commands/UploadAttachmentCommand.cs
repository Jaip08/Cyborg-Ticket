using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Features.Attachments.Commands;

public record UploadAttachmentCommand(
    Guid TicketId,
    Stream Content,
    string FileName,
    string ContentType,
    long Length) : IRequest<AttachmentDto>;

public class UploadAttachmentCommandValidator : AbstractValidator<UploadAttachmentCommand>
{
    private const long MaxBytes = 10 * 1024 * 1024;

    public UploadAttachmentCommandValidator()
    {
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Length)
            .GreaterThan(0).WithMessage("The file is empty.")
            .LessThanOrEqualTo(MaxBytes).WithMessage("Attachments cannot be larger than 10 MB.");
    }
}

public class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, AttachmentDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IApplicationDbContext _db;
    private readonly IFileStorage _storage;
    private readonly ICurrentUser _currentUser;

    public UploadAttachmentCommandHandler(
        IUnitOfWork uow, IApplicationDbContext db, IFileStorage storage, ICurrentUser currentUser)
    {
        _uow = uow;
        _db = db;
        _storage = storage;
        _currentUser = currentUser;
    }

    public async Task<AttachmentDto> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id ?? throw new UnauthorizedException();

        var ticket = await _db.Tickets.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken)
            ?? throw new NotFoundException("Ticket", request.TicketId);

        if (!TicketPolicy.CanView(_currentUser, ticket))
            throw new ForbiddenAccessException("You don't have access to this ticket.");

        var stored = await _storage.SaveAsync(request.Content, request.FileName, cancellationToken);

        var attachment = new TicketAttachment
        {
            TicketId = ticket.Id,
            FileName = request.FileName,
            StoredName = stored.StoredName,
            ContentType = string.IsNullOrWhiteSpace(request.ContentType) ? "application/octet-stream" : request.ContentType,
            FileSize = stored.Size,
            UploadedById = userId
        };

        await _uow.Attachments.AddAsync(attachment, cancellationToken);
        await _uow.Activities.AddAsync(new ActivityLog
        {
            TicketId = ticket.Id,
            UserId = userId,
            Action = "AttachmentAdded",
            Description = $"Attached \"{request.FileName}\"."
        }, cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);

        return new AttachmentDto
        {
            Id = attachment.Id,
            FileName = attachment.FileName,
            ContentType = attachment.ContentType,
            FileSize = attachment.FileSize,
            CreatedAt = attachment.CreatedAt
        };
    }
}
