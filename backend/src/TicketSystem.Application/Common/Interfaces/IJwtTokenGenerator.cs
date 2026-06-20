using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    TokenResult Generate(User user);
}

public record TokenResult(string Token, DateTime ExpiresAtUtc);
