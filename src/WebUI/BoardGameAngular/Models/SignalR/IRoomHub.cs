using Domain.Api.Models.Base.Lobby;
using System.Threading.Tasks;

namespace BoardGameAngular.Models.SignalR
{
    public interface IRoomHub : IConnectionHub
    {
        Task RoomOpened();
        Task RoomPlayerChanged(RoomModel roomData);
        Task RoomStarted(int gameId);
        Task RoomClose();
    }
}
