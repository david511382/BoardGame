using Domain.Api.Models.Base.Lobby;

namespace Domain.Api.Models.Response.Lobby
{
    public class StartRoomResponse : BoolResponseModel
    {
        public int HostID { get; set; }
        public int GameID { get; set; }

        public StartRoomResponse()
        {
            HostID = 0;
            GameID = 0;
        }
    }
}
