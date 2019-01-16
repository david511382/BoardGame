using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameFramework.Game
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