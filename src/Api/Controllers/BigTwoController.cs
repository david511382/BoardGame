using BLL;
using BLL.Interfaces;
using Domain.Api.Interfaces;
using Domain.Api.Models.Base.Game.PokerGame;
using Domain.Api.Models.Base.Game.PokerGame.BigTwo;
using Domain.Api.Models.Request.Game;
using Domain.Api.Models.Response.Game.PokerGame.BigTwo;
using GameLogic.PokerGame;
using GameWebService.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameWebService.Controllers
{
    [Route("api/boardgame/[controller]")]
    [ApiController]
    public class BigTwoController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IResponseService _responseService;
        private readonly ILobbyUser _lobbyUserBll;
        private readonly ILobbyRoom _lobbyRoomBll;
        private readonly ILogger _logger;

        private readonly string _redisConnectString;

        private const int TRY_LOCK_TIMES = 3;
        private const int WAIT_LOCK_MS = 50;

        public BigTwoController(
            ILobbyUser lobbyUserBll,
            ILobbyRoom lobbyRoomBll,
            IGameService gameService,
            IResponseService responseService,
            ILogger<BigTwoController> logger)
        {
            _gameService = gameService;
            _responseService = responseService;
            _lobbyUserBll = lobbyUserBll;
            _lobbyRoomBll = lobbyRoomBll;
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
        [Authorize]
        public async Task<IActionResult> SelectCards([FromBody] IndexesRequest request)
        {
            return await _responseService.Init<SelectCardResponse>(this, _logger)
                 .ValidateToken((user) => { })
                 .Do<SelectCardResponse>(async (result, user) =>
                 {
                     VaildUserGameResult vaildResult = await vaildUserGame(user.Id);
                     BigTwoLogic.BigTwo game = vaildResult.Game;

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
        [Authorize]
        public async Task<IActionResult> PlayCards([FromBody] IndexesRequest request)
        {
            return await _responseService.Init<PlayCardResponse>(this, _logger)
                .ValidateToken((user) => { })
                .Do<PlayCardResponse>(async (result, user) =>
                {
                    VaildUserGameResult vaildResult = await vaildUserGame(user.Id);
                    BigTwoLogic.BigTwo game = vaildResult.Game;
                    GameStatus roomGameStatus = vaildResult.RoomGameStatus;
                    LobbyUserStatus roomPlayer = vaildResult.LobbyUser;
                    int roomID = roomPlayer.RoomID;

                    if (!game.IsTurn(user.Id))
                    {
                        result.Fail("不是你的回合");
                        return result;
                    }

                    result.IsSuccess = game.PlayCard(user.Id, request.Indexes);
                    if (result.IsSuccess)
                    {
                        roomGameStatus.DataJson = game.ExportData();
                        await _lobbyRoomBll.SaveGameStatus(roomGameStatus);

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
                            if (!await _lobbyRoomBll.GameOver(game
                                .GetResource()
                                .Select((s) => s.PlayerId)))
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

                    return result;

                });
        }

        [HttpGet("GameStatus")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GameStatusResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> GameStatus()
        {
            return await _responseService.Init<GameStatusResponse>(this, _logger)
              .ValidateToken((user) => { })
              .Do<GameStatusResponse>(async (result, user) =>
              {
                  VaildUserGameResult vaildResult = await vaildUserGame(user.Id);
                  BigTwoLogic.BigTwo game = vaildResult.Game;

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

        private struct VaildUserGameResult
        {
            public BigTwoLogic.BigTwo Game;
            public LobbyUserStatus LobbyUser;
            public GameStatus RoomGameStatus;
        }

        private async Task<VaildUserGameResult> vaildUserGame(int userId)
        {
            LobbyUserStatus lobbyUser = await _lobbyUserBll.GetUser(userId);
            if (!lobbyUser.IsInGame)
                throw new Exception("不在遊戲中");

            int roomID = lobbyUser.RoomID;
            GameStatus redisGameStatus = await _lobbyRoomBll.GameStatus(roomID);
            if (!redisGameStatus.IsGame(GameEnum.BigTwo))
                throw new Exception("錯誤遊戲");

            BigTwoLogic.BigTwo game = _gameService.LoadGame(redisGameStatus) as BigTwoLogic.BigTwo;
            return new VaildUserGameResult()
            {
                Game = game,
                LobbyUser = lobbyUser,
                RoomGameStatus = redisGameStatus
            };
        }
    }
}
