using Domain.Api.Interfaces;
using Domain.Api.Models.Base.Game.PokerGame;
using Domain.Api.Models.Base.Game.PokerGame.BigTwo;
using Domain.Api.Models.Request.Game;
using Domain.Api.Models.Response.Game.PokerGame.BigTwo;
using Domain.Api.Services;
using GameLogic.PokerGame;
using GameWebService.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedisRepository;
using Services.Game;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BigTwoController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

        private readonly string _redisConnectString;

        private const int TRY_LOCK_TIMES = 3;
        private const int WAIT_LOCK_MS = 50;

        public BigTwoController(ConfigService configService, IGameService gameService, IResponseService responseService, ILogger<BigTwoController> logger)
        {
            _redisConnectString = configService.RedisConnectString;
            _gameService = gameService;
            _responseService = responseService;
            _logger = logger;
        }

        /// <summary>
        /// 自動選牌
        /// </summary>
        /// <param name="request">index越後面權限越高</param>
        /// <returns></returns>
        [Route("SelectCards")]
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SelectCardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SelectCardResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SelectCards([FromBody] IndexesRequest request)
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
                         result.CardIndexes = null;
                         return result;
                     }

                     PokerCard[] pokerCards = game.SelectCardGroup(user.Id, request.Indexes);
                     PokerCard[] handCards = game.GetResource(user.Id).GetHandCards()
                     .OrderBy(d => d.Number)
                     .ThenBy(d => d.Suit)
                     .ToArray();

                     result.CardIndexes = handCards.GetIndexOfCards(pokerCards);

                     return result;

                 });
        }

        /// <summary>
        /// 出牌
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("PlayCards")]
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PlayCardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PlayCardResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PlayCards([FromBody] IndexesRequest request)
        {
            return await _responseService.Init<PlayCardResponse>(this, _logger)
                .ValidateToken((user) => { })
                .Do<PlayCardResponse>(async (result, user) =>
                {
                    using (RedisContext redis = new RedisContext(_redisConnectString))
                    {
                        RedisRepository.Models.UserModel redisUser = await redis.User.Get(user.Id);
                        int gameRoomId = redisUser.GameRoomID.Value;
                        bool isNotInGame = gameRoomId >= 0;
                        if (isNotInGame)
                            throw new Exception("不在遊戲中");

                        int roomID = -gameRoomId;
                        RedisRepository.Models.GameStatusModel redisGameStatus = await redis.GameStatus.Get(roomID);
                        if ((GameEnum)redisGameStatus.Room.Game.ID != GameEnum.BigTwo)
                            throw new Exception("錯誤遊戲");

                        BigTwoLogic.BigTwo game = _gameService.LoadGame(redisGameStatus) as BigTwoLogic.BigTwo;

                        if (!game.IsTurn(user.Id))
                        {
                            result.Fail("不是你的回合");
                            return result;
                        }

                        result.IsSuccess = game.PlayCard(user.Id, request.Indexes);
                        if (result.IsSuccess)
                        {
                            redisGameStatus.DataJson = game.ExportData();
                            await redis.GameStatus.Set(redisGameStatus);

                            result.Cards = game.GetTable().GetLastItem().GetCards()
                                .Select((c) => new PockerCardModel
                                {
                                    Suit = (int)c.Suit,
                                    Number = c.Number

                                }).ToArray();

                            GameLogic.Game.GameStatus gameState = game.GetCondition();
                            bool isGameOver = gameState.WinPlayerIds != null && gameState.WinPlayerIds.Length > 0;
                            if (isGameOver)
                            {
                                if (!await gameOver(
                                    redis,
                                    game.GetResource().Select((s) => s.PlayerId),
                                    roomID))
                                {
                                    result.Fail("遊戲結束失敗");
                                    return result;
                                }
                                result.Condition = new ConditionModel(gameState.TurnId, gameState.WinPlayerIds.First());
                            }
                            else
                            {
                                result.Condition = new ConditionModel(gameState.TurnId, 0);
                            }
                        }
                        else
                        {
                            result.Message = "不能Pass";
                        }
                    }

                    return result;

                });
        }

        [HttpGet("GameStatus")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GameStatusResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GameStatus()
        {
            return await _responseService.Init<GameStatusResponse>(this, _logger)
              .ValidateToken((user) => { })
              .Do<GameStatusResponse>(async (result, user) =>
              {
                  BigTwoLogic.BigTwo game;
                  using (RedisContext redis = new RedisContext(_redisConnectString))
                  {
                      game = await vaildUserGame(redis, user.Id);
                  }

                  result.TableCards = game.GetTable().Items
                      .Select((i) =>
                          i.GetCards()
                              .Select((c) => new PockerCardModel
                              {
                                  Suit = (int)c.Suit,
                                  Number = c.Number
                              }).ToArray()
                      ).ToArray();
                  result.PlayerCards = game.GetResource()
                      .Select((r) =>
                          new GameStatusResponse.PlayerData(
                              r.PlayerId,
                              r._handCards.Select((c) => new PockerCardModel
                              {
                                  Suit = (int)c.Suit,
                                  Number = c.Number
                              }).ToArray()
                          )).ToArray();
                  GameLogic.Game.GameStatus gameState = game.GetCondition();
                  result.Condition = new ConditionModel(
                      gameState.TurnId,
                      (gameState.WinPlayerIds == null || gameState.WinPlayerIds.Length == 0) ? 0 : gameState.WinPlayerIds.First());
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

        private async Task<bool> gameOver(RedisContext redis, IEnumerable<int> playerIds, int roomId)
        {
            try
            {
                foreach (int id in playerIds)
                    if (!await HandlerService.Retry(TRY_LOCK_TIMES, () => redis.User.Lock(id), WAIT_LOCK_MS))
                        return false;

                ITransaction tran = redis.Begin();

                Task<RedisRepository.Models.UserModel>[] getUsers = playerIds.Select((id) => redis.User.Get(id))
                      .Select(async (t) =>
                      {
                          RedisRepository.Models.UserModel u = await t;
                          u.GameRoomID = null;
                          return u;
                      }).ToArray();
                foreach (Task<RedisRepository.Models.UserModel> getUser in getUsers)
                {
                    RedisRepository.Models.UserModel u = await getUser;
                    _ = redis.User.Set(u, tran);
                }

                if (!await tran.ExecuteAsync())
                    return false;

                //刪除gamestatus
                await redis.GameStatus.Delete(roomId);
                return true;
            }
            finally
            {
                foreach (int id in playerIds)
                    await redis.User.Release(id);
            }
        }
    }
}
