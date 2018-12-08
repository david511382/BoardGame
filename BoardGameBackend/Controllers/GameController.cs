using BoardGame.Backend.Models.Game.BoardGame.PokerGame;
using BoardGameBackend.Models.BoardGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BoardGame.Backend.Controllers
{
    [RoutePrefix("api/Game")]
    //[EnableCors(origins: "http://mywebclient.azurewebsites.net", headers: "*", methods: "*")]
    [EnableCors(origins: "*", headers: "*", methods: "get,post")]
    public class GameController : ApiController
    {
        // GET api/Game/HandCards
        [Route("HandCards")]
        public PokerCard[] GetCards()
        {
            return new GameModels().GetCards();
        }

        // GET api/Game/SelectCard/i
        [Route("SelectCard")]
        [HttpPost]
        public PokerCard[] SelectCard(int i)
        {
                return new GameModels().SelectCard(i);
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
