using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Api.Interfaces;
using Domain.Api.Models.Base.Lobby;
using Domain.Api.Models.Response;
using Domain.Api.Models.Response.Lobby;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameInfoDAL _db;
        private readonly IGameService _gameService;
        private readonly ILobbyUser _lobbyUserBll;
        private readonly ILobbyRoom _lobbyRoomBll;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

        private readonly string RedisConnStr;
        public GameController(
            IGameInfoDAL db,
            IConfiguration configuration,
            IGameService gameService,
            ILobbyUser lobbyUserBll,
            ILobbyRoom lobbyRoomBll,
            IResponseService responseService,
            ILogger<GameController> logger)
        {
            RedisConnStr = configuration.GetConnectionString("Redis");
            _responseService = responseService;
            _db = db;
            _gameService = gameService;
            _lobbyUserBll = lobbyUserBll;
            _lobbyRoomBll = lobbyRoomBll;
            _logger = logger;
        }

        /// <summary>
        /// 取得遊戲列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GameListResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            return await _responseService.Init<GameListResponse>(this, _logger)
                .Do<GameListResponse>(async (result, user, logger) =>
                {
                    IEnumerable<DAL.Structs.GameModel> list = await _gameService.GameList();

                    result.Games = list.Select((g) => new GameModel
                    {
                        ID = g.ID,
                        Name = g.Name,
                        Description = g.Description,
                        MaxPlayerCount = g.MaxPlayerCount,
                        MinPlayerCount = g.MinPlayerCount
                    }).ToArray();

                    return result;
                });
        }

        [HttpPost]
        [Route("StartGame")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> StartGame()
        {
            return await _responseService.Init<BoolResponseModel>(this, _logger)
                .ValidateToken((user) => { })
                .Do<BoolResponseModel>(async (result, user, logger) =>
                {
                    int roomID = 0;
                    try
                    {
                        BLL.LobbyUserStatus lobbyUser = await _lobbyUserBll.GetUser(user.Id);
                        if (lobbyUser.IsInLobby)
                        {
                            throw new Exception();
                        }
                        if (lobbyUser.IsInGame)
                        {
                            result.Fail("正在遊戲中");
                            return result;
                        }
                        else if (!lobbyUser.IsRoomHost)
                        {
                            result.Fail("不是房主");
                            return result;
                        }
                        roomID = lobbyUser.RoomID;
                    }
                    catch (Exception e)
                    {
                        logger.Log(e.ToString());

                        result.Fail("不在任何房間");
                        return result;
                    }

                    
                    DAL.Structs.RoomModel oriRoom = await _lobbyRoomBll.Room(roomID);
                    try
                    {
                        DAL.Structs.GameStatusModel gameStatus = new DAL.Structs.GameStatusModel { Room = oriRoom };
                        var newGameStatus = _gameService.InitGame(gameStatus);
                        await _lobbyRoomBll.SaveGameStatus(newGameStatus);
                    }
                    catch
                    {
                        await _lobbyRoomBll.ClearGameStatus(roomID);
                        throw;
                    }

                    result.Success("成功");
                    return result;
                }, async (result, e, logger) =>
                {
                    result.Fail("發生錯誤");
                    return result;
                });
        }
    }
}
