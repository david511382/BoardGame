﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameLogic.PokerGame.CardGroup
{
    public partial class PokerCardGroup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint">order by desc</param>
        /// <param name="card"></param>
        /// <param name="cards"></param>
        /// <param name="containCard">last is last selected</param>
        /// <returns></returns>
        private static PokerCard[] GetMinCardGroupInConstraintGreaterThenCard<T>(int[] constraint, List<PokerCard> cards, PokerCard[] containCards = null, PokerCard card = null) where T : ICardGroupModel, new()
        {
            if (constraint.Length == 0)
                return null;
            constraint = constraint.OrderByDescending(d => d).ToArray();

            return SelectedCardGroupGreaterCard<T>(constraint, cards, containCards, card);
        }

        private static PokerCard[] SelectedCardGroupGreaterCard<T>(int[] constraint, List<PokerCard> cards, PokerCard[] containCards = null, PokerCard card = null) where T : ICardGroupModel, new()
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

            int cardNumber = (card == null) ?   GetMinNumber<T>(): card.Number;

            for (int constraintIndex = 0; constraintIndex < constraint.Length; constraintIndex++)
            {
                requiredCount = constraint[constraintIndex];

                //check is exist same constraint card and bigger number
                int[] numbers = cardNumberCounts
                    .Where(d => d.cardCount >= requiredCount)
                    .Select(d => d.number)
                    .Except(selectedNumbers)
                    .Where(d => !isMaxConstraint || CompareNumber<T>(d, cardNumber) != -1)
                    .ToArray();

                bool isNotFoundFitNumber = numbers.Length == 0;
                if (isNotFoundFitNumber)
                    return null;

                numbers = OrderNumber<T>(numbers);

                //由點選順序看選定的每個牌號能不能當作最大牌組，最選優先
                int[] containNumbers = SpiltContainNumber(ref numbers, containCards);

                int containNumbersCount = containNumbers.Length;
                int number;
                isNotFoundFitNumber = true;
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

                            bool moreConstraint = constraint.Length > 1;
                            if (moreConstraint && !CheckNextConstraint(constraint, constraintIndex, nextCard, nextContainCards))
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
                    isNotFoundFitNumber = false;
                    break;
                }

                if (isNotFoundFitNumber)
                    return null;

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
             PokerCard card = null)
        {
            List<PokerCard> resultBuff = new List<PokerCard>();
            int containSuitsCount = containSuits.Length;
            bool isSameNumber = (card == null) ? false : card.Number == number;
            int cardSuitIndex = (card == null) ? 0 : GetIBySuit(card.Suit);// 0 not care, cause isSameNumber is false, and hasGreaterSuitToCard is true
            bool hasGreaterSuitToCard = (card == null) ? true : false;

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