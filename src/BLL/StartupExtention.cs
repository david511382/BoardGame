using BLL.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public static class StartupExtention
    {
        public static IServiceCollection AddLogicServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<ILobbyUser, LobbyUser>()
                .AddSingleton<ILobbyRoom, LobbyRoom>(); 
        }
    }
}