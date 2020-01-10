using Domain.Api.Interfaces;
using Domain.Api.Models.Base.Game.PokerGame;
using Domain.Api.Models.Request.Game;
using Domain.Api.Models.Response;
using Domain.Api.Models.Response.Game.PokerGame;
using Domain.Api.Models.Response.Game.PokerGame.BigTwo;
using GameLogic.PokerGame;
using GameWebService.Domain;
using GameWebService.Models.BoardGame;
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

        private readonly string _redisConnectString;

        public BigTwoController(ConfigService configService, IGameService gameService, IResponseService responseService, ILogger<BigTwoController> logger)
        {
            _redisConnectString = configService.RedisConnectString;
            _gameService = gameService;
            _responseService = responseService;
            _logger = logger;
        }

        /// <summary>
        /// 取得手牌
        /// </summary>
        /// <returns></returns>
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
                      BigTwoLogic.BigTwo game;
                      using (RedisContext redis = new RedisContext(_redisConnectString))
                      {
                          game = await vaildUserGame(redis, user.Id);
                      }

                      PokerCard[] cards = game.GetResource(user.Id).GetHandCards();
                      result.Cards = cards.Select((c) => new PockerCardModel
                      {
                          Suit = (int)c.Suit,
                          Number = c.Number
                      })
                      .OrderBy(d => d.Number)
                      .ThenBy(d => d.Suit)
                      .ToArray();

                      return result;
                  });
        }

        /// <summary>
        /// 自動選牌
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("SelectCard")]
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SelectCardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SelectCardResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SelectCard([FromBody] IndexesRequest request)
        {
            return await _responseService.Init<SelectCardResponse>(this, _logger)
                 .ValidateToken((user) => { })
                 .Do<SelectCardResponse>(async (result, user) =>
                 {
                     BigTwoLogic.BigTwo game;
                     using (RedisContext redis = new RedisContext(_redisConnectString))
                     {
                         game = await vaildUserGame(redis, user.Id);
                     }

                     if (!game.IsTurn(user.Id))
                     {
                         result.CardIndexs = null;
                         return result;
                     }

                     PokerCard[] pokerCards = game.SelectCardGroup(user.Id, request.Indexes);
                     PokerCard[] handCards = game.GetResource(user.Id).GetHandCards().ToArray();

                     result.CardIndexs = handCards.GetIndexOfCards(pokerCards);

                     return result;

                 });
        }

        /// <summary>
        /// 出牌
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("PlayCard")]
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BoolResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PlayCard([FromBody] IndexesRequest request)
        {
            return await _responseService.Init<BoolResponseModel>(this, _logger)
               .ValidateToken((user) => { })
               .Do<BoolResponseModel>(async (result, user) =>
               {
                   BigTwoLogic.BigTwo game;
                   RedisRepository.Models.GameStatusModel redisGameStatus;
                   using (RedisContext redis = new RedisContext(_redisConnectString))
                   {
                       RedisRepository.Models.UserModel redisUser = await redis.User.Get(user.Id);
                       bool isNotInGame = redisUser.GameRoomID.Value >= 0;
                       if (isNotInGame)
                           throw new Exception("不在遊戲中");

                       int roomID = -redisUser.GameRoomID.Value;
                      redisGameStatus = await redis.GameStatus.Get(roomID);
                       if ((GameEnum)redisGameStatus.Room.Game.ID != GameEnum.BigTwo)
                           throw new Exception("錯誤遊戲");

                       game = _gameService.LoadGame(redisGameStatus) as BigTwoLogic.BigTwo;
                   }

                   if (!game.IsTurn(user.Id))
                   {
                       result.Fail("不是你的回合");
                       return result;
                   }

                   result.IsSuccess = game.PlayCard(user.Id, request.Indexes);
                   if (result.IsSuccess)
                   {
                       redisGameStatus.DataJson = game.ExportData();
                       using (RedisContext redis = new RedisContext(_redisConnectString))
                       {
                           await redis.GameStatus.Set(redisGameStatus);
                       }
                   }

                   return result;

               });
        }

        private async Task<BigTwoLogic.BigTwo> vaildUserGame(RedisContext redis, int userId)
        {
            RedisRepository.Models.UserModel redisUser = await redis.User.Get(userId);
            bool isNotInGame = redisUser.GameRoomID.Value >= 0;
            if (isNotInGame)
                throw new Exception("不在遊戲中");

            int roomID = -redisUser.GameRoomID.Value;
            RedisRepository.Models.GameStatusModel redisGameStatus = await redis.GameStatus.Get(roomID);
            if ((GameEnum)redisGameStatus.Room.Game.ID != GameEnum.BigTwo)
                throw new Exception("錯誤遊戲");

            return _gameService.LoadGame(redisGameStatus) as BigTwoLogic.BigTwo;
        }
    }
}
