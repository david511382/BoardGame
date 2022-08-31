using DAL.Structs;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ILobbyUser
    {
        Task<LobbyUserStatus> GetUser(int userID);
    }
}
