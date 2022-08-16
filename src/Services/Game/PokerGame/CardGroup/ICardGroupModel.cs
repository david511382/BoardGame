using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameLogic.PokerGame.CardGroup.PokerCardGroup;

namespace GameLogic.PokerGame.CardGroup
{
    public interface ICardGroupModel
    {
        int GetMaxNumber();

        int GetCompareValue(int number);

        int CompareCard(PokerCard a, PokerCard b);

        PokerCard[] GetStraightNumber(PokerCard[] cards, int length, PokerCard value = null, SuitEqul suitEqul = SuitEqul.dontCare);
    }
}
