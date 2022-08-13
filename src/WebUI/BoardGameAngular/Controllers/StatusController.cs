using BoardGameAngular.Models.BigTwo.Response;
using BoardGameAngular.Models.Status;
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
    public class StatusController : Controller
    {
        private readonly ConfigService _urlConfig;
        private readonly IResponseService _responseService;
        private readonly ILogger _logger;

        public StatusController(ConfigService config,
            IResponseService responseService,
            ILogger<StatusController> logger)
        {
            _urlConfig = config;
            _responseService = responseService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStatus()
        {
            return await _responseService.Init<StatusResponse>(this, _logger)
               .Do<StatusResponse>(async (result, user) =>
               {
                   KeyValuePair<string, string> header = new KeyValuePair<string, string>("Authorization", $"Bearer {Request.Cookies["token"]}");
                   Domain.Api.Models.Response.Lobby.StatusResponse userStatusResponse = await Util.Http.HttpRequest.New()
                         .AddHeader(header)
                         .To(_urlConfig.UserStatus)
                         .Get<Domain.Api.Models.Response.Lobby.StatusResponse>();

                   result.LoadUserStatus(userStatusResponse);
                   if (userStatusResponse.IsInGame)
                   {
                       GameStatusResponse gameStatusResponse = await Util.Http.HttpRequest.New()
                           .AddHeader(header)
                           .To(_urlConfig.GameStatus)
                           .Get<GameStatusResponse>();
                       result.LoadGameStatus(gameStatusResponse);
                   }

                   return result;
               });
        }
    }
}