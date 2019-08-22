using BigTwoLogic;
using BoardGame.Data.ApiParameters;
using GameLogic.Game;
using GameLogic.PokerGame;
using GameLogic.PokerGame.CardGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGameWebService.Models.BoardGame
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

        public GameStatusModel GetGameStatus(int roomId)
        {
            GameLogic.Game.BoardGame game;
            try
            {
                game = BoardGameManager.GetGameById(roomId);
                GameStatus gameStatus = game.GetGameStatus();

                GameStatusModel model = new GameStatusModel();
                model.CurrentPlayerId =gameStatus.CurrentPlayerId;
                model.State = gameStatus.State;
                model.WinPlayers = BoardGameManager._playerManager
                    .GetPlayers(gameStatus.WinPlayerIds)
                    .Select(d => d.Models)
                    .ToArray();

                return model;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public PokerCard[] GetTable(int playerId)
        {
            GameBoard gameBoard;
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