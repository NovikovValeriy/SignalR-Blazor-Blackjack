using Blackjack.Domain.Models;
using Blackjack.Services.Game;
using Blackjack.Services.Room;
using Microsoft.AspNetCore.SignalR;

namespace Blackjack.Hubs
{
    public class GameHub : Hub
    {
        private readonly IRoomService _roomService;

        public GameHub(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public void CreateRoom()
        {
            Clients.Caller.SendAsync("ReceiveRoomId", _roomService.CreateRoomAsync());
            Context.Abort();
        }

        public async Task JoinRoom(Guid roomId)
        {
            _roomService.JoinRoomAsync(roomId, Context.ConnectionId);
            await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        }

        //public async Task MakeMove(Guid roomId, int i, int j)
        //{
        //    var room = _gameService.MakeMove(roomId, Context.ConnectionId, i, j);
        //    if (room.HostMove)
        //    {
        //        await Clients.Client(room.HostConnection).SendAsync("NotifyTurn", room.Board[0][0]);
        //        Console.WriteLine("sent turn");
        //    }
        //    else
        //    {
        //        await Clients.Client(room.GuestConnection).SendAsync("NotifyTurn", room.Board[0][0]);
        //        Console.WriteLine("sent turn");
        //    }
        //    await Clients.Client(room.HostConnection).SendAsync("ReceiveMove", room.Board);
        //    await Clients.Client(room.GuestConnection).SendAsync("ReceiveMove", room.Board);
        //}

        public async Task MakeMove(Guid roomId, RoomModel room)
        {
            await Clients.Client(room.HostConnection).SendAsync("ReceiveMove", room);
            await Clients.Client(room.GuestConnection).SendAsync("ReceiveMove", room);
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"OnConnectedAsync {Context.ConnectionId}");
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
