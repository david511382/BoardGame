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

        public async Task<RoomModel> CreateRoom(UserInfoModel userInfo, int gameID)
        {
            int hostID = userInfo.ID;
            try
            {
                if (!await Retry(TRY_LOCK_TIMES, () => _dal.LockRoom(hostID), WAIT_LOCK_MS))
                    throw new Exception("LockRoom Fail");
                if (!await Retry(TRY_LOCK_TIMES, () => _dal.LockUser(hostID), WAIT_LOCK_MS))
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
                _ = _dal.SetRoom(room, tran);

                UserModel user = new UserModel
                {
                    UserInfo = currentUser.UserInfo,
                    GameRoomID = hostID
                };
                _ = _dal.SetUser(user, tran);

                if (!await tran.ExecuteAsync())
                    throw new Exception("ExecuteAsync Fail");

                return room;
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

        public async Task<RoomModel> AddRoomPlayer(int hostID, UserInfoModel userInfo)
        {
            int playerID = userInfo.ID;
            try
            {
                if (!await Retry(TRY_LOCK_TIMES, () => _dal.LockRoom(hostID), WAIT_LOCK_MS))
                    throw new Exception("LockRoom Fail");
                if (!await Retry(TRY_LOCK_TIMES, () => _dal.LockUser(playerID), WAIT_LOCK_MS))
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
                _ = _dal.SetRoom(newRoom, tran);

                UserModel user = new UserModel
                {
                    UserInfo = currentUser.UserInfo,
                    GameRoomID = hostID
                };
                _ = _dal.SetUser(user, tran);

                if (!await tran.ExecuteAsync())
                    throw new Exception("ExecuteAsync Fail");

                return newRoom;
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
                    if (userInfo.GameRoomID == null)
                        throw new Exception();
                }
                catch
                {
                    throw new Exception("不在任何房間");
                }

                roomID = userInfo.GameRoomID.Value;
                if (!await Retry(TRY_LOCK_TIMES, () => _dal.LockRoom(roomID), WAIT_LOCK_MS))
                    throw new Exception("LockRoom Fail");

                await removeRoomPlayer(roomID, userInfo);
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

        public async Task StartRoom(int hostID)
        {
            int roomID = hostID;
            RoomModel oriRoom = null;
            try
            {
                if (!await Retry(TRY_LOCK_TIMES, () => _dal.LockRoom(roomID), WAIT_LOCK_MS))
                    throw new Exception("LockRoom Fail");

                oriRoom = await Room(roomID);

                UserModel userInfo = null;
                try
                {
                    userInfo = await User(hostID);
                    if (userInfo.GameRoomID == null)
                        throw new Exception();
                }
                catch
                {
                    throw new Exception("不在任何房間");
                }

                foreach (UserInfoModel player in oriRoom.Players)
                    if (!await Retry(TRY_LOCK_TIMES, () => _dal.LockUser(player.ID), WAIT_LOCK_MS))
                        throw new Exception("LockUser Fail");

                if (!await _dal.SetGameStatus(new GameStatusModel
                {
                    Room = oriRoom
                }))
                    throw new Exception("SetGameStatus Fail");

                try
                {
                    await removeRoomPlayer(roomID, userInfo, -hostID);
                }
                catch
                {
                    await _dal.DeleteGameStatus(hostID);
                    throw;
                }
            }
            finally
            {
                await _dal.ReleaseRoom(roomID);

                if (oriRoom != null)
                    foreach (UserInfoModel player in oriRoom.Players)
                        await _dal.ReleaseUser(player.ID);
            }
        }

        public async Task<GameStatusModel> GameStatus(int hostID)
        {
            return await _dal.GameStatus(hostID);
        }

        private async Task removeRoomPlayer(int roomID, UserModel userInfo, int? GameRoomID = null)
        {
            ITransaction tran = _dal.Begin();

            List<UserInfoModel> removeList = new List<UserInfoModel>();
            RoomModel oriRoom = await Room(roomID);
            bool isHost = roomID == userInfo.UserInfo.ID;
            if (isHost)
                removeList = oriRoom.Players.ToList();
            else
                removeList.Add(userInfo.UserInfo);

            Task<UserModel>[] getUsers = removeList.Select((info) => User(info.ID))
                    .Select(async (t) =>
                    {
                        UserModel u = await t;
                        u.GameRoomID = GameRoomID;
                        return u;
                    }).ToArray();
            foreach (Task<UserModel> getUser in getUsers)
            {
                UserModel u = await getUser;
                _ = _dal.SetUser(u, tran);
            }

            if (isHost)
                _ = _dal.DeleteRoom(roomID, tran);
            else
            {
                UserInfoModel[] newPlayers = oriRoom.Players
                    .Where((p) => p.ID != userInfo.UserInfo.ID)
                    .ToArray();
                RoomModel newRoom = new RoomModel
                {
                    Game = oriRoom.Game,
                    HostID = oriRoom.HostID,
                    Players = newPlayers
                };

                _ = _dal.SetRoom(newRoom, tran);
            }

            if (!await tran.ExecuteAsync())
                throw new Exception("ExecuteAsync Fail");
        }
    }
}