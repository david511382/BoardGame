using System;

namespace Services.Auth
{
    public interface IJWTService
    {
        string NewToken(int id, string username, string name, DateTime expireTime);
    }
}
