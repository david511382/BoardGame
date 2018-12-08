using BoardGame.Backend.Models.Game.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGameBackend.Models.BoardGame
{
    public class GameModels
    {
        public PokerCard[] GetCards()
        {
            return BoardGameManager._thisPlayer.GetHandCards();
        }

        public PokerCard[] SelectCard(int i)
        {
            PokerCard[] pokerCards;
            if (BoardGameManager._thisPlayer.IsOnTurn())
            {
                pokerCards = BoardGameManager._thisPlayer.GetCardGroup(new int[] { i });
                pokerCards = pokerCards.OrderBy(d => d.Number).ThenBy(d => d.Suit).ToArray();
            }
            else
            {
                pokerCards = new PokerCard[] { BoardGameManager._thisPlayer.GetHandCards()[i] };
            }

            return pokerCards;
        }

        private string GetCardInfo(PokerCard pokerCard)
        {
            string suit = string.Empty;
            string result = string.Empty;
            result += pokerCard.Number.ToString();
            switch (pokerCard.Suit)
            {
                case PokerSuit.Club:
                    suit = "C";
                    break;
                case PokerSuit.Diamond:
                    suit = "D";
                    break;
                case PokerSuit.Heart:
                    suit = "H";
                    break;
                case PokerSuit.Spade:
                    suit = "S";
                    break;
            }
            result += suit;
            result += " ";

            return result;
        }
    }
}