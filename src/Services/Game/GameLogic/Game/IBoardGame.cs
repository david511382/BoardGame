namespace GameLogic.Game
{
    public interface IBoardGame
    {
        void StartGame();
        string ExportData();
        void Load(string gameStatus);
        void Join(int playerId);
    }
}