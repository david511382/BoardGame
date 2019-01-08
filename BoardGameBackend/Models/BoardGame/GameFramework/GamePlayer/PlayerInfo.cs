using BoardGame.Data.ApiParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer
{
    public class PlayerInfo
    {
        public string Name;
        public int Id;
        public PlayerInfoModels Models { get { return new PlayerInfoModels(Name, Id); } }

        public PlayerInfo (string name,int id)
        {
            this.Name = name;
            this.Id = id;
        }

        public PlayerInfo(PlayerInfoModels models)
            :this(models.Name,models.Id)
        {
        }
    }
}