﻿using BoardGame.Backend.Models.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.BigTwo
{
    public partial class BigTwo : PokerGame.PokerGame
    {
        public bool PlayGroups(PokerCardGroup cardGroup)
        {
            //check group
            PokerCard cardGroupMaxCard = cardGroup.GetMaxValue();
            if (cardGroupMaxCard == null)
                return false;

            //check cards group type
            PokerGroupType cardGroupType = cardGroup.GetGroupType();

            //check cards playable
            if (!_isFreeType)
            {
                //check previous type
                PokerCardGroup lastGroup =(PokerCardGroup)GetTable().GetLastItem();

                //different type
                if (cardGroupType != lastGroup.GetGroupType())
                    return false;

                //smaller value
                PokerCard maxCard = lastGroup.GetMaxValue();
                if (CompareCard(cardGroupMaxCard, maxCard) != 1)
                    return false;
            }

            //play
            _table.Put(cardGroup);

            //remove hand cards
            CurrentPlayerResource.RemoveHandCards(cardGroup.GetCards());

            //next turn
            NextTurn();

            return true;
        }
    }
}