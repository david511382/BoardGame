using Domain.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OcelotApiGateway.Models.Log;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OcelotApiGateway.Middleware
{
    public class HttpRequestLog
    {
        private readonly RequestDelegate _next;
        private Stopwatch _stopwatch;
        public HttpRequestLog(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<HttpRequestLog> logger)
        {
            _stopwatch = Stopwatch.StartNew();

            await _next(context);

            _stopwatch.Stop();

            HttpLogModel logData = new HttpLogModel(context, (int)_stopwatch.ElapsedMilliseconds);
            logger.Info(logData);
        }
    }
}
