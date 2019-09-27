using Convert;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
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

        public Auth(string authDomain)
        {
            AUTH_URL = $"{authDomain}/api/Auth";
        }

        public override async Task MessageReceived(MessageReceivedContext context)
        {
            string errMsg = "no token";
            string token = context.Request.Headers[TOKEN_HEADER].ToString();
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    HttpHelper.Domain.Model.ResponseModel resp = await HttpHelper.HttpRequest.New()
                        .AddHeader(new KeyValuePair<string, string>(TOKEN_HEADER, token))
                        .To(AUTH_URL)
                        .Get();

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
                    context.Success();
                    return;
                }
                catch (Exception)
                {
                    errMsg = "Unauthorized Fail";
                }
            }

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = errMsg;
            context.Fail(new Exception(errMsg));
        }
    }
}
