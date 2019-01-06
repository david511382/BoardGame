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
        /// <param name="cardIndexs">cardIndexs == null is pass </param>
        /// <returns></returns>
        public bool PlayCard(int[] cardIndexs)
        {
            if (!this.IsOnTurn())
                return false;

            // pass
            if (cardIndexs == null || cardIndexs.Length == 0)
                return Game.Pass();

            List<PokerCard> containCard = GetHandCards(cardIndexs);

            return Game.PlayGroups(new PokerCardGroup(containCard.ToArray()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containCardIndexs">order by select time asc</param>
        /// <returns></returns>
        public PokerCard[] GetCardGroup(int[] containCardIndexs)
        {
            if (!this.IsOnTurn())
                return null;

            PokerCard[] cards = GetHandCards();
            List<PokerCard> containCard = GetHandCards(containCardIndexs);
            bool IsRequiredClub3 = Game.IsRequiredClub3;
            if (IsRequiredClub3)
            {
                bool isContainClub3 = false;
                for(int i = 0; i < containCard.Count; i++)
                {
                    if (BigTwo.IsCLub3(containCard[i]))
                    {
                        isContainClub3 = true;
                        PokerCard p = containCard[i];
                        containCard[i] = containCard.Last();
                        containCard[containCard.Count - 1] = p;
                        break;
                    }
                }

                if (!isContainClub3)
                    containCard.Add(new PokerCard(PokerSuit.Club, 3));
            }

            PokerGroupType type = PokerGroupType.Single;
            PokerCard maxCard;
            bool isFreeType = Game.IsFreeType;
            if (isFreeType)
            {
                maxCard = null;

                TryAllUntilExcetion(containCard, (selectedCards) =>
                {
                    type = PokerCardGroup.GetCardGroupType(cards, selectedCards);
                });         
            }
            else
            {
                //check previous type
                PokerCardGroup lastGroup = GetTableLast();

                type = lastGroup.GetGroupType();
                maxCard = lastGroup.GetMaxValue();
            }

            PokerCardGroup.Max_Number = BigTwo.MAX_CARD_NUMBER;
            PokerCard[] result = null;
            TryAllUntilExcetion(containCard, (selectedCards) =>
            {
                result = PokerCardGroup.GetMinCardGroupInGroupTypeGreaterThenCard(type, maxCard, cards.ToList(), selectedCards);
                if (result == null)
                    throw new Exception();
             });

            return result.OrderBy(d => d.Number).ThenBy(d => d.Suit).ToArray();
        }

        private List<PokerCard> GetHandCards(int[] cardIndex)
        {
            PokerCard[] cards = GetHandCards();
            List<PokerCard> containCard = new List<PokerCard>();
            for (int i = 0; i < cardIndex.Length; i++)
            {
                containCard.Add(cards[cardIndex[i]]);
            }
            return containCard;
        }

        private void TryAllUntilExcetion(List<PokerCard> selectedCards,Action<PokerCard[]> action)
        {
            for (; selectedCards.Count > 0; selectedCards.RemoveAt(0))
            {
                try
                {
                    action(selectedCards.ToArray());
                    break;
                }
                catch { }
            }
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