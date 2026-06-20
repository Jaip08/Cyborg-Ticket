using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Common.Security;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Features.Tickets.Queries;

public record GetTicketsQuery(
    string? Search = null,
    TicketStatus? Status = null,
    TicketPriority? Priority = null,
    Guid? CategoryId = null,
    Guid? AssignedToId = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<TicketDto>>;

public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, PagedResult<TicketDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetTicketsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<TicketDto>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id ?? throw new UnauthorizedException();

        IQueryable<Ticket> query = _db.Tickets.AsNoTracking();

        // Non-staff only see tickets they raised or own.
        if (!TicketPolicy.IsStaff(_currentUser))
            query = query.Where(t => t.CreatedById == userId || t.AssignedToId == userId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(term) ||
                t.TicketNumber.ToLower().Contains(term) ||
                t.Description.ToLower().Contains(term));
        }

        if (request.Status is not null)
            query = query.Where(t => t.Status == request.Status);

        if (request.Priority is not null)
            query = query.Where(t => t.Priority == request.Priority);

        if (request.CategoryId is not null)
            query = query.Where(t => t.CategoryId == request.CategoryId);

        if (request.AssignedToId is not null)
            query = query.Where(t => t.AssignedToId == request.AssignedToId);

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var total = await query.CountAsync(cancellationToken);

        var rows = await TicketQueries
            .Project(query.OrderByDescending(t => t.CreatedAt))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = rows.Select(TicketQueries.Map).ToList();
        return new PagedResult<TicketDto>(items, total, page, pageSize);
    }
}
