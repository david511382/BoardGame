using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameAngular.Models.Auth
{
    public class UserInfo : UserIdentity
    {
        public string Name { get; set; }
    }
}
