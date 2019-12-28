using GameRespository.Models;
using RedisRepository;
using RedisRepository.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LobbyWebService.Services
{
    public class RedisService : IRedisService
    {
        private GameDAL _gameDAL;
        private RoomDAL _roomDAL;
        private UserDAL _userDAL;

        public RedisService(string connectStr)
        {
            _gameDAL = new GameDAL(connectStr);
            _roomDAL = new RoomDAL(connectStr);
            _userDAL = new UserDAL(connectStr);
        }

        public async Task<GameInfo> Game(int ID)
        {
            return await _gameDAL.Game(ID);
        }

        public async Task<GameInfo[]> List()
        {
            return await _gameDAL.List();
        }

        public async Task AddGames(GameInfo[] games)
        {
            await _gameDAL.AddGames(games);
        }

        public async Task CreateRoom(int hostID, GameInfo game)
        {
            RedisRoomModel room = new RedisRoomModel();
            room.Game = game;
            room.HostID = hostID;
            room.PlayerIDs = new int[] { hostID };
            Task createRoomTask = _roomDAL.SetRoom(room);

            UserModel oriUser = new UserModel
            {
                UserID = hostID,
                RoomID = null
            };
            try
            {
                oriUser = await _userDAL.User(hostID);
            }
            catch { }
            UserModel user = new UserModel
            {
                UserID = oriUser.UserID,
                RoomID = hostID
            };
            Task addUserTask = _userDAL.AddUser(user);

            await addUserTask;
            try
            {
                await createRoomTask;
            }
            catch
            {
                await _userDAL.AddUser(oriUser);
                throw;
            }
        }

        public async Task<RedisRoomModel> Room(int hostID)
        {
            return await _roomDAL.Room(hostID);
        }

        public async Task AddRoomPlayer(int hostID, int playerID)
        {
            RedisRoomModel oriRoom = await Room(hostID);

            List<int> playerIDs = oriRoom.PlayerIDs.ToList();
            playerIDs.Add(playerID);
            RedisRoomModel newRoom = new RedisRoomModel
            {
                Game = oriRoom.Game,
                HostID = oriRoom.HostID,
                PlayerIDs = playerIDs.ToArray()
            };

            Task setRoomTask = _roomDAL.SetRoom(newRoom);

            //UserModel oriUser = await _userDAL.User(hostID);
            UserModel user = new UserModel
            {
                UserID = playerID,
                RoomID = hostID
            };
            Task addUserTask = _userDAL.AddUser(user);

            await setRoomTask;
            try
            {
                await addUserTask;
            }
            catch
            {
                await _roomDAL.SetRoom(oriRoom);
                throw;
            }
        }

        public async Task<UserModel> User(int userID)
        {
            return await _userDAL.User(userID);
        }
    }
}