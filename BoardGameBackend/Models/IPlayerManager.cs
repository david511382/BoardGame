using BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGameBackend.Models
{
    public interface IPlayerManager
    {
        PlayerInfo GetPlayer(int playerId);

        PlayerInfo[] GetPlayers(int[] playerId);
    }
}