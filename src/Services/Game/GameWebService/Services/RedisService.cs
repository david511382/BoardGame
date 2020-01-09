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

        public async Task UpdateGameStatus(GameStatusModel gameStatus)
        {
            int hostID = gameStatus.Room.HostID;
            try
            {
                if (!await Retry(TRY_LOCK_TIMES, () => _gameStatus.Lock(hostID), WAIT_LOCK_MS))
                    throw new Exception("LockGameStatus Fail");

                if (!await _gameStatus.Set(gameStatus))
                    throw new Exception("SetGameStatus Fail");
            }
            finally
            {
                await _gameStatus.Release(hostID);
            }
        }

        public async Task<GameStatusModel> GameStatus(int hostID)
        {
            return await _gameStatus.Get(hostID);
        }

        public async Task SubscribeInitGame()
        {
            await _dal.Subscribe(Channel.InitGame, async (channel, msg) =>
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
                       if (await _gameStatus.Lock(hostID))
                           return true;

                       if (await _gameStatus.GetGameStatusNotifyHandler(hostID))
                       {
                           otherApiHandling = true;
                           return true;
                       }

                       return false;
                   }, WAIT_LOCK_MS))
                       throw new Exception("LockGameStatus Fail");
                   if (otherApiHandling)
                       return;

                   if (!await Retry(TRY_LOCK_TIMES, () => _gameStatus.SetGameStatusNotifyHandler(hostID), 0))
                       throw new Exception("SetGameStatusNotifyHandler Fail");

                   GameStatusModel currentGameStatus = await GameStatus(hostID);

                   GameStatusModel newGameStatus = _gameService.InitGame(currentGameStatus);

                   if (!await _gameStatus.Set(newGameStatus))
                       throw new Exception("Update Fail");
               }
               catch (Exception e)
               {
                   _logger.Info(new LoggerEvent(e.Message));
               }
               finally
               {
                   await _gameStatus.DeleteGameStatusNotifyHandler(hostID.ToString(), hostID);
                   await _gameStatus.Release(hostID);
               }
           });
        }
    }
}