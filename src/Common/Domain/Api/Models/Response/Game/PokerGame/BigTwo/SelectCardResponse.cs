using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Api.Models.Response.Game.PokerGame.BigTwo
{
    public class SelectCardResponse : ResponseModel
    {
        public int[] CardIndexs { get; set; }

    }
}
