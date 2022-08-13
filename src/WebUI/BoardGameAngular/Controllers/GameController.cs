using BoardGameAngular.Services.Config;
using Domain.Api.Interfaces;
using Domain.Api.Models.Response.Lobby;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BoardGameAngular.Controllers
{
    [Route("api/[controller]")]
    public class GameController : Controller
    {
        private readonly ConfigService _urlConfig;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

        public GameController(ConfigService config, IResponseService responseService, ILogger<GameController> logger)
        {
            _urlConfig = config;
            _responseService = responseService;
            _logger = logger;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(GameListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GameListResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            return await _responseService.Init<GameListResponse>(this, _logger)
                  .Do<GameListResponse>(async (result, user, logger) =>
                  {
                      result = await Util.Http.HttpRequest.New()
                         .To(_urlConfig.ListGame)
                         .Get<GameListResponse>();

                      return result;
                  });
        }
    }
}
