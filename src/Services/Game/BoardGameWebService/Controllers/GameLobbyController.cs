using BoardGameWebService.Domain;
using BoardGameWebService.Models.GameLobby;
using GameLogic.Models;
using GameLogic.Player;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BoardGameWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [EnableCors()]
    public class GameLobbyController : ControllerBase
    {
        [Route("RegisterPlayer")]
        [HttpPost]
        public PlayerInfoModel Register()
        {
            return new GameLobbyModels().Register().Models;
        }

        [Route("GetPlayerOrRegister")]
        [HttpPost]
        public PlayerInfoModel GetPlayerOrRegister([FromForm] string parameter)
        {
            ApiParameter apiParameter;
            PlayerInfoModel user;
            try
            {
                apiParameter = ApiParameter.Create(parameter);
                user = apiParameter.GetParameter<PlayerInfoModel>(ApiParameterEnum.Player_Info);
            }
            catch
            {
                return null;
            }

            PlayerInfo player = new GameLobbyModels().GetPlayer(new PlayerInfo(user));
            if (player == null)
                return Register();

            return player.Models;
        }

        [Route("CreateGame")]
        [HttpPost]
        public PlayerInfoModel CreateGame([FromForm] string parameter)
        {
            ApiParameter apiParameter;
            PlayerInfoModel user;
            try
            {
                apiParameter = ApiParameter.Create(parameter);
                user = apiParameter.GetParameter<PlayerInfoModel>(ApiParameterEnum.Player_Info);
            }
            catch
            {
                return null;
            }

            try
            {
                return new GameLobbyModels().CreateGame(new PlayerInfo(user)).Models;
            }
            catch
            {
                return null;
            }
        }

        [Route("GetGameRooms")]
        [HttpPost]
        public GameRoomModel[] GetGameRooms()
        {
            return new GameLobbyModels().GetGameRooms();
        }

        [Route("JoinGameRoom")]
        [HttpPost]
        public PlayerInfoModel JoinGameRoom([FromForm] string parameter)
        {
            ApiParameter apiParameter;
            PlayerInfoModel user;
            int gameId;
            try
            {
                apiParameter = ApiParameter.Create(parameter);
                user = apiParameter.GetParameter<PlayerInfoModel>(ApiParameterEnum.Player_Info);
                gameId = apiParameter.GetParameter<int>(ApiParameterEnum.Game_Id);
            }
            catch
            {
                return null;
            }

            try
            {
                return new GameLobbyModels().JoinGameRoom(new PlayerInfo(user), gameId).Models;
            }
            catch
            {
                return null;
            }
        }

        [Route("LeaveGameRoom")]
        [HttpPost]
        public PlayerInfoModel[] LeaveGameRoom([FromForm] string parameter)
        {
            ApiParameter apiParameter;
            PlayerInfoModel user;
            try
            {
                apiParameter = ApiParameter.Create(parameter);
                user = apiParameter.GetParameter<PlayerInfoModel>(ApiParameterEnum.Player_Info);
            }
            catch
            {
                return null;
            }

            try
            {
                return new GameLobbyModels().LeaveGameRoom(new PlayerInfo(user))
                    .Select(d => d.Models)
                    .ToArray();
            }
            catch
            {
                return null;
            }
        }

        [Route("StartGame")]
        [HttpPost]
        public bool StartGame([FromForm] string parameter)
        {
            ApiParameter apiParameter;
            PlayerInfoModel user;
            try
            {
                apiParameter = ApiParameter.Create(parameter);
                user = apiParameter.GetParameter<PlayerInfoModel>(ApiParameterEnum.Player_Info);
            }
            catch
            {
                return false;
            }

            try
            {
                return new GameLobbyModels().StartGame(new PlayerInfo(user));
            }
            catch
            {
                return false;
            }
        }
    }
}
