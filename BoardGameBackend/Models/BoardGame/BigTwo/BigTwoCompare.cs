using BoardGame.Backend.Models.Game.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.Game.BoardGame.BigTwo
{
    public partial class BigTwo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cards"></param>
        /// <returns></returns>
        private List<PokerCard[]> OrderInGroupType(PokerGroupType type, List<PokerCard[]> cards)
        {
            try
            {
                return cards.OrderBy(d => d.Last().Number).ThenBy(d => d.Last().Suit).ToList();
                //bool useGroup = false;
                //switch (type)
                //{
                //    case PokerGroupType.Full_House:
                //        useGroup = true;
                //        break;
                //}

                //PokerCard maxCard;
                //int num, maxNum;
                //PokerSuit suit, maxNumSuit;
                //for (int i = 0; i < cards.Count; i++)
                //{
                //    maxNum = 0;
                //    maxNumSuit = PokerSuit.Club;
                //    for (int j = 0; j < cards[i].Length; j++)
                //    {
                //        num = cards[i][j].Number;
                //        suit = cards[i][j].Suit;

                //        if (maxNum < num)
                //        {
                //            maxNum = num;
                //            maxNumSuit = suit;
                //        }
                //        else if (maxNum == num && Poker.Is_Bigger_Suit(suit, maxNumSuit))
                //        {
                //            maxNumSuit = suit;
                //        }
                //        //if (useGroup) ;
                //    }
                //}
            }
            catch
            {
                throw new Exception("wrong type");
            }
        }
    }
}