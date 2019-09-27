using System;
using System.Security.Claims;

namespace Domain.JWTUser
{
    public static class UserClaim
    {
        public static UserClaimModel Parse(ClaimsPrincipal user)
        {
            UserClaimModel result = new UserClaimModel
            {
                Name = user.FindFirst(ClaimTypes.Surname).Value,
                Username = user.FindFirst(ClaimTypes.Name).Value,
                Id = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value),
                Email = user.FindFirst(ClaimTypes.Email).Value,
                Gender = user.FindFirst(ClaimTypes.Gender).Value,
                Role = user.FindFirst(ClaimTypes.Role).Value,
                ExpireTime = new DateTime(long.Parse(user.FindFirst("exp")?.Value)),
                ValidAudience = user.FindFirst("aud").Value,
                ValidIssuer = user.FindFirst("iss").Value,
            };
            return result;
        }

        public static Claim[] New(int id, string username, string name)
        {
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Surname ,name),
                new Claim(ClaimTypes.Role, "player"),
                new Claim(ClaimTypes.Gender, "man"),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Email, "email"),
            };

            return claims;
        }
    }
}
