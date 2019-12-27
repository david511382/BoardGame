using Domain.Api.Interfaces;
using Domain.Api.Models.Response;
using Domain.JWTUser;
using Domain.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Domain.Api.Services
{
    public class ResponseService : IResponseMaker, IResponseService
    {
        public IActionResult Result { get; private set; }

        private const string TOKEN_HEADER = "Authorization";

        private ResponseModel _response;
        private ControllerBase _controller;
        private UserClaimModel _user;

        private StructLoggerEvent _logEvent;
        private ILogger _logger;

        public IResponseMaker Init<Response>(ControllerBase c, ILogger logger = null) where Response : ResponseModel, new()
        {
            _controller = c;
            Result = null;
            _response = new Response();
            _logEvent = new StructLoggerEvent();
            _logger = logger;

            return this;
        }

        public IResponseMaker ValidateToken(Action<UserClaimModel> validateFun)
        {
            _logEvent.Log("ValidateToken:");

            if (Result == null)
            {
                try
                {
                    string userJson = _controller.Request.Headers[TOKEN_HEADER].ToString();
                    _user = JsonConvert.DeserializeObject<UserClaimModel>(userJson);

                    _logEvent.Log("Parsed User", _user);
                    validateFun(_user);
                }
                catch (Exception e)
                {
                    _logEvent.Log(e.Message);

                    _response.Error(e.Message);
                    Result = _controller.Forbid();
                }
            }

            return this;
        }

        public IResponseMaker ValidateRequest(Action validateFun)
        {
            _logEvent.Log("Start ValidateRequest:");

            if (Result == null)
            {
                try
                {
                    validateFun();
                }
                catch (Exception e)
                {
                    _logEvent.Log(e.Message);

                    StringBuilder msgs = new StringBuilder();
                    msgs.AppendLine("請求參數不合法!");
                    if (!string.IsNullOrEmpty(e.Message))
                        msgs.AppendLine(e.Message);
                    _response.Error(msgs.ToString());
                    Result = _controller.BadRequest(_response);
                }
            }

            return this;
        }

        public async Task<IActionResult> Do<Response>(ActionReturn<Response, UserClaimModel> doFun, ActionReturn<Response, Exception, UserClaimModel> exceptionFun = null) where Response : ResponseModel
        {
            return await Do(async (response, user, logger) =>
            {
                return await doFun(response, user);
            },
            exceptionFun);
        }

        public async Task<IActionResult> Do<Response>(ActionReturn<Response, UserClaimModel, StructLoggerEvent> doFun, ActionReturn<Response, Exception, UserClaimModel> exceptionFun = null) where Response : ResponseModel
        {
            _logEvent.Log("Start Do:");

            if (Result != null)
            {
                _logEvent.Log("Response", Result);
                sendLog();
                return Result;
            }

            Response response = null;

            try
            {
                response = (Response)_response;
            }
            catch
            {
                int statusCode = (int)HttpStatusCode.InternalServerError;

                _logEvent.Log("回傳型態不同");
                _logEvent.Log("HttpStatusCode", statusCode.ToString());
                _logEvent.Log("Response", _response);
                sendLog();
                return _controller.StatusCode(statusCode, "回傳型態不同");
            }

            try
            {
                response = await doFun(response, _user, _logEvent);
            }
            catch (Exception e)
            {
                _logEvent.Log(e.Message);

                if (exceptionFun == null)
                    response.Error(e.Message);
                else
                    response = await exceptionFun(response, e, _user);
            }

            _logEvent.Log("Response", response);
            sendLog();
            return _controller.Ok(response);
        }

        private void sendLog()
        {
            _logger?.Info(_logEvent);
        }

    }
}
