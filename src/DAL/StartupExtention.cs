using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DAL
{
    public static class StartupExtention
    {
        public static IServiceCollection AddRedisContexts(this IServiceCollection services, Func<string> getConnectionAction)
        {
            return services
                .AddSingleton<IRedisContext>(new RedisContext(getConnectionAction()))
                .AddSingleton<IUserKey, UserKey>()
                .AddSingleton<IGameKey, GameKey>()
                .AddSingleton<IGameStatusKey, GameStatusKey>()
                .AddSingleton<IRoomKey, RoomKey>();
        }
    }
}
