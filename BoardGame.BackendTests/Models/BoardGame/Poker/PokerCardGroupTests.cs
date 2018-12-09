using Microsoft.VisualStudio.TestTools.UnitTesting;
using BoardGame.Backend.Models.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGame.Backend.Models.BoardGame.PokerGame.Tests
{
    [TestClass()]
    public class PokerCardGroupTests
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
                    new PokerCard(PokerSuit.Club,11),
                    new PokerCard(PokerSuit.Club,12),
                    new PokerCard(PokerSuit.Club,13)
              };

            PokerCard currentMax = new PokerCard(PokerSuit.Heart, 2);

            PokerCard[] selectCards = new PokerCard[]
             {
                 new PokerCard(PokerSuit.Heart,1)
             };

            PokerCard[] result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard(PokerGroupType.Full_House, currentMax, handCards.ToList(), selectCards);
            PokerCard[] expectResult = {
                    new PokerCard(PokerSuit.Club,1),
                    new PokerCard(PokerSuit.Heart,1),
                    new PokerCard(PokerSuit.Club,2),
                    new PokerCard(PokerSuit.Diamond,2),
                    new PokerCard(PokerSuit.Spade,2)
              };

            CheckEqualPokerCards(result, expectResult);



            result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard(PokerGroupType.Full_House, null, handCards.ToList(), selectCards);
            expectResult = new PokerCard[]{
                    new PokerCard(PokerSuit.Club,1),
                    new PokerCard(PokerSuit.Diamond,1),
                    new PokerCard(PokerSuit.Heart,1),
                    new PokerCard(PokerSuit.Club,2),
                    new PokerCard(PokerSuit.Diamond,2)
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

            PokerGroupType result = PokerCardGroup.GetCardGroupType(handCards, selectCards);
            Assert.AreEqual(result, PokerGroupType.Pair);


            selectCards = new PokerCard[]
           {
                 new PokerCard(PokerSuit.Heart,1),
                 new PokerCard(PokerSuit.Spade,1),
                 new PokerCard(PokerSuit.Spade,2)
           };

            result = PokerCardGroup.GetCardGroupType(handCards, selectCards);
            Assert.AreEqual(result, PokerGroupType.Full_House);


            selectCards = new PokerCard[]
           {
                 new PokerCard(PokerSuit.Heart,1),
                 new PokerCard(PokerSuit.Spade,1),
                 new PokerCard(PokerSuit.Spade,2),
                 new PokerCard(PokerSuit.Club,2)
           };

            result = PokerCardGroup.GetCardGroupType(handCards, selectCards);
            Assert.AreEqual(result, PokerGroupType.Full_House);


            selectCards = new PokerCard[]
           {
                 new PokerCard(PokerSuit.Heart,1),
                 new PokerCard(PokerSuit.Spade,1),
                 new PokerCard(PokerSuit.Diamond,1),
                 new PokerCard(PokerSuit.Heart,2),
                 new PokerCard(PokerSuit.Club,2)
           };

            result = PokerCardGroup.GetCardGroupType(handCards, selectCards);
            Assert.AreEqual(result, PokerGroupType.Full_House);


            selectCards = new PokerCard[]
           {
                 new PokerCard(PokerSuit.Heart,1),
                 new PokerCard(PokerSuit.Club,10),
                 new PokerCard(PokerSuit.Club,11)
           };

            result = PokerCardGroup.GetCardGroupType(handCards, selectCards);
            Assert.AreEqual(result, PokerGroupType.Straight);


            selectCards = new PokerCard[]
           {
                     new PokerCard(PokerSuit.Heart,1),
                     new PokerCard(PokerSuit.Heart,11),
                     new PokerCard(PokerSuit.Heart,10)
           };

            result = PokerCardGroup.GetCardGroupType(handCards, selectCards);
            Assert.AreEqual(result, PokerGroupType.Straight_Flush);
        }

        private bool CheckEqualPokerCards(PokerCard[] aCards, PokerCard[] bCards)
        {
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