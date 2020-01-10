using Domain.Api.Interfaces;
using Domain.Api.Models.Base.Game.PokerGame;
using Domain.Api.Models.Response.Game.PokerGame;
using GameLogic.PokerGame;
using GameWebService.Domain;
using GameWebService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedisRepository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BigTwoController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

        private RedisContext _redis { get { return new RedisContext(_redisConnectString); } }

        private readonly string _redisConnectString;

        public BigTwoController(ConfigService configService, IGameService gameService, IResponseService responseService, ILogger<BigTwoController> logger)
        {
            _redisConnectString = configService.RedisConnectString;
            _gameService = gameService;
            _responseService = responseService;
            _logger = logger;
        }

        [Route("HandCards")]
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PokerCardsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PokerCardsResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCards()
        {
            return await _responseService.Init<PokerCardsResponse>(this, _logger)
                .ValidateToken((user) => { })
                .Do<PokerCardsResponse>(async (result, user) =>
                  {
                      PokerCard[] cards;
                      using (_redis)
                      {
                          RedisRepository.Models.UserModel redisUser = await _redis.User.Get(user.Id);
                          bool isNotInGame = redisUser.GameRoomID.Value >= 0;
                          if (isNotInGame)
                              throw new Exception("不在遊戲中");

                          int roomID = -redisUser.GameRoomID.Value;
                          RedisRepository.Models.GameStatusModel redisGameStatus = await _redis.GameStatus.Get(roomID);
                          if ((GameEnum)redisGameStatus.Room.Game.ID != GameEnum.BigTwo)
                              throw new Exception("錯誤遊戲");

                          BigTwoLogic.BigTwo game = _gameService.LoadGame(redisGameStatus) as BigTwoLogic.BigTwo;

                          cards = ((PokerResource)game.GetResource(user.Id)).GetHandCards();
                      }

                      result.Cards = cards.Select((c) => new PockerCardModel
                      {
                          Suit = (int)c.Suit,
                          Number = c.Number
                      }).ToArray();

                      return result;
                  });
        }
    }
}
