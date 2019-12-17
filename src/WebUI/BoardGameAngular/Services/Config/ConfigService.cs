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

        private readonly string _baseUrl;



        public ConfigService(IOptions<ConnectionsStrings> connectionsStrings)
        {
            _baseUrl = connectionsStrings.Value.BackendDomain + "/api";
        }
    }
}
