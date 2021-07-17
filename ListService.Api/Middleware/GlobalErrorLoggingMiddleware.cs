namespace ListService.Api.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using RockLib.Logging;

    public class GlobalErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public GlobalErrorLoggingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        private async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var logEntry =
                    new LogEntry(
                        $"Response from List Service Api resulting in a {context.Response.StatusCode} status code", ex,
                        LogLevel.Fatal)
                    {
                        CorrelationId = context.TraceIdentifier
                    };

                _logger.Log(logEntry);

                throw;
            }
        }
    }
}
