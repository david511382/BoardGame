using AuthWebService.Models;
using System.Threading.Tasks;

namespace AuthWebService.Sevices
{
    public interface IAuthService
    {
        Task<bool> RegisterPlayer(UserInfo info);
        Task<UserInfoWithID> LoginPlayer(UserIdentity identity);
        Task<bool> UpdatePlayerInfo(int id, UserInfo info);
    }
}
