using System.Net;
using System.Net.Http.Headers;

namespace HttpHelper.Domain.Model
{
    public struct ResponseModel
    {
        public string Content;
        public HttpResponseHeaders Header;
        public CookieCollection Cookies;
    }

}
