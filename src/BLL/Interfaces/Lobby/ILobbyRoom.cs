using DAL.Structs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ILobbyRoom
    {
        Task<RoomModel[]> ListRooms();
        Task<RoomModel> CreateRoom(UserInfoModel info, int gameID);
        Task<RoomModel> Room(int hostID);
        Task<RoomModel> AddRoomPlayer(int hostID, UserInfoModel info);
        Task<RoomModel> LeaveRoom(int playerID);
        Task<RoomModel> StartRoom(int playerID);

        Task<GameStatus> GameStatus(int hostID);
        Task SaveGameStatus(GameStatus roomGameData);
        Task ClearGameStatus(int roomID);
        Task<bool> GameOver(IEnumerable<int> playerIds);
    }
}
