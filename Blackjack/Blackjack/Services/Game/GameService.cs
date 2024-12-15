using Blackjack.Domain.Models;
using Blackjack.Domain.Models.Enums;
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
        private RoomModel _room;

        public GameService(IMemoryCache memoryCache, IHubContext<GameHub> hubContext)
        {
            _memoryCache = memoryCache;
            _hubContext = hubContext;
        }

        public RoomModel StartGame(Guid roomId, string connectionId)
        {
            _room = _memoryCache.Get<RoomModel>(roomId);
            //if(_room.GameState == GameState.Payout)
            //{

            //}
            //else if (
            //    _room.GameState != GameState.NotStarted 
            //    && _room.GameState != GameState.Payout 
            //    || _room != null) 
            //    return _room;
            if (_room.GameState == GameState.Payout)
                NewHand();
            else InitializeHand();
            _memoryCache.Set(roomId, _room);
            return _room;
        }

        public RoomModel MakeStand(Guid roomId, string connectionId)
        {
            _room = _memoryCache.Get<RoomModel>(roomId);
            if (connectionId == _room.HostConnection) Stand(_room.Host);
            else Stand(_room.Guest);
            _memoryCache.Set(roomId, _room);
            return _room;
        }

        public RoomModel MakeHit(Guid roomId, string connectionId)
        {
            _room = _memoryCache.Get<RoomModel>(roomId);
            if (connectionId == _room.HostConnection) Hit(_room.Host);
            else Hit(_room.Guest);
            _memoryCache.Set(roomId, _room);
            return _room;
        }

        /*public RoomModel MakeMove(Guid roomId, string connectionId, string move)
        {
            _room = _memoryCache.Get<RoomModel>(roomId);

            Console.WriteLine(connectionId);
            Console.WriteLine(JsonSerializer.Serialize(_room));
            if (connectionId == _room.HostConnection && move == "stand")
            {
                _room.Host.HasStood = true;
                _room.HostMove = !_room.HostMove;

                if (_room.HostMove
                || _room.Guest.HasNaturalBlackjack && !_room.HostMove)
                {
                    _room.Dealer.Reveal();
                    DealerTurn();
                }

                EndHand();
            }
            else if (connectionId == _room.GuestConnection && !_room.HostMove)
            {
                //room.Board[i][j] = 2;
                //_room.HostMove = !_room.HostMove;
            }

            _memoryCache.Set(roomId, _room);
            //var res = CheckWin(room.Board);
            //if (res != 0)
            //{
            //    _hubContext.Clients.Client(room.HostConnection).SendAsync("GameOver", res);
            //    _hubContext.Clients.Client(room.GuestConnection).SendAsync("GameOver", res);
            //}
            return _room;
        }*/

        private void InitializeHand()
        {
            _room.GameState = GameState.Dealing;
            _room.Dealer.Deck = new CardDeck();
            Deal();
        }

        private void Deal()
        {
            _room.GameState = GameState.Dealing;
            _room.Dealer.DealToPlayer(_room.Host);
            _room.Dealer.DealToPlayer(_room.Guest);

            var dealerCard = _room.Dealer.Deal();
            dealerCard.IsVisible = false;
            _room.Dealer.AddCard(dealerCard);

            _room.Dealer.DealToPlayer(_room.Host);
            _room.Dealer.DealToPlayer(_room.Guest);

            _room.Dealer.DealToSelf();

            _room.GameState = GameState.InProgress;


            if (_room.Host.HasNaturalBlackjack && _room.Guest.HasNaturalBlackjack)
            {
                if(_room.Dealer.HasNaturalBlackjack)
                EndHand();
            }
            if (_room.Host.HasNaturalBlackjack)
            {
                _room.Host.HasStood = true;
                _room.HostMove = false;
                _hubContext.Clients.Client(_room.HostConnection).SendAsync("Blackjack");
            }
            else _room.HostMove = true;
            if (_room.Guest.HasNaturalBlackjack)
            {
                _room.Guest.HasStood = true;
            }
        }

        private void NewHand()
        {

            _room.Host.ClearHand();
            _room.Guest.ClearHand();
            _room.Dealer.ClearHand();

            _room.GameState = GameState.NotStarted;

            InitializeHand();
        }


        private void DealerTurn()
        {
            if (_room.Dealer.Score < 17)
            {
                _room.Dealer.DealToSelf();
                DealerTurn();
            }
        }

        private void Hit(Player player)
        {
            _room.Dealer.DealToPlayer(player);
            if (player.IsBusted)
            {
                _room.HostMove = !_room.HostMove;
                if (_room.HostMove) DealerTurn();
                EndHand();
            }
        }

        private void Stand(Player player)
        {
            player.HasStood = true;
            _room.HostMove = !_room.HostMove;

            if (_room.HostMove
            || _room.Guest.HasNaturalBlackjack && !_room.HostMove)
            {
                _room.Dealer.Reveal();
                DealerTurn();
            }

            EndHand();
        }

        private void EndHand()
        {
            if (_room.Host.HasStood && _room.Guest.HasStood
                || _room.Host.IsBusted && _room.Guest.HasStood
                || _room.Host.HasStood && _room.Guest.IsBusted
                || _room.Host.IsBusted && _room.Guest.IsBusted
                || _room.Host.HasNaturalBlackjack && _room.Guest.HasNaturalBlackjack)
            {
                _room.GameState = GameState.Payout;
                //_hubContext.Clients.Client(_room.HostConnection).SendAsync("Payout");
                //_hubContext.Clients.Client(_room.GuestConnection).SendAsync("Payout");
                _room.Host.HasStood = false;
                _room.Guest.HasStood = false;
            }
        }
    }
}
