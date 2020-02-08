using Domain.Api.Models.Base.Lobby;

namespace Domain.Api.Models.Response.Lobby
{
    public class StatusResponse : ResponseModel
    {
        public RoomModel Room { get; set; }
        public bool IsInRoom { get; set; }
        public bool IsInGame { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }

        public StatusResponse()
        {
            IsInGame = false;
            IsInRoom = false;
            Room = null;
        }
    }
}
