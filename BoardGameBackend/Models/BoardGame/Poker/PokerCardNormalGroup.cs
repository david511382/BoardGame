﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.Game.BoardGame.PokerGame
{
    public static partial class PokerCardGroup
    {
        public static PokerCard[] GetMinCardGroupInGroupTypeGreaterThenCard(PokerGroupType groupType, PokerCard card, List<PokerCard> cards, PokerCard[] containCard)
        {
            int[] constraint = GetConstraintOfType(groupType);
            bool isRightConstraint = CheckConstraint(constraint, cards.ToArray(), containCard);
            bool isRightContain = CheckCardsContainCards(cards.ToArray(), containCard);
            if (!isRightConstraint || !isRightContain)
                return null;

            switch (groupType)
            {
                case PokerGroupType.Single:
                case PokerGroupType.Pair:
                case PokerGroupType.Four_Of_A_Kind:
                case PokerGroupType.Full_House:
                    return GetMinCardGroupInConstraintGreaterThenCard(constraint, card, cards, containCard);
                case PokerGroupType.Straight:
                    return null;// Straight(cards, containCard);
                case PokerGroupType.Straight_Flush:
                    return null;//Straight_Flush(cards, containCard);
                case PokerGroupType.Dragon:
                    return null;
                default:
                    throw new Exception("undefine");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint">order by desc</param>
        /// <param name="card"></param>
        /// <param name="cards"></param>
        /// <param name="containCard">last is last selected</param>
        /// <returns></returns>
        private static PokerCard[] GetMinCardGroupInConstraintGreaterThenCard(int[] constraint, PokerCard card, List<PokerCard> cards, PokerCard[] containCards = null)
        {
            if (constraint.Length == 0)
                return null;
            constraint = constraint.OrderByDescending(d => d).ToArray();

            return SelectedCardGroupGreaterCard(constraint, card, cards, containCards);
        }

        private static PokerCard[] SelectedCardGroupGreaterCard(int[] constraint, PokerCard card, List<PokerCard> cards, PokerCard[] containCards = null)
        {
            bool[,] cardData = TransStruct(cards.ToArray());

            int requiredCount;
            List<PokerCard> result = new List<PokerCard>();
            PokerCard[] resultBuff;
            bool isMaxConstraint = true;

            int[] numberStartIndexs = new int[constraint.Length];
            int[] selectedNumbers = new int[constraint.Length];

            var cardNumberCounts = cards
                    .GroupBy(d => d.Number)
                    .Select(d => new
                    {
                        cardCount = d.Count(),
                        number = d.First().Number
                    })
                    .ToArray();

            for (int constraintIndex = 0; constraintIndex < constraint.Length; constraintIndex++)
            {
                requiredCount = constraint[constraintIndex];

                //check is exist same constraint card and bigger number
                int[] numbers = cardNumberCounts
                    .Where(d => d.cardCount >= requiredCount)
                    .Select(d => d.number)
                    .Except(selectedNumbers)
                    .Where(d => !isMaxConstraint || CompareNumber(d, card.Number) != -1)
                    .ToArray();

                //由點選順序看選定的每個牌號能不能當作最大牌組，最選優先
                int[] containNumbers = SpiltContainNumber(ref numbers, containCards);

                int containNumbersCount = containNumbers.Length;
                int number;
                for (int j = numberStartIndexs[constraintIndex]; j < containNumbersCount + numbers.Length; j++)
                {
                    bool isUseContainNumber = j < containNumbersCount;
                    if (isUseContainNumber)
                        number = containNumbers[j];
                    else
                    {
                        if (j == containNumbersCount)
                        {
                            // next contain cards

                            PokerCard[] nextContainCards = Except(containCards, result.ToArray());
                            PokerCard[] nextCard = Except(cards.ToArray(), result.ToArray());
                            //is constraint satisfied
                            if (!CheckNextConstraint(constraint, constraintIndex, nextCard, nextContainCards))
                                continue;
                        }

                        number = numbers[j - containNumbersCount];
                    }

                    int numberIndex = number - 1;

                    int[] suits = GetStructDataSuitsByNumber(numberIndex, cardData);
                    int[] containSuits = SpiltContainSuit(ref suits, number, containCards);

                    // get suit
                    resultBuff = MapSuit(requiredCount, suits, containSuits, isMaxConstraint, number, card);

                    // card suit is bigger then handcards
                    if (resultBuff.Length < requiredCount)
                        continue;

                    numberStartIndexs[constraintIndex] = ++j;
                    selectedNumbers[constraintIndex] = number;
                    result.AddRange(resultBuff);
                    break;
                }

                isMaxConstraint = false;
            }

            return result.ToArray();
        }

        private static PokerCard[] MapSuit(
            int requiredCount,
            int[] suits,
            int[] containSuits,
            bool isMaxConstraint,
            int number,
             PokerCard card)
        {
            List<PokerCard> resultBuff = new List<PokerCard>();
            int containSuitsCount = containSuits.Length;
            bool isSameNumber = card.Number == number;
            int cardSuitIndex = GetIBySuit(card.Suit);

            bool hasGreaterSuitToCard = false;
            for (int foundCount = 0, k = 0; foundCount < requiredCount && k < containSuitsCount + suits.Length; k++)
            {
                bool isUseContainSuit = k < containSuitsCount;
                int suitIndex = (isUseContainSuit) ?
                    containSuits[k] :
                    suits[k - containSuitsCount];

                bool isLastRequiredCount = foundCount == requiredCount - 1;
                hasGreaterSuitToCard = (!hasGreaterSuitToCard) ?
                    (resultBuff.Count > 0 &&
                        Poker.Is_Bigger_Suit(
                            resultBuff
                                .Select(d => d.Suit).Max(),
                            card.Suit
                        )) ||
                    suitIndex > cardSuitIndex :
                    true;
                if (isMaxConstraint &&
                    isSameNumber &&
                    isLastRequiredCount &&
                    !hasGreaterSuitToCard)
                {
                    continue;
                }

                resultBuff.Add(new PokerCard(GetSuitByI(suitIndex), number));
                foundCount++;
            }

            return resultBuff.ToArray();
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

        private static bool CheckNextConstraint(int[] constraint, int currentConstrainIndex, PokerCard[] cards, PokerCard[] containCards)
        {
            // next constraint
            int startConstraintIndex = currentConstrainIndex + 1;
            int[] nextConstraint = new int[constraint.Length - startConstraintIndex];
            for (int k = startConstraintIndex; k < constraint.Length; k++)
                nextConstraint[k - startConstraintIndex] = constraint[k];

            //is constraint satisfied
            return CheckConstraint(nextConstraint, cards, containCards);
        }
    }
}