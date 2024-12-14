using Blackjack.Domain.Models;

namespace Blackjack.Services.Game
{
    public interface IGameService
    {
        RoomModel MakeMove(Guid roomId, string connectionId, int i, int j);
    }
}
