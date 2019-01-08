using BoardGame.Backend.Models.BoardGame;
using BoardGame.Backend.Models.BoardGame.GameFramework;
using BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer;
using BoardGame.Backend.Models.BoardGame.PokerGame;
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
    [RoutePrefix("api/Game")]
    //[EnableCors(origins: "http://mywebclient.azurewebsites.net", headers: "*", methods: "*")]
    [EnableCors(origins: "*", headers: "*", methods: "get,post")]
    public class GameController : ApiController
    {
        private const string GAME_ID= "GameId";
        private const string PLAYER_INFO = "PlayerInfo";
        private const string CARD_INDEXES = "Indexes";

        [Route("HandCards")]
        [HttpPost]
        public PokerCard[] GetCards(FormDataCollection form)
        {
            string playerIdStr = form.Get(PLAYER_INFO);
            PlayerInfo user = JsonConvert.DeserializeObject<PlayerInfo>(playerIdStr);
            return new GameModels().GetCards(user.Id);
        }

        // GET api/Game/SelectCard/i
        [Route("SelectCard")]
        [HttpPost]
        public PokerCard[] SelectCard(FormDataCollection form)
        {
            try
            {
                string playerIdStr = form.Get(PLAYER_INFO);
                PlayerInfo user = JsonConvert.DeserializeObject<PlayerInfo>(playerIdStr);

                string indexesStr = form.Get(CARD_INDEXES);
                int[] selectedIndex = JsonConvert.DeserializeObject<int[]>(indexesStr);
                
                return new GameModels().SelectCard(user.Id, selectedIndex);
            }
            catch { return null; }
        }

        [Route("PlayCard")]
        [HttpPost]
        public bool PlayCard(FormDataCollection form)
        {
            try
            {
                string playerIdStr = form.Get(PLAYER_INFO);
                PlayerInfo user = JsonConvert.DeserializeObject<PlayerInfo>(playerIdStr);

                string indexesStr = form.Get(CARD_INDEXES);
                int[] selectedIndex = JsonConvert.DeserializeObject<int[]>(indexesStr);

                return new GameModels().PlayCard(user.Id, selectedIndex);
            }
            catch { return false; }
        }

        [Route("GetGameStatus")]
        [HttpPost]
        public GameStatus GetGameStatus(FormDataCollection form)
        {
            try
            {
                string gameIdStr = form.Get(GAME_ID);
                int gameId= JsonConvert.DeserializeObject<int>(gameIdStr);

                return new GameModels().GetGameStatus(gameId);
            }
            catch { return new GameStatus();}
        }

        [Route("GetTable")]
        [HttpPost]
        public PokerCard[] GetTable(FormDataCollection form)
        {
            try
            {
                string playerIdStr = form.Get(PLAYER_INFO);
                PlayerInfo user = JsonConvert.DeserializeObject<PlayerInfo>(playerIdStr);

                return new GameModels().GetTable(user.Id);
            }
            catch { return null; }
        }
    }
}
