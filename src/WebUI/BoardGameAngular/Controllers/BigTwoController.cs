using BoardGameAngular.Models.SignalR;
using BoardGameAngular.Services.Config;
using BoardGameAngular.Services.SignalRHub;
using Domain.Api.Interfaces;
using Domain.Api.Models.Request.Game;
using Domain.Api.Models.Response;
using Domain.Api.Models.Response.Game.PokerGame.BigTwo;
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
    public class BigTwoController : Controller
    {
        private const string WS_CONNECTION_ID_HEADER = "cid";

        private readonly ConfigService _urlConfig;
        private readonly IResponseService _responseService;
        private readonly IHubContext<GameRoomHub, IGameRoomHub> _hubContext;
        private readonly ILogger _logger;

        public BigTwoController(ConfigService config,
            IResponseService responseService,
            IHubContext<GameRoomHub, IGameRoomHub> hubContext,
            ILogger<GameController> logger)
        {
            _urlConfig = config;
            _responseService = responseService;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpPost("SelectCards")]
        [ProducesResponseType(typeof(SelectCardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SelectCardResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SelectCards([FromBody] IndexesRequest request)
        {
            return await _responseService.Init<SelectCardResponse>(this, _logger)
                .Do<SelectCardResponse>(async (result, user, logger) =>
                {
                    result = await Util.Http.HttpRequest.New()
                        .AddHeader(new KeyValuePair<string, string>("Authorization", $"Bearer {Request.Cookies["token"]}"))
                        .SetJson(request)
                        .To(_urlConfig.SelectCards)
                        .Post<SelectCardResponse>();

                    return result;
                });
        }

        [HttpPost("PlayCards")]
        [ProducesResponseType(typeof(SelectCardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SelectCardResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PlayCards([FromBody] IndexesRequest request)
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
                    Task<StatusResponse> statusResponseTask = Util.Http.HttpRequest.New()
                        .AddHeader(new KeyValuePair<string, string>("Authorization", $"Bearer {Request.Cookies["token"]}"))
                        .To(_urlConfig.UserStatus)
                        .Get<StatusResponse>();

                    PlayCardResponse response = await Util.Http.HttpRequest.New()
                        .AddHeader(new KeyValuePair<string, string>("Authorization", $"Bearer {Request.Cookies["token"]}"))
                        .SetJson(request)
                        .To(_urlConfig.PlayCards)
                        .Post<PlayCardResponse>();

                    if (!response.IsError && response.IsSuccess)
                    {
                        StatusResponse statusResponse = await statusResponseTask;
                        string groupName = statusResponse.Room.HostID.ToString();
                        await _hubContext.Clients.Group(groupName).GameBoardUpdate(new GameBoardModel(response.Cards, response.Condition));
                    }

                    return new BoolResponseModel(response);
                });
        }
    }
}
