using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.PokerGame.CardGroup
{
    public partial class PokerCardGroup
    {
        static PokerCardGroup()
        {
        }

        public static int GetMinNumber()
        {
            return GetMinNumber<CardGroupModel>();
        }

        public static int GetMinNumber<T>() where T : ICardGroupModel, new()
        {
            T model = new T();
            return (model.GetMaxNumber() + 1 > Poker.NUMBER_NUM) ? 1 : model.GetMaxNumber() + 1;
        }

        public static PokerCard[] GetMinCardGroupInGroupTypeGreaterThenCard(PokerGroupType groupType, List<PokerCard> cards, PokerCard[] containCard, PokerCard card = null)
        {
            return GetMinCardGroupInGroupTypeGreaterThenCard<CardGroupModel>(groupType, cards, containCard, card);
        }

        public static PokerCard[] GetMinCardGroupInGroupTypeGreaterThenCard<T>(PokerGroupType groupType, List<PokerCard> cards, PokerCard[] containCard, PokerCard card = null) where T : ICardGroupModel, new()
        {
            int[] constraint = GetConstraintOfType(groupType);
            bool isRightConstraint = CheckConstraint(constraint, cards.ToArray(), containCard);
            bool isContainCardInCards = CheckCardsContainCards(cards.ToArray(), containCard);
            if (!isRightConstraint || !isContainCardInCards)
                return null;

            switch (groupType)
            {
                case PokerGroupType.Single:
                case PokerGroupType.Pair:
                case PokerGroupType.Four_Of_A_Kind:
                case PokerGroupType.Full_House:
                    return GetMinCardGroupInConstraintGreaterThenCard<T>(constraint, cards, containCard, card);
                case PokerGroupType.Straight:
                case PokerGroupType.Straight_Flush:
                case PokerGroupType.Dragon:
                    return GetStraightGreater<T>(groupType, cards.ToArray(), containCard, card);
                default:
                    throw new Exception("undefine");
            }
        }

        public static PokerCard[] GetNotSameSuitStraight(IEnumerable<PokerCard> cards)
        {
            return GetNotSameSuitStraight<CardGroupModel>(cards);
        }

        public static PokerCard[] GetNotSameSuitStraight<T>(IEnumerable<PokerCard> cards) where T : ICardGroupModel, new()
        {
            T model = new T();

            bool sameSuit = cards
                       .GroupBy(d => d.Number)
                       .Select(d => d.Min(c => c.Suit))
                       .GroupBy(d => d)
                       .Count() == 1;
            if (sameSuit)
            {
                var freeCards = cards
                   .GroupBy(d => d.Number)
                   .OrderBy(d => model.GetCompareValue(d.First().Number))
                   .ToArray();

                PokerCard changeSuitCard = null;
                for (int i = 0; i < freeCards.Length; i++)
                {
                    if (freeCards[i].Count() > 1)
                    {
                        PokerSuit changeSuit = freeCards[i]
                            .OrderBy(d => d.Suit)
                            .Take(2)
                            .Last()
                            .Suit;
                        int changeNumber = freeCards[i]
                            .First()
                            .Number;
                        changeSuitCard = new PokerCard(changeSuit, changeNumber);
                        break;
                    }
                }
                if (changeSuitCard == null)
                    return null;

                List<PokerCard> bufList = cards
                    .Where(d => d.Number != changeSuitCard.Number)
                    .ToList();
                bufList.Add(changeSuitCard);
                cards = bufList.ToArray();
            }

            return cards.ToArray();
        }

        public static PokerGroupType GetMaxCardGroupType(PokerCard[] cards, PokerCard[] containCards = null)
        {
            return GetCardGroupType(cards, containCards, 1).First();
        }

        public static PokerGroupType GetMinCardGroupType(PokerCard[] cards, PokerCard[] containCards = null)
        {
            return GetCardGroupType(cards, containCards, -1).First();
        }

        public static PokerGroupType GetMaxCardGroupType<T>(PokerCard[] cards, PokerCard[] containCards = null) where T : ICardGroupModel, new()
        {
            return GetCardGroupType<T>(cards, containCards, 1).First();
        }

        public static PokerGroupType GetMinCardGroupType<T>(PokerCard[] cards, PokerCard[] containCards = null) where T : ICardGroupModel, new()
        {
            return GetCardGroupType<T>(cards, containCards, -1).First();
        }

        public static PokerGroupType[] GetCardGroupType(PokerCard[] cards, PokerCard[] containCards = null, int groupCompare = 0)
        {
            return GetCardGroupType<CardGroupModel>(cards, containCards, groupCompare);
        }

        public static PokerGroupType[] GetCardGroupType<T>(PokerCard[] cards, PokerCard[] containCards = null, int groupCompare = 0) where T : ICardGroupModel, new()
        {
            List<PokerGroupType> result = new List<PokerGroupType>();

            IEnumerable<PokerGroupType> groupTypes = Enum.GetValues(typeof(PokerGroupType))
                .Cast<PokerGroupType>();
            groupTypes = OrderGroupType
            (
                groupTypes,
                (groupCompare == 1) ?
                    false :
                    true
            );

            var constraints = groupTypes
                .Select(d => new { type = d, constraint = GetConstraintOfType(d) });

            bool isConstraintOk;
            bool isSpecalConstraintOk;
            foreach (var data in constraints)
            {
                isConstraintOk = CheckConstraint(data.constraint, cards, containCards);
                isSpecalConstraintOk = CheckStraightConstraint<T>(data.type, cards, containCards);

                if (isConstraintOk && isSpecalConstraintOk)
                {
                    result.Add(data.type);
                    if (groupCompare != 0)
                        return result.ToArray();
                }
            }

            if (result.Count == 0)
                throw new Exception("cant identitfy");

            return result.ToArray();
        }
    }
}
