using GameRespository.Models;
using System.Threading.Tasks;

namespace LobbyWebService.Sevices
{
    public interface IGameService
    {
        Task<GameInfo[]> List();
    }
}
