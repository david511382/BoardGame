using GameLogic.Game;
using GameLogic.PokerGame;
using GameLogic.PokerGame.CardGroup;
using GameLogic.PokerGame.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BigTwoLogic
{
    public partial class BigTwo : PokerGame
    {
        private struct LoadModel
        {
            public bool IsFreeType;
            public bool IsRequiredClub3;
            public GameStatus GameStatus;
            public GameBoard<PokerCardGroup> Table;
            public PokerResource[] PlayerResources;
            public int LastPlayTurnId;
        }

        public const int MAX_PLAYERS = 4;
        public const int MIN_PLAYERS = 4;
        public const int MAX_CARD_NUMBER = 2;

        public static readonly PokerCard CLUB_3;

        public bool IsFreeType { get; private set; }
        public bool IsRequiredClub3 { get; private set; }

        private PokerResource CurrentPlayerResource
        {
            get { return GetResourceAt(currentTurn); }
        }

        public static bool IsCLub3(PokerCard card)
        {
            return (card.Number == 3) && (card.Suit == PokerSuit.Club);
        }

        static BigTwo()
        {
            CLUB_3 = new PokerCard(PokerSuit.Club, 3);
        }

        private PokerCardGroup lastGroup
        {
            get
            {
                PokerCardGroup tableLastItem = Table.GetLastItem();
                if (tableLastItem == null)
                    return null;

                return tableLastItem;
            }
        }

        public BigTwo()
            : base(MAX_PLAYERS, MIN_PLAYERS)
        {
            IsFreeType = true;
            IsRequiredClub3 = true;
            _lastPlayTurnId = -1;
        }

        public IEnumerable<PokerResource> GetResource()
        {
            return _playerResources.Select((s) => s as PokerResource);
        }

        public new PokerResource GetResource(int playerId)
        {
            return base.GetResource(playerId) as PokerResource;
        }

        protected override void InitGame()
        {
            base.InitGame();

            PokerCard club3 = new PokerCard(PokerSuit.Club, 3);
            for (int i = 0; i < _playerResources.Count; i++)
            {
                PokerResource playerResource = GetResourceAt(i);

                // order hand cards
                PokerCard[] handcards = playerResource.GetHandCards();
                handcards = handcards
                    .OrderBy(d => d.Number)
                    .ThenBy(d => d.Suit)
                    .ToArray();
                playerResource.SetHandCard(handcards);

                if (_lastPlayTurnId == -1 &&
                    handcards.Where(d => d.Suit == club3.Suit && d.Number == club3.Number).Any())
                {
                    currentTurn = i;
                    _lastPlayTurnId = (currentTurn == 0) ?
                        _playerResources.Count - 1 :
                        currentTurn - 1;
                }
            }
        }

        private void NextTurn()
        {
            ++currentTurn;
        }

        private void tryAllUntilExcetion(List<PokerCard> selectedCards, Action<PokerCard[]> action, bool includeEmpty = false)
        {
            List<PokerCard> cards = new List<PokerCard>();
            cards.AddRange(selectedCards);

            int endCount = (includeEmpty) ? 0 : 1;
            for (; cards.Count >= endCount; cards.RemoveAt(0))
            {
                try
                {
                    action(cards.ToArray());
                    break;
                }
                catch { }
            }
        }
    }
}