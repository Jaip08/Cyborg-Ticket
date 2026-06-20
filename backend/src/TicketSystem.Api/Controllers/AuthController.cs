using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Features.Auth;
using TicketSystem.Application.Features.Auth.Commands;
using TicketSystem.Application.Features.Auth.Queries;

namespace TicketSystem.Api.Controllers;

public class AuthController : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterCommand command)
        => Ok(await Mediator.Send(command));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginCommand command)
        => Ok(await Mediator.Send(command));

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command)
        => Ok(await Mediator.Send(command));

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
        => Ok(await Mediator.Send(command));

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
        => Ok(await Mediator.Send(new GetCurrentUserQuery()));
}
