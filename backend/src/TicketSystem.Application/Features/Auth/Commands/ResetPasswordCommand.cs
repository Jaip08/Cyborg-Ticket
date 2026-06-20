using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Models;

namespace TicketSystem.Application.Features.Auth.Commands;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<MessageResponse>;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6).MaximumLength(100);
    }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, MessageResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IDateTimeProvider _clock;

    public ResetPasswordCommandHandler(IApplicationDbContext db, IPasswordHasher hasher, IDateTimeProvider clock)
    {
        _db = db;
        _hasher = hasher;
        _clock = clock;
    }

    public async Task<MessageResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var tokenHash = Hash(request.Token);

        var resetToken = await _db.PasswordResetTokens
            .Include(t => t.User)
            .Where(t => t.User.Email == email && t.TokenHash == tokenHash && t.UsedAt == null)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (resetToken is null || resetToken.ExpiresAt <= _clock.UtcNow)
            throw new BadRequestException("This reset link is invalid or has expired.");

        resetToken.User.PasswordHash = _hasher.Hash(request.NewPassword);
        resetToken.UsedAt = _clock.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new MessageResponse("Your password has been reset. You can sign in now.");
    }

    private static string Hash(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
