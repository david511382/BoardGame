using GameRespository.Models;
using RedisRepository.Models;
using System.Threading.Tasks;

namespace LobbyWebService.Services
{
    public interface IRedisService
    {
        Task<GameInfo[]> List();
        Task<GameInfo> Game(int ID);
        Task AddGames(GameInfo[] games);

        Task CreateRoom(int hostID, int gameID);
        Task<RedisRoomModel> Room(int hostID);
        Task AddRoomPlayer(int hostID, int playerID);
        Task RemoveRoomPlayer(int playerID);

        Task<UserModel> User(int userID);
    }
}
