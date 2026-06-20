using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Infrastructure.Persistence;

namespace TicketSystem.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var provider = scope.ServiceProvider;
        var logger = provider.GetRequiredService<ILogger<Program>>();

        try
        {
            var db = provider.GetRequiredService<ApplicationDbContext>();

            if (db.Database.GetMigrations().Any())
                await db.Database.MigrateAsync();
            else
                await db.Database.EnsureCreatedAsync();

            var hasher = provider.GetRequiredService<IPasswordHasher>();
            await ApplicationDbContextSeed.SeedAsync(db, hasher);

            logger.LogInformation("Database is ready.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database initialization failed.");
            throw;
        }
    }
}
