using BoardGame.Backend.Models.GameLobby;
using BoardGame.Data.ApiParameters;
using GameLogic.Player;
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

        [Route("GetPlayerOrRegister")]
        [HttpPost]
        public PlayerInfoModels GetPlayerOrRegister(FormDataCollection form)
        {
            string playerIdStr = form.Get(PLAYER_INFO);
            PlayerInfoModels user = JsonConvert.DeserializeObject<PlayerInfoModels>(playerIdStr);

            PlayerInfo player = new GameLobbyModels().GetPlayer(new PlayerInfo(user));
            if (player == null)
                return Register(form);

            return player.Models;
        }

        [Route("CreateGame")]
        [HttpPost]
        public PlayerInfoModels CreateGame(FormDataCollection form)
        {
            try
            {
                string playerIdStr = form.Get(PLAYER_INFO);
                PlayerInfoModels user = JsonConvert.DeserializeObject<PlayerInfoModels>(playerIdStr);

                return new GameLobbyModels().CreateGame(new PlayerInfo(user)).Models;
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
            return new GameLobbyModels().GetGameRooms();
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

                return new GameLobbyModels().JoinGameRoom(new PlayerInfo(user), gameId).Models;
            }
            catch
            {
                return new PlayerInfoModels();
            }
        }

        [Route("LeaveGameRoom")]
        [HttpPost]
        public PlayerInfoModels[] LeaveGameRoom(FormDataCollection form)
        {
            try
            {
                string playerIdStr = form.Get(PLAYER_INFO);
                PlayerInfoModels user = JsonConvert.DeserializeObject<PlayerInfoModels>(playerIdStr);

                return new GameLobbyModels().LeaveGameRoom(new PlayerInfo(user))
                    .Select(d=>d.Models)
                    .ToArray();
            }
            catch
            {
                return new PlayerInfoModels[0];
            }
        }

        [Route("StartGame")]
        [HttpPost]
        public bool StartGame(FormDataCollection form)
        {
            try
            {
                string playerIdStr = form.Get(PLAYER_INFO);
                PlayerInfoModels user = JsonConvert.DeserializeObject<PlayerInfoModels>(playerIdStr);

                return new GameLobbyModels().StartGame(new PlayerInfo(user));
            }
            catch
            {
                return false;
            }
        }
    }
}
