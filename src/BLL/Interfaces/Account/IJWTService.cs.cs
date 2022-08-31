using System;

namespace BLL.Interfaces
{
    public interface IJWTService
    {
        string NewToken(int id, string username, string name, DateTime expireTime);
    }
}
