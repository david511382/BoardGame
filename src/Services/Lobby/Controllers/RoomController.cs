using Domain.Api.Interfaces;
using Domain.Api.Models.Response.Lobby;
using LobbyWebService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LobbyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRedisService _redisService;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

        public RoomController(IRedisService redisService, IResponseService responseService, ILogger<RoomController> logger)
        {
            _redisService = redisService;
            _responseService = responseService;
            _logger = logger;
        }

        /// <summary>
        /// 創建遊戲房間
        /// </summary>
        /// <param name="gameID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GameListResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRoom([FromForm] int gameID)
        {
            return await _responseService.Init<GameListResponse>(this, _logger)
                .ValidateToken((user) => { })
                .ValidateRequest(() =>
                {
                    if (gameID <= 0)
                        throw new Exception("不合法的遊戲編號");
                })
                .Do<GameListResponse>(async (result, user, logger) =>
                {
                    int hostID = user.Id;
                    try
                    {
                        GameRespository.Models.GameInfo game;
                        try
                        {
                            game = await _redisService.Game(gameID);
                        }
                        catch
                        {
                            result.Error("無指定的遊戲");
                            throw;
                        }

                        try
                        {
                            await _redisService.CreateRoom(hostID, game);
                            result.IsError = false;
                            result.Message = "創建成功";
                        }
                        catch
                        {
                            result.Error("創建失敗");
                            throw;
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Log("Create_Room_Exception", e.Message);
                    }

                    return result;
                });
        }
    }
}
