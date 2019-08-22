using BoardGameWebService.Models.BoardGame;
using BoardGame.Data.ApiParameters;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BoardGameWebService.Models;
using System.Net.Http.Formatting;
using System.Web.Http;
using GameLogic.PokerGame;

namespace BoardGameWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors(origins: "http://mywebclient.azurewebsites.net", headers: "*", methods: "*")]
    [EnableCors()]
    public class GameController : ControllerBase
    {
        [Route("HandCards")]
        [HttpPost]
        public PokerCard[] GetCards([FromForm] string parameter)
        {
            ApiParameter apiParameter;
            PlayerInfoModel user;
            try
            {
                apiParameter = ApiParameter.Create(parameter);
                user = apiParameter.GetParameter<PlayerInfoModel>(ApiParameterEnum.Player_Info);
            }
            catch
            {
                return null;
            }

            return new GameModels().GetCards(user.Id);
        }

        // GET api/Game/SelectCard/i
        [Route("SelectCard")]
        [HttpPost]
        public int[] SelectCard([FromForm] string parameter)
        {
            ApiParameter apiParameter;
            PlayerInfoModel user;
            int[] selectedIndex;
            try
            {
                apiParameter = ApiParameter.Create(parameter);
                user = apiParameter.GetParameter<PlayerInfoModel>(ApiParameterEnum.Player_Info);
                selectedIndex = apiParameter.GetParameter<int[]>(ApiParameterEnum.Hand_Card_Indexes);
            }
            catch
            {
                return null;
            }

            try
            {
                return new GameModels().SelectCard(user.Id, selectedIndex);
            }
            catch { return null; }
        }

        [Route("PlayCard")]
        [HttpPost]
        public bool PlayCard([FromForm] string parameter)
        {
            ApiParameter apiParameter;
            PlayerInfoModel user;
            int[] selectedIndex;
            try
            {
                apiParameter = ApiParameter.Create(parameter);
                user = apiParameter.GetParameter<PlayerInfoModel>(ApiParameterEnum.Player_Info);
                selectedIndex = apiParameter.GetParameter<int[]>(ApiParameterEnum.Hand_Card_Indexes);
            }
            catch
            {
                return false;
            }

            try
            {
                return new GameModels().PlayCard(user.Id, selectedIndex);
            }
            catch { return false; }
        }

        [Route("GetGameStatus")]
        [HttpPost]
        public GameStatusModel GetGameStatus([FromForm] string parameter)
        {
            ApiParameter apiParameter;
            int gameId;
            try
            {
                apiParameter = ApiParameter.Create(parameter);
                gameId = apiParameter.GetParameter<int>(ApiParameterEnum.Game_Id);
            }
            catch
            {
                return null;
            }

            return new GameModels().GetGameStatus(gameId);
        }

        [Route("GetTable")]
        [HttpPost]
        public PokerCard[] GetTable([FromForm] string parameter)
        {
            ApiParameter apiParameter;
            PlayerInfoModel user;
            try
            {
                apiParameter = ApiParameter.Create(parameter);
                user = apiParameter.GetParameter<PlayerInfoModel>(ApiParameterEnum.Player_Info);
            }
            catch
            {
                return null;
            }

            try
            {
                return new GameModels().GetTable(user.Id);
            }
            catch { return null; }
        }
    }
}
