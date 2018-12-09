using BoardGame.Backend.Models.BoardGame.GameFramework;
using BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer;
using BoardGame.Backend.Models.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.BigTwo
{
    public class BigTwoPlayer : GamePlayer
    {
        private BigTwo Game
        {
            get { return (BigTwo)_game; }
        }

        public BigTwoPlayer()
            : base()
        {
            _resource = new PokerResource(Id);
        }

        public BigTwoPlayer(GamePlayer gamePlayer)
            : base(gamePlayer)
        {
            _resource = new PokerResource(Id);
        }

        public PokerCard[] GetHandCards()
        {
            GameObj[] playerResource = Game.GetResource<PokerResource>(Id).GetHandCards();
            PokerCard[] drawCards = (PokerCard[])playerResource;
            drawCards = drawCards.OrderBy(d => d.Number).ThenBy(d => d.Suit).ToArray();

            return drawCards;
        }

        public bool IsOnTurn()
        {
            return _game.IsTurn(Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containCardIndexs">order by select time asc</param>
        /// <returns></returns>
        public PokerCard[] GetCardGroup(int[] containCardIndexs)
        {
            PokerCard[] cards = GetHandCards();
            PokerCard[] containCard = new PokerCard[containCardIndexs.Length];
            for (int i = 0; i < containCardIndexs.Length; i++)
            {
                containCard[i] = cards[containCardIndexs[i]];
            }

            PokerGroupType type;
            PokerCard maxCard;
            bool isFreeType = Game.IsFreeType();
            if (isFreeType)
            {
                try
                {
                    type =PokerCardGroup.GetCardGroupType(cards,containCard);
                }
                catch { type = PokerGroupType.Dragon; }
                maxCard = null;
            }
            else
            {
                //check previous type
                PokerCardGroup lastGroup = GetTableLast();

                type = lastGroup.GetGroupType();
                maxCard = lastGroup.GetMaxValue();
            }

            PokerCardGroup.Max_Number = BigTwo.MAX_CARD_NUMBER;
            PokerCard[] result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard(type, maxCard, cards.ToList(), containCard);
            result = result ?? containCard;
            return result.OrderBy(d => d.Number).ThenBy(d => d.Suit).ToArray();
        }

        private PokerCardGroup GetTableLast()
        {
            GameObj tableLastItem = _game.GetTable().GetLastItem();
            if (tableLastItem == null)
                return null;

            return (PokerCardGroup)tableLastItem;
        }
    }
}