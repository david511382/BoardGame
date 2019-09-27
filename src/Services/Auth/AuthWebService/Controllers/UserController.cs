using AuthWebService.Models;
using AuthWebService.Sevices;
using Domain.ApiResponse;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AuthWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IJWTService _jwtService { get; }
        private IAuthService _service;
        private IResponseService _responseService;

        public UserController(IAuthService authService, IJWTService jwtService, IResponseService responseService)
        {
            _service = authService;
            _jwtService = jwtService;
            _responseService = responseService;
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="username">帳號</param>
        /// <param name="password">密碼</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(LoginResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(LoginResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
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
        [AllowAnonymous]
        [Route("Register")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BoolResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BoolResponseModel), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserInfo info)
        {
            return await _responseService.Init<BoolResponseModel>(this)
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BoolResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BoolResponseModel), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Put([FromBody] UserInfo info)
        {
            return await _responseService.Init<BoolResponseModel>(this)
                .ValidateToken((user) => { })
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
