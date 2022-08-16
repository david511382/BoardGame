using AuthWebService.Models;
using AuthWebService.Sevices;
using Domain.Api.Interfaces;
using Domain.Api.Models.Response;
using Domain.Api.Models.Response.User;
using Domain.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AuthWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private const string TOKEN_COOKIE_NAME = "token";
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
                    vailidateUserPsw(username, password);
                })
                .Do<LoginResponse>(async (result, user) =>
                {
                    UserInfoWithID userInfo = await _service.LoginPlayer(new UserIdentity
                    {
                        Username = username,
                        Password = password
                    });
                    DateTime expireTime = DateTime.UtcNow.AddDays(1);
                    string token = _jwtService.NewToken(
                        userInfo.Id,
                        userInfo.Username,
                        userInfo.Name,
                        expireTime);

                    Response.Cookies.Append(
                        TOKEN_COOKIE_NAME,
                        token,
                        new CookieOptions() { Expires = expireTime }
                    );
                    result.Name = userInfo.Name;
                    result.Username = userInfo.Username;

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
                    vailidateUserPsw(info.Username, info.Password);
                    if (string.IsNullOrWhiteSpace(info.Name))
                        throw new Exception("名稱不得為空");
                })
                .Do<BoolResponseModel>(async (result, user, logger) =>
                {
                    try
                    {
                        await _service.RegisterPlayer(info);
                        result.IsSuccess = true;
                    }
                    catch (DbUpdateException e)
                    {
                        SqlException sqlE = e.InnerException as SqlException;
                        logger.Log("Sql Exception", sqlE);

                        string errorMsg;
                        switch ((DbErrorNumber)sqlE.Number)
                        {
                            case DbErrorNumber.DuplicateKey:
                                errorMsg = "資料已被使用";
                                break;
                            default:
                                errorMsg = "未知的錯誤";
                                break;
                        }

                        result.Error(errorMsg);
                    }
                    catch (Exception e)
                    {
                        logger.Log("Exception", e);

                        result.Error("資料庫異常");
                    }

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
                .ValidateToken((user) =>
                {
                    if (user.Username != info.Username)
                        throw new Exception("不合法認證");
                    //string.IsNullOrEmpty(user.ValidAudience);
                })
                .ValidateRequest(() =>
                {
                    if (string.IsNullOrWhiteSpace(info.Name))
                        throw new Exception("名稱不得為空");
                })
                .Do<BoolResponseModel>(async (result, user) =>
                {
                    result.IsSuccess = await _service.UpdatePlayerInfo(user.Id, info);
                    result.Message = (result.IsSuccess) ?
                        "修改成功" :
                        "修改失敗";

                    return result;
                });
        }

        private void vailidateUserPsw(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
                throw new Exception("帳號或密碼不得為空");
        }
    }
}
