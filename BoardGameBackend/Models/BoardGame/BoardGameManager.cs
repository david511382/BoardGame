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
        private static List<GameRoom> _gameRooms;
        private static Dictionary<int, PlayerInfo> _players;

        private static int _newGameRoomId;

        static BoardGameManager()
        {
            _gameRooms = new List<GameRoom>();
            _players = new Dictionary<int, PlayerInfo>();
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
            _players.Add(player.Id, player);

            return player;
        }

        public static PlayerInfo GetPlayerById(int playerId)
        {
            try
            {
                return _players[playerId];
            }
            catch
            {
                throw new Exception("no player");
            }
        }

        public static BigTwoPlayer GetGamePlayerById(int playerId)
        {
            try
            {
                int roomId = GetPlayerById(playerId).RoomId;
                return (BigTwoPlayer)GetRoomById(roomId).GetGamePlayer(playerId);
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
                PlayerInfo player = GetPlayerById(playerId);
                GameRoom gameRoom = GetRoomById(player.RoomId);
                GamePlayer gp = gameRoom.GetGamePlayer(player.Id);

                return gp.GetGameTable();
            }
            catch
            {
                throw new Exception("not in room");
            }
        }

        public static PlayerInfo CreateGame(PlayerInfo host)
        {
            PlayerInfo player;
            try
            {
                player = GetPlayerById(host.Id);
            }
            catch (Exception e)
            {
                throw e;
            }

            if (player.IsInRoom)
                return player;

            _gameRooms.Add(NewGameRoom(ref player));

            return player;
        }

        public static PlayerInfo JoinGameRoom(PlayerInfo player, int roomId)
        {
            try
            {
                player = GetPlayerById(player.Id);
            }
            catch (Exception e)
            {
                throw e;
            }
            if (player.IsInRoom)
                return player;

            GameRoom gameRoom;
            try
            {
                gameRoom = GetRoomById(roomId);
                if (!gameRoom.AddPlayer(ref player))
                    return player;
            }
            catch
            {
                throw new Exception("no room");
            }

            return player;
        }

        public static PlayerInfo LeaveGameRoom(PlayerInfo player)
        {
            try
            {
                player = GetPlayerById(player.Id);
            }
            catch (Exception e)
            {
                throw e;
            }

            if (!player.IsInRoom)
                return player;


            int roomId = player.RoomId;
            GameRoom gameRoom;
            try
            {
                gameRoom = GetRoomById(roomId);
                gameRoom.LeavePlayer(ref player);
                return player;
            }
            catch
            {
                throw new Exception("no room");
            }
        }

        public static GameRoom[] GetGameRooms()
        {
            return _gameRooms.ToArray();
        }

        public static GameFramework.BoardGame GetGameById(int roomId)
        {
            try
            {
                return GetRoomById(roomId).GetGame();
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

        private static GameRoom NewGameRoom(ref PlayerInfo host)
        {
            BigTwo.BigTwo bigTwo = new BigTwo.BigTwo();

            return new GameRoom(bigTwo, _newGameRoomId++, ref host, BigTwo.BigTwo.MAX_PLAYERS, BigTwo.BigTwo.MIN_PLAYERS);
        }
    }
}