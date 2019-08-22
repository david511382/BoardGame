using GameLogic.PokerGame;
using GameLogic.PokerGame.CardGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static GameLogic.PokerGame.CardGroup.PokerCardGroup;

namespace BigTwoLogic
{
    public class BigTwoCardGroupModel : ICardGroupModel
    {
        public int GetMaxNumber()
        {
            return BigTwo.MAX_CARD_NUMBER;
        }

        public int GetCompareValue(int number)
        {
            return BigTwo.GetCompareValue(number);
        }

        public int CompareCard(PokerCard a, PokerCard b)
        {
            return BigTwo.CompareCard(a, b);
        }

        public PokerCard[] GetStraightNumber(PokerCard[] cards, int length, PokerCard value = null, PokerCardGroup.SuitEqul suitEqul = PokerCardGroup.SuitEqul.dontCare)
        {
            int[] cardNumbers = cards
                .GroupBy(d => d.Number)
                .Select(d => d.First().Number)
                .OrderBy(d => d)
                .ToArray();

            List<PokerCard> twoTosix = new List<PokerCard>();
            for (int i = 0, startNumber = cardNumbers.Min(d => d), count = 0, number; i < cardNumbers.Length; i++)
            {
                number = cardNumbers[i];

                if (startNumber + count == number)
                    count++;
                else
                {
                    startNumber = number;
                    count = 1;
                }

                if (count == length)
                {
                    List<PokerCard> result = new List<PokerCard>();
                    PokerCard[] bufCards = cards;
                    if (value != null)
                    {
                        int compareNumber = number;
                        if (startNumber == BigTwo.MAX_CARD_NUMBER)
                            compareNumber = startNumber;

                        IEnumerable<PokerCard> maxValues = cards
                          .Where(d => d.Number == compareNumber);
                        maxValues = maxValues
                            .Where(d => BigTwo.CompareCard(d, value) > 0);

                        if (maxValues.Count() <= 0)
                        {
                            count--;
                            startNumber++;
                            continue;
                        }


                        bufCards = cards
                           .Where(d => d.Number != compareNumber)
                           .ToArray();

                        result.Add(
                            maxValues
                                .OrderBy(d => d.Suit)
                                .First()
                        );
                    }

                    bufCards = bufCards
                        .Where(d => d.Number <= number && d.Number >= startNumber)
                        .ToArray();

                    if (suitEqul == SuitEqul.notSame)
                        bufCards = PokerCardGroup.GetNotSameSuitStraight(bufCards);

                    result.AddRange(
                        bufCards
                            .GroupBy(d => d.Number)
                            .Select(d => new PokerCard(d.Min(c => c.Suit), d.First().Number))
                    );

                    if (startNumber == BigTwo.MAX_CARD_NUMBER)
                    {
                        twoTosix.AddRange(result);
                        result.Clear();
                        count--;
                        startNumber++;
                        continue;
                    }

                    return result.ToArray();
                }
            }

            if (twoTosix.Count == 0)
                return null;
            return twoTosix.ToArray();
        }
    }
}