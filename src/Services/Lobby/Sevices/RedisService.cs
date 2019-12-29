using GameRespository.Models;
using RedisRepository;
using RedisRepository.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LobbyWebService.Services
{
    public class RedisService : IRedisService
    {
        private RedisDAL _dal;

        public RedisService(string connectStr)
        {
            _dal = new RedisDAL(connectStr);
        }

        public async Task<GameInfo> Game(int ID)
        {
            return await _dal.Game(ID);
        }

        public async Task<GameInfo[]> List()
        {
            return await _dal.List();
        }

        public async Task AddGames(GameInfo[] games)
        {
            await _dal.AddGames(games);
        }

        public async Task CreateRoom(int hostID, GameInfo game)
        {
            ITransaction tran = _dal.Begin();

            RedisRoomModel room = new RedisRoomModel();
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

        public async Task<RedisRoomModel> Room(int hostID)
        {
            return await _dal.Room(hostID);
        }

        public async Task AddRoomPlayer(int hostID, int playerID)
        {
            ITransaction tran = _dal.Begin();

            RedisRoomModel oriRoom = await Room(hostID);
            List<int> playerIDs = oriRoom.PlayerIDs.ToList();
            playerIDs.Add(playerID);
            RedisRoomModel newRoom = new RedisRoomModel
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

        public async Task RemoveRoomPlayer(int hostID, int playerID)
        {
            ITransaction tran = _dal.Begin();

            List<int> removeList = new List<int>();
            RedisRoomModel oriRoom = await Room(hostID);
            bool isHost = hostID == playerID;
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
                _ = _dal.DeleteRoom(hostID, tran);
            else
            {
                List<int> newPlayerIDs = oriRoom.PlayerIDs.ToList();
                newPlayerIDs.Remove(playerID);
                RedisRoomModel newRoom = new RedisRoomModel
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

        public async Task<UserModel> User(int userID)
        {
            return await _dal.User(userID);
        }
    }
}