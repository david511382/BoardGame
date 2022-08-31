using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Structs;
using System.Threading.Tasks;

namespace BLL
{
    public class LobbyUser : ILobbyUser
    {
        private readonly IUserKey _userDal;

        public LobbyUser(
            IUserKey userDal)
        {
            _userDal = userDal;
        }

        public async Task<LobbyUserStatus> GetUser(int userID)
        {
            UserModel redisUser = await _userDal.Get(userID);
            return new LobbyUserStatus(redisUser);
        }
    }

    public class LobbyUserStatus : UserModel
    {
        public LobbyUserStatus()
        {

        }

        public LobbyUserStatus(UserModel data)
            : base(data)
        {
        }

        public int RoomID => (IsInGame) ?
            -GameRoomID.Value :
            GameRoomID.Value;

        public bool IsInGame => !IsInLobby && GameRoomID.Value < 0;
        public bool IsInRoom => !IsInLobby && GameRoomID.Value > 0;
        public bool IsInLobby => GameRoomID == null;
        public bool IsRoomHost => !IsInLobby && LobbyRoom.IsHost(UserInfo.ID, RoomID);
    }
}