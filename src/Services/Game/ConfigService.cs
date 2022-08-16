using Microsoft.Extensions.Configuration;

namespace Services.Game
{
    public class ConfigService
    {
        public readonly string RedisConnectString;

        public ConfigService(IConfiguration Configuration)
        {

            RedisConnectString = Configuration.GetConnectionString("Redis");
        }
    }
}