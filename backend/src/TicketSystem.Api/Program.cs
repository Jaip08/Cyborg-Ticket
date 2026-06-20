using Serilog;
using TicketSystem.Api;
using TicketSystem.Api.Extensions;
using TicketSystem.Api.Middleware;
using TicketSystem.Application;
using TicketSystem.Infrastructure;

// Npgsql is strict about DateTime kinds by default; this keeps demo/seed dates painless.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Hosts like Render hand us the port to listen on via $PORT.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
    builder.WebHost.UseUrls($"http://+:{port}");

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebApi(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticket System API v1"));

app.UseCors(TicketSystem.Api.DependencyInjection.CorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

await app.InitializeDatabaseAsync();

app.Run();
