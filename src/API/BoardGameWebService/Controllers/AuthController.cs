using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using Util;

namespace BoardGameWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public AuthController() { }

        /// <summary>
        /// 驗證Token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public IActionResult Authenticate()
        {
            try
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(memStream, Encoding.UTF8))
                    {
                        HttpContext.User.WriteTo(writer);
                    }
                    byte[] bs = memStream.ToArray();
                    string result = bs.BytesToHex();

                    return Ok(result);
                }
            }
            catch
            {
                return Unauthorized();
            }
        }
    }
}
