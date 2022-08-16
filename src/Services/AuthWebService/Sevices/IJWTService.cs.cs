using AuthWebService.Models;
using System;
using System.Security.Claims;

namespace AuthWebService.Sevices
{
    public interface IJWTService
    {
        string NewToken(int id, string username, string name, DateTime expireTime);
    }
}
