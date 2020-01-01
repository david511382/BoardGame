using Domain.Api.Models.Base.Lobby;

namespace Domain.Api.Models.Response.Lobby
{
    public class RoomListResponse : ResponseModel
    {
        public RoomModel[] Rooms { get; set; }
    }
}
