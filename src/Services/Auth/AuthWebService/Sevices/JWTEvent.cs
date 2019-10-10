using AuthWebService.Models.Log;
using Domain.JWTUser;
using Domain.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
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
            context.NoResult();

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = context.Exception.Message;

            AuthLogModel logData = new AuthLogModel
            {
                Message = context.Exception.Message,
                IsAuth = false
            };
            _logger.Info(logData);

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
