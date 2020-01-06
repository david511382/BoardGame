using Domain.Api.Interfaces;
using Domain.Api.Models.Response;
using Domain.Api.Models.Response.Lobby;
using Domain.JWTUser;
using LobbyWebService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedisRepository.Models;
using System;
using System.Linq;
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
                        var rooms = await _redisService.ListRooms();

                        result.Rooms = rooms.Select((room) => ToRoom(room)).ToArray();
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
        [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRoom([FromForm] int gameID)
        {
            return await _responseService.Init<RoomResponse>(this, _logger)
                .ValidateToken((user) => { })
                .ValidateRequest(() =>
                {
                    if (gameID <= 0)
                        throw new Exception("不合法的遊戲編號");
                })
                .Do<RoomResponse>(async (result, user, logger) =>
                {
                    try
                    {
                        UserInfoModel userInfo = GetUserInfo(user);
                        RoomModel room = await _redisService.CreateRoom(userInfo, gameID);

                        result.Room = ToRoom(room);
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
        [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> JoinRoom([FromForm] int hostID)
        {
            return await _responseService.Init<RoomResponse>(this, _logger)
                .ValidateToken((user) => { })
                .ValidateRequest(() =>
                {
                    if (hostID <= 0)
                        throw new Exception("不合法的房間編號");
                })
                .Do<RoomResponse>(async (result, user, logger) =>
                {
                    try
                    {
                        UserInfoModel userInfo = GetUserInfo(user);
                        RoomModel room = await _redisService.AddRoomPlayer(hostID, userInfo);

                        result.Room = ToRoom(room);
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

        [HttpDelete]
        [Route("Start")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> StartGame()
        {
            return await _responseService.Init<BoolResponseModel>(this, _logger)
                .ValidateToken((user) => { })
                .Do<BoolResponseModel>(async (result, user, logger) =>
                {
                    try
                    {
                        await _redisService.StartRoom(user.Id);
                    }
                    catch (Exception e)
                    {
                        logger.Log("Exception", e);

                        result.Error("無法開始遊戲");
                        return result;
                    }

                    result.Success("開始遊戲");
                    return result;
                });
        }

        private UserInfoModel GetUserInfo(UserClaimModel user)
        {
            return new UserInfoModel
            {
                ID = user.Id,
                Name = user.Name,
                Username = user.Username
            };
        }

        private Domain.Api.Models.Base.Lobby.RoomModel ToRoom(RoomModel room)
        {
            return new Domain.Api.Models.Base.Lobby.RoomModel
            {
                Game = new Domain.Api.Models.Base.Lobby.GameModel
                {
                    Description = room.Game.Description,
                    ID = room.Game.ID,
                    MaxPlayerCount = room.Game.MaxPlayerCount,
                    MinPlayerCount = room.Game.MinPlayerCount,
                    Name = room.Game.Name,
                },
                HostID = room.HostID,
                Players = room.Players.Select((p) => new Domain.Api.Models.Base.User.UserModel
                {
                    ID = p.ID,
                    Name = p.Name,
                    Username = p.Username
                }).ToArray()
            };
        }
    }
}
