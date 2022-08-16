using System.Collections.Generic;
using System.Linq;

namespace GameLogic.PokerGame
{
    public static class PokerCards
    {
        public static IEnumerable<PokerCard> Choose(this IEnumerable<PokerCard> cards, int[] cardIndex)
        {
            PokerCard[] cardArr = cards.ToArray();
            List<PokerCard> containCard = new List<PokerCard>();
            for (int i = 0; i < cardIndex.Length; i++)
            {
                containCard.Add(cardArr[cardIndex[i]]);
            }
            return containCard;
        }

        public static int[] GetIndexOfCards(this PokerCard[] cards, PokerCard[] subCards)
        {
            List<int> result = new List<int>();

            if (subCards != null)
            {
                foreach (PokerCard card in subCards)
                {
                    for (int i = 0; i < cards.Length; i++)
                    {
                        PokerCard handcard = cards[i];
                        if (handcard.Number == card.Number &&
                            handcard.Suit == card.Suit)
                        {
                            result.Add(i);
                            break;
                        }
                    }
                }
            }

            return result.ToArray();
        }
    }
}