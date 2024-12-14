namespace Blackjack.Services.Room
{
    public interface IRoomService
    {
        Guid CreateRoomAsync();
        void JoinRoomAsync(Guid roomId, string connectionId);
        void DisconnectRoomAsync(string connectionId);
    }
}
