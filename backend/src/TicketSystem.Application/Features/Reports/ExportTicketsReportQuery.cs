using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Security;

namespace TicketSystem.Application.Features.Reports;

public record ExportTicketsReportQuery(ExportFormat Format) : IRequest<ExportFile>;

public class TicketExportRow
{
    public string TicketNumber { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Priority { get; set; } = default!;
    public string Category { get; set; } = default!;
    public string CreatedBy { get; set; } = default!;
    public string AssignedTo { get; set; } = "Unassigned";
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class ExportTicketsReportQueryHandler : IRequestHandler<ExportTicketsReportQuery, ExportFile>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IReportExporter _exporter;

    public ExportTicketsReportQueryHandler(IApplicationDbContext db, ICurrentUser currentUser, IReportExporter exporter)
    {
        _db = db;
        _currentUser = currentUser;
        _exporter = exporter;
    }

    public async Task<ExportFile> Handle(ExportTicketsReportQuery request, CancellationToken cancellationToken)
    {
        var raw = await _db.Tickets.AsNoTracking()
            .VisibleTo(_currentUser)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new
            {
                t.TicketNumber,
                t.Title,
                t.Status,
                t.Priority,
                Category = t.Category.Name,
                CreatedBy = t.CreatedBy.FullName,
                AssignedTo = t.AssignedTo != null ? t.AssignedTo.FullName : null,
                t.CreatedAt,
                t.DueDate,
                t.ResolvedAt
            })
            .ToListAsync(cancellationToken);

        var rows = raw.Select(t => new TicketExportRow
        {
            TicketNumber = t.TicketNumber,
            Title = t.Title,
            Status = t.Status.ToString(),
            Priority = t.Priority.ToString(),
            Category = t.Category,
            CreatedBy = t.CreatedBy,
            AssignedTo = t.AssignedTo ?? "Unassigned",
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate,
            ResolvedAt = t.ResolvedAt
        }).ToList();

        return _exporter.Write(rows, request.Format, "tickets-report");
    }
}
