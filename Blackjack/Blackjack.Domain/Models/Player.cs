using Blackjack.Domain.Models.Enums;

namespace Blackjack.Domain.Models
{
    public class Player
    {
        public List<Card> Cards { get; set; } = new List<Card>();
        public bool HasStood { get; set; }

        public string ScoreDisplay
        {
            get
            {
                if (HasNaturalBlackjack && Cards.All(x => x.IsVisible))
                    return "Blackjack!";

                var score = VisibleScore;

                if (score > 21)
                    return "Busted!";
                else return score.ToString();
            }
        }

        public async Task AddCard(Card card)
        {
            Cards.Add(card);
            //await Task.Delay(300);
        }

        public int Score
        {
            get
            {
                return ScoreCalculation();
            }
        }

        public int VisibleScore
        {
            get
            {
                return ScoreCalculation(true);
            }
        }

        private int ScoreCalculation(bool onlyVisible = false)
        {
            var cards = Cards;

            if (onlyVisible)
            {
                cards = Cards.Where(x => x.IsVisible).ToList();
            }

            var totalScore = cards.Sum(x => x.Score);
            if (totalScore <= 21) return totalScore;

            bool hasAces = cards.Any(x => x.Value == CardValue.Ace);
            if (!hasAces && totalScore > 21) return totalScore;

            var acesCount = cards.Where(x => x.Value == CardValue.Ace).Count();
            var acesScore = cards.Sum(x => x.Score);

            while (acesCount > 0)
            {
                acesCount -= 1;
                acesScore -= 10;

                if (acesScore <= 21) return acesScore;
            }

            return cards.Sum(x => x.Score);
        }

        public bool IsBusted => Score > 21;

        public void ClearHand()
        {
            Cards.Clear();
        }

        public bool HasNaturalBlackjack =>
            Cards.Count == 2
            && Score == 21
            && Cards.Any(x => x.Value == CardValue.Ace)
            && Cards.Any(x => x.IsTenCard);
    }
}
