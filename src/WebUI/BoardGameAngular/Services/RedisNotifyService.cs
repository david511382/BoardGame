using BoardGameAngular.Models.SignalR;
using BoardGameAngular.Services.SignalRHub;
using Domain.Logger;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedisRepository;
using RedisRepository.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebService.Services
{
    public class RedisNotifyService : IHostedService
    {
        private const int TRY_LOCK_TIMES = 5;
        private const int WAIT_LOCK_MS = 50;

        private readonly IHubContext<GameRoomHub, IGameRoomHub> _gameRoomHub;

        private UserKey _user => _redis.User;
        private RoomKey _room => _redis.Room;
        private GameKey _game => _redis.Game;
        private ChannelStatusManager _channel => _redis.ChannelManager;
        private GameStatusKey _gameStatus => _redis.GameStatus;
        private readonly RedisContext _redis;

        private readonly ILogger _logger;

        public RedisNotifyService(IConfiguration configuration, IHubContext<GameRoomHub, IGameRoomHub> gameRoomHub, ILogger<RedisNotifyService> logger)
        {
            string connectStr = configuration.GetConnectionString("Redis");
            _redis = new RedisContext(connectStr);
            _gameRoomHub = gameRoomHub;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await subscribeStartGame();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // do nothinig
            return Task.CompletedTask;
        }

        private Task subscribeStartGame()
        {
            const Channel CHANNEL = Channel.StartGameDone;
            Task.Run(async () =>
            {
                await _redis.Subscribe(CHANNEL, async (channel, msg) =>
                {
                    StartGameDoneModel msgData;
                    try
                    {
                        msgData = JsonConvert.DeserializeObject<StartGameDoneModel>(msg);
                    }
                    catch
                    {
                        _logger.Info(new LoggerEvent($"{channel},{msg} msg parse int fail"));
                        return;
                    }

                    string groupId = msgData.HostID.ToString();
                    await _gameRoomHub.Clients.Group(groupId).GameStarted();
                });
            });
            return Task.CompletedTask;
        }

        private static async Task<bool> Retry(int times, Func<Task<bool>> tryThing, int delayMs = 0)
        {
            if (times == 0)
                return false;

            if (await tryThing())
                return true;

            Thread.Sleep(delayMs);

            return await Retry(times - 1, tryThing);
        }
    }
}