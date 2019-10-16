using Domain.JWTUser;
using Domain.Logger;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace AuthWebService.Models.Log
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
        private UserClaimModel _user;
        public string UserJson { get; private set; }

        public SecurityToken SecurityToken
        {
            get { return _securityToken; }
            set
            {
                _securityToken = value;
                if (_securityToken != null)
                    SecurityTokenJson = JsonConvert.SerializeObject(_securityToken);
                else
                    SecurityTokenJson = "";
            }
        }
        public string SecurityTokenJson { get; private set; }
        private SecurityToken _securityToken;

        public AuthLogModel()
        {
            IsAuth = false;
            User = null;
            SecurityToken = null;
        }
    }
}
