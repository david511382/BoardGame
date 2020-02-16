using BoardGameAngular.Models.BigTwo.Response;
using Domain.Api.Models.Base.Lobby;

namespace BoardGameAngular.Models.Status
{
    public class StatusResponse : GameStatusResponse
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

        public void LoadUserStatus(Domain.Api.Models.Response.Lobby.StatusResponse data)
        {
            Id = data.Id;
            Username = data.Username;
            IsInGame = data.IsInGame;
            Name = data.Name;
            Room = data.Room;
            IsInRoom = data.IsInRoom;
            IsError = data.IsError;
            ErrorMessage = data.ErrorMessage;
            Message = data.Message;
        }

        public void LoadGameStatus(GameStatusResponse data)
        {
            PlayerCards = data.PlayerCards;
            TableCards = data.TableCards;
            Condition = data.Condition;

            ErrorMessage = data.ErrorMessage;
            IsError = data.IsError;
            Message = data.Message;
        }
    }
}
