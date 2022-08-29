using BigTwoLogic;
using DAL.Structs;
using GameLogic.Game;
using GameWebService.Domain;
using System;
using System.Linq;

namespace Services.Game
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