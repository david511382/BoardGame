using Domain.Api.Models.Base.Game.PokerGame;
using System.Threading.Tasks;

namespace BoardGameAngular.Models.SignalR
{
    public interface IBigTwoHub
    {
        Task GameBoardUpdate(PockerCardModel[] data);
    }
}
