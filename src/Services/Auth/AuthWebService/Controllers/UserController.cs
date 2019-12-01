using AuthWebService.Models;
using AuthWebService.Sevices;
using Domain.Api.Interfaces;
using Domain.Api.Models.Response;
using Domain.Api.Models.Response.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AuthWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IJWTService _jwtService { get; }
        private readonly IAuthService _service;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

        public UserController(IAuthService authService, IJWTService jwtService, IResponseService responseService, ILogger<UserController> logger)
        {
            _service = authService;
            _jwtService = jwtService;
            _responseService = responseService;
            _logger = logger;
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="username">帳號</param>
        /// <param name="password">密碼</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        [Produces("application/json")]
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
                .Do<LoginResponse>(async (result, user) =>
                {
                    UserInfoWithID userInfo = await _service.LoginPlayer(new UserIdentity
                    {
                        Username = username,
                        Password = password
                    });
                    string token = _jwtService.NewToken(
                        userInfo.Id,
                        userInfo.Username,
                        userInfo.Name,
                        DateTime.UtcNow.AddDays(1));

                    result.Token = token;
                    result.Name = userInfo.Name;

                    return result;
                });
        }

        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserInfo info)
        {
            return await _responseService.Init<BoolResponseModel>(this, _logger)
                .ValidateRequest(() =>
                {
                    if (string.IsNullOrWhiteSpace(info.Username) || string.IsNullOrWhiteSpace(info.Password))
                        throw new Exception("帳號或密碼不得為空");
                })
                .Do<BoolResponseModel>(async (result, user) =>
                {
                    result.IsSuccess = await _service.RegisterPlayer(info);

                    return result;
                });
        }

        /// <summary>
        /// 修改會員資料
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] UserInfo info)
        {
            return await _responseService.Init<BoolResponseModel>(this, _logger)
                //.ValidateToken((user) => {
                //    string.IsNullOrEmpty(user.ValidAudience); })
                .ValidateRequest(() =>
                {
                    if (string.IsNullOrWhiteSpace(info.Username) || string.IsNullOrWhiteSpace(info.Password))
                        throw new Exception("帳號或密碼不得為空");
                })
                .Do<BoolResponseModel>(async (result, user) =>
                {
                    result.IsSuccess = await _service.UpdatePlayerInfo(user.Id, info);

                    return result;
                });
        }
    }
}
