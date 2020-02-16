using Domain.Api.Models.Base.Game.PokerGame;
using Domain.Api.Models.Base.Game.PokerGame.BigTwo;

namespace BoardGameAngular.Models.SignalR
{
    public struct GameBoardModel
    {
        public PockerCardModel[] Cards { get; set; }
        public ConditionModel Condition { get; set; }

        public GameBoardModel(PockerCardModel[] cards, ConditionModel condition)
        {
            Cards = cards;
            Condition = condition;
        }
    }
}
