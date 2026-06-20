using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Application.Features.Categories.Queries;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly IApplicationDbContext _db;

    public GetCategoriesQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _db.Categories.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description })
            .ToListAsync(cancellationToken);
    }
}
