using Microsoft.Extensions.Configuration;

namespace BLL
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