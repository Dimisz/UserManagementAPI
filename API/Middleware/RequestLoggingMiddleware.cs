public class RequestLoggingMiddleware
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
        var method = context.Request.Method;
        var path = context.Request.Path;

        // Capture the response status code after the request is processed
        var originalBody = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await _next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var statusCode = context.Response.StatusCode;

        _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode}", method, path, statusCode);

        // Copy the response back to the original stream
        await memoryStream.CopyToAsync(originalBody);
        context.Response.Body = originalBody;
    }
}
