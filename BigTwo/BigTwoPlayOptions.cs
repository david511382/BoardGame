using GameFramework.Player;
using GameFramework.PokerGame;
using GameFramework.PokerGame.CardGroup;
using GameFramework.PokerGame.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BigTwo
{
    public partial class BigTwo : PokerGame
    {
        private int _lastPlayTurnId;

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
                PokerCardGroup lastGroup =(PokerCardGroup)GetTable().GetLastItem();
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