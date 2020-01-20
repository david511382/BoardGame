using BoardGameAngular.Models.BigTwo.Response;
using BoardGameAngular.Services.Config;
using Domain.Api.Interfaces;
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
    }
}
