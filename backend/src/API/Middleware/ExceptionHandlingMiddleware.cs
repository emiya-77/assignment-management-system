using Microsoft.AspNetCore.Mvc;

namespace API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger
    )
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
        catch (KeyNotFoundException exception)
        {
            _logger.LogWarning(
                exception,
                "Resource not found: {Message}",
                exception.Message
            );

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status404NotFound,
                "Resource not found",
                exception.Message
            );
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogWarning(
                exception,
                "Invalid operation: {Message}",
                exception.Message
            );

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Invalid operation",
                exception.Message
            );
        }
        catch (UnauthorizedAccessException exception)
        {
            _logger.LogWarning(
                exception,
                "Unauthorized access: {Message}",
                exception.Message
            );

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status403Forbidden,
                "Forbidden",
                exception.Message
            );
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An unhandled exception occurred."
            );

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred",
                "An unexpected error occurred. Please try again later."
            );
        }
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail
    )
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}