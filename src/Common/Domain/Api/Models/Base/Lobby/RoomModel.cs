﻿using Domain.Api.Models.Base.User;

namespace Domain.Api.Models.Base.Lobby
{
    public class RoomModel
    {
        public int HostID { get; set; }
        public GameModel Game { get; set; }
        public UserModel[] Players { get; set; }

        public bool IsFull()
        {
            return Players.Length == Game.MaxPlayerCount;
        }
    }
}
