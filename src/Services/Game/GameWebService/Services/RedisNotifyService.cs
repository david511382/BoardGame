namespace GameWebService.Services
{
    class RedisNotifyService
    {

        IRedisService _redisService;

        public RedisNotifyService(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public void Run()
        {
            _redisService.SubscribeInitGame();
        }
    }
}