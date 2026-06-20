using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Models;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Features.Tickets;

// Flat row pulled from the database; enums stay as enums here and become strings when mapped to the DTO.
internal record TicketRow(
    Guid Id,
    string TicketNumber,
    string Title,
    string Description,
    TicketStatus Status,
    TicketPriority Priority,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? ResolvedAt,
    DateTime? ClosedAt,
    Guid CategoryId,
    string CategoryName,
    Guid CreatedById,
    string CreatedByName,
    Guid? AssignedToId,
    string? AssignedToName,
    int CommentCount,
    int AttachmentCount);

internal static class TicketQueries
{
    public static IQueryable<TicketRow> Project(IQueryable<Domain.Entities.Ticket> source) =>
        source.Select(t => new TicketRow(
            t.Id, t.TicketNumber, t.Title, t.Description, t.Status, t.Priority,
            t.DueDate, t.CreatedAt, t.UpdatedAt, t.ResolvedAt, t.ClosedAt,
            t.CategoryId, t.Category.Name,
            t.CreatedById, t.CreatedBy.FullName,
            t.AssignedToId, t.AssignedTo != null ? t.AssignedTo.FullName : null,
            t.Comments.Count, t.Attachments.Count));

    public static TicketDto Map(TicketRow r) => new()
    {
        Id = r.Id,
        TicketNumber = r.TicketNumber,
        Title = r.Title,
        Description = r.Description,
        Status = r.Status.ToString(),
        Priority = r.Priority.ToString(),
        DueDate = r.DueDate,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
        ResolvedAt = r.ResolvedAt,
        ClosedAt = r.ClosedAt,
        IsOverdue = r.DueDate.HasValue && r.DueDate.Value.Date < DateTime.UtcNow.Date && r.Status != TicketStatus.Closed,
        CommentCount = r.CommentCount,
        AttachmentCount = r.AttachmentCount,
        Category = new LookupDto { Id = r.CategoryId, Name = r.CategoryName },
        CreatedBy = new UserSummaryDto { Id = r.CreatedById, FullName = r.CreatedByName },
        AssignedTo = r.AssignedToId is null
            ? null
            : new UserSummaryDto { Id = r.AssignedToId.Value, FullName = r.AssignedToName! }
    };

    public static async Task<TicketDto?> GetDtoAsync(IApplicationDbContext db, Guid id, CancellationToken ct)
    {
        var row = await Project(db.Tickets.AsNoTracking().Where(t => t.Id == id)).FirstOrDefaultAsync(ct);
        return row is null ? null : Map(row);
    }
}
