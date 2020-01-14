using Domain.Api.Interfaces;
using Domain.Api.Models.Response.Lobby;
using LobbyWebService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedisRepository.Models;
using System;
using System.Threading.Tasks;

namespace LobbyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRedisService _redisService;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

        public UserController(IRedisService redisService, IResponseService responseService, ILogger<UserController> logger)
        {
            _redisService = redisService;
            _responseService = responseService;
            _logger = logger;
        }

        /// <summary>
        /// 取得用戶大廳狀態
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserStatus()
        {
            return await _responseService.Init<StatusResponse>(this, _logger)
                .ValidateToken((user) => { })
                .Do<StatusResponse>(async (result, user, logger) =>
                {
                    UserModel userStatus;
                    try
                    {
                        userStatus = await _redisService.User(user.Id);
                        if (userStatus.GameRoomID == null)
                            throw new Exception();
                    }
                    catch
                    {
                        result.Room = null;
                        return result;
                    }

                    bool isInGame = userStatus.GameRoomID.Value < 0;
                    bool isInRoom = userStatus.GameRoomID.Value > 0;
                    int roomId = Math.Abs(userStatus.GameRoomID.Value);

                    RoomModel room;
                    if (isInRoom)
                        room = await _redisService.Room(roomId);
                    else if (isInGame)
                    {
                        GameStatusModel gameStatus = await _redisService.GameStatus(roomId);
                        room = gameStatus.Room;
                    }
                    else
                        throw new Exception("資料錯誤");

                    result.IsInGame = isInGame;
                    result.IsInRoom = isInRoom;
                    result.Room = room.ToApiRoom();

                    return result;
                }, async (result, e, logger) =>
                {
                    result.Error(e.Message);
                    return result;
                });
        }
    }
}
