using Domain.Api.Models.Response;

namespace BoardGameAngular.Models.BigTwo.Response
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
        public struct ConditionData
        {
            public int TurnId { get; set; }

            public ConditionData(int turnId)
            {
                TurnId = turnId;
            }
        }

        public PockerCardModel[][] TableCards { get; set; }
        public PlayerData[] PlayerCards { get; set; }
        public ConditionData Condition { get; set; }
    }
}
