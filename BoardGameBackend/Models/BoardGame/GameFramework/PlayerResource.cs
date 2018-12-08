using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.Game.BoardGame.GameFramework
{
    public abstract class PlayerResource
    {
        public readonly int PlayerId;

        public PlayerResource(int playerId)
        {
            PlayerId = playerId;
        }
    }
}