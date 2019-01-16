using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameFramework.PokerGame
{
    public class Poker
    {
        public static readonly int CARD_NUM = 52;
        public static readonly int NUMBER_NUM = 13;
        public static readonly int SUIT_NUM = 4;
        public static readonly int MAX_NUMBER = 1;
        public static readonly int MIN_NUMBER = 2;

        private List<PokerCard> _cards;

        public static bool Is_Bigger_Suit(PokerSuit suitA, PokerSuit suitB)
        {
            if ((int)suitA > (int)suitB)
                return true;
            return false;
        }

        public static PokerSuit Get_Bigger_Suit(PokerSuit suit)
        {
            if (PokerSuit.Spade == suit)
                throw new Exception("no bigger");
            return (PokerSuit)((int)suit + 1);
        }

        public static int Plus_Number(int i,int plus)
        {
            if (i == MAX_NUMBER)
                i = NUMBER_NUM + 1;

            i += plus;

            if (i > NUMBER_NUM)
                i = MAX_NUMBER;
            else if (i < MIN_NUMBER)
                i = MIN_NUMBER;

            return i;
        }

        public static int Compare_Number(int a, int b)
        {
            a = Get_Compare_Value(a);
            b = Get_Compare_Value(b);

            if (a > b)
                return 1;
            else if (a == b)
                return 0;
            else return -1;
        }

        public static int Get_Compare_Value(int a)
        {
            if (a == MAX_NUMBER)
                a = NUMBER_NUM + 1;

            return a;
        }

        public Poker()
        {
            _cards = new List<PokerCard>();

            for (int i = 1; i <= NUMBER_NUM; i++)
                _cards.Add(new PokerCard(PokerSuit.Spade, i));

            for (int i = 1; i <= NUMBER_NUM; i++)
                _cards.Add(new PokerCard(PokerSuit.Heart, i));

            for (int i = 1; i <= NUMBER_NUM; i++)
                _cards.Add(new PokerCard(PokerSuit.Diamond, i));

            for (int i = 1; i <= NUMBER_NUM; i++)
                _cards.Add(new PokerCard(PokerSuit.Club, i));
        }

        public void Shuffle()
        {
            Random random = new Random();

            PokerCard card;
            int j;
            for (int i = 0; i < CARD_NUM; i++)
            {
                card = _cards[i];
                j = random.Next(CARD_NUM - 1);
                _cards[i] = _cards[j];
                _cards[j] = card;
            }
        }

        public PokerCard[][] DealTo(int pieces)
        {
            int remainder = CARD_NUM % pieces;
            int cardCountPerPiece = CARD_NUM / pieces;

            pieces += (remainder > 0) ? 1 : 0;

            PokerCard[][] result = new PokerCard[pieces][];
            int cardIndex = 0;
            for (int pieceIndex = 0; pieceIndex < pieces; pieceIndex++)
            {
                result[pieceIndex] = new PokerCard[cardCountPerPiece];
                for (int i = 0; i < cardCountPerPiece; i++)
                {
                    result[pieceIndex][i] = _cards[cardIndex];
                    cardIndex++;

                    if (cardIndex >= CARD_NUM)
                        break;
                }
            }

            return result;
        }

        public PokerCard[] GetCards()
        {
            return _cards.ToArray();
        }
    }
}