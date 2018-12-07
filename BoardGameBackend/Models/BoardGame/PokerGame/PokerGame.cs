using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.Game.BoardGame.PokerGame
{
    public abstract class PokerGame : BoardGame<PokerResource>
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

        protected override void InitGame()
        {
            _poker.Shuffle();

            PokerCard[][] dealResult = _poker.DealTo(base.playerNum);
            for (int i = 0; i < base.playerNum; i++)
                GetResourceAt(i).SetHandCard(dealResult[i]);
        }
    }
}