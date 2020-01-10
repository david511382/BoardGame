using GameLogic.Game;
using GameLogic.PokerGame;
using GameLogic.PokerGame.CardGroup;
using GameLogic.PokerGame.Game;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace BigTwoLogic
{
    public partial class BigTwo : PokerGame
    {
        private int _lastPlayTurnId;

        public override void Load(string json)
        {
            LoadModel data = JsonConvert.DeserializeObject<LoadModel>(json);
            _gameStaus = data.GameStatus;
            _playerResources = data.PlayerResources.Select((p) => p as PlayerResource).ToList();
            base.currentTurn = data.CurrentTurn;
            IsFreeType = data.IsFreeType;
            IsRequiredClub3 = data.IsRequiredClub3;
            _table = data.Table;
        }

        public override string ExportData()
        {
            LoadModel data = new LoadModel
            {
                CurrentTurn = base.currentTurn,
                IsFreeType = IsFreeType,
                IsRequiredClub3 = IsRequiredClub3,
                GameStatus = _gameStaus,
                Table = _table,
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

            //check cards playable
            if (!IsFreeType)
            {
                //check previous type
                PokerCardGroup lastGroup = (PokerCardGroup)GetTable().GetLastItem();
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
            _table.Put(cardGroup);

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