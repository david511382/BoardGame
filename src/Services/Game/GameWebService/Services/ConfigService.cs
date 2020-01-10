using Microsoft.Extensions.Configuration;

namespace GameWebService.Services
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