using System;

namespace Domain.JWTUser
{
    public class UserClaimModel
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Gender { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
