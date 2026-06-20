namespace TicketSystem.Application.Features.Auth;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
    public bool IsActive { get; set; }
}

public class AuthResponse
{
    public string Token { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public UserDto User { get; set; } = default!;
}
