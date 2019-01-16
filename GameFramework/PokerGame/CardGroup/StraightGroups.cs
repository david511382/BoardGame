using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameFramework.PokerGame.CardGroup
{
    public partial class PokerCardGroup
    {
        protected const int STRAIGHT_LENGHT = 5;
        protected const int DRAGON_LENGHT = 13;

        private static PokerCard[] GetStraightGreater<T>(PokerGroupType groupType, PokerCard[] cards, PokerCard[] containCard = null, PokerCard value = null) where T : ICardGroupModel, new()
        {
            SuitEqul suitEqul;
            int length;
            switch (groupType)
            {
                case PokerGroupType.Straight_Flush:
                    return GetStraightFlushGreater<T>(cards, containCard, value);
                case PokerGroupType.Straight:
                    suitEqul = SuitEqul.notSame;
                    length = STRAIGHT_LENGHT;
                    break;
                case PokerGroupType.Dragon:
                    suitEqul = SuitEqul.dontCare;
                    length = DRAGON_LENGHT;
                    break;
                default:
                    return null;
            }

            return GetStraightGreater<T>(cards, length, suitEqul, containCard, value);
        }

        private static PokerCard[] GetStraightGreater<T>(PokerCard[] cards, int length, SuitEqul suitEqul, PokerCard[] containCard = null, PokerCard value = null) where T : ICardGroupModel, new()
        {
            cards = CombineStraightContainCards(cards, containCard);

            PokerCard[] result = GetStaright10ToAGreater<T>(cards, containCard, value, suitEqul);
            if (result != null)
                return result;

            cards = GetCardsInContainCardGreater(length, cards, containCard, value);
            if (cards == null)
                return null;

            T model = new T();
            return model.GetStraightNumber(cards, length, value, suitEqul);
        }

        private static bool CheckStraightConstraint<T>(PokerGroupType groupType, PokerCard[] cards, PokerCard[] containCard = null) where T : ICardGroupModel, new()
        {
            switch (groupType)
            {
                case PokerGroupType.Straight_Flush:
                case PokerGroupType.Straight:
                case PokerGroupType.Dragon:
                    PokerCard[] result = GetStraightGreater<T>(groupType, cards, containCard);
                    if (result == null)
                        return false;
                    else
                        return true;
                default:
                    return true;
            }
        }

        private static PokerCard[] GetCardsInContainCardGreater(int length, PokerCard[] cards, PokerCard[] containCard = null, PokerCard value = null)
        {
            int straightMinus = length - 1;

            List<PokerCard> resultCards = new List<PokerCard>();
            if (containCard != null && containCard.Length != 0)
            {
                int maxNumberOfContain = containCard.Max(d => d.Number);
                int minNumberOfContain = containCard.Min(d => d.Number);

                int minNumber = maxNumberOfContain - straightMinus;
                if (minNumber <= 0)
                    minNumber = 1;
                if (minNumber > minNumberOfContain)
                    return null;

                int maxNumber = minNumberOfContain + straightMinus;
                if (maxNumber > Poker.NUMBER_NUM)
                    maxNumber = Poker.NUMBER_NUM;
                if (maxNumber < maxNumberOfContain)
                    return null;

                resultCards.AddRange(
                    cards
                        .Where(d =>
                            d.Number >= minNumber &&
                            d.Number <= maxNumber
                        )
                );
            }
            else
                resultCards.AddRange(cards);

            if (value != null)
            {
                int straightFirstNumber = (value.Number == Poker.MAX_NUMBER) ?
                    10 :
                    value.Number - straightMinus;

                resultCards = resultCards
                    .Where(d => d.Number >= straightFirstNumber)
                    .ToList();
            }

            return resultCards.ToArray();
        }

        private static PokerCard[] CombineStraightContainCards(PokerCard[] cards, PokerCard[] containCard = null)
        {
            if (containCard == null)
                return cards;

            var containCardGroup = containCard
               .GroupBy(d => d.Number)
               .Select(d => new { number = d.First().Number, count = d.Count() });

            IEnumerable<int> containCardNumber = containCardGroup
                .Select(d => d.number);
            int[] numbers = cards
                .Select(d => d.Number)
                .Except(containCardNumber)
                .ToArray();

            List<PokerCard> resultCards = new List<PokerCard>();
            resultCards = cards
                .Where(d => numbers.Contains(d.Number))
                .Select(d => new PokerCard(d.Suit, d.Number))
                .ToList();

            resultCards.AddRange(containCard);
            return resultCards.ToArray();
        }

        private static bool IsContainCard10ToA(PokerCard[] containCard = null)
        {
            bool containCardNotIn10ToA = true;

            if (containCard != null && containCard.Length != 0)
            {
                containCardNotIn10ToA = containCard
                    .Where(d => !IsNumberIn10ToA(d))
                    .Count()
                    >
                    0;
            }
            return !containCardNotIn10ToA;
        }

        private static PokerCard[] GetStaright10ToAGreater<T>(PokerCard[] handCards, PokerCard[] containCard = null, PokerCard value = null, SuitEqul suitEqul = SuitEqul.dontCare) where T : ICardGroupModel, new()
        {
            if (!IsContainCard10ToA(containCard))
                return null;

            PokerCard[] cards = handCards
               .Where(d => IsNumberIn10ToA(d))
               .ToArray();

            PokerCard A = GetMinA<T>(cards, containCard, value);
            if (A == null)
                return null;

            if (suitEqul == SuitEqul.notSame)
            {
                cards = GetNotSameSuitStraight(cards);
                if (cards == null)
                    return null;
            }

            cards = cards
                .Where(d => d.Number != Poker.MAX_NUMBER)
                .GroupBy(d => d.Number)
                .Select(d => new PokerCard(d.Min(c => c.Suit), d.First().Number))
                .OrderBy(d => d.Number)
                .ToArray();
            if (cards.Length != STRAIGHT_LENGHT - 1)
                return null;

            const int START_NUMBER = 10;
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].Number != START_NUMBER + i)
                    return null;
            }

            List<PokerCard> result = new List<PokerCard>();
            result.AddRange(cards);
            result.Add(A);

            return result.ToArray();
        }
        
        private static PokerCard GetMinA<T>(PokerCard[] cards, PokerCard[] containCard = null, PokerCard value = null) where T : ICardGroupModel, new()
        {
            IEnumerable<PokerCard> As;
            if (containCard != null && containCard.Where(d => d.Number == Poker.MAX_NUMBER).Count() > 0)
                As = containCard
                   .Where(d => d.Number == Poker.MAX_NUMBER);
            else
                As = cards
                    .Where(d => d.Number == Poker.MAX_NUMBER);

            if (value != null)
            {
                T model = new T();
                As = As
                    .Where(d => model.CompareCard(d, value) > 0);
            }

            if (As.Count() == 0)
                return null;

            return As
                .OrderBy(d => d.Suit)
                .First();
        }

        private static bool CheckStraightNumber(PokerCard[] cards, int length)
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
                    return true;
            }

            return false;
        }

        private static bool IsNumberIn10ToA(PokerCard card)
        {
            int cardNum = card.Number;
            return cardNum >= 10 || cardNum == 1;
        }

        private static PokerCard[] GetStraightFlushGreater<T>(PokerCard[] cards, PokerCard[] containCard = null, PokerCard value = null) where T : ICardGroupModel, new()
        {
            SuitEqul suitEqul;
            int length;
            suitEqul = SuitEqul.same;
            length = STRAIGHT_LENGHT;

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
                    return null;

                suits = new PokerSuit[] { containCard.First().Suit };
            }

            PokerCard[][] result = new PokerCard[4][];
            PokerCard[] bufCards;
            for (int i = 0; i < suits.Length; i++)
            {
                bufCards = cards
                   .Where(d => d.Suit == suits[i])
                   .ToArray();

                bufCards = GetStraightGreater<T>(bufCards, length, suitEqul, containCard, value);

                if (bufCards != null)
                    return bufCards;
            }

            return null;
        }
    }
}