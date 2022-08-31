using BLL;
using BLL.Interfaces;
using DAL;
using DAL.Structs;
using Domain.Api.Interfaces;
using Domain.Api.Models.Response.Lobby;
using Domain.JWTUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly BLL.Interfaces.ILobbyRoom _lobbyBll;
        private readonly ILobbyUser _lobbyUserBll;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

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
        private readonly string RedisConnStr;

        private const int TRY_LOCK_TIMES = 3;
        private const int WAIT_LOCK_MS = 50;

        public RoomController(
            IConfiguration configuration,
            ILobbyRoom redisService,
            ILobbyUser lobbyUserBll,
            IResponseService responseService,
            ILogger<RoomController> logger)
        {
            _lobbyBll = redisService;
            _responseService = responseService;
            _lobbyUserBll = lobbyUserBll;
            _logger = logger;

            RedisConnStr = configuration.GetConnectionString("Redis");
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
                        RoomModel[] rooms = await _lobbyBll.ListRooms();

                        result.Rooms = rooms.Select((room) => room.ToApiRoom()).ToArray();
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
        [Authorize]
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
                        RoomModel room = await _lobbyBll.CreateRoom(userInfo, gameID);

                        result.Room = room.ToApiRoom();
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
        [Authorize]
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
                        RoomModel room = await _lobbyBll.AddRoomPlayer(hostID, userInfo);

                        result.Room = room.ToApiRoom();
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
        [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> LeaveRoom()
        {
            return await _responseService.Init<RoomResponse>(this, _logger)
                .ValidateToken((user) => { })
                .Do<RoomResponse>(async (result, user, logger) =>
                {
                    try
                    {
                        UserInfoModel userInfo = GetUserInfo(user);
                        RoomModel room = await _lobbyBll.LeaveRoom(userInfo.ID);
                        result.Room = room.ToApiRoom();
                    }
                    catch (Exception e)
                    {
                        logger.Log("Exception", e);

                        result.Error(e.Message);
                        return result;
                    }

                    result.Success("離開房間完成");
                    return result;
                }, async (result, e, logger) =>
                {
                    result.Error("離開房間出錯");
                    return result;
                });
        }

        [HttpDelete]
        [Route("Start")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(StartRoomResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StartRoomResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> StartGame()
        {
            return await _responseService.Init<StartRoomResponse>(this, _logger)
                .ValidateToken((user) => { })
                .Do<StartRoomResponse>(async (result, user, logger) =>
                {
                    RoomModel room = await _lobbyBll.StartRoom(user.Id);
                    result.HostID = room.HostID;
                    result.GameID = room.Game.ID;

                    result.Success("開始遊戲");
                    return result;
                }, async (result, e, logger) =>
                {
                    result.Error("無法開始遊戲");
                    return result;
                });
        }

        /// <summary>
        /// 取得用戶大廳狀態
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("User")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> UserStatus()
        {
            return await _responseService.Init<StatusResponse>(this, _logger)
                .ValidateToken((user) => { })
                .Do<StatusResponse>(async (result, user, logger) =>
                {
                    result.Id = user.Id;
                    result.Name = user.Name;
                    result.Username = user.Username;

                    LobbyUserStatus userStatus;
                    try
                    {
                        userStatus = await _lobbyUserBll.GetUser(user.Id);
                        if (userStatus.IsInLobby)
                            throw new Exception();
                    }
                    catch
                    {
                        result.Room = null;
                        return result;
                    }

                    bool isInGame = userStatus.IsInGame;
                    bool isInRoom = userStatus.IsInRoom;
                    int roomId = Math.Abs(userStatus.GameRoomID.Value);

                    RoomModel room;
                    if (isInRoom)
                        room = await _lobbyBll.Room(roomId);
                    else if (isInGame)
                    {
                        GameStatusModel gameStatus = await _lobbyBll.GameStatus(roomId);
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

        private UserInfoModel GetUserInfo(UserClaimModel user)
        {
            return new UserInfoModel
            {
                ID = user.Id,
                Name = user.Name,
                Username = user.Username
            };
        }
    }
}
