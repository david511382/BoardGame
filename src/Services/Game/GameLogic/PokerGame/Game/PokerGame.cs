using GameLogic.PokerGame.CardGroup;

namespace GameLogic.PokerGame.Game
{
    public abstract class PokerGame<TCondition> : GameLogic.Game.BoardGame<PokerCardGroup, TCondition>
    {
        protected Poker _poker;

        public PokerGame(int MIN_PLAYERS, int MAX_PLAYERS)
          : base(MAX_PLAYERS, MIN_PLAYERS)
        {
            _poker = new Poker();
        }

        protected override void AddPlayer(int playerId)
        {
            base._playerResources.Add(new PokerResource(playerId));
        }

        public new PokerResource GetResourceAt(int i)
        {
            return (PokerResource)_playerResources[i];
        }

        protected override void InitGame()
        {
            _poker.Shuffle();

            PokerCard[][] dealResult = _poker.DealTo(base.playerNum);
            for (int i = 0; i < base.playerNum; i++)
                GetResourceAt(i).SetHandCard(dealResult[i]);
        }
    }
}