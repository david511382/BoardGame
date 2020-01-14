using System.Threading.Tasks;

namespace BoardGameAngular.Models.SignalR
{
    public interface IConnectionHub
    {
        Task SetConnectionId(string connectionId);
    }
}
