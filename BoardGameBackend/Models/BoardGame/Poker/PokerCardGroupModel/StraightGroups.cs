using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.PokerGame
{
    public partial class PokerCardGroup
    {
        private const int STRAIGHT_LENGHT = 5;
        private const int DRAGON_LENGHT = 13;

        private static bool CheckStraightConstraint(PokerGroupType groupType, PokerCard[] cards, PokerCard[] containCard = null)
        {
            int length;
            switch (groupType)
            {
                case PokerGroupType.Straight_Flush:
                    return CheckStraightFlush(cards, containCard);
                case PokerGroupType.Straight:
                    length = STRAIGHT_LENGHT;
                    break;
                case PokerGroupType.Dragon:
                    length = DRAGON_LENGHT;
                    break;
                default:
                    return true;
            }

            return CheckStraight(length, cards, containCard);
        }

        private static bool CheckStraight(int length, PokerCard[] cards, PokerCard[] containCard = null)
        {
            if (CheckStaright10ToA(cards, containCard))
                return true;

            if (containCard != null && containCard.Length != 0)
            {
                int maxNumberOfContain = containCard.Max(d => d.Number);
                int minNumberOfContain = containCard.Min(d => d.Number);

                int minNumber = maxNumberOfContain - (length - 1);
                if (minNumber <= 0)
                    minNumber = 1;
                if (minNumber > minNumberOfContain)
                    return false;

                int maxNumber = minNumberOfContain + (length - 1);
                if (maxNumber > Poker.NUMBER_NUM)
                    maxNumber = Poker.NUMBER_NUM;
                if (maxNumber < maxNumberOfContain)
                    return false;

                cards = cards
                    .Where(d =>
                        d.Number >= minNumber &&
                        d.Number <= maxNumber
                    )
                    .ToArray();
            }

            return CheckStraightNumber(cards, length);
        }

        private static bool CheckStraightNumber(PokerCard[] cards, int length)
        {
            int[] cardNumbers = cards
                .GroupBy(d=>d.Number)
                .Select(d=>d.First().Number)
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
                    return true;
            }

            return false;
        }

        private static bool CheckStaright10ToA(PokerCard[] cards, PokerCard[] containCard = null)
        {
            if (containCard != null && containCard.Length != 0)
            {
                bool containCardNotIn10ToA = containCard
                    .Where(d => !IsNumberIn10ToA(d))
                    .Count()
                    >
                    0;
                if (containCardNotIn10ToA)
                    return false;
            }
                        
            PokerCard[] cardsBuf = cards
                .Where(d => IsNumberIn10ToA(d))
                .Select(d => d)
                .ToArray();
            
            PokerCard[] checkCards=new PokerCard[cardsBuf.Length];
            for (int i = 0,num; i < checkCards.Length; i++) {
                num = cardsBuf[i].Number;
                if (num==1)
                    num = Poker.NUMBER_NUM + 1;

                checkCards[i] = new PokerCard(cardsBuf[i].Suit, num);
            }

            return CheckStraightNumber(checkCards, STRAIGHT_LENGHT);
        }

        private static bool IsNumberIn10ToA(PokerCard card)
        {
            int cardNum = card.Number;
            return cardNum >= 10 || cardNum == 1;
        }

        private static bool CheckStraightFlush(PokerCard[] cards, PokerCard[] containCard = null)
        {
            PokerSuit[] suits;

            if (containCard == null || containCard.Length == 0)
            {
                suits = cards
                    .GroupBy(d => d.Suit)
                    .Select(d => new { suit = d.First().Suit, count = d.Count() })
                    .Where(d => d.count >= STRAIGHT_LENGHT)
                    .Select(d => d.suit)
                    .ToArray();
            }
            else
            {
                bool isContainCardSameSuit = containCard
                          .GroupBy(d => d.Suit)
                          .Count()
                          ==
                          1;

                if (!isContainCardSameSuit)
                    return false;

                suits = new PokerSuit[] { containCard.First().Suit };
            }

            PokerCard[] checkCards;
            for (int i = 0; i < suits.Length; i++)
            {
                checkCards = cards
               .Where(d => d.Suit == suits[i])
               .ToArray();

                if (CheckStraightConstraint(PokerGroupType.Straight, checkCards, containCard))
                    return true;
            }

            return false;
        }
    }
}