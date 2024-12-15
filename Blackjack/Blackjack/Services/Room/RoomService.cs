
using Blackjack.Domain.Models;
using Blackjack.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Blackjack.Services.Room
{
    public class RoomService : IRoomService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHubContext<GameHub> _hubContext;

        public RoomService(IMemoryCache memoryCache, IHubContext<GameHub> hubContext)
        {
            _memoryCache = memoryCache;
            _hubContext = hubContext;
        }
        public Guid CreateRoomAsync()
        {
            var roomId = Guid.NewGuid();
            var rand = new Random();
            var newRoom = new RoomModel();
            _memoryCache.Set(roomId, newRoom);
            return roomId;
        }

        public void JoinRoomAsync(Guid roomId, string connectionId)
        {
            var room = _memoryCache.Get<RoomModel>(roomId);
            Console.WriteLine("Requested id: " + connectionId);
            Console.WriteLine(JsonSerializer.Serialize(room));
            if (room == null || (!string.IsNullOrEmpty(room.GuestConnection) && !string.IsNullOrEmpty(room.HostConnection)) || room.GuestConnection == connectionId || room.HostConnection == connectionId)
            {
                Console.WriteLine("Bad request on " + connectionId);
                return;
            }
            _memoryCache.Set(connectionId, roomId);
            _memoryCache.Set(roomId, room);

            if (string.IsNullOrEmpty(room.HostConnection))
            {
                room.HostConnection = connectionId;
                if (room.HostMove)
                {
                    _hubContext.Clients.Client(room.HostConnection).SendAsync("NotifyTurn", 1);
                }
            }
            else
            {
                room.GuestConnection = connectionId;

                if (!room.HostMove)
                {
                    _hubContext.Clients.Client(room.GuestConnection).SendAsync("NotifyTurn", 1);
                }
            }
            if (!string.IsNullOrEmpty(room.HostConnection) && !string.IsNullOrEmpty(room.GuestConnection))
            {
                _hubContext.Clients.Client(room.HostConnection).SendAsync("PlayerJoin", room);
                _hubContext.Clients.Client(room.GuestConnection).SendAsync("PlayerJoin", room);
            }
        }

        public void DisconnectRoomAsync(string connectionId)
        {
            var roomId = _memoryCache.Get<Guid>(connectionId);
            var room = _memoryCache.Get<RoomModel>(roomId);
            if (room is null)
                return;
            if (room.HostConnection == connectionId)
            {
                room.HostConnection = string.Empty;
                _hubContext.Clients.Client(room.GuestConnection).SendAsync("PlayerLeft", 1);
            }
            else if (room.GuestConnection == connectionId)
            {
                room.GuestConnection = string.Empty;
                _hubContext.Clients.Client(room.HostConnection).SendAsync("PlayerLeft", 1);
            }
            _memoryCache.Set(roomId, room);
            _memoryCache.Remove(connectionId);
        }
    }
}
