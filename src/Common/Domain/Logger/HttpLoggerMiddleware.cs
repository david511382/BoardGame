using Domain.Logger.Models.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Domain.Logger
{
    public class HttpLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private Stopwatch _stopwatch;
        public HttpLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<HttpLoggerMiddleware> logger)
        {
            _stopwatch = Stopwatch.StartNew();

            await _next(context);

            _stopwatch.Stop();

            HttpLogModel logData = new HttpLogModel(context, (int)_stopwatch.ElapsedMilliseconds);
            logger.Info(logData);
        }
    }
}
