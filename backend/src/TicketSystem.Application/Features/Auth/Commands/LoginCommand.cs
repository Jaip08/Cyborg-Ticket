using FluentValidation;
using MediatR;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IDateTimeProvider _clock;

    public LoginCommandHandler(IUnitOfWork uow, IPasswordHasher hasher, IJwtTokenGenerator jwt, IDateTimeProvider clock)
    {
        _uow = uow;
        _hasher = hasher;
        _jwt = jwt;
        _clock = clock;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _uow.Users.GetByEmailAsync(email, cancellationToken);

        // Same response whether the email is unknown or the password is wrong.
        if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedException("This account has been deactivated. Contact an administrator.");

        user.LastLoginAt = _clock.UtcNow;
        await _uow.SaveChangesAsync(cancellationToken);

        var token = _jwt.Generate(user);

        return new AuthResponse
        {
            Token = token.Token,
            ExpiresAtUtc = token.ExpiresAtUtc,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.Name,
                IsActive = user.IsActive
            }
        };
    }
}
