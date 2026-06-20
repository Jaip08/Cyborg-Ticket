using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Infrastructure.Persistence;

// Used by `dotnet ef` at design time. The runtime wires the context up through DI instead.
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=ticketsystem;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new ApplicationDbContext(options, new DesignTimeClock());
    }

    private sealed class DesignTimeClock : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
