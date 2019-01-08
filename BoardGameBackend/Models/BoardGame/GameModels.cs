using BoardGame.Backend.Models.BoardGame.BigTwo;
using BoardGame.Backend.Models.BoardGame.GameFramework;
using BoardGame.Backend.Models.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame
{
    public class GameModels
    {
        public PokerCard[] GetCards(int playerId)
        {
            try
            {
                return BoardGameManager.GetPlayerById(playerId).GetHandCards();
            }
            catch
            {
                return null;
            }
        }

        public PokerCard[] SelectCard(int playerId, int[] selectedIndex)
        {
            BigTwoPlayer player;
            try
            {
                player = BoardGameManager.GetPlayerById(playerId);

                PokerCard[] pokerCards;
                pokerCards = player.GetCardGroup(selectedIndex);
                pokerCards = pokerCards.OrderBy(d => d.Number).ThenBy(d => d.Suit).ToArray();

                return pokerCards;
            }
            catch
            {
                return null;
            }
        }

        public GameStatus GetGameStatus(int gameId)
        {
            GameFramework.BoardGame game;
            try
            {
                game = BoardGameManager.GetGameById(gameId);

                return game.GetGameStatus();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public PokerCard[] GetTable(int playerId)
        {
            GameFramework.GameBoard gameBoard;
            try
            {
                gameBoard = BoardGameManager.GetGameBoardByPlayerId(playerId);

                return ((PokerCardGroup) gameBoard.GetLastItem()).GetCards();
            }
            catch
            {
                return null;
            }
        }

        public bool PlayCard(int playerId, int[] indexs)
        {
            BigTwoPlayer player;
            try
            {
                player = BoardGameManager.GetPlayerById(playerId);

                return player.PlayCard(indexs);
            }
            catch
            {
                return false;
            }
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