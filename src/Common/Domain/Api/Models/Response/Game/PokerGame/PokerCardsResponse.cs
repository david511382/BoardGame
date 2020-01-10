using Domain.Api.Models.Base.Game.PokerGame;

namespace Domain.Api.Models.Response.Game.PokerGame
{
    public class PokerCardsResponse : ResponseModel
    {
        public PockerCardModel[] Cards { get; set; }

    }
}
