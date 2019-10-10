using Convert;
using Domain.JWTUser;
using Domain.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using OcelotApiGateway.Models.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OcelotApiGateway
{
    public class Auth : JwtBearerEvents
    {
        private const string TOKEN_HEADER = "Authorization";
        private readonly string AUTH_URL;
        private ILogger<Auth> _logger;

        public Auth(string authDomain, ILogger<Auth> logger)
        {
            _logger = logger;
            AUTH_URL = $"{authDomain}/api/Auth";
        }

        public override async Task MessageReceived(MessageReceivedContext context)
        {
            AuthLogModel logData = new AuthLogModel();
            Exception exception;
            string token = context.Request.Headers[TOKEN_HEADER].ToString();
            LogLevel logLevel = LogLevel.Information;
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    HttpHelper.Domain.Model.ResponseModel resp = await HttpHelper.HttpRequest.New()
                        .AddHeader(new KeyValuePair<string, string>(TOKEN_HEADER, token))
                        .To(AUTH_URL, true)
                        .Get();

                    if (resp.StatusCode == HttpStatusCode.Unauthorized)
                        throw new UnauthorizedAccessException();
                    else if (resp.StatusCode != HttpStatusCode.OK)
                        throw new Exception($"訪問auth伺服器返回{resp.StatusCode.ToString()}");

                    byte[] respBytes = resp.Content
                           .Replace(@"""", "")
                           .HexToBytes();

                    using (MemoryStream memStream = new MemoryStream(respBytes))
                    {
                        using (BinaryReader reader = new BinaryReader(memStream, Encoding.UTF8))
                        {
                            context.Principal = new System.Security.Claims.ClaimsPrincipal(reader);
                        }
                    }

                    logData.User = context.Principal.Parse();
                    logData.IsAuth = true;
                    _logger.Info(logData);

                    context.Success();
                    return;
                }
                catch (UnauthorizedAccessException)
                {
                    exception = new Exception("Unauthorized");
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                    logData.Message = "認證失敗";
                }
                catch (Exception e)
                {
                    exception = e;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    logData.Message = e.Message;
                    logLevel = LogLevel.Error;
                }
            }
            else
            {
                exception = new Exception("Unauthorized");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                logData.Message = "沒有Token";
            }

            _logger.Log(logLevel, logData);

            context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = exception.Message;
            context.Fail(exception);
        }
    }
}
