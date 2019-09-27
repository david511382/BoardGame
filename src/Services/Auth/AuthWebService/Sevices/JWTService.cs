using AuthWebService.Models;
using CommonUtil.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthWebService.Sevices
{
    class JWTService : IJWTService
    {
        private readonly SymmetricSecurityKey _securityKey;
        private JWTConfigModel _config { get; }

        public JWTService(IOptions<JWTConfigModel> options)
        {
            _config = options.Value;

            _securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config.IssuerSigningKey));
        }

        public string NewToken(int id, string username, string name, DateTime expireTime)
        {
            Claim[] claims = UserClaim.New(id, username, name);

            JwtSecurityToken token = new JwtSecurityToken
            (
                issuer: _config.ValidIssuer,
                audience: _config.ValidAudience,
                claims: claims,
                expires: expireTime,
                signingCredentials: signCredentiial()
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        private SigningCredentials signCredentiial()
        {
            return new SigningCredentials(
                _securityKey,
                SecurityAlgorithms.HmacSha256);
        }
    }
}
