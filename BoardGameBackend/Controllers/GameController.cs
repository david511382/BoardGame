using BoardGame.Backend.Models.BoardGame;
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
    [RoutePrefix("api/Game")]
    //[EnableCors(origins: "http://mywebclient.azurewebsites.net", headers: "*", methods: "*")]
    [EnableCors(origins: "*", headers: "*", methods: "get,post")]
    public class GameController : ApiController
    {
        private const string GAME_ID= Data.ApiParameters.ApiParameterNames.GAME_ID;
        private const string PLAYER_INFO = Data.ApiParameters.ApiParameterNames.PLAYER_INFO;
        private const string CARD_INDEXES = Data.ApiParameters.ApiParameterNames.HAND_CARD_INDEXES;

        [Route("HandCards")]
        [HttpPost]
        public PokerCard[] GetCards(FormDataCollection form)
        {
            string playerIdStr = form.Get(PLAYER_INFO);
            PlayerInfoModels user = JsonConvert.DeserializeObject<PlayerInfoModels>(playerIdStr);
            return new GameModels().GetCards(user.Id);
        }

        // GET api/Game/SelectCard/i
        [Route("SelectCard")]
        [HttpPost]
        public int[] SelectCard(FormDataCollection form)
        {
            try
            {
                string playerIdStr = form.Get(PLAYER_INFO);
                PlayerInfoModels user = JsonConvert.DeserializeObject<PlayerInfoModels>(playerIdStr);

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
                PlayerInfoModels user = JsonConvert.DeserializeObject<PlayerInfoModels>(playerIdStr);

                string indexesStr = form.Get(CARD_INDEXES);
                int[] selectedIndex = JsonConvert.DeserializeObject<int[]>(indexesStr);

                return new GameModels().PlayCard(user.Id, selectedIndex);
            }
            catch { return false; }
        }

        [Route("GetGameStatus")]
        [HttpPost]
        public GameStatusModels GetGameStatus(FormDataCollection form)
        {
            int gameId;
            try
            {
                string gameIdStr = form.Get(GAME_ID);
                gameId = JsonConvert.DeserializeObject<int>(gameIdStr);
            }
            catch { return new GameStatusModels(); }

            return new GameModels().GetGameStatus(gameId);
        }

        [Route("GetTable")]
        [HttpPost]
        public PokerCard[] GetTable(FormDataCollection form)
        {
            try
            {
                string playerIdStr = form.Get(PLAYER_INFO);
                PlayerInfoModels user = JsonConvert.DeserializeObject<PlayerInfoModels>(playerIdStr);

                return new GameModels().GetTable(user.Id);
            }
            catch { return null; }
        }
    }
}
