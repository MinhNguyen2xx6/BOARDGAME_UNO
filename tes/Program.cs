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

        // Thêm property để kiểm tra số lượng
        public int Count => _cards.Count;

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

        // Sửa lỗi Null: Cho phép trả về null (UnoCard?)
        public UnoCard? DrawCard() => _cards.Count > 0 ? _cards.Pop() : null;

        // Set lá đầu là màu xanh, số 0 
        public UnoCard ExtractSpecificCard(UnoColor c, UnoValue v)
        {
            var list = _cards.ToList();
            var targetCard = list.FirstOrDefault(x => x.Color == c && x.Value == v);

            if (targetCard != null)
            {
                list.Remove(targetCard);
                _cards = new Stack<UnoCard>(list);
                return targetCard;
            }
            // Nếu không tìm thấy thì rút đại 1 lá, dùng ! để báo biên dịch là sẽ xử lý null sau
            return DrawCard()!;
        }

        public void Replenish(List<UnoCard> discards)
        {
            if (discards.Count == 0) return;

            Console.WriteLine($"♻ Đang xào lại {discards.Count} lá bài đã đánh vào bộ bài rút...");

            foreach (var c in discards)
            {
                if (c.Value == UnoValue.Wild || c.Value == UnoValue.WildDrawFour)
                {
                    c.Color = UnoColor.Wild;
                }
            }

            var currentCards = _cards.ToList();
            currentCards.AddRange(discards);
            _cards = new Stack<UnoCard>(currentCards);
            Shuffle();
        }
    }

    // ===== PLAYER =====
    public class Player
    {
        public string Name { get; set; }
        public List<UnoCard> Hand { get; set; } = new List<UnoCard>();
        public bool CalledUno { get; set; } = false;
        public Player(string name) { Name = name; }

        // Hàm này nhận vào UnoRoom để rút an toàn
        public void DrawCard(UnoRoom room)
        {
            var card = room.DrawCardSafe();
            if (card != null) Hand.Add(card);
        }
        public void PlayCard(UnoCard card) => Hand.Remove(card);
    }

    // ===== GAME LOGIC =====
    public class UnoGameLogic
    {
        public bool CanPlay(UnoCard card, UnoCard? topCard)
        {
            if (topCard == null) return false;
            return card.Color == topCard.Color ||
                   card.Value == topCard.Value ||
                   card.Color == UnoColor.Wild;
        }

        public void ApplySpecialEffect(UnoCard card, List<Player> players,
            ref int currentIndex, ref bool clockwise, UnoRoom room)
        {
            switch (card.Value)
            {
                case UnoValue.Skip:
                    currentIndex = GetNextPlayerIndex(players.Count, currentIndex, clockwise);
                    break;

                case UnoValue.Reverse:
                    clockwise = !clockwise;
                    if (players.Count == 2)
                    {
                        currentIndex = GetNextPlayerIndex(players.Count, currentIndex, clockwise);
                    }
                    break;

                case UnoValue.DrawTwo:
                    int next2 = GetNextPlayerIndex(players.Count, currentIndex, clockwise);
                    // Sửa lỗi: Dùng room thay vì deck
                    players[next2].DrawCard(room);
                    players[next2].DrawCard(room);
                    currentIndex = next2;
                    break;

                case UnoValue.WildDrawFour:
                    int next4 = GetNextPlayerIndex(players.Count, currentIndex, clockwise);
                    // Sửa lỗi: Dùng room thay vì deck
                    for (int i = 0; i < 4; i++) players[next4].DrawCard(room);
                    currentIndex = next4;
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

        // Sửa lỗi Null: TopCard có thể null khi mới khởi tạo hoặc lỗi
        public UnoCard? TopCard { get; set; }

        public RoomState State { get; set; } = RoomState.Waiting;
        public List<UnoCard> DiscardPile { get; set; } = new List<UnoCard>();

        public UnoRoom(string name) { RoomName = name; }

        public void StartGame()
        {
            State = RoomState.Playing;
            TopCard = Deck.ExtractSpecificCard(UnoColor.Blue, UnoValue.Zero);

            foreach (var p in Players)
                // Sửa lỗi: Truyền 'this' (UnoRoom) thay vì Deck
                for (int i = 0; i < 7; i++) p.DrawCard(this);
        }

        // Sửa lỗi Null: Cho phép trả về null
        public UnoCard? DrawCardSafe()
        {
            if (Deck.Count == 0)
            {
                if (DiscardPile.Count > 0)
                {
                    Deck.Replenish(DiscardPile);
                    DiscardPile.Clear();
                }
                else
                {
                    return null;
                }
            }
            return Deck.DrawCard();
        }

        // Hàm này ít dùng vì đã chuyển logic sang CheckUnoTimeout, nhưng cứ để lại
        public bool LastCardCallUno(Player p)
        {
            if (p.Hand.Count == 1 && !p.CalledUno)
            {
                p.DrawCard(this); // Sửa thành this
                p.DrawCard(this); // Sửa thành this
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

        private async Task CheckUnoTimeout(Player p)
        {
            await Task.Delay(4000);

            lock (_clients)
            {
                if (p.Hand.Count == 1 && !p.CalledUno)
                {
                    Console.WriteLine($"⚠ PHẠT: {p.Name} không hô UNO sau 4s!");
                    p.DrawCard(_room); // Đã đúng (dùng _room)
                    p.CalledUno = false;
                    BroadcastLog($"⚠ {p.Name} bị phạt vì không hô UNO trong 4 giây!");
                    BroadcastState();
                }
                else
                {
                    if (p.Hand.Count > 1) p.CalledUno = false;
                }
            }
        }

        public async Task StartAsync()
        {
            _listener.Start();
            Console.WriteLine("UNO Room Server started.");

            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                lock (_clients) { _clients.Add(client); }
                _ = HandleClientAsync(client);
                Console.WriteLine("Client connected.");
            }
        }

        private void SendStartGame()
        {
            Console.WriteLine("📢 Phát bài cho người chơi...");

            for (int i = 0; i < _room.Players.Count; i++)
            {
                var player = _room.Players[i];
                var client = _clients[i];

                var startMsg = new
                {
                    type = "start",
                    yourName = player.Name,
                    yourIndex = i,
                    topCard = new
                    {
                        // Kiểm tra null cho TopCard
                        color = _room.TopCard?.Color.ToString() ?? "Blue",
                        value = _room.TopCard?.Value.ToString() ?? "Zero"
                    },
                    yourHand = player.Hand.Select(c => new
                    {
                        color = c.Color.ToString(),
                        value = c.Value.ToString()
                    })
                };

                string json = JsonConvert.SerializeObject(startMsg);
                byte[] data = Encoding.UTF8.GetBytes(json);

                try
                {
                    client.GetStream().Write(data, 0, data.Length);
                    Console.WriteLine($"✔ Phát {player.Hand.Count} lá cho {player.Name}");
                }
                catch
                {
                    Console.WriteLine($"❌ Không gửi được bài cho {player.Name}");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            var stream = client.GetStream();
            var buffer = new byte[4096];

            while (client.Connected)
            {
                try
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
                            if (!_room.Players.Any(p => p.Name == name))
                            {
                                _room.Players.Add(new Player(name));
                                Console.WriteLine($"Player joined: {name} ({_room.Players.Count})");
                            }

                            if (_room.Players.Count >= 4 && _room.State == RoomState.Waiting)
                            {
                                Console.WriteLine("=== GAME START ===");
                                _room.StartGame();
                                SendStartGame();
                                BroadcastState();
                            }
                            break;

                        case "play":
                            string? nColor = msg.nextColor != null ? (string)msg.nextColor : null;
                            HandlePlay((string)msg.player,
                                new UnoCard(Enum.Parse<UnoColor>((string)msg.card.color),
                                            Enum.Parse<UnoValue>((string)msg.card.value)),
                                nColor);
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
                catch { break; }
            }

            lock (_clients) { _clients.Remove(client); }
            client.Close();
        }

        private void HandlePlay(string playerName, UnoCard card, string? nextColor)
        {
            try
            {
                if (_room.State != RoomState.Playing) return;
                var current = _room.Players[_currentIndex];
                if (current.Name != playerName) return;

                var logic = new UnoGameLogic();
                // TopCard có thể null (lý thuyết) nhưng thực tế đã init
                if (_room.TopCard == null || !logic.CanPlay(card, _room.TopCard)) return;

                var handCard = current.Hand.FirstOrDefault(c => c.Color == card.Color && c.Value == card.Value);
                if (handCard == null) return;

                // 1. Đánh bài
                current.PlayCard(handCard);
                BroadcastLog($"⚔ {playerName} đánh lá {handCard.Color} {handCard.Value}");

                if (_room.TopCard != null) _room.DiscardPile.Add(_room.TopCard);

                UnoCard newTopCard = new UnoCard(handCard.Color, handCard.Value);

                // 2. Xử lý đổi màu Wild
                if (card.Color == UnoColor.Wild)
                {
                    UnoColor selectedColor = UnoColor.Red;
                    if (!string.IsNullOrEmpty(nextColor) && Enum.TryParse<UnoColor>(nextColor, out var parsedColor))
                    {
                        selectedColor = parsedColor;
                    }
                    newTopCard.Color = selectedColor;

                    string colorName = selectedColor.ToString();
                    BroadcastLog($"🎨 {playerName} chọn màu: {colorName}");
                }

                _room.TopCard = newTopCard;

                // 3. Xử lý hiệu ứng - Truyền _room vào
                logic.ApplySpecialEffect(newTopCard, _room.Players, ref _currentIndex, ref _clockwise, _room);

                // 4. Check UNO / End Game
                if (current.Hand.Count == 0)
                {
                    _room.State = RoomState.Finished;
                    BroadcastLog($"🏆 {playerName} ĐÃ CHIẾN THẮNG!");
                }
                else if (current.Hand.Count == 1)
                {
                    _ = CheckUnoTimeout(current);
                }

                // 5. Chuyển lượt
                if (_room.State != RoomState.Finished)
                {
                    _currentIndex = (_clockwise ? (_currentIndex + 1) : (_currentIndex - 1 + _room.Players.Count)) % _room.Players.Count;
                }

                BroadcastState();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ LỖI CRASH tại HandlePlay: {ex.Message}");
            }
        }

        private void HandleDraw(string playerName)
        {
            var current = _room.Players[_currentIndex];
            if (current.Name != playerName) return;

            // Rút từ room (an toàn)
            current.DrawCard(_room);

            _currentIndex = (_clockwise ? (_currentIndex + 1) : (_currentIndex - 1 + _room.Players.Count)) % _room.Players.Count;
            BroadcastState();
        }

        private void BroadcastState()
        {
            if (_room.TopCard == null) return;

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
            lock (_clients) { snapshot = _clients.ToList(); }

            foreach (var c in snapshot)
            {
                try
                {
                    if (c.Connected) c.GetStream().Write(data, 0, data.Length);
                }
                catch { }
            }
        }

        private void BroadcastLog(string message)
        {
            var logMsg = new { type = "log", message = message };
            string json = JsonConvert.SerializeObject(logMsg);
            byte[] data = Encoding.UTF8.GetBytes(json);

            List<TcpClient> snapshot;
            lock (_clients) { snapshot = _clients.ToList(); }

            foreach (var c in snapshot)
            {
                try
                {
                    if (c.Connected) c.GetStream().Write(data, 0, data.Length);
                }
                catch { }
            }
        }
    }
}