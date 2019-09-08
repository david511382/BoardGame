using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthLogic.Domain.Models
{
    public class UserIdentity
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public UserIdentity()
        {

        }
    }
}
