using RedisRepository.Models;
using System.Threading.Tasks;

namespace LobbyWebService.Services
{
    public interface IRedisService
    {
        Task<GameModel[]> ListGames();
        Task<GameModel> Game(int ID);
        Task AddGames(GameModel[] games);

        Task<RoomModel[]> ListRooms();
        Task<RoomModel> CreateRoom(UserInfoModel info, int gameID);
        Task<RoomModel> Room(int hostID);
        Task<RoomModel> AddRoomPlayer(int hostID, UserInfoModel info);
        Task RemoveRoomPlayer(int playerID);
        Task StartRoom(int hostID);

        Task<GameStatusModel> GameStatus(int hostID);

        Task<UserModel> User(int userID);
    }
}
