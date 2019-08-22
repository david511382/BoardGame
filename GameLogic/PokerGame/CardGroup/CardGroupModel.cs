using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static GameLogic.PokerGame.CardGroup.PokerCardGroup;

namespace GameLogic.PokerGame.CardGroup
{
    public class CardGroupModel : ICardGroupModel
    {
        public int GetMaxNumber()
        {
            return 1;
        }

        public int GetCompareValue(int number)
        {
            return Poker.Get_Compare_Value(number);
        }

        public int CompareCard(PokerCard a, PokerCard b)
        {
            return PokerCard.CompareCard(a, b);
        }

        public PokerCard[] GetStraightNumber(PokerCard[] cards, int length, PokerCard value = null, SuitEqul suitEqul = SuitEqul.dontCare)
        {
            int[] cardNumbers = cards
                .GroupBy(d => d.Number)
                .Select(d => d.First().Number)
                .OrderBy(d => d)
                .ToArray();

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
                        IEnumerable<PokerCard> maxValues = cards
                          .Where(d => d.Number == compareNumber);
                        maxValues = maxValues
                            .Where(d => CompareCard(d, value) > 0);

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

                    if (suitEqul == SuitEqul.notSame)
                        bufCards = GetNotSameSuitStraight(bufCards.Where(d => d.Number <= number && d.Number >= startNumber));

                    result.AddRange(
                        bufCards
                            .GroupBy(d => d.Number)
                            .Select(d => new PokerCard(d.Min(c => c.Suit), d.First().Number))
                    );

                    return result.ToArray();
                }
            }

            return null;
        }
    }
}