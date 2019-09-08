using AuthLogic.Domain.Interfaces;
using AuthLogic.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AuthWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuth _service;

        public AuthController(IAuth authService)
        {
            _service = authService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<UserInfoWithID> Login([FromForm] string username, [FromForm] string password)
        {
            try
            {
                UserInfoWithID result = await _service.LoginPlayer(new UserIdentity
                {
                    Username = username,
                    Password = password
                });

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("Register")]
        public async Task<bool> Register([FromBody] UserInfo info)
        {
            try
            {
                bool result = await _service.RegisterPlayer(info);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(int id, [FromBody] UserInfo info)
        {
            try
            {
                bool result = await _service.UpdatePlayerInfo(id, info);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
