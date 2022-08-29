using BLL.Interfaces;
using DAL;
using DAL.Structs;
using Domain.Api.Interfaces;
using Domain.Api.Models.Response.Lobby;
using Domain.Api.Services;
using Domain.JWTUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRedisService _redisService;
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

        public RoomController(IConfiguration configuration, IRedisService redisService, IResponseService responseService, ILogger<RoomController> logger)
        {
            _redisService = redisService;
            _responseService = responseService;
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
                        RoomModel[] rooms = await _redisService.ListRooms();

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
                        RoomModel room = await _redisService.CreateRoom(userInfo, gameID);

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
                        RoomModel room = await _redisService.AddRoomPlayer(hostID, userInfo);

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
                    int roomID = 0;
                    int playerID = user.Id;
                    RoomModel room = null;

                    UserKey redisUser = rdsCtx.User;
                    RoomKey redisRoom = rdsCtx.Room;
                    GameKey redisGame = rdsCtx.Game;
                    GameStatusKey redisGameStatus = rdsCtx.GameStatus;

                    try
                    {
                        if (!await HandlerService.Retry(TRY_LOCK_TIMES, () => redisUser.Lock(playerID), WAIT_LOCK_MS))
                            throw new Exception("LockUser Fail");

                        UserModel userInfo = null;
                        try
                        {
                            userInfo = await redisUser.Get(playerID);
                            if (userInfo.GameRoomID == null)
                                throw new Exception();
                        }
                        catch
                        {
                            throw new Exception("不在任何房間");
                        }

                        roomID = userInfo.GameRoomID.Value;
                        if (!await HandlerService.Retry(TRY_LOCK_TIMES, () => redisRoom.Lock(roomID), WAIT_LOCK_MS))
                            throw new Exception("LockRoom Fail");

                        room = await removeRoomPlayer(rdsCtx, roomID, userInfo);
                    }
                    finally
                    {
                        await redisRoom.Release(roomID);
                        await redisUser.Release(playerID);
                    }

                    bool isRoomClose = room == null;
                    result.Room = (isRoomClose) ?
                    new Domain.Api.Models.Base.Lobby.RoomModel { HostID = roomID, Players = null } :
                    room.ToApiRoom();

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
                    int hostID = user.Id;
                    result.HostID = hostID;

                    UserKey redisUser = rdsCtx.User;
                    RoomKey redisRoom = rdsCtx.Room;
                    GameKey redisGame = rdsCtx.Game;
                    GameStatusKey redisGameStatus = rdsCtx.GameStatus;

                    int roomID = hostID;
                    RoomModel oriRoom = null;
                    try
                    {
                        if (!await HandlerService.Retry(TRY_LOCK_TIMES, () => redisRoom.Lock(roomID), WAIT_LOCK_MS))
                            throw new Exception("LockRoom Fail");

                        oriRoom = await redisRoom.Get(roomID);

                        result.GameID = oriRoom.Game.ID;

                        foreach (UserInfoModel player in oriRoom.Players)
                            if (!await HandlerService.Retry(TRY_LOCK_TIMES, () => redisUser.Lock(player.ID), WAIT_LOCK_MS))
                                throw new Exception("LockUser Fail");

                        UserModel userInfo = null;
                        try
                        {
                            userInfo = await redisUser.Get(hostID);
                            if (userInfo.GameRoomID == null)
                                throw new Exception();
                        }
                        catch
                        {
                            throw new Exception("不在任何房間");
                        }

                        await removeRoomPlayer(rdsCtx, roomID, userInfo, -hostID);
                    }
                    catch
                    {
                        await redisGameStatus.Delete(hostID);
                        throw;
                    }
                    finally
                    {
                        await redisRoom.Release(roomID);
                        if (oriRoom != null)
                            foreach (UserInfoModel player in oriRoom.Players)
                                await redisUser.Release(player.ID);
                    }

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

        private UserInfoModel GetUserInfo(UserClaimModel user)
        {
            return new UserInfoModel
            {
                ID = user.Id,
                Name = user.Name,
                Username = user.Username
            };
        }


        private async Task<RoomModel> removeRoomPlayer(RedisContext redis, int roomID, UserModel userInfo, int? GameRoomID = null)
        {
            UserKey redisUser = redis.User;
            RoomKey redisRoom = redis.Room;
            GameKey redisGame = redis.Game;
            GameStatusKey redisGameStatus = redis.GameStatus;

            ITransaction tran = redis.Begin();

            List<UserInfoModel> removeList = new List<UserInfoModel>();
            RoomModel oriRoom = await redisRoom.Get(roomID);
            bool isHost = roomID == userInfo.UserInfo.ID;
            if (isHost)
                removeList = oriRoom.Players.ToList();
            else
                removeList.Add(userInfo.UserInfo);

            Task<UserModel>[] getUsers = removeList.Select((info) => redisUser.Get(info.ID))
                    .Select(async (t) =>
                    {
                        UserModel u = await t;
                        u.GameRoomID = GameRoomID;
                        return u;
                    }).ToArray();
            foreach (Task<UserModel> getUser in getUsers)
            {
                UserModel u = await getUser;
                _ = redisUser.Set(u, tran);
            }

            RoomModel newRoom = null;
            if (isHost)
                _ = redisRoom.Delete(roomID, tran);
            else
            {
                UserInfoModel[] newPlayers = oriRoom.Players
                    .Where((p) => p.ID != userInfo.UserInfo.ID)
                    .ToArray();
                newRoom = new RoomModel
                {
                    Game = oriRoom.Game,
                    HostID = oriRoom.HostID,
                    Players = newPlayers
                };

                _ = redisRoom.Set(newRoom, tran);
            }

            if (!await tran.ExecuteAsync())
                throw new Exception("ExecuteAsync Fail");

            return newRoom;
        }
    }
}
