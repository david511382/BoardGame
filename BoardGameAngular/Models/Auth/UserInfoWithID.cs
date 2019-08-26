using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameAngular.Models.Auth
{
    public class UserInfoWithID : UserInfo
    {
        public int Id { get; set; }

        public UserInfoWithID()
        {
        }
    }
}
