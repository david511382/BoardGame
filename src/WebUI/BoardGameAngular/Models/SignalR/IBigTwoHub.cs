using System.Threading.Tasks;

namespace BoardGameAngular.Models.SignalR
{
    public interface IBigTwoHub
    {
        Task GameBoardUpdate(GameBoardModel data);
    }
}
