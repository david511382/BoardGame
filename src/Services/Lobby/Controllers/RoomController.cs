using Domain.Api.Interfaces;
using Domain.Api.Models.Response;
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
        /// 列出房間
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RoomListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RoomListResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListRoom()
        {
            return await _responseService.Init<RoomListResponse>(this, _logger)
                .Do<RoomListResponse>(async (result, user, logger) =>
                {
                    try
                    {
                        result.Rooms = await _redisService.ListRooms();
                    }
                    catch (Exception e)
                    {
                        result.Error(e.Message);
                    }

                    return result;
                });
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
                    try
                    {
                        await _redisService.CreateRoom(user.Id, gameID);
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
                    try
                    {
                        await _redisService.RemoveRoomPlayer(user.Id);
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
