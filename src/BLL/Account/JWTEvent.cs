using BoardGameWebService.Models.Log;
using Domain.JWTUser;
using Domain.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Services.Auth
{
    public class JWTEvent : JwtBearerEvents
    {
        private const string TOKEN_HEADER = "Authorization";
        private ILogger<JWTEvent> _logger;

        public JWTEvent(ILogger<JWTEvent> logger)
            : base()
        {
            _logger = logger;
        }

        // 如果要求處理期間擲回例外狀況，則叫用。 此事件之後會重新擲回例外狀況，除非受到抑制。
        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            Exception exception = context.Exception;
            AuthLogModel logData = new AuthLogModel
            {
                Message = exception.Message,
                IsAuth = false
            };
            if (context.Exception as UnauthorizedAccessException != null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }

            context.Fail(exception);
            _logger.Info(logData);

            return Task.CompletedTask;
        }

        public override Task MessageReceived(MessageReceivedContext context)
        {
            string token = context.Request.Headers[TOKEN_HEADER].ToString();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("沒有Token");
            }

            return Task.CompletedTask;
        }

        public override Task TokenValidated(TokenValidatedContext context)
        {
            try
            {
                UserClaimModel user = context.Principal.Parse();
                string userJson = JsonConvert.SerializeObject(user);
                context.Request.Headers[TOKEN_HEADER] = userJson;

                AuthLogModel logData = new AuthLogModel
                {
                    IsAuth = true,
                    User = user,
                    SecurityToken = context.SecurityToken
                };
                _logger.Info(logData);
            }
            catch (Exception e)
            {
                throw new UnauthorizedAccessException(e.Message);
            }

            return Task.CompletedTask;
        }
    }
}
