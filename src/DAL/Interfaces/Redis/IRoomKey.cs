using DAL.Structs;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRoomKey : IRedisKey<RoomModel>
    {
        Task<RoomModel[]> ListRooms();
    }
}
