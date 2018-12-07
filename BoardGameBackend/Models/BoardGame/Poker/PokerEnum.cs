using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.Game.BoardGame.PokerGame
{
    public enum PokerSuit
    {
        Club,
        Diamond,
        Heart,
        Spade
    }

    public enum PokerGroupType
    {
        Single,//單
        Pair,//對子
        //Royal_Flush,// 同花大順
        Straight_Flush,// 同花順
        Straight,// 順子
        Four_Of_A_Kind,//鐵支囉
        Full_House,//葫蘆
        Dragon
    }
}