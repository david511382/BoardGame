﻿using BoardGame.Backend.Models.BoardGame.BigTwo;
using BoardGame.Backend.Models.BoardGame.GameFramework;
using BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer;
using BoardGame.Backend.Models.GameLobby;
using BoardGameBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame
{
    public static class BoardGameManager
    {
        public static PlayerManager _playerManager;

        private static Dictionary<int,GameRoom<BigTwo.BigTwo,BigTwoPlayer>> _gameRooms;

        private static int _newGameRoomId;

        static BoardGameManager()
        {
            _gameRooms =new Dictionary<int, GameRoom<BigTwo.BigTwo, BigTwoPlayer>>();
            _playerManager = new PlayerManager();
            _newGameRoomId = 0;

            //CreateGame(Register());
            //JoinGameRoom(Register(), 0);
            //JoinGameRoom(Register(), 0);
            //JoinGameRoom(Register(), 0);
            //StartGame(_playerManager[1]);
        }

        public static PlayerInfo Register()
        {
            PlayerInfo player = new GamePlayer().Info;
            _playerManager.AddPlayer(player);

            return player;
        }

        public static PlayerInfo GetPlayerById(int playerId)
        {
            try
            {
                return _playerManager.GetPlayer(playerId);
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
                return GetRoomById(roomId)
                    .GetGamePlayer(playerId);
            }
            catch (Exception e)
            {
                throw new Exception("no player");
            }
        }

        public static GameBoard GetGameBoardByPlayerId(int playerId)
        {
            try
            {
                PlayerInfo player = GetPlayerById(playerId);
                GameRoom<BigTwo.BigTwo, BigTwoPlayer> gameRoom = GetRoomById(player.RoomId);
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

            GameRoom<BigTwo.BigTwo,BigTwoPlayer> gameRoom = NewGameRoom(ref player);
            _gameRooms.Add(gameRoom.RoomId, gameRoom);

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

            GameRoom<BigTwo.BigTwo, BigTwoPlayer> gameRoom;
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

        public static PlayerInfo[] LeaveGameRoom(PlayerInfo player)
        {
            try
            {
                player = GetPlayerById(player.Id);

                if (!player.IsInRoom)
                    throw new Exception();
            }
            catch (Exception e)
            {
                throw e;
            }
            
            PlayerInfo[] result = new PlayerInfo[2];
            int roomId = player.RoomId;
            GameRoom<BigTwo.BigTwo, BigTwoPlayer> gameRoom;
            try
            {
                gameRoom = GetRoomById(roomId);
                int oldHostId = gameRoom.HostId;
                gameRoom.LeavePlayer(ref player);
                result[0] = player;
                result[1] = player;

                if (gameRoom.CurrentPlayerCount == 0)
                {
                    // close room
                    _gameRooms.Remove(gameRoom.RoomId);
                }
                else
                {
                    int newHostId = gameRoom.HostId;
                    bool isChangeHost = oldHostId != newHostId;
                    if (isChangeHost)
                    {
                        _playerManager[newHostId].IsHost = true;
                        result[1] = GetPlayerById(newHostId);
                    }
                }

                return result;
            }
            catch
            {
                throw new Exception("no room");
            }
        }

        public static bool StartGame(PlayerInfo player)
        {
            try
            {
                player = GetPlayerById(player.Id);

                if (!player.IsInRoom)
                    throw new Exception();
            }
            catch (Exception e)
            {
                throw e;
            }
            
            int roomId = player.RoomId;
            GameRoom<BigTwo.BigTwo, BigTwoPlayer> gameRoom;
            try
            {
                gameRoom = GetRoomById(roomId);
                if (!gameRoom.IsEnoughPlayer())
                    return false;

                return gameRoom.Start();
            }
            catch
            {
                throw new Exception("no room");
            }
        }

        public static GameRoom<BigTwo.BigTwo, BigTwoPlayer>[] GetGameRooms()
        {
            return _gameRooms
                .Select(d=>d.Value)
                .ToArray();
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

        public static GameRoom<BigTwo.BigTwo, BigTwoPlayer> GetRoomById(int roomId)
        {
            try
            {
                return _gameRooms[roomId];
            }
            catch
            {
                throw new Exception("no room");
            }
        }

        private static GameRoom<BigTwo.BigTwo, BigTwoPlayer> NewGameRoom(ref PlayerInfo host)
        {
            BigTwo.BigTwo bigTwo = new BigTwo.BigTwo();

            return new GameRoom<BigTwo.BigTwo, BigTwoPlayer>(bigTwo, _newGameRoomId++, ref host, BigTwo.BigTwo.MAX_PLAYERS, BigTwo.BigTwo.MIN_PLAYERS);
        }
    }
}