using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
