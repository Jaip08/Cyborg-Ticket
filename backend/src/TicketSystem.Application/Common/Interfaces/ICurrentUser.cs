namespace TicketSystem.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid? Id { get; }
    string? Email { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}
