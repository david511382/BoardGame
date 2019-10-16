namespace Domain.Api.Models.Response.User
{
    public class LoginResponse : ResponseModel
    {
        public string Token { get; set; }
        public string Name { get; set; }
    }
}
