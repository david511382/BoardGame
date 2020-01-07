using GameLogic.Domain;

namespace GameLogic.Game
{
    public class GameStatus
    {
        public GameState State { get; set; }
        public int[] WinPlayerIds { get; set; }
        public int CurrentPlayerId { get; set; }

        public GameStatus(GameState state)
        {
            State = state;
            WinPlayerIds = null;
            CurrentPlayerId = -1;
        }
    }
}