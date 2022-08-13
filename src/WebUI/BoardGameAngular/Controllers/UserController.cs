using BoardGameAngular.Models.Auth;
using BoardGameAngular.Services.Config;
using Domain.Api.Interfaces;
using Domain.Api.Models.Response;
using Domain.Api.Models.Response.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BoardGameAngular.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ConfigService _urlConfig;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

        public UserController(ConfigService config, IResponseService responseService, ILogger<UserController> logger)
        {
            _urlConfig = config;
            _responseService = responseService;
            _logger = logger;
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            return await _responseService.Init<LoginResponse>(this, _logger)
                  .ValidateRequest(() =>
                  {
                      if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                          throw new Exception("帳號或密碼不得為空");
                  })
                  .Do<LoginResponse>(async (result, user, logger) =>
                  {
                      Util.Http.ResponseModel response = await login(username, password);
                      if (response.StatusCode != HttpStatusCode.OK)
                          throw new Exception(response.Content);

                      foreach (Cookie cookie in response.Cookies)
                          Response.Cookies.Append(
                              cookie.Name,
                              cookie.Value,
                              new CookieOptions() { Expires = cookie.Expires }
                          );
                      result = JsonConvert.DeserializeObject<LoginResponse>(response.Content);

                      return result;
                  });
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAndLogin([FromBody] UserInfo userInfo)
        {
            return await _responseService.Init<LoginResponse>(this, _logger)
                  .ValidateRequest(() =>
                  {
                      if (string.IsNullOrWhiteSpace(userInfo.Name) || string.IsNullOrWhiteSpace(userInfo.Username) || string.IsNullOrWhiteSpace(userInfo.Password))
                          throw new Exception("資料不得為空");
                  })
                  .Do<LoginResponse>(async (result, user, logger) =>
                  {
                      // register
                      BoolResponseModel registerResponse = await Util.Http.HttpRequest.New()
                        .SetJson(userInfo)
                        .To(_urlConfig.UserRegister)
                        .Post<BoolResponseModel>();
                      if (registerResponse.IsError)
                      {
                          result.Error(registerResponse.ErrorMessage);
                          return result;
                      }
                      else if (!registerResponse.IsSuccess)
                      {
                          result.Error(registerResponse.Message);
                          return result;
                      }

                      // login
                      Util.Http.ResponseModel response = await login(userInfo.Username, userInfo.Password);
                      if (response.StatusCode != HttpStatusCode.OK)
                          throw new Exception(response.Content);

                      foreach (Cookie cookie in response.Cookies)
                          Response.Cookies.Append(
                              cookie.Name,
                              cookie.Value,
                              new CookieOptions() { Expires = cookie.Expires }
                          );
                      result = JsonConvert.DeserializeObject<LoginResponse>(response.Content);

                      return result;
                  });
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> Update([FromBody] UserInfo request)
        {
            return await _responseService.Init<BoolResponseModel>(this, _logger)
                 .ValidateRequest(() =>
                 {
                     if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Username))
                         throw new Exception("參數不得為空");
                 })
                 .Do<BoolResponseModel>(async (result, user) =>
                 {
                     BoolResponseModel response = await Util.Http.HttpRequest.New()
                          .SetJson(request)
                          .AddHeader(new KeyValuePair<string, string>("Authorization", $"Bearer {Request.Cookies["token"]}"))
                          .To(_urlConfig.UserUpdate)
                          .Put<BoolResponseModel>();

                     result.Message = response.Message;

                     if (response.IsError)
                     {
                         result.Error(response.ErrorMessage);
                         return result;
                     }

                     return result;
                 });
        }

        private async Task<Util.Http.ResponseModel> login(string username, string password)
        {
            return await Util.Http.HttpRequest.New()
                .SetForm(new Dictionary<string, string>
                {
                    { "username",username },
                    { "password",password }
                })
                .To(_urlConfig.UserLogin)
                .Send(HttpMethod.Post);
        }
    }
}