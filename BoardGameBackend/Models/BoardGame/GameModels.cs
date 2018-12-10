﻿using BoardGame.Backend.Models.BoardGame;
using BoardGame.Backend.Models.BoardGame.BigTwo;
using BoardGame.Backend.Models.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGameBackend.Models.BoardGame
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
            PokerCard[] pokerCards;
            BigTwoPlayer player;
            try
            {
                player = BoardGameManager.GetPlayerById(playerId);
            }
            catch
            {
                return null;
            }

            if (player.IsOnTurn())
            {
                pokerCards = player.GetCardGroup(selectedIndex);
                pokerCards = pokerCards.OrderBy(d => d.Number).ThenBy(d => d.Suit).ToArray();
            }
            else
            {
                int lastSelectedIndex = selectedIndex.Last();
                pokerCards = new PokerCard[] { player.GetHandCards()[lastSelectedIndex] };
            }

            return pokerCards;
        }

        public bool PlayCard(int playerId, int[] indexs)
        {
            if (indexs==null || indexs.Length == 0)
                return false;

            BigTwoPlayer player;
            try
            {
                player = BoardGameManager.GetPlayerById(playerId);
            }
            catch
            {
                return false;
            }

            if (!player.IsOnTurn())
            {
                return false;
            }

            return player.PlayCard(indexs); 
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