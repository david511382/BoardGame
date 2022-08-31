using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAuthService
    {
        Task RegisterPlayer(UserInfo info);
        Task<UserInfoWithID> LoginPlayer(UserIdentity identity);
        Task<bool> UpdatePlayerInfo(int id, UserInfo info);
    }
}
