using Blackjack.Domain.Models;

namespace Blackjack.Services.Game
{
    public interface IGameService
    {
        public RoomModel StartGame(Guid roomId, string connectionId);
        public RoomModel MakeStand(Guid roomId, string connectionId);
        public RoomModel MakeHit(Guid roomId, string connectionId);
    }
}
