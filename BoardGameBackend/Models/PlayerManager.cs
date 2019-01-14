using BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer;
using BoardGame.Data.ApiParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGameBackend.Models
{
    public class PlayerManager:IPlayerManager
    {
        public PlayerInfo this[int id]
        {
            get
            {
                return _players[id];
            }
        }

        private Dictionary<int, PlayerInfo> _players;

        public PlayerManager()
        {
            _players = new Dictionary<int, PlayerInfo>();
        }

        public void AddPlayer(PlayerInfo player)
        {
            _players.Add(player.Id, player);
        }

        public PlayerInfo GetPlayer(int playerId)
        {
            return _players[playerId];
        }

        public PlayerInfo[] GetPlayers(int[] playerId)
        {
            if (playerId == null)
                return new PlayerInfo[0];

            return _players
                .Where(d => playerId.Contains(d.Key))
                .Select(d => d.Value)
                .ToArray();
        }
    }
}