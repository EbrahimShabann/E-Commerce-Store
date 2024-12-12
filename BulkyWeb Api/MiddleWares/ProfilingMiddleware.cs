using System.Diagnostics;

namespace BulkyWeb_Api.MiddleWares
{
    public class ProfilingMiddleware
    {
        private readonly ILogger<ProfilingMiddleware> _logger;
        private readonly RequestDelegate _next;
        public ProfilingMiddleware(RequestDelegate next , ILogger<ProfilingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await _next(context);
            stopwatch.Stop();
            _logger.LogInformation($"Request '{context.Request.Path}' took {stopwatch.ElapsedMilliseconds}ms");

        }
    }
}
