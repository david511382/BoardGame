using BigTwoLogic;
using Newtonsoft.Json;
using RedisRepository.Models;
using System.Linq;

namespace GameWebService.Services
{
    public class GameService : IGameService
    {
        public GameStatusModel InitGame(GameStatusModel gameStatus)
        {
            BigTwo bigTwo = new BigTwo();

            foreach (int pId in gameStatus.Room.Players.Select((p) => p.ID).ToArray())
                bigTwo.Join(pId);
            bigTwo.StartGame();

            gameStatus.DataJson = JsonConvert.SerializeObject(bigTwo);

            return gameStatus;
        }
    }
}