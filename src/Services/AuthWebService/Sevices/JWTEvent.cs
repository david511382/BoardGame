using AuthWebService.Models.Log;
using Domain.JWTUser;
using Domain.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace AuthWebService.Sevices
{
    class JWTEvent : JwtBearerEvents
    {
        private ILogger<JWTEvent> _logger;

        public JWTEvent(ILogger<JWTEvent> logger)
        {
            _logger = logger;
        }

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            string failMsg = context.Exception.Message;
            AuthLogModel logData = new AuthLogModel
            {
                Message = failMsg,
                IsAuth = false
            };
            _logger.Info(logData);

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Fail(failMsg);

            return Task.CompletedTask;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            AuthLogModel logData = new AuthLogModel
            {
                IsAuth = true,
                User = context.Principal.Parse(),
                SecurityToken = context.SecurityToken
            };
            _logger.Info(logData);
        }
    }
}
