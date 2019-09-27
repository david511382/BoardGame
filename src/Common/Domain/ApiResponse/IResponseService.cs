using Domain.JWTUser;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Domain.ApiResponse
{
    public delegate Task<R> ActionReturn<R, E>(R resp, E exp);
    public delegate Task<R> ActionReturn<R, E, U>(R resp, E exp, U user);

    public interface IResponseMaker
    {
        IResponseMaker ValidateToken(Action<UserClaimModel> validateFun);
        IResponseMaker ValidateRequest(Action validateFun);
        Task<IActionResult> Do<Response>(ActionReturn<Response, UserClaimModel> doFun, ActionReturn<Response, Exception, UserClaimModel> exceptionFun = null) where Response : ResponseModel;
    }

    public interface IResponseService
    {
        IResponseMaker Init<Response>(ControllerBase c) where Response : ResponseModel, new();
    }
}
