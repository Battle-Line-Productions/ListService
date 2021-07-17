namespace ListService.Api.Middleware
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    [ExcludeFromCodeCoverage]
    public class CorrelationTokenMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Headers.TryGetValue("X-Correlation-Id", out StringValues correlationId))
            {
                context.TraceIdentifier = correlationId;
            }

            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add("X-Correlation-Id", new[] { context.TraceIdentifier });
                return Task.CompletedTask;
            });

            await next(context);
        }
    }
}
