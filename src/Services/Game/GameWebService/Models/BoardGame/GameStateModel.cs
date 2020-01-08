using GameLogic.Domain;
using GameLogic.Models;
using Newtonsoft.Json;

namespace GameWebService.Models.BoardGame
{
    public class GameStatusModel
    {
        [JsonProperty("State")]
        public GameState State { get; set; }

        [JsonProperty("WinPlayers")]
        public PlayerInfoModel[] WinPlayers { get; set; }

        [JsonProperty("CurrentPlayerID")]
        public int CurrentPlayerId { get; set; }

        public GameStatusModel()
        {
        }

        public GameStatusModel(GameState state, PlayerInfoModel[] winPlayers, int currentPlayerId)
        {
            State = state;
            WinPlayers = winPlayers;
            CurrentPlayerId = currentPlayerId;
        }
    }
}
