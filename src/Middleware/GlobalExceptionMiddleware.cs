using System.Net;
using System.Text.Json;

namespace BookManager.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");

            var statusCode = exception switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,
                InvalidOperationException => HttpStatusCode.BadRequest,
                ArgumentException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            var payload = new
            {
                type = $"https://httpstatuses.com/{(int)statusCode}",
                title = statusCode switch
                {
                    HttpStatusCode.NotFound => "Not Found",
                    HttpStatusCode.BadRequest => "Bad Request",
                    _ => "Internal Server Error"
                },
                status = (int)statusCode,
                detail = exception is KeyNotFoundException
                    ? exception.Message
                    : statusCode == HttpStatusCode.InternalServerError
                        ? "An unexpected error occurred while processing the request."
                        : exception.Message,
                instance = context.Request.Path
            };

            context.Response.Clear();
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}