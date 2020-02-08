namespace BoardGameAngular.Services.Config
{
    public class ConfigService
    {
        public string UserLogin => _userBase + "/Login";
        public string UserRegister => _userBase + "/Register";
        public string UserUpdate => _userBase + "/Update";
        private string _userBase => _baseUrl + "/User";

        public string UserStatus => _roomBase + "/User";
        public string RoomStart => _roomBase + "/Start";
        public string LeaveRoom => _roomBase;
        public string JoinRoom => _roomBase;
        public string CreateRoom => _roomBase;
        public string ListRoom => _roomBase;
        private string _roomBase => _baseUrl + "/Room";

        public string ListGame => _gameBase;
        public string StartGame => _gameBase + "/StartGame";
        private string _gameBase => _baseUrl + "/Game";

        public string GameStatus => _bigTwoBase + "/GameStatus";
        public string PlayCards => _bigTwoBase + "/PlayCards";
        public string SelectCards => _bigTwoBase + "/SelectCards";
        public string HandCards => _bigTwoBase + "/HandCards";
        private string _bigTwoBase => _boardgameBase + "/BigTwo";

        private string _boardgameBase => _baseUrl + "/boardgame";

        private readonly string _baseUrl;

        public ConfigService(string backendDomain)
        {
            _baseUrl = backendDomain + "/api";
        }
    }
}
