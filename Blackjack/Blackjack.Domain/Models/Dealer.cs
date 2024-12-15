using System.Numerics;
using System;

namespace Blackjack.Domain.Models
{
    public class Dealer : Player
    {
        public CardDeck Deck { get; set; } = new CardDeck();

        public Card Deal()
        {
            return Deck.Draw();
        }

        public bool HasAceShowing => Cards.Count == 2 && VisibleScore == 11 && Cards.Any(x => x.IsVisible == false);

        public void Reveal()
        {
            Cards.ForEach(x => x.IsVisible = true);
        }

        public void DealToSelf()
        {
            AddCard(Deal());
        }

        public void DealToPlayer(Player player)
        {
            player.AddCard(Deal());
        }
    }
}
