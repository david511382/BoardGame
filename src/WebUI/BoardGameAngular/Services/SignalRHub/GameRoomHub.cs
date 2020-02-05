using BoardGameAngular.Models.SignalR;
using System.Threading.Tasks;

namespace BoardGameAngular.Services.SignalRHub
{
    public class GameRoomHub : ConnectionIdHub<IGameRoomHub>
    {
        public async Task GoInGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
