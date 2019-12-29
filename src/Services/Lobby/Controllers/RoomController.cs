using Domain.Api.Interfaces;
using Domain.Api.Models.Response;
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
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRoom([FromForm] int gameID)
        {
            return await _responseService.Init<BoolResponseModel>(this, _logger)
                .ValidateToken((user) => { })
                .ValidateRequest(() =>
                {
                    if (gameID <= 0)
                        throw new Exception("不合法的遊戲編號");
                })
                .Do<BoolResponseModel>(async (result, user, logger) =>
                {
                    int hostID = user.Id;

                    Task<RedisRepository.Models.UserModel> getUserTask = _redisService.User(hostID);
                    Task<GameRespository.Models.GameInfo> getGameTask = _redisService.Game(gameID);

                    RedisRepository.Models.UserModel userInfo;
                    try
                    {
                        userInfo = await getUserTask;
                        if (userInfo.RoomID != null)
                        {
                            result.Fail("已加入其他房間");
                            return result;
                        }
                    }
                    catch { }

                    GameRespository.Models.GameInfo game;
                    try
                    {
                        game = await getGameTask;
                    }
                    catch (Exception e)
                    {
                        logger.Log("Exception", e);

                        result.Fail("無指定的遊戲");
                        return result;
                    }

                    try
                    {
                        await _redisService.CreateRoom(hostID, game);
                    }
                    catch (Exception e)
                    {
                        logger.Log("Exception", e);

                        result.Fail("創建失敗");
                        return result;
                    }

                    result.Success("創建成功");
                    return result;
                });
        }

        /// <summary>
        /// 加入房間
        /// </summary>
        /// <param name="hostID"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> JoinRoom([FromForm] int hostID)
        {
            return await _responseService.Init<BoolResponseModel>(this, _logger)
                .ValidateToken((user) => { })
                .ValidateRequest(() =>
                {
                    if (hostID <= 0)
                        throw new Exception("不合法的房間編號");
                })
                .Do<BoolResponseModel>(async (result, user, logger) =>
                {
                    Task<RedisRepository.Models.UserModel> getUserTask = _redisService.User(user.Id);
                    Task<RedisRepository.Models.RedisRoomModel> getRoomTask = _redisService.Room(hostID);

                    RedisRepository.Models.UserModel userInfo;
                    try
                    {
                        userInfo = await getUserTask;
                        if (userInfo.RoomID != null)
                        {
                            result.Fail("已加入其他房間");
                            return result;
                        }
                    }
                    catch { }

                    RedisRepository.Models.RedisRoomModel room;
                    try
                    {
                        room = await getRoomTask;
                        if (room.IsFull())
                        {
                            result.Fail("房間已滿");
                            return result;
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Log("Exception", e);

                        result.Fail("查無指定房間");
                        return result;
                    }

                    try
                    {
                        await _redisService.AddRoomPlayer(hostID, user.Id);
                    }
                    catch (Exception e)
                    {
                        logger.Log("Exception", e);

                        result.Error("加入房間出錯");
                        return result;
                    }

                    result.Success("加入成功");
                    return result;
                });
        }

        /// <summary>
        /// 離開房間
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LeaveRoom()
        {
            return await _responseService.Init<BoolResponseModel>(this, _logger)
                .ValidateToken((user) => { })
                .Do<BoolResponseModel>(async (result, user, logger) =>
                {
                    RedisRepository.Models.UserModel userInfo;
                    try
                    {
                        userInfo = await _redisService.User(user.Id);
                        if (userInfo.RoomID == null)
                            throw new Exception();
                    }
                    catch
                    {
                        result.Fail("不在任何房間");
                        return result;
                    }

                    int roomID = userInfo.RoomID.Value;
                    try
                    {
                        await _redisService.RemoveRoomPlayer(roomID, user.Id);
                    }
                    catch (Exception e)
                    {
                        logger.Log("Exception", e);

                        result.Error("離開房間出錯");
                        return result;
                    }

                    result.Success("離開房間完成");
                    return result;
                });
        }
    }
}
