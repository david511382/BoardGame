using BoardGameAngular.Models.Auth;
using BoardGameAngular.Models.Config;
using HttpHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BoardGameAngular.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private string _backendURL;

        public AuthController(IOptions<BackendConfig> backendConfig)
        {
            _backendURL = backendConfig.Value.AuthURL;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<UserInfoWithID> Login([FromBody] LoginRequest request)
        {
            try
            {
                HttpHelper.Domain.Model.ResponseModel response = await HttpRequest.New()
                    .SetForm(request)
                    .To($"{_backendURL}/Login")
                    .Post();
                UserInfoWithID result = JsonConvert.DeserializeObject<UserInfoWithID>(response.Content);
                return result;
            }
            catch
            {
                throw new Exception();
            }
        }

        [HttpPost]
        [Route("Register")]
        public async Task<UserInfoWithID> Register([FromBody] UserInfo request)
        {
            try
            {
                HttpHelper.Domain.Model.ResponseModel response = await HttpRequest.New()
                    .SetJson(request)
                    .To($"{_backendURL}/Register")
                    .Post();
                UserInfoWithID result = JsonConvert.DeserializeObject<UserInfoWithID>(response.Content);
                return result;
            }
            catch
            {
                throw new Exception();
            }
        }

        [HttpPut("{id}")]
        public async Task<bool> Put([FromRoute]int id, [FromBody] UserInfo request)
        {
            try
            {
                HttpHelper.Domain.Model.ResponseModel response = await HttpRequest.New()
                   .SetJson(request)
                   .To($"{_backendURL}")
                   .Put();
                if (response.Content.Equals("True"))
                    return true;
            }
            catch
            {
            }

            return false;
        }
    }
}