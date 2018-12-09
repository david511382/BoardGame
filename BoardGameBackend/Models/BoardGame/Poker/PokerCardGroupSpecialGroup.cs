using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.PokerGame
{
    public partial class PokerCardGroup
    {
        private static List<PokerCard[]> Straight(PokerCard[] cards, PokerCard[] containCard)
        {
            //if (containCard.Length != 0)
            //    if (containCard.Last().Number > containCard.First().Number + 4)
            //        throw new Exception("fail");

            //bool[,] cardData = TransStruct(cards);

            //List<PokerCard[]> result = new List<PokerCard[]>();
            //PokerCard[] group = new PokerCard[5];
            //PokerSuit suit;
            //int count;
            //for (int suitIndex = 0; suitIndex < cardData.GetLength(1); suitIndex++)
            //{
            //    count = 0;
            //    for (int numIndex = 0; numIndex < cardData.GetLength(0); numIndex++)
            //    {
            //        if (cardData[numIndex, suitIndex])
            //        {
            //            count++;
            //        }
            //        else
            //            count = 0;

            //        if (count == 5)
            //        {
            //            if (suitIndex == 0)
            //                suit = PokerSuit.Club;
            //            else if (suitIndex == 1)
            //                suit = PokerSuit.Diamond;
            //            else if (suitIndex == 2)
            //                suit = PokerSuit.Heart;
            //            else
            //                suit = PokerSuit.Spade;

            //            for (int i = 0; i < count; i++)
            //                group[i] = new PokerCard(suit, numIndex - 3 + i);

            //            result.Add(group);
            //        }
            //    }
            //}

            throw new Exception("fail");
        }

        private static List<PokerCard[]> Straight_Flush(PokerCard[] cards, PokerCard[] containCard)
        {
            if (containCard.Length != 0)
                if (containCard.Last().Number > containCard.First().Number + 4)
                throw new Exception("fail");

            bool[,] cardData = TransStruct(cards);

            List<PokerCard[]> result = new List<PokerCard[]>();
            PokerCard[] group ;
            PokerSuit suit;
            int count, numIndex,value;
            for(int suitIndex = 0; suitIndex < cardData.GetLength(1); suitIndex++)
            {
                count = 0;
                for(int k = 0; k < cardData.GetLength(0) + 1; k++)
                {
                    numIndex = k;
                    if (numIndex == cardData.GetLength(0))
                        numIndex = 0;

                    if (cardData[numIndex, suitIndex])
                        count++;
                    else
                        count = 0;

                    if (count == 5)
                    {
                        suit = GetSuitByI(suitIndex);

                        group = new PokerCard[5];

                        for (int i = 0; i < count; i++)
                        {
                            value = k - count +2 + i;
                            value = (value > Poker.NUMBER_NUM) ? 1 : value;
                            group[i] = new PokerCard(suit, value);
                        }

                        if (IsOrderedContain(group.OrderBy(d=>d.Number).ThenBy(d=>d.Suit).ToArray(),containCard))
                            result.Add(group);

                        count = 4;
                    }
                }
            }

            return result;
        }
        
        private static bool IsOrderedContain(PokerCard[] cards,PokerCard[] containCard)
        {
            if (containCard.Length == 0)
                return true;

            int containCardCount = containCard.Length;
            int containCardIndex = 0;
            for(int i = 0; i < cards.Length; i++)
            {
                if (containCard[containCardIndex].isSame(cards[i]))
                {
                    containCardIndex++;
                    if (containCardIndex == containCardCount)
                        return true;
                }
            }
            return false;
        }

    }
}