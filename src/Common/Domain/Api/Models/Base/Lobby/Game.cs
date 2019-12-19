﻿namespace Domain.Api.Models.Base.Lobby
{
    public class Game
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description{ get; set; }
        public int MaxPlayerCount { get; set; }
        public int MinPlayerCount { get; set; }
    }
}
