﻿using Domain.Api.Models.Response;
using Domain.JWTUser;
using Domain.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Domain.Api.Interfaces
{
    public delegate Task<R> ActionReturn<R, E>(R resp, E exp);
    public delegate Task<R> ActionReturn<R, E, U>(R resp, E exp, U user);

    public interface IResponseMaker
    {
        IResponseMaker ValidateToken(Action<UserClaimModel> validateFun);
        IResponseMaker ValidateRequest(Action validateFun);
        Task<IActionResult> Do<Response>(ActionReturn<Response, UserClaimModel> doFun, ActionReturn<Response, Exception, StructLoggerEvent> exceptionFun = null) where Response : ResponseModel;
        Task<IActionResult> Do<Response>(ActionReturn<Response, UserClaimModel, StructLoggerEvent> doFun, ActionReturn<Response, Exception, StructLoggerEvent> exceptionFun = null) where Response : ResponseModel;
    }

    public interface IResponseService
    {
        IResponseMaker Init<Response>(ControllerBase c, ILogger logger = null) where Response : ResponseModel, new();
    }
}
