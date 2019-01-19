using BigTwoLogic;
using GameLogic.PokerGame;
using GameLogic.PokerGame.CardGroup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGame.GameLogicTests.BigTwoTest
{
    [TestClass()]
    public class BigTwoCardGroupTests
    {
        [TestMethod()]
        public void GetMinCardGroupInGroupTypeGreaterThenCardTest()
        {
            PokerCard[] handCards = new PokerCard[]
              {
                    new PokerCard(PokerSuit.Club,1),
                    new PokerCard(PokerSuit.Diamond,1),
                    new PokerCard(PokerSuit.Heart,1),
                    new PokerCard(PokerSuit.Spade,1),
                    new PokerCard(PokerSuit.Club,2),
                    new PokerCard(PokerSuit.Diamond,2),
                    new PokerCard(PokerSuit.Heart,2),
                    new PokerCard(PokerSuit.Spade,2),
                    new PokerCard(PokerSuit.Diamond,3),
                    new PokerCard(PokerSuit.Spade,4),
                    new PokerCard(PokerSuit.Spade,5),
                    new PokerCard(PokerSuit.Spade,6),
                    new PokerCard(PokerSuit.Heart,7),
                    new PokerCard(PokerSuit.Heart,9),
                    new PokerCard(PokerSuit.Diamond,10),
                    new PokerCard(PokerSuit.Club,11),
                    new PokerCard(PokerSuit.Club,12),
                    new PokerCard(PokerSuit.Club,13)
              };

            PokerCard currentMax = new PokerCard(PokerSuit.Heart, 2);

            PokerCard[] selectCards = new PokerCard[]
             {
                 new PokerCard(PokerSuit.Heart,1)
             };

            PokerCard[] result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard<BigTwoCardGroupModel>(PokerGroupType.Full_House, handCards.ToList(), selectCards, currentMax);
            PokerCard[] expectResult = {
                    new PokerCard(PokerSuit.Club,1),
                    new PokerCard(PokerSuit.Heart,1),
                    new PokerCard(PokerSuit.Club,2),
                    new PokerCard(PokerSuit.Diamond,2),
                    new PokerCard(PokerSuit.Spade,2)
              };
            CheckEqualPokerCards(result, expectResult);


            result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard<BigTwoCardGroupModel>(PokerGroupType.Full_House, handCards.ToList(), selectCards);
            expectResult = new PokerCard[]{
                    new PokerCard(PokerSuit.Club,1),
                    new PokerCard(PokerSuit.Diamond,1),
                    new PokerCard(PokerSuit.Heart,1),
                    new PokerCard(PokerSuit.Club,2),
                    new PokerCard(PokerSuit.Diamond,2)
              };
            CheckEqualPokerCards(result, expectResult);


            selectCards = new PokerCard[]{
                new PokerCard(PokerSuit.Diamond,3)
            };
            result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard<BigTwoCardGroupModel>(PokerGroupType.Straight, handCards.ToList(), selectCards, currentMax);
            expectResult = new PokerCard[]{
                    new PokerCard(PokerSuit.Spade,2),
                    new PokerCard(PokerSuit.Diamond,3),
                    new PokerCard(PokerSuit.Spade,4),
                    new PokerCard(PokerSuit.Spade,5),
                    new PokerCard(PokerSuit.Spade,6)
              };
            CheckEqualPokerCards(result, expectResult);


            selectCards = new PokerCard[]{
                new PokerCard(PokerSuit.Spade,2)
            };
            result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard<BigTwoCardGroupModel>(PokerGroupType.Straight, handCards.ToList(), selectCards, currentMax);
            expectResult = new PokerCard[]{
                    new PokerCard(PokerSuit.Club,1),
                    new PokerCard(PokerSuit.Spade,2),
                    new PokerCard(PokerSuit.Diamond,3),
                    new PokerCard(PokerSuit.Spade,4),
                    new PokerCard(PokerSuit.Spade,5)
              };
            CheckEqualPokerCards(result, expectResult);


            selectCards = new PokerCard[]{
                new PokerCard(PokerSuit.Diamond,10)
            };
            currentMax = new PokerCard(PokerSuit.Heart, 13);
            result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard<BigTwoCardGroupModel>(PokerGroupType.Straight, handCards.ToList(), selectCards, currentMax);
            expectResult = new PokerCard[]{
                    new PokerCard(PokerSuit.Club,1),
                    new PokerCard(PokerSuit.Diamond,10),
                    new PokerCard(PokerSuit.Club,11),
                    new PokerCard(PokerSuit.Club,12),
                    new PokerCard(PokerSuit.Club,13)
              };
            CheckEqualPokerCards(result, expectResult);


            selectCards = new PokerCard[]{
                new PokerCard(PokerSuit.Club,1)
            };
            currentMax = new PokerCard(PokerSuit.Spade, 2);
            result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard<BigTwoCardGroupModel>(PokerGroupType.Full_House, handCards.ToList(), selectCards, currentMax);
            expectResult = null;
            CheckEqualPokerCards(result, expectResult);


            handCards = new PokerCard[]
             {
                    new PokerCard(PokerSuit.Diamond,1),
                    new PokerCard(PokerSuit.Heart,1),
                    new PokerCard(PokerSuit.Club,2),
                    new PokerCard(PokerSuit.Club,4),
                    new PokerCard(PokerSuit.Heart,4),
                    new PokerCard(PokerSuit.Spade,6),
                    new PokerCard(PokerSuit.Diamond,8),
                    new PokerCard(PokerSuit.Heart,8),
                    new PokerCard(PokerSuit.Club,10),
                    new PokerCard(PokerSuit.Spade,10),
                    new PokerCard(PokerSuit.Spade,12),
                    new PokerCard(PokerSuit.Diamond,13)
             };
            selectCards = new PokerCard[]{
                new PokerCard(PokerSuit.Club,10)
            };
            currentMax = new PokerCard(PokerSuit.Heart, 12);
            result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard<BigTwoCardGroupModel>(PokerGroupType.Single, handCards.ToList(), selectCards, currentMax);
            expectResult = new PokerCard[]{
                    new PokerCard(PokerSuit.Spade,12)
              };
            CheckEqualPokerCards(result, expectResult);


            handCards = new PokerCard[]
            {
                    new PokerCard(PokerSuit.Heart,1),
                    new PokerCard(PokerSuit.Heart,2),
                    new PokerCard(PokerSuit.Heart,3),
                    new PokerCard(PokerSuit.Heart,4),
                    new PokerCard(PokerSuit.Heart,5),
                    new PokerCard(PokerSuit.Heart,6),
                    new PokerCard(PokerSuit.Heart,7),
                    new PokerCard(PokerSuit.Heart,8),
                    new PokerCard(PokerSuit.Heart,9),
                    new PokerCard(PokerSuit.Heart,10),
                    new PokerCard(PokerSuit.Heart,11),
                    new PokerCard(PokerSuit.Heart,12),
                    new PokerCard(PokerSuit.Heart,13)
            };
            selectCards = new PokerCard[]{
                new PokerCard(PokerSuit.Heart,2),
                new PokerCard(PokerSuit.Heart,7)
            };
            result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard<BigTwoCardGroupModel>(PokerGroupType.Dragon, handCards.ToList(), selectCards);
            expectResult = new PokerCard[]
            {
                    new PokerCard(PokerSuit.Heart,1),
                    new PokerCard(PokerSuit.Heart,2),
                    new PokerCard(PokerSuit.Heart,3),
                    new PokerCard(PokerSuit.Heart,4),
                    new PokerCard(PokerSuit.Heart,5),
                    new PokerCard(PokerSuit.Heart,6),
                    new PokerCard(PokerSuit.Heart,7),
                    new PokerCard(PokerSuit.Heart,8),
                    new PokerCard(PokerSuit.Heart,9),
                    new PokerCard(PokerSuit.Heart,10),
                    new PokerCard(PokerSuit.Heart,11),
                    new PokerCard(PokerSuit.Heart,12),
                    new PokerCard(PokerSuit.Heart,13)
            };
            CheckEqualPokerCards(result, expectResult);


            selectCards = new PokerCard[]{
                new PokerCard(PokerSuit.Heart,3),
                new PokerCard(PokerSuit.Heart,6)
            };
            result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard<BigTwoCardGroupModel>(PokerGroupType.Straight_Flush, handCards.ToList(), selectCards);
            expectResult = new PokerCard[]
            {
                    new PokerCard(PokerSuit.Heart,3),
                    new PokerCard(PokerSuit.Heart,4),
                    new PokerCard(PokerSuit.Heart,5),
                    new PokerCard(PokerSuit.Heart,6),
                    new PokerCard(PokerSuit.Heart,7)
            };
            CheckEqualPokerCards(result, expectResult);


            handCards = new PokerCard[]
            {
                new PokerCard(PokerSuit.Heart,1),
                new PokerCard(PokerSuit.Spade,2),
                new PokerCard(PokerSuit.Club,3),
                new PokerCard(PokerSuit.Club,4),
                new PokerCard(PokerSuit.Heart,4),
                new PokerCard(PokerSuit.Heart,5),
                new PokerCard(PokerSuit.Diamond,6),
                new PokerCard(PokerSuit.Diamond,7),
                new PokerCard(PokerSuit.Diamond,10),
                new PokerCard(PokerSuit.Spade,10),
                new PokerCard(PokerSuit.Club,13),
                new PokerCard(PokerSuit.Heart,13),
                new PokerCard(PokerSuit.Spade,13),
            };
            selectCards = new PokerCard[]
            {
                new PokerCard(PokerSuit.Diamond,6),
                new PokerCard(PokerSuit.Club,3)
            };
            result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard<BigTwoCardGroupModel>(PokerGroupType.Straight, handCards.ToList(), selectCards);
            expectResult = new PokerCard[]
            {
                new PokerCard(PokerSuit.Club,3),
                new PokerCard(PokerSuit.Club,4),
                new PokerCard(PokerSuit.Heart,5),
                new PokerCard(PokerSuit.Diamond,6),
                new PokerCard(PokerSuit.Diamond,7)
            };
            CheckEqualPokerCards(result, expectResult);
        }

        [TestMethod()]
        public void GetCardGroupTypeTest()
        {
            PokerCard[] handCards = new PokerCard[52];
            for (int i = 0; i < Poker.NUMBER_NUM; i++)
            {
                for (int j = 0; j < Poker.SUIT_NUM; j++)
                    handCards[i * Poker.SUIT_NUM + j] = new PokerCard(PokerCardGroup.GetSuitByI(j), i + 1);
            }

            PokerCard[] selectCards = new PokerCard[]
             {
                 new PokerCard(PokerSuit.Heart,1),
                 new PokerCard(PokerSuit.Spade,1)
             };

            PokerGroupType[] results;
            PokerGroupType result = PokerCardGroup.GetMinCardGroupType<BigTwoCardGroupModel>(handCards, selectCards);
            Assert.AreEqual(result, PokerGroupType.Pair);


            selectCards = new PokerCard[]
           {
                 new PokerCard(PokerSuit.Heart,1),
                 new PokerCard(PokerSuit.Spade,1),
                 new PokerCard(PokerSuit.Spade,2)
           };

            result = PokerCardGroup.GetMinCardGroupType<BigTwoCardGroupModel>(handCards, selectCards);
            Assert.AreEqual(result, PokerGroupType.Full_House);


            selectCards = new PokerCard[]
           {
                 new PokerCard(PokerSuit.Heart,1),
                 new PokerCard(PokerSuit.Spade,1),
                 new PokerCard(PokerSuit.Spade,2),
                 new PokerCard(PokerSuit.Club,2)
           };

            result = PokerCardGroup.GetMinCardGroupType<BigTwoCardGroupModel>(handCards, selectCards);
            Assert.AreEqual(result, PokerGroupType.Full_House);


            selectCards = new PokerCard[]
           {
                 new PokerCard(PokerSuit.Heart,1),
                 new PokerCard(PokerSuit.Spade,1),
                 new PokerCard(PokerSuit.Diamond,1),
                 new PokerCard(PokerSuit.Heart,2),
                 new PokerCard(PokerSuit.Club,2)
           };

            result = PokerCardGroup.GetMinCardGroupType<BigTwoCardGroupModel>(handCards, selectCards);
            Assert.AreEqual(result, PokerGroupType.Full_House);


            selectCards = new PokerCard[]
            {
                new PokerCard(PokerSuit.Heart,1),
                new PokerCard(PokerSuit.Club,10),
                new PokerCard(PokerSuit.Club,11)
            };

            result = PokerCardGroup.GetMinCardGroupType<BigTwoCardGroupModel>(handCards, selectCards);
            Assert.AreEqual(result, PokerGroupType.Straight);


            selectCards = new PokerCard[]
            {
                new PokerCard(PokerSuit.Club,1),
                new PokerCard(PokerSuit.Club,3)
            };
            handCards = new PokerCard[]
            {
                new PokerCard(PokerSuit.Club,1),
                new PokerCard(PokerSuit.Heart,1),
                new PokerCard(PokerSuit.Club,3),
                new PokerCard(PokerSuit.Spade,3)
            };

            bool isFail = false;
            try
            {
                result = PokerCardGroup.GetMinCardGroupType<BigTwoCardGroupModel>(handCards, selectCards);
                isFail = true;
            }
            catch
            {

            }
            if (isFail)
                Assert.Fail();


            isFail = false;
            selectCards = new PokerCard[]
            {
                new PokerCard(PokerSuit.Heart,6),
                new PokerCard(PokerSuit.Club,3)
            };
            handCards = new PokerCard[]
            {
                new PokerCard(PokerSuit.Heart,1),
                new PokerCard(PokerSuit.Club,3),
                new PokerCard(PokerSuit.Diamond,3),
                new PokerCard(PokerSuit.Spade,3),
                new PokerCard(PokerSuit.Diamond,4),
                new PokerCard(PokerSuit.Diamond,5),
                new PokerCard(PokerSuit.Heart,6),
                new PokerCard(PokerSuit.Club,8),
                new PokerCard(PokerSuit.Diamond,8),
                new PokerCard(PokerSuit.Club,9),
                new PokerCard(PokerSuit.Heart,10),
                new PokerCard(PokerSuit.Heart,11),
                new PokerCard(PokerSuit.Heart,12),
                new PokerCard(PokerSuit.Heart,13)
            };
            try
            {
                result = PokerCardGroup.GetMinCardGroupType<BigTwoCardGroupModel>(handCards, selectCards);
                isFail = true;
            }
            catch
            {

            }
            if (isFail)
                Assert.Fail();


            selectCards = new PokerCard[]
            {
                new PokerCard(PokerSuit.Heart,1),
                new PokerCard(PokerSuit.Heart,11),
                new PokerCard(PokerSuit.Heart,10)
            };
            result = PokerCardGroup.GetMinCardGroupType<BigTwoCardGroupModel>(handCards, selectCards);
            Assert.AreEqual(result, PokerGroupType.Straight_Flush);


            handCards = new PokerCard[]
            {
                new PokerCard(PokerSuit.Diamond,1),
                new PokerCard(PokerSuit.Heart,2),
                new PokerCard(PokerSuit.Spade,2),
                new PokerCard(PokerSuit.Spade,4),
                new PokerCard(PokerSuit.Club,5),
                new PokerCard(PokerSuit.Heart,6),
                new PokerCard(PokerSuit.Club,8),
                new PokerCard(PokerSuit.Heart,9),
                new PokerCard(PokerSuit.Spade,9),
                new PokerCard(PokerSuit.Diamond,10),
                new PokerCard(PokerSuit.Club,11),
                new PokerCard(PokerSuit.Spade,11),
                new PokerCard(PokerSuit.Club,12),
            };
            selectCards = new PokerCard[]
            {
                new PokerCard(PokerSuit.Diamond,10)
            };
            results = PokerCardGroup.GetCardGroupType<BigTwoCardGroupModel>(handCards, selectCards);
            Assert.AreEqual(results.First(), PokerGroupType.Single);
            Assert.AreEqual(results.Last(), PokerGroupType.Straight);
            Assert.AreEqual(results.Count(), 2);
            results = PokerCardGroup.GetCardGroupType<BigTwoCardGroupModel>(handCards);
            Assert.AreEqual(results.First(), PokerGroupType.Single);
            Assert.AreEqual(results[1], PokerGroupType.Pair);
            Assert.AreEqual(results.Last(), PokerGroupType.Straight);
            Assert.AreEqual(results.Count(), 3);
        }

        private bool CheckEqualPokerCards(PokerCard[] aCards, PokerCard[] bCards)
        {
            if (aCards == null && aCards == bCards)
                return true;

            bool isFound = false;
            for (int i = 0; i < aCards.Length; i++)
            {
                for (int j = 0; j < bCards.Length; j++)
                {
                    if (aCards[i].isSame(bCards[j]))
                    {
                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                    Assert.Fail();
            }
            return true;
        }
    }
}