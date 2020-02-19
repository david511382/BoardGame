namespace Domain.Api.Models.Base.Game.PokerGame.BigTwo
{
    public struct ConditionModel
    {
        public int TurnId { get; set; }
        public int WinPlayerId { get; set; }

        public ConditionModel(int turnId)
        {
            TurnId = turnId;
            WinPlayerId = 0;
        }

        public ConditionModel(int turnId, int winPlayerId)
        {
            TurnId = turnId;
            WinPlayerId = winPlayerId;
        }
    }
}
