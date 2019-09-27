using AuthWebService.Models;
using AuthWebService.Sevices;
using CommonUtil.User;
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

        public UserController(IAuthService authService, IJWTService jwtService)
        {
            _service = authService;
            _jwtService = jwtService;
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
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return BadRequest("資料不得為空");

            try
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

                LoginResponse result = new LoginResponse
                {
                    Token = token,
                    Name = userInfo.Name
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        [Produces("application/text")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserInfo info)
        {
            if (string.IsNullOrWhiteSpace(info.Username) || string.IsNullOrWhiteSpace(info.Password))
                return BadRequest("資料不得為空");

            try
            {
                bool result = await _service.RegisterPlayer(info);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// 修改會員資料
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize]
        [Produces("application/text")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Put([FromBody] UserInfo info)
        {
            UserClaimModel userInfo = UserClaim.Parse(HttpContext.User);

            if (string.IsNullOrWhiteSpace(info.Username) || string.IsNullOrWhiteSpace(info.Password))
                return BadRequest("資料不得為空");

            try
            {
                bool result = await _service.UpdatePlayerInfo(userInfo.Id, info);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
