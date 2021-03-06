﻿namespace Domain.Api.Models.Response
{
    public class BoolResponseModel : ResponseModel
    {
        public bool IsSuccess { get; set; }

        public void Success(string msg)
        {
            IsSuccess = true;
            Message = msg;
        }

        public void Fail(string msg)
        {
            IsSuccess = false;
            Message = msg;
        }
        public BoolResponseModel()
        {
        }

        public BoolResponseModel(BoolResponseModel d)
        {
            IsSuccess = d.IsSuccess;
            ErrorMessage = d.ErrorMessage;
            IsError = d.IsError;
            Message = d.Message;
        }
    }
}
