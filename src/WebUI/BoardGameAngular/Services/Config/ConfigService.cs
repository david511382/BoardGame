using BoardGameAngular.Models.Config;
using Microsoft.Extensions.Options;

namespace BoardGameAngular.Services.Config
{
    public class ConfigService
    {
        public string UserLogin => _userBase + "/Login";
        public string UserRegister => _userBase + "/Register";
        public string UserUpdate => _userBase + "/Update";
        public string UserInfo => _userBase;
        private string _userBase => _baseUrl + "/User";

        public string UserStatus => _roomBase + "/User";
        public string RoomStart => _roomBase + "/Start";
        public string LeaveRoom => _roomBase;
        public string JoinRoom => _roomBase;
        public string CreateRoom => _roomBase;
        public string ListRoom => _roomBase;
        private string _roomBase => _baseUrl + "/Room";

        public string ListGame => _gameBase;
        private string _gameBase => _baseUrl + "/Game";

        private readonly string _baseUrl;



        public ConfigService(IOptions<ConnectionsStrings> connectionsStrings)
        {
            _baseUrl = connectionsStrings.Value.BackendDomain + "/api";
        }
    }
}
