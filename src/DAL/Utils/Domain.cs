﻿namespace DAL.Utils
{
    internal sealed class Key
    {
        public static readonly string Game = "Game";
        public static readonly string GameStatus = "GameStatus";
        public static readonly string Room = "{Lobby}Room";
        public static readonly string User = "{Lobby}User";

        public static readonly string Channel = "Channel";

        // 分配到三台redis的hash tag
        public static readonly string Lock1 = $"{10}";
        public static readonly string Lock2 = $"{0}";
        public static readonly string Lock3 = $"{1}";
    }

    public enum Channel
    {
    }
}
