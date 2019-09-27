using System;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ApiResponse
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
