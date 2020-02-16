using Domain.Api.Models.Base.Game.PokerGame;
using Domain.Api.Models.Base.Game.PokerGame.BigTwo;

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
                Id = id;
                HandCards = handCards;
            }
        }

        public PockerCardModel[][] TableCards { get; set; }
        public PlayerData[] PlayerCards { get; set; }
        public ConditionModel Condition { get; set; }
    }
}
