using Blackjack.Domain.Models;
using Blackjack.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Blackjack.Services.Game
{
    public class GameService : IGameService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHubContext<GameHub> _hubContext;

        public GameService(IMemoryCache memoryCache, IHubContext<GameHub> hubContext)
        {
            _memoryCache = memoryCache;
            _hubContext = hubContext;
        }

        public RoomModel MakeMove(Guid roomId, string connectionId, int i, int j)
        {
            var room = _memoryCache.Get<RoomModel>(roomId);

            //Console.WriteLine(connectionId);
            //Console.WriteLine(JsonSerializer.Serialize(room));
            //if (connectionId == room.HostConnection && room.HostMove)
            //{
            //    room.Board[i][j] = 1;
            //    room.HostMove = !room.HostMove;
            //}
            //else if (connectionId == room.GuestConnection && !room.HostMove)
            //{
            //    room.Board[i][j] = 2;
            //    room.HostMove = !room.HostMove;
            //}

            //_memoryCache.Set(roomId, room);
            //var res = CheckWin(room.Board);
            //if (res != 0)
            //{
            //    _hubContext.Clients.Client(room.HostConnection).SendAsync("GameOver", res);
            //    _hubContext.Clients.Client(room.GuestConnection).SendAsync("GameOver", res);
            //}
            return room;
        }

        private int CheckWin(List<List<int>> board)
        {
            int n = board.Count;
            int winLength = 5;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j <= n - winLength; j++)
                {
                    bool isWin = true;
                    int first = board[i][j];
                    if (first == 0) continue;

                    for (int k = 1; k < winLength; k++)
                    {
                        if (board[i][j + k] != first)
                        {
                            isWin = false;
                            break;
                        }
                    }
                    if (isWin) return first;
                }
            }

            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i <= n - winLength; i++)
                {
                    bool isWin = true;
                    int first = board[i][j];
                    if (first == 0) continue;

                    for (int k = 1; k < winLength; k++)
                    {
                        if (board[i + k][j] != first)
                        {
                            isWin = false;
                            break;
                        }
                    }
                    if (isWin) return first;
                }
            }

            for (int i = 0; i <= n - winLength; i++)
            {
                for (int j = 0; j <= n - winLength; j++)
                {
                    bool isWin = true;
                    int first = board[i][j];
                    if (first == 0) continue;

                    for (int k = 1; k < winLength; k++)
                    {
                        if (board[i + k][j + k] != first)
                        {
                            isWin = false;
                            break;
                        }
                    }
                    if (isWin) return first;
                }
            }

            for (int i = 0; i <= n - winLength; i++)
            {
                for (int j = winLength - 1; j < n; j++)
                {
                    bool isWin = true;
                    int first = board[i][j];
                    if (first == 0) continue;

                    for (int k = 1; k < winLength; k++)
                    {
                        if (board[i + k][j - k] != first)
                        {
                            isWin = false;
                            break;
                        }
                    }
                    if (isWin) return first;
                }
            }

            return 0;
        }
    }
}
