using BoardGameAngular.Models.BigTwo.Response;
using BoardGameAngular.Services.Config;
using Domain.Api.Interfaces;
using Domain.Api.Models.Request.Game;
using Domain.Api.Models.Response;
using Domain.Api.Models.Response.Game.PokerGame.BigTwo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoardGameAngular.Controllers
{
    [Route("api/[controller]")]
    public class BigTwoController : Controller
    {
        private readonly ConfigService _urlConfig;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

        public BigTwoController(ConfigService config, IResponseService responseService, ILogger<GameController> logger)
        {
            _urlConfig = config;
            _responseService = responseService;
            _logger = logger;
        }

        [HttpGet("HandCards")]
        [ProducesResponseType(typeof(HandCardsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HandCardsResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> HandCards()
        {
            return await _responseService.Init<HandCardsResponse>(this, _logger)
                .Do<HandCardsResponse>(async (result, user, logger) =>
                {
                    result = await HttpHelper.HttpRequest.New()
                        .AddHeader(new KeyValuePair<string, string>("Authorization", $"Bearer {Request.Cookies["token"]}"))
                        .To(_urlConfig.HandCards)
                        .Get<HandCardsResponse>();

                    return result;
                });
        }

        [HttpPost("SelectCards")]
        [ProducesResponseType(typeof(SelectCardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SelectCardResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SelectCards([FromBody] IndexesRequest request)
        {
            return await _responseService.Init<SelectCardResponse>(this, _logger)
                .Do<SelectCardResponse>(async (result, user, logger) =>
                {
                    result = await HttpHelper.HttpRequest.New()
                        .AddHeader(new KeyValuePair<string, string>("Authorization", $"Bearer {Request.Cookies["token"]}"))
                        .SetJson(request)
                        .To(_urlConfig.SelectCards)
                        .Post<SelectCardResponse>();

                    return result;
                });
        }

        [HttpPost("PlayCards")]
        [ProducesResponseType(typeof(SelectCardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SelectCardResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PlayCards([FromBody] IndexesRequest request)
        {
            return await _responseService.Init<BoolResponseModel>(this, _logger)
                .Do<BoolResponseModel>(async (result, user, logger) =>
                {
                    result = await HttpHelper.HttpRequest.New()
                        .AddHeader(new KeyValuePair<string, string>("Authorization", $"Bearer {Request.Cookies["token"]}"))
                        .SetJson(request)
                        .To(_urlConfig.PlayCards)
                        .Post<BoolResponseModel>();

                    return result;
                });
        }
    }
}
