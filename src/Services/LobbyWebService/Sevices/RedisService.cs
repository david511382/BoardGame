using RedisRepository;
using RedisRepository.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LobbyWebService.Services
{
    public class RedisService : IRedisService
    {
        private const int TRY_LOCK_TIMES = 3;
        private const int WAIT_LOCK_MS = 50;

        private UserKey _user => _dal.User;
        private RoomKey _room => _dal.Room;
        private GameKey _game => _dal.Game;
        private GameStatusKey _gameStatus => _dal.GameStatus;

        private RedisContext _dal;

        private static async Task<bool> Retry(int times, Func<Task<bool>> tryThing, int delayMs = 0)
        {
            if (times == 0)
                return false;

            if (await tryThing())
                return true;

            Thread.Sleep(delayMs);

            return await Retry(times - 1, tryThing);
        }

        public RedisService(string connectStr)
        {
            _dal = new RedisContext(connectStr);
        }

        public async Task<GameModel> Game(int id)
        {
            return await _game.Get(id);
        }

        public async Task<RoomModel> CreateRoom(UserInfoModel userInfo, int gameID)
        {
            int hostID = userInfo.ID;
            try
            {
                if (!await Retry(TRY_LOCK_TIMES, () => _room.Lock(hostID), WAIT_LOCK_MS))
                    throw new Exception("LockRoom Fail");
                if (!await Retry(TRY_LOCK_TIMES, () => _user.Lock(hostID), WAIT_LOCK_MS))
                    throw new Exception("LockUser Fail");

                Task<UserModel> getUserTask = User(hostID);
                Task<GameModel> getGameTask = Game(gameID);

                UserModel currentUser = null;
                try
                {
                    currentUser = await getUserTask;
                }
                catch { }
                if (currentUser != null)
                {
                    if (currentUser.GameRoomID != null)
                        throw new Exception("已加入其他房間");
                }
                else
                {
                    currentUser = new UserModel
                    {
                        UserInfo = userInfo,
                        GameRoomID = null
                    };
                }

                GameModel game;
                try
                {
                    game = await getGameTask;
                }
                catch
                {
                    throw new Exception("無指定的遊戲");
                }

                ITransaction tran = _dal.Begin();

                RoomModel room = new RoomModel();
                room.Game = game;
                room.HostID = hostID;
                room.Players = new UserInfoModel[] { currentUser.UserInfo };
                _ = _room.Set(room, tran);

                UserModel user = new UserModel
                {
                    UserInfo = currentUser.UserInfo,
                    GameRoomID = hostID
                };
                _ = _user.Set(user, tran);

                if (!await tran.ExecuteAsync())
                    throw new Exception("ExecuteAsync Fail");

                return room;
            }
            finally
            {
                await _room.Release(hostID);
                await _user.Release(hostID);
            }
        }

        public async Task<RoomModel[]> ListRooms()
        {
            return await _room.ListRooms();
        }

        public async Task<RoomModel> Room(int hostID)
        {
            return await _room.Get(hostID);
        }

        public async Task<RoomModel> AddRoomPlayer(int hostID, UserInfoModel userInfo)
        {
            int playerID = userInfo.ID;
            try
            {
                if (!await Retry(TRY_LOCK_TIMES, () => _room.Lock(hostID), WAIT_LOCK_MS))
                    throw new Exception("LockRoom Fail");
                if (!await Retry(TRY_LOCK_TIMES, () => _user.Lock(playerID), WAIT_LOCK_MS))
                    throw new Exception("LockUser Fail");

                Task<UserModel> getUserTask = User(playerID);
                Task<RoomModel> getRoomTask = Room(hostID);

                UserModel currentUser = null;
                try
                {
                    currentUser = await getUserTask;
                }
                catch { }
                if (currentUser != null)
                {
                    if (currentUser.GameRoomID != null)
                        throw new Exception("已加入其他房間");
                }
                else
                {
                    currentUser = new UserModel
                    {
                        UserInfo = userInfo,
                        GameRoomID = null
                    };
                }

                RoomModel oriRoom;
                try
                {
                    oriRoom = await getRoomTask;
                    if (oriRoom.IsFull())
                        throw new Exception("房間已滿");
                }
                catch
                {
                    throw new Exception("查無指定房間");
                }

                ITransaction tran = _dal.Begin();

                List<UserInfoModel> players = oriRoom.Players.ToList();
                players.Add(currentUser.UserInfo);
                RoomModel newRoom = new RoomModel
                {
                    Game = oriRoom.Game,
                    HostID = oriRoom.HostID,
                    Players = players.ToArray()
                };
                _ = _room.Set(newRoom, tran);

                UserModel user = new UserModel
                {
                    UserInfo = currentUser.UserInfo,
                    GameRoomID = hostID
                };
                _ = _user.Set(user, tran);

                if (!await tran.ExecuteAsync())
                    throw new Exception("ExecuteAsync Fail");

                return newRoom;
            }
            finally
            {
                await _room.Release(hostID);
                await _user.Release(playerID);
            }
        }

        public async Task<UserModel> User(int userID)
        {
            return await _user.Get(userID);
        }

        public async Task<GameStatusModel> GameStatus(int hostID)
        {
            return await _gameStatus.Get(hostID);
        }
    }
}