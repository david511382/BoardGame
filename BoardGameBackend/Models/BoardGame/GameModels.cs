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
                return BoardGameManager.GetGamePlayerById(playerId).GetHandCards();
            }
            catch
            {
                return null;
            }
        }

        public int[] SelectCard(int playerId, int[] selectedIndex)
        {
            BigTwoPlayer player;
            try
            {
                player = BoardGameManager.GetGamePlayerById(playerId);

                PokerCard[] pokerCards;
                pokerCards = player.GetCardGroup(selectedIndex);
                pokerCards = pokerCards.OrderBy(d => d.Number).ThenBy(d => d.Suit).ToArray();

                return GetIndexOfCards(pokerCards, player.GetHandCards());
            }
            catch
            {
                return new int[0];
            }
        }

        private static int[] GetIndexOfCards(PokerCard[] cards, PokerCard[] handCards)
        {
            List<int> result = new List<int>();

            if (cards != null)
            {
                foreach (PokerCard card in cards)
                {
                    for (int i = 0; i < handCards.Length; i++)
                    {
                        PokerCard handcard = handCards[i];
                        if (handcard.Number == card.Number &&
                            handcard.Suit == card.Suit)
                        {
                            result.Add(i);
                            break;
                        }
                    }
                }
            }

            return result.ToArray();
        }

        public GameStatus GetGameStatus(int roomId)
        {
            GameFramework.BoardGame game;
            try
            {
                game = BoardGameManager.GetGameById(roomId);

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
                player = BoardGameManager.GetGamePlayerById(playerId);

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