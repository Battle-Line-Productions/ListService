namespace ListService.Api.Middleware
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    [ExcludeFromCodeCoverage]
    public class CorrelationTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CorrelationIdOptions _options;

        public CorrelationTokenMiddleware(RequestDelegate next, CorrelationIdOptions options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));

            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(_options.Header, out StringValues correlationId))
            {
                context.TraceIdentifier = correlationId;
            }

            if (_options.IncludeInResponse)
            {
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add(_options.Header, new[] {context.TraceIdentifier});
                    return Task.CompletedTask;
                });
            }

            await _next(context);
        }

        public class CorrelationIdOptions
        {
            private const string DefaultHeader = "X-Correlation-Id";

            public string Header { get; set; } = DefaultHeader;

            public bool IncludeInResponse { get; set; } = true;
        }
    }
}
