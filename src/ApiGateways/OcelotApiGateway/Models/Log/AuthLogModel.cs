using Domain.JWTUser;
using Domain.Logger;
using Newtonsoft.Json;

namespace OcelotApiGateway.Models.Log
{
    public class AuthLogModel : StructLoggerEvent
    {
        public bool IsAuth { get; set; }
        public UserClaimModel User
        {
            get { return _user; }
            set
            {
                _user = value;
                if (_user != null)
                    UserJson = JsonConvert.SerializeObject(_user);
                else
                    UserJson = "";
            }
        }
        public string UserJson { get; private set; }

        private UserClaimModel _user;
        public AuthLogModel()
        {
            IsAuth = false;
            User = null;
        }
    }
}
