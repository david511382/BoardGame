using BoardGameAngular.Models.SignalR;
using BoardGameAngular.Services.Config;
using BoardGameAngular.Services.SignalRHub;
using Domain.Api.Interfaces;
using Domain.Api.Models.Response;
using Domain.Api.Models.Response.Lobby;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoardGameAngular.Controllers
{
    [Route("api/[controller]")]
    public class RoomController : Controller
    {
        private const string WS_CONNECTION_ID_HEADER = "cid";

        private readonly ConfigService _urlConfig;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;
        private readonly IHubContext<GameRoomHub, IGameRoomHub> _hubContext;

        public RoomController(
            ConfigService config,
            IResponseService responseService,
            IHubContext<GameRoomHub, IGameRoomHub> hubContext,
            ILogger<RoomController> logger)
        {
            _urlConfig = config;
            _responseService = responseService;
            _hubContext = hubContext;
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
            string cid = "";
            return await _responseService.Init<RoomResponse>(this, _logger)
                .ValidateRequest(() =>
                {
                    Microsoft.Extensions.Primitives.StringValues id = new Microsoft.Extensions.Primitives.StringValues();
                    if (!Request.Headers.TryGetValue(WS_CONNECTION_ID_HEADER, out id))
                        throw new Exception("缺少ws連線Id");

                    cid = id.ToString();
                    if (string.IsNullOrEmpty(cid))
                        throw new Exception("ws Id為空");
                })
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

                    if (result.IsSuccess)
                    {
                        await _hubContext.Clients.All.RoomOpened();

                        string groupName = result.Room.HostID.ToString();
                        await _hubContext.Groups.AddToGroupAsync(cid, groupName);
                    }

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
            string cid = "";
            return await _responseService.Init<RoomResponse>(this, _logger)
                .ValidateRequest(() =>
                {
                    Microsoft.Extensions.Primitives.StringValues id = new Microsoft.Extensions.Primitives.StringValues();
                    if (!Request.Headers.TryGetValue(WS_CONNECTION_ID_HEADER, out id))
                        throw new Exception("缺少ws連線Id");

                    cid = id.ToString();
                    if (string.IsNullOrEmpty(cid))
                        throw new Exception("ws Id為空");
                })
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

                    if (result.IsSuccess)
                    {
                        string groupName = result.Room.HostID.ToString();
                        await _hubContext.Clients.Group(groupName).RoomPlayerChanged(result.Room);

                        await _hubContext.Groups.AddToGroupAsync(cid, groupName);
                    }

                    return result;
                });
        }

        [HttpDelete]
        [Route("")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LeaveRoom()
        {
            string cid = "";
            return await _responseService.Init<BoolResponseModel>(this, _logger)
                .ValidateRequest(() =>
                {
                    Microsoft.Extensions.Primitives.StringValues id = new Microsoft.Extensions.Primitives.StringValues();
                    if (!Request.Headers.TryGetValue(WS_CONNECTION_ID_HEADER, out id))
                        throw new Exception("缺少ws連線Id");

                    cid = id.ToString();
                    if (string.IsNullOrEmpty(cid))
                        throw new Exception("ws Id為空");
                })
                .Do<BoolResponseModel>(async (result, user, logger) =>
                {
                    RoomResponse response = await HttpHelper.HttpRequest.New()
                        .AddHeader(new KeyValuePair<string, string>("Authorization", $"Bearer {Request.Cookies["token"]}"))
                        .To(_urlConfig.LeaveRoom)
                        .Delete<RoomResponse>();

                    if (response.IsSuccess)
                    {
                        Domain.Api.Models.Base.Lobby.RoomModel room = response.Room;
                        string groupName = room.HostID.ToString();
                        IGameRoomHub group = _hubContext.Clients.Group(groupName);

                        if (room.Players == null)
                            await group.RoomClose();
                        else
                            await group.RoomPlayerChanged(room);

                        await _hubContext.Groups.RemoveFromGroupAsync(cid, groupName);
                    }

                    result.ErrorMessage = response.ErrorMessage;
                    result.IsError = response.IsError;
                    result.IsSuccess = response.IsSuccess;
                    result.Message = response.Message;

                    return result;
                });
        }

        [HttpDelete]
        [Route("Start")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(StartRoomResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StartRoomResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Start()
        {
            return await _responseService.Init<StartRoomResponse>(this, _logger)
                .Do<StartRoomResponse>(async (result, user, logger) =>
                {
                    KeyValuePair<string, string> header =
                        new KeyValuePair<string, string>("Authorization", $"Bearer {Request.Cookies["token"]}");
                    BoolResponseModel startGameResponse = await HttpHelper.HttpRequest.New()
                     .AddHeader(header)
                     .To(_urlConfig.StartGame)
                     .Post<BoolResponseModel>();

                    if (!startGameResponse.IsSuccess)
                    {
                        result.Fail(startGameResponse.Message);
                        return result;
                    }

                    result = await HttpHelper.HttpRequest.New()
                        .AddHeader(header)
                        .To(_urlConfig.RoomStart)
                        .Delete<StartRoomResponse>();

                    if (result.IsSuccess)
                    {
                        string groupName = result.HostID.ToString();
                        await _hubContext.Clients.Group(groupName).RoomStarted(result.GameID);
                    }

                    return result;
                });
        }
    }
}
