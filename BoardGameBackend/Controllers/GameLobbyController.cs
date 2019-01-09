using BoardGame.Backend.Models.BoardGame;
using BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer;
using BoardGame.Backend.Models.GameLobby;
using BoardGame.Data.ApiParameters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BoardGame.Backend.Controllers
{
    [RoutePrefix("api/GameLobby")]
    //[EnableCors(origins: "http://mywebclient.azurewebsites.net", headers: "*", methods: "*")]
    [EnableCors(origins: "*", headers: "*", methods: "get,post")]
    public class GameLobbyController : ApiController
    {
        private const string GAME_ID = ApiParameterNames.GAME_ID;
        private const string PLAYER_INFO = ApiParameterNames.PLAYER_INFO;
        private const string CARD_INDEXES = ApiParameterNames.HAND_CARD_INDEXES;

        [Route("RegisterPlayer")]
        [HttpPost]
        public PlayerInfoModels Register(FormDataCollection form)
        {
            return new GameLobbyModels().Register().Models;
        }

        [Route("CreateGame")]
        [HttpPost]
        public PlayerInfoModels CreateGame(FormDataCollection form)
        {
            try
            {
                string playerIdStr = form.Get(PLAYER_INFO);
                PlayerInfoModels user = JsonConvert.DeserializeObject<PlayerInfoModels>(playerIdStr);

                return new GameLobbyModels().CreateGame(new PlayerInfo(user.Name, user.Id)).Models;
            }
            catch
            {
                return new PlayerInfoModels();
            }
        }

        [Route("GetGameRooms")]
        [HttpPost]
        public GameRoomModels[] GetGameRooms()
        {
            GameRoom[] gameRooms = new GameLobbyModels()
                .GetGameRooms();
            if (gameRooms == null || gameRooms.Length == 0)
                return null;

            return gameRooms
                .Select(d => d.Models)
                .ToArray();
        }

        [Route("JoinGameRoom")]
        [HttpPost]
        public PlayerInfoModels JoinGameRoom(FormDataCollection form)
        {
            try
            {
                string playerIdStr = form.Get(PLAYER_INFO);
                PlayerInfoModels user = JsonConvert.DeserializeObject<PlayerInfoModels>(playerIdStr);

                string gameIdStr = form.Get(GAME_ID);
                int gameId = JsonConvert.DeserializeObject<int>(gameIdStr);

                return BoardGameManager.JoinGameRoom(new PlayerInfo(user), gameId).Models;
            }
            catch
            {
                return new PlayerInfoModels();
            }
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
