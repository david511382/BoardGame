using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer
{
    public struct PlayerInfo
    {
        public string Name;
        public int Id;

        public PlayerInfo (string name,int id)
        {
            this.Name = name;
            this.Id = id;
        }
    }
}