using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Util;

namespace Domain.Logger.Models.Log
{
    public class HttpLogModel : StructLoggerEvent
    {
        public string ClientIP { get; set; }
        public string Method { get; set; }
        public string Url { get; set; }
        public int StatusCode { get; set; }
        public int ResponseTime { get; set; }
        public string Request { get; set; }

        public HttpLogModel()
        {
            ResponseTime = -1;
            StatusCode = -1;
            ClientIP = "";
            Method = "";
            Url = "";
            Request = "";
        }

        public HttpLogModel(HttpContext context, int responseTime)
        {
            ClientIP = context.Connection.RemoteIpAddress.ToString();
            ResponseTime = responseTime;
            StatusCode = context.Response.StatusCode;
            Method = context.Request.Method;
            Url = context.Request.Path;
            Request = (context.Request.HasFormContentType) ?
                "Form:" + JsonConvert.SerializeObject(context.Request.Form) :
                "Body:" + context.Request.Body.StreamToString();
        }
    }
}
