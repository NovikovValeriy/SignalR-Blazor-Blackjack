using Blackjack.Domain.Models;
using Blackjack.Services.Game;
using Blackjack.Services.Room;
using Microsoft.AspNetCore.SignalR;

namespace Blackjack.Hubs
{
    public class GameHub : Hub
    {
        private readonly IRoomService _roomService;
        private readonly IGameService _gameService;

        public GameHub(IRoomService roomService, IGameService gameService)
        {
            _roomService = roomService;
            _gameService = gameService;
        }

        public void CreateRoom()
        {
            Clients.Caller.SendAsync("ReceiveRoomId", _roomService.CreateRoomAsync());
            Context.Abort();
        }

        public async Task JoinRoom(Guid roomId)
        {
            _roomService.JoinRoomAsync(roomId, Context.ConnectionId);
            //await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        }

        public async Task StartGame(Guid roomId)
        {
            var room = _gameService.StartGame(roomId, Context.ConnectionId);
            await Clients.Client(room.HostConnection).SendAsync("ReceiveMove", room);
            await Clients.Client(room.GuestConnection).SendAsync("ReceiveMove", room);
        }

        public async Task Hit(Guid roomId)
        {
            var room = _gameService.MakeHit(roomId, Context.ConnectionId);
            await Clients.Client(room.HostConnection).SendAsync("ReceiveMove", room);
            await Clients.Client(room.GuestConnection).SendAsync("ReceiveMove", room);
        }
        public async Task Stand(Guid roomId)
        {
            var room = _gameService.MakeStand(roomId, Context.ConnectionId);
            await Clients.Client(room.HostConnection).SendAsync("ReceiveMove", room);
            await Clients.Client(room.GuestConnection).SendAsync("ReceiveMove", room);
            if (room.HostMove)
            {
                await Clients.Client(room.HostConnection).SendAsync("NotifyTurn", 1);
            }
            else
            {
                await Clients.Client(room.GuestConnection).SendAsync("NotifyTurn", 1);
            }
        }


        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("ReceiveConnectionId", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("Disconnect" + Context.ConnectionId);
            _roomService.DisconnectRoomAsync(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
