﻿using BoardGame.Data.ApiParameters;
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
        public bool IsInRoom { get { return RoomId != -1; } }
        public int RoomId { get; private set; }
        public PlayerInfoModels Models { get { return new PlayerInfoModels(Name, Id, RoomId); } }

        public PlayerInfo(string name, int id)
        {
            this.Name = name;
            this.Id = id;
            RoomId = -1;
        }

        public PlayerInfo(PlayerInfoModels models)
            : this(models.Name, models.Id)
        {
        }

        public void JoinRoom(int roomId)
        {
            RoomId = roomId;
        }

        public void LeaveRoom()
        {
            RoomId = -1;
        }
    }
}