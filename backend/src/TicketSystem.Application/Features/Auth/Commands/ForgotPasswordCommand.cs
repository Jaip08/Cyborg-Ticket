using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Features.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest<ForgotPasswordResult>;

public record ForgotPasswordResult(string Message, string? ResetToken);

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResult>
{
    private const string GenericMessage = "If an account exists for that email, a reset link has been sent.";

    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;

    public ForgotPasswordCommandHandler(IApplicationDbContext db, IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<ForgotPasswordResult> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null)
            return new ForgotPasswordResult(GenericMessage, null);

        var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        _db.PasswordResetTokens.Add(new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = Hash(rawToken),
            ExpiresAt = _clock.UtcNow.AddHours(1)
        });

        await _db.SaveChangesAsync(cancellationToken);

        // In a real deployment we'd email the link; for this demo we hand the token back so the flow is testable.
        return new ForgotPasswordResult(GenericMessage, rawToken);
    }

    private static string Hash(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
