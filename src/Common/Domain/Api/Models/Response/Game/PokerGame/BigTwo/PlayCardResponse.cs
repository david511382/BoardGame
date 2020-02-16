using Domain.Api.Models.Base.Game.PokerGame;
using Domain.Api.Models.Base.Game.PokerGame.BigTwo;

namespace Domain.Api.Models.Response.Game.PokerGame.BigTwo
{
    public class PlayCardResponse : BoolResponseModel
    {
        public PockerCardModel[] Cards { get; set; }
        public ConditionModel Condition { get; set; }
    }
}
