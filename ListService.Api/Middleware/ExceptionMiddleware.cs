namespace ListService.Api.Middleware
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using ApiModels;
    using Microsoft.AspNetCore.Http;
    using RockLib.Logging;

    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public ExceptionMiddleware(ILogger logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
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

                var errorDetails = new ErrorDetails
                    {Message = ex.Message, StatusCode = (int) HttpStatusCode.InternalServerError};

                await HandleExceptionAsync(context, errorDetails);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, ErrorDetails details)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = details.StatusCode;

            return context.Response.WriteAsync(details.ToString());
        }
    }
}
