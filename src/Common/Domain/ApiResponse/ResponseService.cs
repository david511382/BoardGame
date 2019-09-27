using Domain.JWTUser;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ApiResponse
{
    public class ResponseService : IResponseMaker, IResponseService
    {
        public IActionResult Result { get; private set; }
        private ResponseModel _response;
        private ControllerBase _controller;
        private UserClaimModel _user;

        public IResponseMaker Init<Response>(ControllerBase c) where Response : ResponseModel, new()
        {
            _controller = c;
            Result = null;
            _response = new Response();

            return this;
        }

        public IResponseMaker ValidateToken(Action<UserClaimModel> validateFun)
        {
            if (Result == null)
            {
                try
                {
                    _user = UserClaim.Parse(_controller.HttpContext.User);

                    validateFun(_user);
                }
                catch (Exception e)
                {
                    _response.Error(e.Message);
                    Result = _controller.Forbid();
                }
            }

            return this;
        }

        public IResponseMaker ValidateRequest(Action validateFun)
        {
            if (Result == null)
            {
                try
                {
                    validateFun();
                }
                catch (Exception e)
                {
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
            if (Result != null)
                return Result;

            Response response = null;

            try
            {
                response = (Response)_response;
            }
            catch
            {
                return _controller.StatusCode((int)HttpStatusCode.InternalServerError, "回傳型態不同");
            }

            try
            {
                response = await doFun(response, _user);
            }
            catch (Exception e)
            {
                if (exceptionFun == null)
                    response.Error(e.Message);
                else
                    response = await exceptionFun(response, e, _user);
            }

            return _controller.Ok(response);
        }
    }
}
