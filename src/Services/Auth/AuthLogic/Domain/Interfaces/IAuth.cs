using AuthLogic.Domain.Models;
using System.Threading.Tasks;

namespace AuthLogic.Domain.Interfaces
{
    public interface IAuth
    {
        Task<bool> RegisterPlayer(UserInfo info);
        Task<UserInfoWithID> LoginPlayer(UserIdentity identity);
        Task<bool> UpdatePlayerInfo(int id, UserInfo info);
    }
}
