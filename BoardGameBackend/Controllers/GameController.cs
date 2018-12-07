using BoardGame.Backend.Models.Game.BoardGame;
using BoardGame.Backend.Models.Game.BoardGame.BigTwo;
using BoardGame.Backend.Models.Game.BoardGame.PokerGame;
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
        private BigTwoPlayer _thisPlayer;

        public GameController() {
            BigTwo bigTwo = new BigTwo();
            _thisPlayer = new BigTwoPlayer();
            _thisPlayer.JoinGame(bigTwo);

            GamePlayer<PokerResource> bigTwoP2, bigTwoP3, bigTwoP4;
            bigTwoP2 = new BigTwoPlayer();
            bigTwoP3 = new BigTwoPlayer();
            bigTwoP4 = new BigTwoPlayer();

            bigTwoP2.JoinGame(bigTwo);
            bigTwoP3.JoinGame(bigTwo);
            bigTwoP4.JoinGame(bigTwo);

            bigTwo.StartGame();
        }

        // GET api/Game/HandCards
        [Route("HandCards")]
        public PokerCard[] GetCards()
        {
            return _thisPlayer.GetHandCards();
        }

        // GET api/Game/SelectCard/i
        [Route("SelectCard")]
        [HttpPost]
        public PokerCard[] SelectCard(int i)
        {
            string result = string.Empty;
            DateTime start = DateTime.Now;
            if (_thisPlayer.IsOnTurn())
            {
                string suit = string.Empty;
                PokerCard[] pokerCards = _thisPlayer.GetCardGroup(new int[] { i });
                pokerCards = pokerCards.OrderBy(d => d.Number).ThenBy(d => d.Suit).ToArray();

                return pokerCards;
            }
            else
            {
                result += GetCardInfo(_thisPlayer.GetHandCards()[i]);
                result += "\r\n" + "not your turn";
            }

            result += "\r\n" + (DateTime.Now - start).Milliseconds.ToString();
            throw new Exception(result);
        }

        private string GetCardInfo(PokerCard pokerCard)
        {
            string suit = string.Empty;
            string result = string.Empty;
            result += pokerCard.Number.ToString();
            switch (pokerCard.Suit)
            {
                case PokerSuit.Club:
                    suit = "C";
                    break;
                case PokerSuit.Diamond:
                    suit = "D";
                    break;
                case PokerSuit.Heart:
                    suit = "H";
                    break;
                case PokerSuit.Spade:
                    suit = "S";
                    break;
            }
            result += suit;
            result += " ";

            return result;
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
