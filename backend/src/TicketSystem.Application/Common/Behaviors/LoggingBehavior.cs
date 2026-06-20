using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUser _currentUser;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, ICurrentUser currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var name = typeof(TRequest).Name;
        var watch = Stopwatch.StartNew();

        try
        {
            return await next();
        }
        finally
        {
            watch.Stop();
            var elapsed = watch.ElapsedMilliseconds;

            if (elapsed > 500)
                _logger.LogWarning("{Request} handled in {Elapsed} ms (user {User}) - slow", name, elapsed, _currentUser.Id);
            else
                _logger.LogInformation("{Request} handled in {Elapsed} ms (user {User})", name, elapsed, _currentUser.Id);
        }
    }
}
