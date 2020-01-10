using Domain.Logger;
using Microsoft.Extensions.Logging;
using RedisRepository;
using RedisRepository.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebService.Services
{
    public class RedisNotifyService
    {
        private const int TRY_LOCK_TIMES = 5;
        private const int WAIT_LOCK_MS = 50;

        private UserKey _user => _redis.User;
        private RoomKey _room => _redis.Room;
        private GameKey _game => _redis.Game;
        private ChannelStatusManager _channel => _redis.ChannelManager;
        private GameStatusKey _gameStatus => _redis.GameStatus;
        private readonly RedisContext _redis;

        private readonly ILogger _logger;
        private readonly IGameService _gameService;

        public RedisNotifyService(ConfigService configService, IGameService gameService, ILogger<RedisNotifyService> logger)
        {
            _redis = new RedisContext(configService.RedisConnectString);
            _gameService = gameService;
            _logger = logger;
        }

        public void Run()
        {
            subscribeInitGame();
        }

        private Task subscribeInitGame()
        {
            const Channel CHANNEL = Channel.InitGame;
            Task.Run(async () =>
            {
                await _redis.Subscribe(CHANNEL, async (channel, msg) =>
                {
                    int hostID;
                    try
                    {
                        hostID = int.Parse(msg);
                    }
                    catch
                    {
                        _logger.Info(new LoggerEvent($"{channel},{msg} msg parse int fail"));
                        return;
                    }

                    try
                    {
                        bool otherApiHandling = false;
                        if (!await Retry(TRY_LOCK_TIMES, async () =>
                        {
                            bool isLockSuccess = await _gameStatus.Lock(hostID);
                            bool isWaitingForHandle = await _channel.IsWaitingForHandle(CHANNEL, hostID);

                            if (isWaitingForHandle && isLockSuccess)
                                return true;

                            if (!isWaitingForHandle)
                            {
                                otherApiHandling = true;
                                return true;
                            }

                            return false;
                        }, WAIT_LOCK_MS))
                            throw new Exception("LockGameStatus Fail");
                        if (otherApiHandling)
                            return;
                        await _channel.Handle(CHANNEL, hostID);

                        try
                        {
                            GameStatusModel currentGameStatus = await _gameStatus.Get(hostID);

                            GameStatusModel newGameStatus = _gameService.InitGame(currentGameStatus);

                            await _gameStatus.Set(newGameStatus);
                        }
                        catch
                        {
                            await _gameStatus.Delete(hostID);
                            throw;
                        }
                        finally
                        {
                            await _channel.Done(CHANNEL, hostID);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Info(new LoggerEvent(e.Message));
                    }
                    finally
                    {
                        await _gameStatus.Release(hostID);
                    }
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