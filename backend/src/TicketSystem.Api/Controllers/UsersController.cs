using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Features.Auth;
using TicketSystem.Application.Features.Users.Commands;
using TicketSystem.Application.Features.Users.Queries;
using TicketSystem.Domain.Constants;

namespace TicketSystem.Api.Controllers;

[Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
public class UsersController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
        => Ok(await Mediator.Send(new GetUsersQuery()));

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:guid}/role")]
    public async Task<ActionResult<UserDto>> ChangeRole(Guid id, ChangeUserRoleRequest request)
        => Ok(await Mediator.Send(new ChangeUserRoleCommand(id, request.Role)));
}

public record ChangeUserRoleRequest(string Role);
