using BoardGameAngular.Services.Config;
using Domain.Api.Interfaces;
using Domain.Api.Models.Response.Lobby;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoardGameAngular.Controllers
{
    [Route("api/[controller]")]
    public class RoomController : Controller
    {
        private readonly ConfigService _urlConfig;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

        public RoomController(ConfigService config, IResponseService responseService, ILogger<RoomController> logger)
        {
            _urlConfig = config;
            _responseService = responseService;
            _logger = logger;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(RoomListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RoomListResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListRoom()
        {
            return await _responseService.Init<RoomListResponse>(this, _logger)
                  .Do<RoomListResponse>(async (result, user, logger) =>
                  {
                      result = await HttpHelper.HttpRequest.New()
                         .To(_urlConfig.ListRoom)
                         .Get<RoomListResponse>();

                      return result;
                  });
        }

        [HttpPost]
        [Route("")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRoom([FromForm] int gameID)
        {
            return await _responseService.Init<RoomResponse>(this, _logger)
                .Do<RoomResponse>(async (result, user, logger) =>
                {
                    result = await HttpHelper.HttpRequest.New()
                        .SetForm(new Dictionary<string, string>
                        {
                            { "gameID",gameID.ToString() },
                        })
                        .AddHeader(new KeyValuePair<string, string>("Authorization", $"Bearer {Request.Cookies["token"]}"))
                        .To(_urlConfig.CreateRoom)
                        .Post<RoomResponse>();

                    return result;
                });
        }

        [HttpPatch]
        [Route("")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> JoinRoom([FromForm] int hostID)
        {
            return await _responseService.Init<RoomResponse>(this, _logger)
                .Do<RoomResponse>(async (result, user, logger) =>
                {
                    result = await HttpHelper.HttpRequest.New()
                        .SetForm(new Dictionary<string, string>
                        {
                            { "hostID",hostID.ToString() },
                        })
                        .AddHeader(new KeyValuePair<string, string>("Authorization", $"Bearer {Request.Cookies["token"]}"))
                        .To(_urlConfig.JoinRoom)
                        .Patch<RoomResponse>();

                    return result;
                });
        }

        [HttpDelete]
        [Route("")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LeaveRoom()
        {
            return await _responseService.Init<RoomResponse>(this, _logger)
                .Do<RoomResponse>(async (result, user, logger) =>
                {
                    result = await HttpHelper.HttpRequest.New()
                        .AddHeader(new KeyValuePair<string, string>("Authorization", $"Bearer {Request.Cookies["token"]}"))
                        .To(_urlConfig.LeaveRoom)
                        .Delete<RoomResponse>();

                    return result;
                });
        }

        [HttpDelete]
        [Route("Start")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Start()
        {
            return await _responseService.Init<RoomResponse>(this, _logger)
                .Do<RoomResponse>(async (result, user, logger) =>
                {
                    result = await HttpHelper.HttpRequest.New()
                        .AddHeader(new KeyValuePair<string, string>("Authorization", $"Bearer {Request.Cookies["token"]}"))
                        .To(_urlConfig.RoomStart)
                        .Delete<RoomResponse>();

                    return result;
                });
        }
    }
}
