using Domain.ApiResponse;

namespace AuthWebService.Models
{
    public class LoginResponse : ResponseModel
    {
        public string Token { get; set; }
        public string Name { get; set; }
    }
}
