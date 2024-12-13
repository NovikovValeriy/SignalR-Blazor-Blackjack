namespace Blackjack.Domain.Models.Enums
{
    public enum GameState
    {
        NotStarted,
        Betting,
        Dealing,
        InProgress,
        Insurance,
        Payout,
        Shuffling,
        EscortedOut
    }
}
