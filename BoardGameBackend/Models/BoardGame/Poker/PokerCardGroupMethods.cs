﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.PokerGame
{
    public partial class PokerCardGroup
    {
        public static int Max_Number;

        public static int Min_Number
        {
            get { return (Max_Number + 1 > Poker.NUMBER_NUM) ? 1 : Max_Number + 1; }
        }

        static PokerCardGroup()
        {
            Max_Number = 1;
        }

        public static PokerGroupType GetCardGroupType(PokerCard[] cards)
        {
            int[] constraints;
            foreach (PokerGroupType type in Enum.GetValues(typeof(PokerGroupType)))
            {
                constraints = GetConstraintOfType(type);
                if (CheckConstraint(constraints, cards))
                    return type;
            }

            throw new Exception("cant identitfy");
        }

        private static PokerCard GetMaxValueOfCardGroup(PokerCard[] cards)
        {
            var groupedCards = cards
                .GroupBy(d => d.Number)
                .Select(d => new { number = d.First(), count = d.Count() });

            int maxCount = groupedCards
                .Select(d => d.count)
                .Max();

            int[] numbers = groupedCards
                .Where(d => d.count == maxCount)
                .Select(d=>d.count)
                .ToArray();

            numbers = OrderNumber(numbers);

            int maxNumber = numbers
                .Last();

            PokerSuit maxSuit = cards
                .Where(d => d.Number == maxNumber)
                .Max(d => d.Suit);

            return new PokerCard(maxSuit, maxNumber);
        }

        private static int[] GetConstraintOfType(PokerGroupType type)
        {
            switch (type)
            {
                case PokerGroupType.Single:
                    return new int[] { 1 };
                case PokerGroupType.Pair:
                    return new int[] { 2 };
                case PokerGroupType.Four_Of_A_Kind:
                    return new int[] { 1, 4 };
                case PokerGroupType.Full_House:
                    return new int[] { 2, 3 };
                case PokerGroupType.Straight:
                case PokerGroupType.Straight_Flush:
                    return new int[] { 1, 1, 1, 1, 1 };
                case PokerGroupType.Dragon:
                    return new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                default:
                    throw new Exception("undefine");
            }
        }

        private static int[] SpiltContainNumber(ref int[] numbers, PokerCard[] containCards)
        {
            if (containCards == null)
                return new int[0];

            List<int> numberList = new List<int>();
            List<int> containNumberList = new List<int>();
            numberList.AddRange(numbers);
            int number;
            for (int i = containCards.Length - 1; i >= 0; i--)
            {
                number = containCards[i].Number;
                if (numberList.Contains(number))
                {
                    numberList.Remove(number);
                    containNumberList.Add(number);
                }
            }

            numbers = numberList.ToArray();
            return containNumberList.ToArray();
        }

        private static int[] SpiltContainSuit(ref int[] suits, int number, PokerCard[] containCards)
        {
            if (containCards == null)
                return new int[0];

            containCards = containCards
                .Where(d => d.Number == number)
                .OrderBy(d => d.Suit)
                .ToArray();

            List<int> suitList = new List<int>();
            List<int> containSuitList = new List<int>();
            suitList.AddRange(suits);
            int suitIndex;
            for (int i = 0; i < containCards.Length; i++)
            {
                suitIndex = GetIBySuit(containCards[i].Suit);
                if (suitList.Contains(suitIndex))
                {
                    suitList.Remove(suitIndex);
                    containSuitList.Add(suitIndex);
                }
            }

            suits = suitList.ToArray();
            return containSuitList.ToArray();
        }

        private static PokerCard[] Except(PokerCard[] cards, PokerCard[] exceptCards)
        {
            List<PokerCard> cardList = new List<PokerCard>();
            cardList.AddRange(cards);

            for (int i = 0; i < cardList.Count; i++)
            {
                for (int j = 0; j < exceptCards.Length; j++)
                    if (exceptCards[j].isSame(cardList[i]))
                    {
                        cardList.RemoveAt(i);
                        i--;
                        break;
                    }
            }

            return cardList.ToArray();
        }

        private static PokerCard[] Intersect(PokerCard[] cards, PokerCard[] intersectCards)
        {
            List<PokerCard> cardList = new List<PokerCard>();
            cardList.AddRange(cards);
            List<PokerCard> intersectList = new List<PokerCard>();
            intersectList.AddRange(intersectCards);
            List<PokerCard> resultList = new List<PokerCard>();

            for (int i = 0; i < cardList.Count; i++)
            {
                for (int j = 0; j < intersectList.Count; j++)
                    if (intersectList[j].isSame(cardList[i]))
                    {
                        resultList.Add(cards[i]);
                        intersectList.RemoveAt(j);
                        cardList.RemoveAt(i);
                        j--;
                    }
            }

            return resultList.ToArray();
        }

        private static int[] GetStructDataSuitsByNumber(int numberIndex, bool[,] data)
        {
            List<int> suitList = new List<int>();
            for (int suitIndex = 0; suitIndex < Poker.SUIT_NUM; suitIndex++)
            {
                if (data[numberIndex, suitIndex])
                {
                    suitList.Add(suitIndex);
                }
            }
            return suitList.ToArray();
        }

        private static int[] OrderNumber(int[] numbers)
        {
            int numberCount = numbers.Length;
            if (numberCount > 1)
            {
                int value = numbers[0];
                List<int> result = new List<int>();
                result.AddRange(OrderNumber(numbers.Where(d => CompareNumber(d, value) == -1).ToArray()));
                result.Add(value);
                result.AddRange(OrderNumber(numbers.Where(d => CompareNumber(d, value) == 1).ToArray()));
                return result.ToArray();
            }
            else if (numberCount == 1)
                return numbers;
            else
                return new int[0];
        }

        private static int CompareNumber(int a, int b)
        {
            a += (a <= Max_Number) ? Poker.NUMBER_NUM : 0;
            b += (b <= Max_Number) ? Poker.NUMBER_NUM : 0;

            if (a > b)
                return 1;
            else if (a < b)
                return -1;
            else
                return 0;
        }

        private static bool[,] TransStruct(PokerCard[] cards)
        {
            bool[,] cardData = new bool[Poker.NUMBER_NUM, Poker.SUIT_NUM];
            int suit;
            int index;
            for (int i = 0; i < cards.Length; i++)
            {
                index = cards[i].Number - 1;
                suit = GetIBySuit(cards[i].Suit);
                cardData[index, suit] = true;
            }
            return cardData;
        }

        private static PokerSuit GetSuitByI(int i)
        {
            if (i == 0)
                return PokerSuit.Club;
            else if (i == 1)
                return PokerSuit.Diamond;
            else if (i == 2)
                return PokerSuit.Heart;
            else
                return PokerSuit.Spade;
        }

        private static int GetIBySuit(PokerSuit suit)
        {
            switch (suit)
            {
                case PokerSuit.Club:
                    return 0;
                case PokerSuit.Diamond:
                    return 1;
                case PokerSuit.Heart:
                    return 2;
                case PokerSuit.Spade:
                    return 3;
                default:
                    throw new Exception("fail");
            }
        }

        private static bool CheckCardsContainCards(PokerCard[] cards, PokerCard[] containCard)
        {
            for (int i = 0; i < containCard.Length; i++)
            {
                for (int j = 0; ; j++)
                {
                    if (cards[j].isSame(containCard[i]))
                        break;
                    else if (j == cards.Length - 1)
                        return false;
                }
            }
            return true;
        }

        private static bool CheckConstraint(int[] constraint, PokerCard[] cards, PokerCard[] containCard = null)
        {
            int requiredCount = constraint.Sum();
            int requiredNumberCount = constraint.Length;
            int minRequiredSuitCount = (requiredNumberCount == 0) ? 0 : constraint.Min();

            bool isCardsEnough = cards.Length >= requiredCount;
            bool isContainCountAcceptable = true;
            bool isContainNumbberCountAcceptable = true;
            bool isContainSuitCountAcceptable = true;
            bool isContaintExistInCards = true;

            if (containCard != null && containCard.Length > 0)
            {
                isContainCountAcceptable = containCard.Length <= requiredCount;

                int[] containCardNumbers = containCard
                    .GroupBy(d => d.Number)
                    .Select(d => d.First().Number)
                    .ToArray();
                int containNumberCount = containCardNumbers.Count();
                isContainNumbberCountAcceptable = containNumberCount <= requiredNumberCount;

                int containSuitMaxCount = 0;
                for (int i = 0, count; i < containCard.Length; i++)
                {
                    count = cards.Where(d => d.Number == containCard[i].Number).Count();
                    if (count > containSuitMaxCount)
                        containSuitMaxCount = count;
                }
                isContainSuitCountAcceptable = containSuitMaxCount >= minRequiredSuitCount;

                isContaintExistInCards = Intersect(cards, containCard).Length == containCard.Length;
            }

            return isCardsEnough && isContainCountAcceptable && isContainNumbberCountAcceptable && isContainSuitCountAcceptable && isContaintExistInCards;
        }
    }
}