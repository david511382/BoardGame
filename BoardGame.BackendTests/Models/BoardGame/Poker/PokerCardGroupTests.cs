using Microsoft.VisualStudio.TestTools.UnitTesting;
using BoardGame.Backend.Models.Game.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGame.Backend.Models.Game.BoardGame.PokerGame.Tests
{
    [TestClass()]
    public class PokerCardGroupTests
    {
        [TestMethod()]
        public void GetMinCardGroupInGroupTypeGreaterThenCardTest()
        {
            PokerCard[] handCards= new PokerCard[]
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

            PokerCard currentMax = new PokerCard(PokerSuit.Heart,2);

            PokerCard[] selectCards = new PokerCard[]
             {
                 new PokerCard(PokerSuit.Heart,1)
             };

            PokerCard[]  result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard(PokerGroupType.Full_House, currentMax, handCards.ToList(), selectCards);
            PokerCard[] expectResult = {
                    new PokerCard(PokerSuit.Club,1),
                    new PokerCard(PokerSuit.Heart,1),
                    new PokerCard(PokerSuit.Club,2),
                    new PokerCard(PokerSuit.Diamond,2),
                    new PokerCard(PokerSuit.Spade,2)
              };

            bool isFound = false;
            for(int i = 0; i < expectResult.Length; i++)
            {
                for(int j = 0; j< result.Length; j++)
                {
                    if (expectResult[i].isSame(result[j]))
                    {
                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                    Assert.Fail();
            }
        }
    }
}