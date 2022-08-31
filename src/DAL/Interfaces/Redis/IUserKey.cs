using DAL.Structs;

namespace DAL.Interfaces
{
    public interface IUserKey : IRedisKey<UserModel>
    {
    }
}
