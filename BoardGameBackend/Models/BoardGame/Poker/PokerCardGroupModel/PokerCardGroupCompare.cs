using BoardGame.Backend.Models.BoardGame.GameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.PokerGame
{
    public partial class PokerCardGroup 
    {
        private static readonly PokerGroupType[] GROUP_TYPE_ORDERS =
        {
             PokerGroupType.Single,
             PokerGroupType.Pair,
             PokerGroupType.Straight,
             PokerGroupType.Full_House,
             PokerGroupType.Four_Of_A_Kind,
             PokerGroupType.Straight_Flush,
             PokerGroupType.Dragon
        };

        public static IEnumerable<PokerGroupType> OrderGroupType(IEnumerable<PokerGroupType> groupTypes,bool asc = true)
        {
            List<PokerGroupType> orderList = new List<PokerGroupType>();
            orderList.AddRange(GROUP_TYPE_ORDERS);

            return (asc) ?
                groupTypes
                    .OrderBy(d => orderList.IndexOf(d)) :
                groupTypes
                    .OrderByDescending(d => orderList.IndexOf(d));
        }

        public static int Compare_Type(PokerGroupType a, PokerGroupType b)
        {
            bool aEql, bEql;
            foreach(PokerGroupType t in GROUP_TYPE_ORDERS)
            {
                aEql = t.Equals(a);
                bEql = t.Equals(b);

                if (aEql && bEql)
                    return 0;
                else if (aEql)
                    return -1;
                else if (bEql)
                    return 1;
            }

            throw new Exception("undefind type");
        }
    }
}