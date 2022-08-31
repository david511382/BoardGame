using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Structs;
using GameWebService.Domain;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BLL
{
    public class LobbyRoom : ILobbyRoom
    {
        public static bool IsHost(int playerID, int roomID) => playerID == roomID;

        private const int TRY_LOCK_TIMES = 3;
        private const int WAIT_LOCK_MS = 50;

        private readonly IUserKey _user;
        private readonly IRoomKey _room;
        private readonly IGameStatusKey _gameStatus;
        private readonly ILobbyUser _lobbyUserBll;
        private readonly IGameService _gameBll;

        private static async Task<bool> Retry(int times, Func<Task<bool>> tryThing, int delayMs = 0)
        {
            if (times == 0)
                return false;

            if (await tryThing())
                return true;

            Thread.Sleep(delayMs);

            return await Retry(times - 1, tryThing);
        }

        public LobbyRoom(
            IUserKey userDal,
            IRoomKey roomDal,
            IGameStatusKey gameStatusDal,
            ILobbyUser lobbyUserBll,
            IGameService gameBll)
        {
            _user = userDal;
            _room = roomDal;
            _gameStatus = gameStatusDal;
            _lobbyUserBll = lobbyUserBll;
            _gameBll = gameBll;
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

                Task<LobbyUserStatus> getUserTask = _lobbyUserBll.GetUser(hostID);
                Task<GameModel> getGameTask = _gameBll.Game(gameID);

                LobbyUserStatus currentUser = null;
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
                    currentUser = new LobbyUserStatus
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

                ITransaction tran = _room.Begin();

                RoomModel room = new RoomModel();
                room.Game = game;
                room.HostID = hostID;
                room.Players = new UserInfoModel[] { currentUser.UserInfo };
                _ = _room.Set(room, tran);

                LobbyUserStatus user = new LobbyUserStatus
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

                Task<LobbyUserStatus> getUserTask = _lobbyUserBll.GetUser(playerID);
                Task<RoomModel> getRoomTask = Room(hostID);

                LobbyUserStatus currentUser = null;
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
                    currentUser = new LobbyUserStatus
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

                ITransaction tran = _room.Begin();

                List<UserInfoModel> players = oriRoom.Players.ToList();
                players.Add(currentUser.UserInfo);
                RoomModel newRoom = new RoomModel
                {
                    Game = oriRoom.Game,
                    HostID = oriRoom.HostID,
                    Players = players.ToArray()
                };
                _ = _room.Set(newRoom, tran);

                LobbyUserStatus user = new LobbyUserStatus
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

        public async Task<RoomModel> LeaveRoom(int playerID)
        {
            return await updateRoomPlayer(playerID);
        }

        public async Task<RoomModel> StartRoom(int playerID)
        {
            int startRoomID = -playerID;
            return await updateRoomPlayer(playerID, startRoomID);
        }

        public async Task<GameStatus> GameStatus(int hostID)
        {
            GameStatusModel redisGameStatus = await _gameStatus.Get(hostID);
            return new GameStatus(redisGameStatus);
        }

        public async Task SaveGameStatus(GameStatus roomGameData)
        {
            await _gameStatus.Set(roomGameData);
        }

        public async Task ClearGameStatus(int roomID)
        {
            await _gameStatus.Delete(roomID);
        }

        public async Task<bool> GameOver(IEnumerable<int> playerIds)
        {
            try
            {
                foreach (int id in playerIds)
                    if (!await Retry(TRY_LOCK_TIMES, () => _user.Lock(id), WAIT_LOCK_MS))
                        return false;

                ITransaction tran = _user.Begin();
                int? targetRoomID = null;
                Task<LobbyUserStatus>[] getUsers = playerIds.Select((id) => _user.Get(id))
                      .Select(async (t) =>
                      {
                          LobbyUserStatus u = new LobbyUserStatus(await t);
                          var currentRoomID = u.RoomID;
                          if (targetRoomID!=null)
                          {
                              if (targetRoomID != currentRoomID)
                                  throw new Exception("房間不一致");
                          }
                          else
                          {
                              targetRoomID = currentRoomID;
                          }
                          u.GameRoomID = null;
                          return u;
                      }).ToArray();
                if (targetRoomID == null) 
                    throw new Exception("沒可關閉的遊戲");

                foreach (Task<LobbyUserStatus> getUser in getUsers)
                {
                    UserModel u = await getUser;
                    _ = _user.Set(u, tran);
                }

                if (!await tran.ExecuteAsync())
                    return false;

                //刪除 gamestatus
                await _gameStatus.Delete(targetRoomID.Value);
                return true;
            }
            finally
            {
                foreach (int id in playerIds)
                    await _user.Release(id);
            }
        }

        private async Task<RoomModel> updateRoomPlayer(int playerID, int? newGameRoomID = null)
        {
            List<int> lockedPlayerIDList = new List<int>();
            lockedPlayerIDList.Add(playerID);
            try
            {
                if (!await Retry(TRY_LOCK_TIMES, () => _user.Lock(playerID), WAIT_LOCK_MS))
                    throw new Exception("LockUser Fail");

                LobbyUserStatus currentUser = null;
                try
                {
                    currentUser = await _lobbyUserBll.GetUser(playerID);
                }
                catch
                {
                    throw new Exception("不在任何房間");
                }

                int roomID = currentUser.RoomID;
                if (!await Retry(TRY_LOCK_TIMES, () => _room.Lock(roomID), WAIT_LOCK_MS))
                    throw new Exception("LockRoom Fail");

                RoomModel oriRoom = await _room.Get(roomID);

                List<LobbyUserStatus> roomOtherPlayers = new List<LobbyUserStatus>();
                List<LobbyUserStatus> roomPlayers = new List<LobbyUserStatus>();
                roomPlayers.Add(currentUser);
                bool isHost = IsHost(playerID, roomID);
                if (isHost)
                {
                    foreach (UserInfoModel player in oriRoom.Players)
                    {
                        if (player.ID == playerID)
                            continue;

                        lockedPlayerIDList.Add(player.ID);
                        if (!await Retry(TRY_LOCK_TIMES, () => _user.Lock(player.ID), WAIT_LOCK_MS))
                            throw new Exception("LockUser Fail");
                        LobbyUserStatus u = await _lobbyUserBll.GetUser(player.ID);
                        roomPlayers.Add(u);
                        roomOtherPlayers.Add(u);
                    }
                }

                // 更新玩家房間
                foreach (LobbyUserStatus roomPlayer in roomPlayers)
                {
                    roomPlayer.GameRoomID = newGameRoomID;
                }

                ITransaction tran = _user.Begin();
                foreach (LobbyUserStatus roomPlayer in roomPlayers)
                {
                    _ = _user.Set(roomPlayer, tran);
                }

                RoomModel newRoom = null;
                if (isHost)
                {
                    _ = _room.Delete(roomID, tran);
                    newRoom = new RoomModel
                    {
                        Game = oriRoom.Game,
                        HostID = oriRoom.HostID,
                        Players = null
                    };
                }
                else
                {
                    UserInfoModel[] newPlayers = roomOtherPlayers
                        .Select((p) => p.UserInfo)
                        .ToArray();
                    newRoom = new RoomModel
                    {
                        Game = oriRoom.Game,
                        HostID = oriRoom.HostID,
                        Players = newPlayers
                    };

                    _ = _room.Set(newRoom, tran);
                }

                if (!await tran.ExecuteAsync())
                    throw new Exception("ExecuteAsync Fail");

                return newRoom;
            }
            finally
            {
                foreach (int lockedPlayerID in lockedPlayerIDList)
                {
                    await _room.Release(lockedPlayerID);
                }
                await _user.Release(playerID);
            }
        }
    }

    public class GameStatus : GameStatusModel
    {
        public GameStatus(GameStatusModel data)
            : base(data)
        {
        }

        public GameEnum GameID => (GameEnum)Room.Game.ID;

        public bool IsGame(GameEnum gameID)
        {
            return GameID == gameID;
        }
    }
}