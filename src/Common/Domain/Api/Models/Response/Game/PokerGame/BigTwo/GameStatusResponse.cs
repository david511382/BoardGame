using Domain.Api.Models.Base.Game.PokerGame;

namespace Domain.Api.Models.Response.Game.PokerGame.BigTwo
{
    public class GameStatusResponse : ResponseModel
    {
        public struct PlayerData
        {
            public int Id { get; set; }
            public PockerCardModel[] HandCards { get; set; }

            public PlayerData(int id, PockerCardModel[] handCards)
            {
                this.Id = id;
                this.HandCards = handCards;
            }
        }

        public PockerCardModel[][] TableCards { get; set; }
        public PlayerData[] PlayerCards { get; set; }

    }
}
