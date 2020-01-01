using Domain.Api.Models.Base.Lobby;
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

        private RedisDAL _dal;

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
            _dal = new RedisDAL(connectStr);
        }

        public async Task<GameModel> Game(int ID)
        {
            return await _dal.Game(ID);
        }

        public async Task<GameModel[]> ListGames()
        {
            return await _dal.ListGames();
        }

        public async Task AddGames(GameModel[] games)
        {
            await _dal.AddGames(games);
        }

        public async Task CreateRoom(int hostID, int gameID)
        {
            try
            {
                if (!await Retry(TRY_LOCK_TIMES, () => _dal.LockRoom(hostID), WAIT_LOCK_MS))
                    throw new Exception("LockRoom Fail");
                if (!await Retry(TRY_LOCK_TIMES, () => _dal.LockUser(hostID), WAIT_LOCK_MS))
                    throw new Exception("LockUser Fail");

                Task<UserModel> getUserTask = User(hostID);
                Task<GameModel> getGameTask = Game(gameID);

                UserModel userInfo = null;
                try
                {
                    userInfo = await getUserTask;
                }
                catch { }
                if (userInfo?.RoomID != null)
                    throw new Exception("已加入其他房間");

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
                room.PlayerIDs = new int[] { hostID };
                _ = _dal.SetRoom(room, tran);

                UserModel user = new UserModel
                {
                    UserID = hostID,
                    RoomID = hostID
                };
                _ = _dal.SetUser(user, tran);

                if (!await tran.ExecuteAsync())
                    throw new Exception("ExecuteAsync Fail");
            }
            finally
            {
                await _dal.ReleaseRoom(hostID);
                await _dal.ReleaseUser(hostID);
            }
        }

        public async Task<RoomModel[]> ListRooms()
        {
            return await _dal.ListRooms();
        }

        public async Task<RoomModel> Room(int hostID)
        {
            return await _dal.Room(hostID);
        }

        public async Task AddRoomPlayer(int hostID, int playerID)
        {
            try
            {
                if (!await Retry(TRY_LOCK_TIMES, () => _dal.LockRoom(hostID), WAIT_LOCK_MS))
                    throw new Exception("LockRoom Fail");
                if (!await Retry(TRY_LOCK_TIMES, () => _dal.LockUser(playerID), WAIT_LOCK_MS))
                    throw new Exception("LockUser Fail");

                Task<UserModel> getUserTask = User(playerID);
                Task<RoomModel> getRoomTask = Room(hostID);

                UserModel userInfo = null;
                try
                {
                    userInfo = await getUserTask;
                }
                catch { }
                if (userInfo?.RoomID != null)
                    throw new Exception("已加入其他房間");

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

                List<int> playerIDs = oriRoom.PlayerIDs.ToList();
                playerIDs.Add(playerID);
                RoomModel newRoom = new RoomModel
                {
                    Game = oriRoom.Game,
                    HostID = oriRoom.HostID,
                    PlayerIDs = playerIDs.ToArray()
                };
                _ = _dal.SetRoom(newRoom, tran);

                UserModel user = new UserModel
                {
                    UserID = playerID,
                    RoomID = hostID
                };
                _ = _dal.SetUser(user, tran);

                if (!await tran.ExecuteAsync())
                    throw new Exception("ExecuteAsync Fail");
            }
            finally
            {
                await _dal.ReleaseRoom(hostID);
                await _dal.ReleaseUser(playerID);
            }
        }

        public async Task RemoveRoomPlayer(int playerID)
        {
            int roomID = 0;
            try
            {
                if (!await Retry(TRY_LOCK_TIMES, () => _dal.LockUser(playerID), WAIT_LOCK_MS))
                    throw new Exception("LockUser Fail");

                UserModel userInfo = null;
                try
                {
                    userInfo = await User(playerID);
                    if (userInfo.RoomID == null)
                        throw new Exception();
                }
                catch
                {
                    throw new Exception("不在任何房間");
                }

                roomID = userInfo.RoomID.Value;
                if (!await Retry(TRY_LOCK_TIMES, () => _dal.LockRoom(roomID), WAIT_LOCK_MS))
                    throw new Exception("LockRoom Fail");

                ITransaction tran = _dal.Begin();

                List<int> removeList = new List<int>();
                RoomModel oriRoom = await Room(roomID);
                bool isHost = roomID == playerID;
                if (isHost)
                    removeList = oriRoom.PlayerIDs.ToList();
                else
                    removeList.Add(playerID);

                _ = removeList.Select((id) => User(id))
                    .Select(async (t) =>
                    {
                        UserModel u = await t;
                        u.RoomID = null;
                        return _dal.SetUser(u, tran);
                    }).ToArray();

                if (isHost)
                    _ = _dal.DeleteRoom(roomID, tran);
                else
                {
                    List<int> newPlayerIDs = oriRoom.PlayerIDs.ToList();
                    newPlayerIDs.Remove(playerID);
                    RoomModel newRoom = new RoomModel
                    {
                        Game = oriRoom.Game,
                        HostID = oriRoom.HostID,
                        PlayerIDs = newPlayerIDs.ToArray()
                    };

                    _ = _dal.SetRoom(newRoom, tran);
                }

                if (!await tran.ExecuteAsync())
                    throw new Exception("ExecuteAsync Fail");
            }
            finally
            {
                await _dal.ReleaseRoom(roomID);
                await _dal.ReleaseUser(playerID);
            }
        }

        public async Task<UserModel> User(int userID)
        {
            return await _dal.User(userID);
        }
    }
}