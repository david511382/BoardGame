namespace Domain.Api.Models.Base.Game.PokerGame.BigTwo
{
    public struct ConditionModel
    {
        public int TurnId { get; set; }

        public ConditionModel(int turnId)
        {
            TurnId = turnId;
        }
    }
}
