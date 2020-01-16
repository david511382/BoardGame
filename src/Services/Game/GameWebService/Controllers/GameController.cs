using Domain.Api.Interfaces;
using Domain.Api.Models.Base.Lobby;
using Domain.Api.Models.Response.Lobby;
using GameRespository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RedisRepository;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebService.Controllers
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

        private GameInfoDAL dbCtx
        {
            get
            {
                if (_db == null)
                    _db = new GameInfoDAL(GameDbConnStr);
                return _db;
            }
        }
        private GameInfoDAL _db;

        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

        private readonly string GameDbConnStr;
        private readonly string RedisConnStr;
        public GameController(IConfiguration configuration, IResponseService responseService, ILogger<GameController> logger)
        {
            GameDbConnStr = configuration.GetConnectionString("GameDb");
            RedisConnStr = configuration.GetConnectionString("Redis");
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
                .Do<GameListResponse>(async (result, user, logger) =>
                {
                    RedisRepository.Models.GameModel[] list = await rdsCtx.Game.ListGames();

                    if (list.Length == 0)
                    {
                        list = (await dbCtx.List())
                        .Select((g) => new RedisRepository.Models.GameModel
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
    }
}
