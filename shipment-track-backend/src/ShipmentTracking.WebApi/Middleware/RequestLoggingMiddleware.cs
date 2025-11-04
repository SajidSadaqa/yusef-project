using System.Diagnostics;
using System.Linq;

namespace ShipmentTracking.WebApi.Middleware;

/// <summary>
/// Emits structured logs for every HTTP request and response pair.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        await _next(context);
        stopwatch.Stop();

        var correlationId = context.Response.Headers[CorrelationIdMiddleware.HeaderName].FirstOrDefault()
            ?? context.TraceIdentifier;

        _logger.LogInformation(
            "HTTP {Method} {Path} responded {StatusCode} in {Elapsed} ms (CorrelationId: {CorrelationId})",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            correlationId);
    }
}
