using Domain.Api.Models.Base.Lobby;

namespace Domain.Api.Models.Response.Lobby
{
    public class RoomResponse : BoolResponseModel
    {
        public RoomModel Room { get; set; }

        public RoomResponse()
        {
            IsSuccess = true;
            Room = null;
        }
    }
}
