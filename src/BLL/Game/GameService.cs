using BigTwoLogic;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Structs;
using GameLogic.Game;
using GameWebService.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL
{
    public class GameService : IGameService
    {
        private readonly IGameInfoDAL _db;
        private readonly IGameKey _gameDal;
        private readonly ILogger _logger;

        public GameService(
            IGameInfoDAL db,
            IGameKey gameDal,
            ILogger<GameService> logger)
        {
            _db = db;
            _gameDal = gameDal;
            _logger = logger;
        }

        public async Task<GameModel> Game(int id)
        {
            return await _gameDal.Get(id);
        }

        public async Task<IEnumerable<GameModel>> GameList()
        {
            GameModel[] games = await _gameDal.ListGames();
            if (games.Length == 0)
            {
                games = (await _db.List())
                .Select((g) => new GameModel
                {
                    ID = g.ID,
                    Description = g.Description,
                    MaxPlayerCount = g.MaxPlayerCount,
                    MinPlayerCount = g.MinPlayerCount,
                    Name = g.Name
                })
               .ToArray();

                try
                {
                    _ = _gameDal.AddGames(games);
                }
                catch
                {
                    _logger.LogWarning("add db games to redis fail");
                }
            }
            return games;
        }

        public GameStatusModel StartGame(GameStatusModel gameStatus)
        {
            IBoardGame game = LoadGame(gameStatus);

            foreach (int pId in gameStatus.Room.Players.Select((p) => p.ID).ToArray())
                game.Join(pId);
            game.StartGame();

            gameStatus.DataJson = game.ExportData();

            return gameStatus;
        }

        public GameStatus InitGame(GameStatusModel gameStatus)
        {
            IBoardGame game = LoadGame(gameStatus);

            foreach (int pId in gameStatus.Room.Players.Select((p) => p.ID).ToArray())
                game.Join(pId);
            game.StartGame();

            gameStatus.DataJson = game.ExportData();

            return new GameStatus(gameStatus);
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