using FluentValidation;
using MediatR;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Constants;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Features.Auth.Commands;

public record RegisterCommand(string FullName, string Email, string Password) : IRequest<AuthResponse>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;

    public RegisterCommandHandler(IUnitOfWork uow, IPasswordHasher hasher, IJwtTokenGenerator jwt)
    {
        _uow = uow;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _uow.Users.EmailExistsAsync(email, cancellationToken))
            throw new ConflictException("An account with this email already exists.");

        // Self-registered users always come in as Employees - promotions go through an admin.
        var roles = await _uow.Roles.ListAsync(cancellationToken);
        var employeeRole = roles.First(r => r.Name == Roles.Employee);

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = _hasher.Hash(request.Password),
            RoleId = employeeRole.Id,
            Role = employeeRole,
            IsActive = true
        };

        await _uow.Users.AddAsync(user, cancellationToken);
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
                Role = employeeRole.Name,
                IsActive = user.IsActive
            }
        };
    }
}
