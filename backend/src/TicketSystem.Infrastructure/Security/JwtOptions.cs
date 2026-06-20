namespace TicketSystem.Infrastructure.Security;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "TicketSystem";
    public string Audience { get; set; } = "TicketSystemClient";
    public string Key { get; set; } = default!;
    public int ExpiryMinutes { get; set; } = 120;
}
