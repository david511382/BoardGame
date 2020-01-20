using Domain.Api.Models.Response;

namespace BoardGameAngular.Models.BigTwo.Response
{
    public class HandCardsResponse : ResponseModel
    {
        public PockerCardModel[] Cards { get; set; }
    }
}
