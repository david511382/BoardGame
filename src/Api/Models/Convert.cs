using DAL.Structs;
using System.Linq;

namespace BoardGameWebService
{
    public static class Convert
    {

        public static Domain.Api.Models.Base.Lobby.RoomModel ToApiRoom(this RoomModel room)
        {
            return new Domain.Api.Models.Base.Lobby.RoomModel
            {
                Game = new Domain.Api.Models.Base.Lobby.GameModel
                {
                    Description = room.Game.Description,
                    ID = room.Game.ID,
                    MaxPlayerCount = room.Game.MaxPlayerCount,
                    MinPlayerCount = room.Game.MinPlayerCount,
                    Name = room.Game.Name,
                },
                HostID = room.HostID,
                Players = room.Players.Select((p) => new Domain.Api.Models.Base.User.UserModel
                {
                    ID = p.ID,
                    Name = p.Name,
                    Username = p.Username
                }).ToArray()
            };
        }
    }
}
