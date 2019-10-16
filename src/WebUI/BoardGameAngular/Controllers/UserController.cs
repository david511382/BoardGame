using BoardGameAngular.Models.Auth;
using BoardGameAngular.Services.Config;
using Domain.Api.Interfaces;
using Domain.Api.Models.Response.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoardGameAngular.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ConfigService _urlConfig;
        private readonly IResponseService _responseService;

        public UserController(ConfigService config, IResponseService responseService)
        {
            _urlConfig = config;
            _responseService = responseService;
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            return await _responseService.Init<LoginResponse>(this)
                  .ValidateRequest(() =>
                  {
                      if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                          throw new Exception("帳號或密碼不得為空");
                  })
                  .Do<LoginResponse>(async (result, user) =>
                  {
                      result = await HttpHelper.HttpRequest.New()
                          .SetForm(new Dictionary<string, string>
                          {
                                {"username",username },
                                {"password",password }
                          })
                          .To(_urlConfig.UserLogin)
                          .Post<LoginResponse>();

                      return result;
                  });
        }

        [HttpPost("[action]")]
        public async Task<UserInfoWithID> Register([FromBody] UserInfo request)
        {
            try
            {
                UserInfoWithID result = await HttpHelper.HttpRequest.New()
                    .SetJson(request)
                    .To(_urlConfig.UserRegister)
                    .Post<UserInfoWithID>();
                return result;
            }
            catch
            {
                throw new Exception();
            }
        }

        [HttpPut("[action]")]
        public async Task<bool> Update([FromRoute]int id, [FromBody] UserInfo request)
        {
            try
            {
                HttpHelper.Domain.Model.ResponseModel response = await HttpHelper.HttpRequest.New()
                   .SetJson(request)
                   .To(_urlConfig.UserUpdate)
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