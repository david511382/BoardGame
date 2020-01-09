using BigTwoLogic;
using GameLogic.Game;
using RedisRepository.Models;
using System;
using System.Linq;

namespace GameWebService.Services
{
    public class GameService : IGameService
    {
        public GameStatusModel InitGame(GameStatusModel gameStatus)
        {
            BoardGame game;
            switch (gameStatus.Room.Game.ID)
            {
                case 2:
                    game = new BigTwo();
                    break;
                default:
                    throw new Exception("undefind game");
            }

            foreach (int pId in gameStatus.Room.Players.Select((p) => p.ID).ToArray())
                game.Join(pId);
            game.StartGame();

            gameStatus.DataJson = game.ExportData();

            return gameStatus;
        }
    }
}