using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UnoServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new UnoRoomServer(5000, "TestRoom");
            await server.StartAsync();

            Console.WriteLine("Server đang chạy. Nhấn Enter để thoát...");
            Console.ReadLine();
        }
    }

    // ===== ENUMS =====
    public enum UnoColor { Red, Blue, Green, Yellow, Wild }
    public enum UnoValue
    {
        Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine,
        Skip, Reverse, DrawTwo,
        Wild, WildDrawFour
    }
    public enum RoomState { Waiting, Playing, Finished }

    // ===== CARD =====
    public class UnoCard
    {
        public UnoColor Color { get; set; }
        public UnoValue Value { get; set; }
        public UnoCard(UnoColor c, UnoValue v) { Color = c; Value = v; }
        public override string ToString() => $"{Color} {Value}";
    }

    // ===== DECK =====
    public class UnoDeck
    {
        private Stack<UnoCard> _cards;
        public UnoDeck()
        {
            _cards = new Stack<UnoCard>(GenerateDeck());
            Shuffle();
        }
        private List<UnoCard> GenerateDeck()
        {
            var deck = new List<UnoCard>();
            var colors = new[] { UnoColor.Red, UnoColor.Blue, UnoColor.Green, UnoColor.Yellow };
            foreach (var color in colors)
            {
                foreach (UnoValue value in Enum.GetValues(typeof(UnoValue)))
                {
                    if (value == UnoValue.Wild || value == UnoValue.WildDrawFour) continue;
                    deck.Add(new UnoCard(color, value));
                }
            }
            for (int i = 0; i < 4; i++)
            {
                deck.Add(new UnoCard(UnoColor.Wild, UnoValue.Wild));
                deck.Add(new UnoCard(UnoColor.Wild, UnoValue.WildDrawFour));
            }
            return deck;
        }
        private void Shuffle()
        {
            var rnd = new Random();
            _cards = new Stack<UnoCard>(_cards.OrderBy(c => rnd.Next()));
        }
        public UnoCard DrawCard() => _cards.Count > 0 ? _cards.Pop() : null;
    }

    // ===== PLAYER =====
    public class Player
    {
        public string Name { get; set; }
        public List<UnoCard> Hand { get; set; } = new List<UnoCard>();
        public bool CalledUno { get; set; } = false;
        public Player(string name) { Name = name; }
        public void DrawCard(UnoDeck deck)
        {
            var card = deck.DrawCard();
            if (card != null) Hand.Add(card);
        }
        public void PlayCard(UnoCard card) => Hand.Remove(card);
    }

    // ===== GAME LOGIC =====
    public class UnoGameLogic
    {
        public bool CanPlay(UnoCard card, UnoCard topCard)
        {
            return card.Color == topCard.Color ||
                   card.Value == topCard.Value ||
                   card.Color == UnoColor.Wild;
        }

        public void ApplySpecialEffect(UnoCard card, List<Player> players,
            ref int currentIndex, ref bool clockwise, UnoDeck deck)
        {
            switch (card.Value)
            {
                case UnoValue.Skip:
                    currentIndex = GetNextPlayerIndex(players.Count, currentIndex, clockwise);
                    break;
                case UnoValue.Reverse:
                    clockwise = !clockwise;
                    break;
                case UnoValue.DrawTwo:
                    int nextIndex = GetNextPlayerIndex(players.Count, currentIndex, clockwise);
                    players[nextIndex].DrawCard(deck);
                    players[nextIndex].DrawCard(deck);
                    break;
                case UnoValue.WildDrawFour:
                    int next = GetNextPlayerIndex(players.Count, currentIndex, clockwise);
                    for (int i = 0; i < 4; i++) players[next].DrawCard(deck);
                    break;
            }
        }

        private int GetNextPlayerIndex(int total, int currentIndex, bool clockwise)
        {
            return clockwise ? (currentIndex + 1) % total : (currentIndex - 1 + total) % total;
        }
    }

    // ===== ROOM =====
    public class UnoRoom
    {
        public string RoomName { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public UnoDeck Deck { get; set; } = new UnoDeck();
        public UnoCard TopCard { get; set; }
        public RoomState State { get; set; } = RoomState.Waiting;

        public UnoRoom(string name) { RoomName = name; }

        public void StartGame()
        {
            State = RoomState.Playing;
            TopCard = Deck.DrawCard();
            foreach (var p in Players)
                for (int i = 0; i < 7; i++) p.DrawCard(Deck);
        }

        public bool LastCardCallUno(Player p)
        {
            if (p.Hand.Count == 1 && !p.CalledUno)
            {
                p.DrawCard(Deck);
                p.DrawCard(Deck);
            }
            else if (p.Hand.Count == 0)
            {
                State = RoomState.Finished;
                return true;
            }
            p.CalledUno = false;
            return false;
        }
    }

    // ===== SERVER =====
    public class UnoRoomServer
    {
        private TcpListener _listener;
        private readonly List<TcpClient> _clients = new();
        private UnoRoom _room;
        private int _currentIndex = 0;
        private bool _clockwise = true;

        public UnoRoomServer(int port, string roomName)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _room = new UnoRoom(roomName);
        }

        public async Task StartAsync()
        {
            _listener.Start();
            Console.WriteLine("UNO Room Server started.");

            while (_clients.Count < 4)
            {
                var client = await _listener.AcceptTcpClientAsync();
                lock (_clients) { _clients.Add(client); }
                _ = HandleClientAsync(client);
                Console.WriteLine("Client connected.");
            }

            _room.StartGame();
            BroadcastState();
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            var stream = client.GetStream();
            var buffer = new byte[4096];

            while (client.Connected)
            {
                int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (read <= 0) break;

                string json = Encoding.UTF8.GetString(buffer, 0, read);
                dynamic msg = JsonConvert.DeserializeObject(json);
                string type = msg.type;

                switch (type)
                {
                    case "join":
                        string name = msg.player;
                        if (_room.Players.Count < 4 && !_room.Players.Any(p => p.Name == name))
                            _room.Players.Add(new Player(name));
                        break;

                    case "play":
                        HandlePlay((string)msg.player,
                            new UnoCard(Enum.Parse<UnoColor>((string)msg.card.color),
                                        Enum.Parse<UnoValue>((string)msg.card.value)));
                        break;

                    case "draw":
                        HandleDraw((string)msg.player);
                        break;

                    case "uno":
                        var p = _room.Players.FirstOrDefault(x => x.Name == (string)msg.player);
                        if (p != null) p.CalledUno = true;
                        break;
                }
            }

            lock (_clients) { _clients.Remove(client); }
            client.Close();
        }

        private void HandlePlay(string playerName, UnoCard card)
        {
            if (_room.State != RoomState.Playing) return;
            var current = _room.Players[_currentIndex];
            if (current.Name != playerName) return;

            var logic = new UnoGameLogic();
            if (!logic.CanPlay(card, _room.TopCard)) return;

            var handCard = current.Hand.FirstOrDefault(c => c.Color == card.Color && c.Value == card.Value);
            if (handCard == null) return;

            current.PlayCard(handCard);
            _room.TopCard = handCard;
            logic.ApplySpecialEffect(handCard, _room.Players, ref _currentIndex, ref _clockwise, _room.Deck);

            if (_room.LastCardCallUno(current))
                _room.State = RoomState.Finished;
            else
                _currentIndex = (_clockwise ? (_currentIndex + 1) : (_currentIndex - 1 + _room.Players.Count)) % _room.Players.Count;

            BroadcastState();
        }

        private void HandleDraw(string playerName)
        {
            var current = _room.Players[_currentIndex];
            if (current.Name != playerName) return;

            // Rút 1 lá nếu còn
            var card = _room.Deck.DrawCard();
            if (card != null) current.Hand.Add(card);

            // Chuyển lượt
            _currentIndex = (_clockwise ? (_currentIndex + 1) : (_currentIndex - 1 + _room.Players.Count)) % _room.Players.Count;

            // Cập nhật cho tất cả client
            BroadcastState();
        }
        private void BroadcastState()
        {
            // If game hasn't started or there is no top card yet, guard.
            if (_room.TopCard == null)
                return;

            var state = new
            {
                type = "state",
                room = _room.RoomName,
                stateEnum = _room.State.ToString(),
                topCard = new { color = _room.TopCard.Color.ToString(), value = _room.TopCard.Value.ToString() },
                players = _room.Players.Select(p => new
                {
                    name = p.Name,
                    hand = p.Hand.Select(c => new { color = c.Color.ToString(), value = c.Value.ToString() }),
                    calledUno = p.CalledUno
                }),
                currentIndex = _currentIndex,
                clockwise = _clockwise
            };

            string json = JsonConvert.SerializeObject(state);
            byte[] data = Encoding.UTF8.GetBytes(json);

            List<TcpClient> snapshot;
            lock (_clients)
            {
                snapshot = _clients.ToList(); // safe copy
            }

            var toRemove = new List<TcpClient>();

            foreach (var c in snapshot)
            {
                try
                {
                    var stream = c.GetStream();
                    if (stream.CanWrite)
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    else
                    {
                        toRemove.Add(c);
                    }
                }
                catch
                {
                    // mark client for removal if writing fails
                    toRemove.Add(c);
                }
            }

            if (toRemove.Count > 0)
            {
                lock (_clients)
                {
                    foreach (var dead in toRemove)
                    {
                        try { dead.Close(); } catch { /* ignore */ }
                        _clients.Remove(dead);
                    }
                }
            }
        }

    }
}