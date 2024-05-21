using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ApiApplication.Middleware
{
    public class RequestTimeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimeMiddleware> _logger;

        public RequestTimeMiddleware(RequestDelegate next, ILogger<RequestTimeMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();
            var requestTime = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation($"Request {context.Request.Path} took {requestTime} ms");
        }

    }
}
