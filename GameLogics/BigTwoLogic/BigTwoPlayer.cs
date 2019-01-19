using GameLogic.Game;
using GameLogic.Player;
using GameLogic.PokerGame;
using GameLogic.PokerGame.CardGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BigTwoLogic
{
    public class BigTwoPlayer : GamePlayer
    {
        private BigTwo Game
        {
            get { return (BigTwo)_game; }
        }

        public BigTwoPlayer(PlayerInfo player)
            : base(player.Id,player.Name)
        {
            _resource = new PokerResource(Id);
        }

        public BigTwoPlayer(GamePlayer gamePlayer)
            : base(gamePlayer.Id,gamePlayer.Name)
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
            return _game.GetGameStatus().CurrentPlayerId == Id;
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
                for(int i = 0; i < containCard.Count; i++)
                {
                    if (BigTwo.IsCLub3(containCard[i]))
                    {
                        containCard.RemoveAt(i);
                        break;
                    }
                }

                containCard.Add(BigTwo.CLUB_3);
            }

            PokerCard maxCard;
            List<PokerGroupType> types = new List<PokerGroupType>();
            PokerGroupType type = PokerGroupType.Single;
            bool isFreeType = Game.IsFreeType;
            if (isFreeType)
            {
                maxCard = null;
                TryAllUntilExcetion(containCard, (selectedCards) =>
                {
                    type = PokerCardGroup.GetMinCardGroupType<BigTwoCardGroupModel>(cards, selectedCards);
                });

                types.Add(type);
            }
            else
            {
                //check previous type
                PokerCardGroup lastGroup = GetTableLast();
                type = lastGroup.GetGroupType();
                maxCard = lastGroup.GetMaxValue();

                types.Add(type);
                types.AddRange(
                    BigTwo.SUPER_GROUP_TYPE_ORDERS
                        .Where(d => PokerCardGroup.Compare_Type(d, type) > 0)
                );
            }

            PokerCard[] result = null;
            TryAllUntilExcetion(containCard, (selectedCards) =>
            {
                foreach(PokerGroupType t in types)
                {
                    PokerCard maxValueCard= null;
                    if (t.Equals(type))
                        maxValueCard = maxCard;

                    result = PokerCardGroup
                        .GetMinCardGroupInGroupTypeGreaterThenCard<BigTwoCardGroupModel>(
                            t,
                            cards.ToList(),
                            selectedCards,
                            maxValueCard
                        );
                    if (result != null)
                        break;
                }

                if (result == null)
                    throw new Exception();
            });

            return result.OrderBy(d => d.Number).ThenBy(d => d.Suit).ToArray();
        }

        private PokerGroupType[] GetSuperGroupTypeGreater(PokerGroupType type)
        {
            if (type == PokerGroupType.Dragon)
                return null;

            PokerGroupType[] superGroupTypes = BigTwo.SUPER_GROUP_TYPE_ORDERS;
            superGroupTypes = superGroupTypes.Reverse().ToArray();
            int index = superGroupTypes.ToList().IndexOf(type);
            if (index == -1)
                index = superGroupTypes.Length;

            List<PokerGroupType> result = new List<PokerGroupType>();
            for (int i = 0; i < index; i++)
                result.Add(superGroupTypes[i]);

            result.Reverse();

            return result.ToArray();
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
            List<PokerCard> cards = new List<PokerCard>();
            cards.AddRange(selectedCards);

            for (; cards.Count > 0; cards.RemoveAt(0))
            {
                try
                {
                    action(cards.ToArray());
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