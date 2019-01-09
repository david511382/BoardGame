using BoardGame.Backend.Models.BoardGame.BigTwo;
using BoardGame.Backend.Models.BoardGame.GameFramework;
using BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer;
using BoardGame.Backend.Models.GameLobby;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame
{
    public static class BoardGameManager
    {
        private static List<GameFramework.BoardGame> _games;
        private static List<GameRoom> _gameRooms;
        private static List<GamePlayer> _gamePlayers;

        private static int _newGameRoomId;

        static BoardGameManager()
        {
            _gameRooms = new List<GameRoom>();
            _games = new List<GameFramework.BoardGame>();
            _gamePlayers = new List<GamePlayer>();
            _newGameRoomId = 0;

            //CreateGame(Register());
            //JoinGameRoom(Register(),0);
            //JoinGameRoom(Register(),0);
            //JoinGameRoom(Register(), 0);
            //StartGame();
        }

        public static PlayerInfo Register()
        {
            PlayerInfo player = new GamePlayer().Info;
            BigTwoPlayer gamePlayer = new BigTwoPlayer(player);
            _gamePlayers.Add(gamePlayer);

            return player;
        }

        public static BigTwoPlayer GetPlayerById(int playerId)
        {
            try
            {
                return (BigTwoPlayer)_gamePlayers
                    .Where(d => d.Id == playerId)
                    .First();
            }
            catch
            {
                throw new Exception("no player");
            }
        }

        public static GameBoard GetGameBoardByPlayerId(int playerId)
        {
            try
            {
                 GamePlayer gp = _gamePlayers
                    .Where(d => d.Id == playerId)
                    .First();

                return gp.GetGameTable();
            }
            catch
            {
                throw new Exception("not in room");
            }
        }

        public static PlayerInfo CreateGame(PlayerInfo host)
        {
            GamePlayer gamePlayer;
            try
            {
                gamePlayer = GetPlayerById(host.Id);
                host = gamePlayer.Info;
            }
            catch(Exception e)
            {
                throw e;
            }

            if (host.IsInRoom)
                return host;
            
            BigTwo.BigTwo bigTwo = new BigTwo.BigTwo();
            _games.Add(bigTwo);

            _gameRooms.Add(NewGameRoom(host));

            gamePlayer.JoinGame(_games.Last());

            return host;
        }

        public static PlayerInfo JoinGameRoom(PlayerInfo player, int roomId)
        {
            GameRoom gameRoom;
            try
            {
                gameRoom = GetRoomById(roomId);
                if (!gameRoom.AddPlayer(player))
                    return player;
            }
            catch
            {
                throw new Exception("no room");
            }

            BigTwoPlayer gamePlayer;
            try
            {
                gamePlayer = GetPlayerById(player.Id);
            }
            catch (Exception e)
            {
                throw e;
            }

            player = gamePlayer.Info;
            if (player.IsInRoom)
                return player;

            gamePlayer.JoinGame(_games.Last());

            player.JoinRoom(gameRoom.RoomId);

            return player;
        }

        public static GameRoom[] GetGameRooms()
        {
            return _gameRooms.ToArray();
        }

        public static GameFramework.BoardGame GetGameById(int gameId)
        {
            try
            {
                return _games[gameId];
            }
            catch
            {
                throw new Exception("no game");
            }
        }

        public static GameRoom GetRoomById(int roomId)
        {
            try
            {
                return _gameRooms
                    .Where(d => d.RoomId == roomId)
                    .First();
            }
            catch
            {
                throw new Exception("no room");
            }
        }

        public static bool StartGame()
        {
            try
            {
                _games.Last().StartGame();
                return true;
            }
            catch { return false; }
        }

        private static GameRoom NewGameRoom(PlayerInfo host)
        {
            return new GameRoom(_newGameRoomId++, host, BigTwo.BigTwo.MAX_PLAYERS, BigTwo.BigTwo.MIN_PLAYERS);
        }
    }
}