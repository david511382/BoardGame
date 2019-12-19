using Domain.Api.Models.Base.Lobby;

namespace Domain.Api.Models.Response.Lobby
{
    public class GameListResponse : ResponseModel
    {
        public Game[] Games { get; set; }
    }
}
