namespace Domain.Api.Models.Response
{
    public class ResponseModel
    {
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsError { get; set; }

        public void Error(string msg)
        {
            ErrorMessage = msg;
            IsError = true;
        }
    }
}
