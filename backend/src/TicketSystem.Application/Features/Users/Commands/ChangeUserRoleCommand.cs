using FluentValidation;
using MediatR;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Features.Auth;
using TicketSystem.Domain.Constants;

namespace TicketSystem.Application.Features.Users.Commands;

public record ChangeUserRoleCommand(Guid UserId, string Role) : IRequest<UserDto>;

public class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
{
    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => Roles.All.Contains(r))
            .WithMessage($"Role must be one of: {string.Join(", ", Roles.All)}.");
    }
}

public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand, UserDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public ChangeUserRoleCommandHandler(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<UserDto> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == _currentUser.Id)
            throw new BadRequestException("You cannot change your own role.");

        var user = await _uow.Users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        var roles = await _uow.Roles.ListAsync(cancellationToken);
        var role = roles.First(r => r.Name == request.Role);

        user.RoleId = role.Id;
        await _uow.SaveChangesAsync(cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = role.Name,
            IsActive = user.IsActive
        };
    }
}
