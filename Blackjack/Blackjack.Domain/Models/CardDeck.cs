using Blackjack.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack.Domain.Models
{
    public class CardDeck
    {
        protected Stack<Card> Cards { get; set; } = new Stack<Card>();

        public int Count
        {
            get
            {
                return Cards.Count;
            }
        }

        public CardDeck()
        {
            List<Card> cards = new List<Card>();

            foreach (CardSuit suit in (CardSuit[])Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardValue value in (CardValue[])Enum.GetValues(typeof(CardValue)))
                {
                    Card newCard = new Card()
                    {
                        Suit = suit,
                        Value = value,
                        ImageName = "card" + suit.GetDisplayName() + value.GetDisplayName()
                    };

                    cards.Add(newCard);
                }
            }

            var array = cards.ToArray();

            Random rnd = new Random();

            for (int n = array.Count() - 1; n > 0; --n)
            {
                int k = rnd.Next(n + 1);

                Card temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }

            for (int n = 0; n < array.Count(); n++)
            {
                Cards.Push(array[n]);
            }
        }

        public void Add(Card card)
        {
            Cards.Push(card);
        }

        public Card Draw()
        {
            return Cards.Pop();
        }
    }
}
