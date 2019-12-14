using AuthWebService.Models;
using Domain.Api.Models.Response;
using System.Threading.Tasks;

namespace AuthWebService.Sevices
{
    public interface IAuthService
    {
        Task RegisterPlayer(UserInfo info);
        Task<UserInfoWithID> LoginPlayer(UserIdentity identity);
        Task<bool> UpdatePlayerInfo(int id, UserInfo info);
    }
}
