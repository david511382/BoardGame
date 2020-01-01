using Domain.Api.Models.Base.Lobby;
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
        Task CreateRoom(int hostID, int gameID);
        Task<RoomModel> Room(int hostID);
        Task AddRoomPlayer(int hostID, int playerID);
        Task RemoveRoomPlayer(int playerID);

        Task<UserModel> User(int userID);
    }
}
