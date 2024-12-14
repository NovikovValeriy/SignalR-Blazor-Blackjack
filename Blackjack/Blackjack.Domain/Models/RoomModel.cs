namespace Blackjack.Domain.Models
{
    public class RoomModel
    {
        public Guid Id { get; set; }
        public string HostConnection { get; set; }
        public string GuestConnection { get; set; }
        public bool HostMove { get; set; }

        public Dealer Dealer { get; set; } = new Dealer();
        public Player Host { get; set; } = new Player();
        public Player Guest { get; set; } = new Player();
    }
}
