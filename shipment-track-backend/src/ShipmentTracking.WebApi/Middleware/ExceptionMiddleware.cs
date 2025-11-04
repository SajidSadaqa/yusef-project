namespace ShipmentTracking.WebApi.Middleware;

/// <summary>
/// Logs unhandled exceptions while letting the ProblemDetails middleware produce the response payload.
/// </summary>
public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            var correlationId = context.Response.Headers[CorrelationIdMiddleware.HeaderName].FirstOrDefault()
                ?? context.TraceIdentifier;
            _logger.LogError(ex, "Unhandled exception for request {Path} with correlation id {CorrelationId}", context.Request.Path, correlationId);
            throw;
        }
    }
}
