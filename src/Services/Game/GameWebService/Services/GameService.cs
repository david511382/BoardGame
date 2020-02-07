using BigTwoLogic;
using GameLogic.Game;
using GameWebService.Domain;
using RedisRepository.Models;
using System;
using System.Linq;

namespace GameWebService.Services
{
    public class GameService : IGameService
    {
        public GameStatusModel InitGame(GameStatusModel gameStatus)
        {
            IBoardGame game = LoadGame(gameStatus);

            foreach (int pId in gameStatus.Room.Players.Select((p) => p.ID).ToArray())
                game.Join(pId);
            game.StartGame();

            gameStatus.DataJson = game.ExportData();

            return gameStatus;
        }

        public IBoardGame LoadGame(GameStatusModel gameStatus)
        {
            IBoardGame game;
            switch ((GameEnum)gameStatus.Room.Game.ID)
            {
                case GameEnum.BigTwo:
                    game = new BigTwo();
                    break;
                default:
                    throw new Exception("undefind game");
            }

            if (!string.IsNullOrEmpty(gameStatus.DataJson))
                game.Load(gameStatus.DataJson);

            return game;
        }
    }
}