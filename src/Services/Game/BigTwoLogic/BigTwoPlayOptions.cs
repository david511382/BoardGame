using GameLogic.Game;
using GameLogic.PokerGame;
using GameLogic.PokerGame.CardGroup;
using GameLogic.PokerGame.Game;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BigTwoLogic
{
    public partial class BigTwo : PokerGame<int>
    {
        private int _lastPlayTurnId;

        public override void Load(string json)
        {
            LoadModel data = JsonConvert.DeserializeObject<LoadModel>(json);
            _lastPlayTurnId = data.LastPlayTurnId;
            _gameStaus = data.GameStatus;
            _playerResources = data.PlayerResources.Select((p) => p as PlayerResource).ToList();
            base.currentTurn = data.CurrentTurn;
            IsFreeType = data.IsFreeType;
            IsRequiredClub3 = data.IsRequiredClub3;
            Table = data.Table;
        }

        public override string ExportData()
        {
            LoadModel data = new LoadModel
            {
                CurrentTurn = base.currentTurn,
                IsFreeType = IsFreeType,
                IsRequiredClub3 = IsRequiredClub3,
                GameStatus = _gameStaus,
                Table = Table,
                LastPlayTurnId = _lastPlayTurnId,
                PlayerResources = _playerResources.Select((p) => p as PokerResource).ToArray()
            };

            return JsonConvert.SerializeObject(data);
        }

        public bool Pass()
        {
            if (IsGameOver())
                return false;

            //can not pass
            if (IsFreeType)
                return false;

            //next turn
            NextTurn();

            if (_lastPlayTurnId == currentTurn)
                IsFreeType = true;

            return true;
        }

        public PokerCard[] SelectCardGroup(int playerId, int[] containCardIndexs)
        {
            if (!IsTurn(playerId))
                return null;

            PokerResource playerResource = GetResource(playerId);
            PokerCard[] cards = GetResource(playerId).GetHandCards()
                .OrderBy(d => d.Number)
                .ThenBy(d => d.Suit)
                .ToArray();
            List<PokerCard> containCard = cards.Choose(containCardIndexs)
                .ToList();

            if (IsRequiredClub3)
            {
                for (int i = 0; i < containCard.Count; i++)
                {
                    if (IsCLub3(containCard[i]))
                    {
                        containCard.RemoveAt(i);
                        break;
                    }
                }

                containCard.Add(CLUB_3);
            }

            PokerCard maxCard;
            List<PokerGroupType> types = new List<PokerGroupType>();
            PokerGroupType type = PokerGroupType.Single;
            if (IsFreeType)
            {
                maxCard = null;
                tryAllUntilExcetion(containCard, (selectedCards) =>
                {
                    type = PokerCardGroup.GetMinCardGroupType<BigTwoCardGroupModel>(cards, selectedCards);
                });

                types.Add(type);
            }
            else
            {
                //check previous type
                type = lastGroup.GetGroupType();
                maxCard = lastGroup.GetMaxValue();

                types.Add(type);
                types.AddRange(
                    SUPER_GROUP_TYPE_ORDERS
                        .Where(d => PokerCardGroup.Compare_Type(d, type) > 0)
                );
            }

            PokerCard[] result = null;
            tryAllUntilExcetion(containCard, (selectedCards) =>
            {
                PokerCard[] bufResult = null;
                foreach (PokerGroupType t in types)
                {
                    PokerCard maxValueCard = null;
                    if (t.Equals(type))
                        maxValueCard = maxCard;

                    bufResult = PokerCardGroup
                        .GetMinCardGroupInGroupTypeGreaterThenCard<BigTwoCardGroupModel>(
                            t,
                            cards.ToList(),
                            selectedCards,
                            maxValueCard
                        );

                    if (bufResult != null)
                    {
                        result = bufResult;

                        bool isUseSameContainCard = PokerCardGroup.Intersect(
                                                        bufResult,
                                                        containCard.ToArray()
                                                        ).Length == containCard.Count;
                        if (isUseSameContainCard)
                            break;
                    }
                }

                if (result == null)
                    throw new Exception();
            }, true);

            return result.OrderBy(d => d.Number).ThenBy(d => d.Suit).ToArray();
        }

        public bool PlayCard(int playerId, int[] cardIndexs)
        {
            if (!IsTurn(playerId))
                return false;

            // pass
            if (cardIndexs == null || cardIndexs.Length == 0)
                return Pass();

            PokerCard[] cards = GetResource(playerId).GetHandCards()
                .OrderBy(d => d.Number)
                .ThenBy(d => d.Suit)
                .ToArray();
            PokerCard[] containCard = cards.Choose(cardIndexs).ToArray();

            return PlayGroups(new PokerCardGroup(containCard));
        }

        public bool PlayGroups(PokerCardGroup cardGroup)
        {
            if (IsGameOver())
                return false;

            //check group
            PokerCard cardGroupMaxCard = cardGroup.GetMaxValue();
            if (cardGroupMaxCard == null)
                return false;

            //check cards group type
            PokerGroupType cardGroupType = cardGroup.GetGroupType();

            if (IsRequiredClub3)
            {
                if (!cardGroup.GetCards().Any((c) => IsCLub3(c)))
                    return false;
            }

            //check cards playable
            if (!IsFreeType)
            {
                //check previous type
                PokerCardGroup lastGroup = GetTable().GetLastItem();
                PokerGroupType lastGroupType = lastGroup.GetGroupType();

                bool isSameType = cardGroupType == lastGroupType;
                if (isSameType)
                {
                    //smaller value
                    PokerCard maxCard = lastGroup.GetMaxValue();
                    if (CompareCard(cardGroupMaxCard, maxCard) != 1)
                        return false;
                }
                else
                {
                    //different type
                    bool isSuperGroupType = SUPER_GROUP_TYPE_ORDERS.Contains(cardGroupType);
                    if (isSuperGroupType)
                    {
                        bool isSmallerType = PokerCardGroup.Compare_Type(cardGroupType, lastGroupType) < 0;
                        if (isSmallerType)
                            return false;
                    }
                    else
                        return false;
                }
            }

            //play
            Table.Put(cardGroup);

            IsFreeType = false;
            IsRequiredClub3 = false;

            //remove hand cards
            CurrentPlayerResource.RemoveHandCards(cardGroup.GetCards());

            _lastPlayTurnId = currentTurn;

            if (CurrentPlayerResource.GetHandCards().Length == 0)
            {
                GameOver();
            }
            else
            {
                //next turn
                NextTurn();
            }

            return true;
        }

        private void GameOver()
        {
            base.GameOver(new int[] { CurrentPlayerResource.PlayerId });
        }
    }
}