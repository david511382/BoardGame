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

        private static PokerCard[] GetStraightGreater(PokerGroupType groupType, PokerCard[] cards, PokerCard[] containCard = null, PokerCard value = null, bool isSameSuit = false)
        {
            int length;
            switch (groupType)
            {
                case PokerGroupType.Straight_Flush:
                    return GetStraightFlushGreater(cards, containCard, value);
                case PokerGroupType.Straight:
                    length = STRAIGHT_LENGHT;
                    break;
                case PokerGroupType.Dragon:
                    length = DRAGON_LENGHT;
                    break;
                default:
                    return null;
            }

            cards = CombineStraightContainCards(cards, containCard);

            PokerCard[] result = GetStaright10ToAGreater(cards, containCard, value, isSameSuit);
            if (result != null)
                return result;

            cards = GetCardsInContainCardGreater(length, cards, containCard, value);
            if (cards == null)
                return null;

            return GetStraightNumber(cards, length, value, isSameSuit);
        }

        private static bool CheckStraightConstraint(PokerGroupType groupType, PokerCard[] cards, PokerCard[] containCard = null)
        {
            switch (groupType)
            {
                case PokerGroupType.Straight_Flush:
                case PokerGroupType.Straight:
                case PokerGroupType.Dragon:
                    PokerCard[] result = GetStraightGreater(groupType, cards, containCard);
                    if (result == null)
                        return false;
                    else
                        return true;
                default:
                    return true;
            }
        }

        private static PokerCard[] GetCardsInContainCardGreater(int length, PokerCard[] cards, PokerCard[] containCard = null,PokerCard value = null)
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
                    .Where(d => d.Number >= straightFirstNumber ||
                        (d.Number >= BigTwo.BigTwo.MAX_CARD_NUMBER &&
                            d.Number <= BigTwo.BigTwo.MAX_CARD_NUMBER + straightMinus)
                    )
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

        private static PokerCard[] GetStaright10ToAGreater(PokerCard[] handCards, PokerCard[] containCard = null,PokerCard value=null, bool isSameSuit = false)
        {
            if (!IsContainCard10ToA(containCard))
                return null;

            PokerCard[] cards = handCards
               .Where(d => IsNumberIn10ToA(d))
               .ToArray();
            
            PokerCard A = GetMinA(cards, containCard, value);
            if (A == null)
                return null;

            if (!isSameSuit)
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
            for(int i = 0; i < cards.Length; i++)
            {
                if (cards[i].Number != START_NUMBER + i)
                    return null;
            }

            List<PokerCard> result = new List<PokerCard>();
            result.AddRange(cards);
            result.Add(A);

            return result.ToArray();
        }

        private static PokerCard[] GetNotSameSuitStraight(IEnumerable<PokerCard> cards)
        {
            bool sameSuit = cards
                       .GroupBy(d => d.Number)
                       .Select(d => d.Min(c => c.Suit))
                       .GroupBy(d => d)
                       .Count() == 1;
            if (sameSuit)
            {
                var freeCards = cards
                   .GroupBy(d => d.Number)
                   .OrderBy(d => BigTwo.BigTwo.GetCompareValue(d.First().Number))
                   .ToArray();

                PokerCard changeSuitCard = null;
                for (int i = 0; i < freeCards.Length; i++)
                {
                    if (freeCards[i].Count() > 1)
                    {
                        PokerSuit changeSuit = freeCards[i]
                            .OrderBy(d => d.Suit)
                            .Take(2)
                            .Last()
                            .Suit;
                        int changeNumber = freeCards[i]
                            .First()
                            .Number;
                        changeSuitCard = new PokerCard(changeSuit, changeNumber);
                        break;
                    }
                }
                if (changeSuitCard == null)
                    return null;

                List<PokerCard> bufList = cards
                    .Where(d => d.Number != changeSuitCard.Number)
                    .ToList();
                bufList.Add(changeSuitCard);
                cards = bufList.ToArray();
            }

            return cards.ToArray();
        }

        private static PokerCard GetMinA(PokerCard[] cards, PokerCard[] containCard = null, PokerCard value = null)
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
                As = As
                    .Where(d => BigTwo.BigTwo.CompareCard(d, value) > 0);
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

        private static PokerCard[] GetStraightNumber(PokerCard[] cards, int length,PokerCard value = null,bool isRequireSameSuit = false)
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

                    if (value != null)
                    {
                        int compareNumber = number;
                        if (startNumber == BigTwo.BigTwo.MAX_CARD_NUMBER)
                            compareNumber = startNumber;

                        IEnumerable<PokerCard> maxValues = cards
                          .Where(d => d.Number == compareNumber);
                        maxValues = maxValues
                            .Where(d => BigTwo.BigTwo.CompareCard(d, value) > 0);

                        if (maxValues.Count() <= 0)
                        {
                            count--;
                            startNumber++;
                            continue;
                        }

                        cards = cards
                           .Where(d => d.Number != compareNumber)
                           .ToArray();

                        result.Add(
                            maxValues
                                .OrderBy(d => d.Suit)
                                .First()
                        );
                    }

                    if (!isRequireSameSuit)
                        cards = GetNotSameSuitStraight(cards.Where(d => d.Number <= number && d.Number >= startNumber));

                    result.AddRange(
                        cards
                            .GroupBy(d => d.Number)
                            .Select(d => new PokerCard(d.Min(c => c.Suit), d.First().Number))
                    );

                    if (startNumber == BigTwo.BigTwo.MAX_CARD_NUMBER)
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

        private static bool IsNumberIn10ToA(PokerCard card)
        {
            int cardNum = card.Number;
            return cardNum >= 10 || cardNum == 1;
        }

        private static PokerCard[] GetStraightFlushGreater(PokerCard[] cards, PokerCard[] containCard = null, PokerCard value = null)
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
                    return null;

                suits = new PokerSuit[] { containCard.First().Suit };
            }

            PokerCard[] checkCards;
            for (int i = 0; i < suits.Length; i++)
            {
                checkCards = cards
                   .Where(d => d.Suit == suits[i])
                   .ToArray();

                checkCards = GetStraightGreater(PokerGroupType.Straight, checkCards, containCard, value, true);
                if (checkCards != null)
                    return checkCards;
            }

            return null;
        }
    }
}