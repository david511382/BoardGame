using DAL;
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
using Services.Game;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private RedisContext rdsCtx
        {
            get
            {
                if (_redis == null)
                    _redis = new RedisContext(RedisConnStr);
                return _redis;
            }
        }
        private RedisContext _redis;

        private readonly IGameInfoDAL _db;
        private readonly IGameService _gameService;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

        private readonly string RedisConnStr;
        public GameController(IGameInfoDAL db, IConfiguration configuration, IGameService gameService, IResponseService responseService, ILogger<GameController> logger)
        {
            RedisConnStr = configuration.GetConnectionString("Redis");
            _responseService = responseService;
            _db = db;
            _gameService = gameService;
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
                    DAL.Structs.GameModel[] list = await rdsCtx.Game.ListGames();

                    if (list.Length == 0)
                    {
                        list = (await _db.List())
                        .Select((g) => new DAL.Structs.GameModel
                        {
                            ID = g.ID,
                            Description = g.Description,
                            MaxPlayerCount = g.MaxPlayerCount,
                            MinPlayerCount = g.MinPlayerCount,
                            Name = g.Name
                        })
                       .ToArray();

                        try
                        {
                            await rdsCtx.Game.AddGames(list);
                        }
                        catch
                        {
                            logger.Log("add db games to redis fail");
                        }
                    }

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
                    int hostID = user.Id;

                    DAL.Structs.UserModel userInfo = null;
                    try
                    {
                        userInfo = await rdsCtx.User.Get(hostID);
                        if (userInfo.GameRoomID == null)
                            throw new Exception("userInfo.GameRoomID == null");
                    }
                    catch (Exception e)
                    {
                        logger.Log(e.ToString());

                        result.Fail("不在任何房間");
                        return result;
                    }
                    if (userInfo.GameRoomID < 0)
                    {
                        result.Fail("正在遊戲中");
                        return result;
                    }
                    else if (userInfo.GameRoomID != hostID)
                    {
                        result.Fail("不是房主");
                        return result;
                    }

                    DAL.Structs.RoomModel oriRoom = await rdsCtx.Room.Get(hostID);

                    try
                    {
                        DAL.Structs.GameStatusModel gameStatus = new DAL.Structs.GameStatusModel { Room = oriRoom };
                        DAL.Structs.GameStatusModel newGameStatus = _gameService.InitGame(gameStatus);

                        await rdsCtx.GameStatus.Set(newGameStatus);
                    }
                    catch
                    {
                        await rdsCtx.GameStatus.Delete(hostID);
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
