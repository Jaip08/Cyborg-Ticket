using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Features.Auth;

namespace TicketSystem.Application.Features.Users.Queries;

public record GetUsersQuery : IRequest<List<UserDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IApplicationDbContext _db;

    public GetUsersQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _db.Users.AsNoTracking()
            .OrderBy(u => u.FullName)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role.Name,
                IsActive = u.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}
