using Domain.Logger;
using Microsoft.Extensions.Logging;
using RedisRepository;
using RedisRepository.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebService.Services
{
    public class RedisService : IRedisService
    {
        private const int TRY_LOCK_TIMES = 5;
        private const int WAIT_LOCK_MS = 50;

        private readonly ILogger _logger;
        private readonly IGameService _gameService;

        private UserKey _user => _dal.User;
        private RoomKey _room => _dal.Room;
        private GameKey _game => _dal.Game;
        private ChannelStatusManager _channel => _dal.ChannelManager;
        private GameStatusKey _gameStatus => _dal.GameStatus;
        private RedisContext _dal;

        private static async Task<bool> Retry(int times, Func<Task<bool>> tryThing, int delayMs = 0)
        {
            if (times == 0)
                return false;

            if (await tryThing())
                return true;

            Thread.Sleep(delayMs);

            return await Retry(times - 1, tryThing);
        }

        public RedisService(string connectStr, IGameService gameService, ILogger<RedisService> logger)
        {
            _gameService = gameService;
            _logger = logger;
            _dal = new RedisContext(connectStr);
        }

        public async Task<GameStatusModel> GameStatus(int hostID)
        {
            return await _gameStatus.Get(hostID);
        }

        public async Task SubscribeInitGame()
        {
            const Channel CHANNEL = Channel.InitGame;

            await _dal.Subscribe(CHANNEL, async (channel, msg) =>
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
                       GameStatusModel currentGameStatus = await GameStatus(hostID);

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
        }
    }
}