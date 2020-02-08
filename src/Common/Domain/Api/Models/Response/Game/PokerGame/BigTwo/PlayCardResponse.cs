using Domain.Api.Models.Base.Game.PokerGame;

namespace Domain.Api.Models.Response.Game.PokerGame.BigTwo
{
    public class PlayCardResponse : BoolResponseModel
    {
        public PockerCardModel[] Cards { get; set; }

    }
}
