using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Common.Exceptions;
using ValidationException = TicketSystem.Application.Common.Exceptions.ValidationException;

namespace TicketSystem.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await WriteProblemAsync(context, ex);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception exception)
    {
        var (status, title) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "One or more validation errors occurred."),
            BadRequestException => (StatusCodes.Status400BadRequest, "Bad request."),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized."),
            ForbiddenAccessException => (StatusCodes.Status403Forbidden, "Forbidden."),
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found."),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        if (status == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
        else
            _logger.LogWarning("{Title} on {Path}: {Message}", title, context.Request.Path, exception.Message);

        var friendlyMessage = status == StatusCodes.Status500InternalServerError
            ? "Please try again later."
            : exception.Message;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Type = $"https://httpstatuses.io/{status}",
            Detail = friendlyMessage
        };

        // Surface a plain message the client can show directly.
        problem.Extensions["message"] = friendlyMessage;

        if (exception is ValidationException validation)
            problem.Extensions["errors"] = validation.Errors;

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
    }
}
