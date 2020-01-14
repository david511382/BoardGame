using BoardGameAngular.Models.SignalR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace BoardGameAngular.Services
{
    // HubName 一定要小寫開頭。
    // 如果沒有指定 HubName，第一個字元會被自動轉為小寫。例：ChatHub => chatHub
    public class RoomHub : Hub<IRoomHub>
    {
        public async Task GetConnectionId()
        {
            await Clients.Caller.SetConnectionId(Context.ConnectionId);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
