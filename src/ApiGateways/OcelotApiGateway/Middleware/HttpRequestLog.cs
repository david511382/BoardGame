using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
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

            logger.Log(LogLevel.Information,
             default,
             new HttpRequestEvent()
             .AddProp("ClientIP", context.Connection.RemoteIpAddress.ToString())
             .AddProp("Method", context.Request.Method)
             .AddProp("UrlPath", context.Request.Path)
             .AddProp("StatusCode", context.Response.StatusCode)
             .AddProp("ResponseTime", _stopwatch.ElapsedMilliseconds),
             null,
             HttpRequestEvent.Formatter);
        }
    }

    class HttpRequestEvent : IEnumerable<KeyValuePair<string, object>>
    {
        List<KeyValuePair<string, object>> _properties = new List<KeyValuePair<string, object>>();

        public string Message { get; }

        public HttpRequestEvent(string message = "")
        {
            Message = message;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public HttpRequestEvent AddProp(string name, object value)
        {
            _properties.Add(new KeyValuePair<string, object>(name, value));
            return this;
        }

        public static Func<HttpRequestEvent, Exception, string> Formatter { get; } = (l, e) => l.Message;
    }
}
