using Domain.Api.Interfaces;
using Domain.Api.Models.Base.Lobby;
using Domain.Api.Models.Response.Lobby;
using LobbyWebService.Services;
using LobbyWebService.Sevices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LobbyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IRedisService _redisService;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

        public GameController(IGameService gameService, IRedisService redisService, IResponseService responseService, ILogger<GameController> logger)
        {
            _gameService = gameService;
            _redisService = redisService;
            _responseService = responseService;
            _logger = logger;
        }

        /// <summary>
        /// 取得遊戲列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("List")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GameListResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            return await _responseService.Init<GameListResponse>(this, _logger)
                .Do<GameListResponse>(async (result, user) =>
                {
                    try
                    {
                        GameModel[] list = await _redisService.ListGames();

                        result.Games = list.Select((g) => new GameModel
                        {
                            ID = g.ID,
                            Name = g.Name,
                            Description = g.Description,
                            MaxPlayerCount = g.MaxPlayerCount,
                            MinPlayerCount = g.MinPlayerCount
                        }).ToArray();
                    }
                    catch (Exception e)
                    {
                        result.Error(e.Message);
                    }

                    return result;
                });
        }
    }
}
