using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace UNO_Client
{
    public partial class FormGame : Form
    {
       
        private string _roomName;
        private List<string> _players;
        public FormGame(string roomName, List<string> players)
        {
            InitializeComponent();
            _roomName = roomName;
            _players = players;
        }
        private System.Windows.Forms.Timer _waitForPlayersTimer;


        private UnoRoom _room;
        private UnoGameLogic _logic;
        private System.Windows.Forms.Timer _pollTimer;
        public class IntentDto
        {
            public string action { get; set; }
            public string player { get; set; }
            public string color { get; set; }
            public string value { get; set; }
        }
        private Player GetMe()
        {
            return _room.Players.FirstOrDefault(p => p.Name == Session.UserEmail);
        }

        private int GetMyIndex()
        {
            int idx = _room.Players.FindIndex(p => p.Name == Session.UserEmail);
            return idx < 0 ? 0 : idx;
        }
        private void btnUno_Click(object sender, EventArgs e)
        {
            var me = GetMe();
            if (me == null) return;
            me.CalledUno = true;
            MessageBox.Show("Bạn đã bấm UNO!");
        }
        private static readonly HttpClient client = new HttpClient();
        private static readonly string firebaseUrl = "https://doan-36be7-default-rtdb.asia-southeast1.firebasedatabase.app";
        private async Task CheckPlayerCount()
        {
            var response = await client.GetAsync($"{firebaseUrl}/rooms/{_roomName}.json?auth={Session.IdToken}");
            if (!response.IsSuccessStatusCode) return;

            string data = await response.Content.ReadAsStringAsync();
            var roomInfo = JsonConvert.DeserializeObject<RoomInfo>(data);
            if (roomInfo == null || roomInfo.Players == null) return;

            if (roomInfo.Players.Count == 4)
            {
                _waitForPlayersTimer.Stop();

                _room.Players.Clear();
                foreach (var name in roomInfo.Players)
                    _room.Players.Add(new Player(name));

                // Nếu là host thì mới gọi StartGame
                string hostName = roomInfo.Players[0]; // giả sử host là player đầu tiên
                if (Session.UserEmail == hostName)
                {
                    await StartGame(); // gọi hàm FormGame.StartGame để push dữ liệu
                }
                else
                {
                    // Client chỉ chờ dữ liệu từ Firebase
                    lblCurrentTurn.Text = "Đang chờ host bắt đầu...";
                    _pollTimer = new System.Windows.Forms.Timer();
                    _pollTimer.Interval = 3000;
                    _pollTimer.Tick += async (s, ev) => await PollFirebase();
                    _pollTimer.Start();
                }
            }
        }

        private async Task PollFirebase()
        {
            try
            {
                // 1. Lấy intents
                var responseIntents = await client.GetAsync($"{firebaseUrl}/rooms/{_roomName}/intents.json?auth={Session.IdToken}");
                if (responseIntents.IsSuccessStatusCode)
                {
                    string data = await responseIntents.Content.ReadAsStringAsync();
                    var intents = JsonConvert.DeserializeObject<Dictionary<string, IntentDto>>(data);

                    if (intents != null)
                    {
                        foreach (var kvp in intents)
                        {
                            string intentKey = kvp.Key;
                            var intent = kvp.Value;
                            string action = intent.action;
                            string player = intent.player;

                            if (action == "PlayCard")
                            {
                                string colorStr = intent.color;
                                string valueStr = intent.value;

                                if (Enum.TryParse<UnoColor>(colorStr, out var colorEnum) &&
                                    Enum.TryParse<UnoValue>(valueStr, out var valueEnum))
                                {
                                    _room.TopCard = new UnoCard(colorEnum, valueEnum);

                                    // Áp dụng hiệu ứng đặc biệt và cập nhật lượt
                                    _logic.ApplySpecialEffect(_room.TopCard, _room.Players,
                                                              ref _room.CurrentPlayerIndex, ref _room.IsClockwise);

                                    RenderHands();
                                    UpdateTurnLabel();

                                    // Đồng bộ currentTurn lên Firebase
                                    var turnObj = new { currentTurn = _room.CurrentPlayerIndex };
                                    string jsonTurn = JsonConvert.SerializeObject(turnObj);
                                    var contentTurn = new StringContent(jsonTurn, Encoding.UTF8, "application/json");
                                    await client.PutAsync($"{firebaseUrl}/rooms/{_roomName}/currentTurn.json?auth={Session.IdToken}", contentTurn);
                                }
                            }
                            else if (action == "DrawOne")
                            {
                                var p = _room.Players.FirstOrDefault(x => x.Name == player);
                                if (p != null)
                                {
                                    p.DrawCard(_room.Deck);
                                    RenderHands();
                                }
                            }
                            else if (action == "UNO")
                            {
                                var p = _room.Players.FirstOrDefault(x => x.Name == player);
                                if (p != null)
                                {
                                    p.CalledUno = true;
                                    MessageBox.Show($"{player} đã bấm UNO!");
                                }
                            }

                            // Xóa intent sau khi xử lý
                            await client.DeleteAsync($"{firebaseUrl}/rooms/{_roomName}/intents/{intentKey}.json?auth={Session.IdToken}");
                        }
                    }
                }

                // 2. Lấy currentTurn
                var responseTurn = await client.GetAsync($"{firebaseUrl}/rooms/{_roomName}/currentTurn.json?auth={Session.IdToken}");
                if (responseTurn.IsSuccessStatusCode)
                {
                    string turnData = await responseTurn.Content.ReadAsStringAsync();
                    var turnObj = JsonConvert.DeserializeObject<Dictionary<string, int>>(turnData);
                    if (turnObj != null && turnObj.ContainsKey("currentTurn"))
                    {
                        _room.CurrentPlayerIndex = turnObj["currentTurn"];
                        UpdateTurnLabel();
                    }
                }

                // 3. Lấy topCard
                var responseCard = await client.GetAsync($"{firebaseUrl}/rooms/{_roomName}/topCard.json?auth={Session.IdToken}");
                if (responseCard.IsSuccessStatusCode)
                {
                    string cardData = await responseCard.Content.ReadAsStringAsync();
                    var cardObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(cardData);
                    if (cardObj != null && cardObj.TryGetValue("color", out var colorStr) && cardObj.TryGetValue("value", out var valueStr))
                    {
                        if (Enum.TryParse<UnoColor>(colorStr, out var colorEnum) &&
                            Enum.TryParse<UnoValue>(valueStr, out var valueEnum))
                        {
                            _room.TopCard = new UnoCard(colorEnum, valueEnum);
                            RenderHands();
                        }
                    }
                }

                // 4. Lấy winner
                var responseWinner = await client.GetAsync($"{firebaseUrl}/rooms/{_roomName}/winner.json?auth={Session.IdToken}");
                if (responseWinner.IsSuccessStatusCode)
                {
                    string winData = await responseWinner.Content.ReadAsStringAsync();
                    var winObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(winData);
                    if (winObj != null && winObj.ContainsKey("winner"))
                    {
                        string winnerName = winObj["winner"];
                        MessageBox.Show($"Người thắng là: {winnerName}");
                        _pollTimer.Stop(); // dừng game
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi để debug, tránh crash
                Console.WriteLine("PollFirebase error: " + ex.Message);
            }
        }


        private async void btnPlay_Click(object sender, EventArgs e)
        {
            var me = GetMe();
            if (me == null || lstPlayerBottom.SelectedItem == null) return;

            string selected = lstPlayerBottom.SelectedItem.ToString();
            var card = me.Hand.FirstOrDefault(c => c.ToString() == selected);
            if (card == null || !_logic.CanPlay(card, _room.TopCard))
            {
                MessageBox.Show("Bạn không thể đánh lá này!");
                return;
            }

            // Đánh lá bài
            me.PlayCard(card);

            // Wild chọn màu
            if (card.Value == UnoValue.Wild || card.Value == UnoValue.WildDrawFour)
            {
                using (var picker = new ColorPickerForm())
                {
                    if (picker.ShowDialog() == DialogResult.OK)
                        _room.TopCard = new UnoCard(picker.SelectedColor, card.Value);
                }
            }
            else
            {
                _room.TopCard = card;
            }

            // Áp dụng hiệu ứng và cập nhật lượt
            _logic.ApplySpecialEffect(_room.TopCard, _room.Players,
                                      ref _room.CurrentPlayerIndex, ref _room.IsClockwise);

            RenderHands();
            UpdateTurnLabel();

            // Gửi intent
            var intent = new
            {
                action = "PlayCard",
                player = me.Name,
                color = _room.TopCard.Color.ToString(),
                value = _room.TopCard.Value.ToString()
            };
            string json = JsonConvert.SerializeObject(intent);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync($"{firebaseUrl}/rooms/{_roomName}/intents.json?auth={Session.IdToken}", content);

            // UNO phạt
            if (me.Hand.Count == 1 && !me.CalledUno)
            {
                MessageBox.Show($"{me.Name} còn 1 lá nhưng chưa bấm UNO → phạt rút 2 lá!");
                me.DrawCard(_room.Deck);
                me.DrawCard(_room.Deck);
            }

            // Thắng
            if (me.Hand.Count == 0)
            {
                var winData = new { winner = me.Name };
                string jsonWin = JsonConvert.SerializeObject(winData);
                var contentWin = new StringContent(jsonWin, Encoding.UTF8, "application/json");
                await client.PutAsync($"{firebaseUrl}/rooms/{_roomName}/winner.json?auth={Session.IdToken}", contentWin);

                MessageBox.Show($"{me.Name} đã thắng!");
                _pollTimer?.Stop();
            }
        }
        private async void btnDraw_Click(object sender, EventArgs e)
        {

            var me = GetMe();
            if (me == null) return;

            me.DrawCard(_room.Deck);
            RenderHands();

            var intent = new { action = "DrawOne", player = me.Name };
            string json = JsonConvert.SerializeObject(intent);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync($"{firebaseUrl}/rooms/{_roomName}/intents.json?auth={Session.IdToken}", content);
        }
        private void btnExitRoom_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Exit!");
        }
        private void UpdateTurnLabel()
        {
            if (_room.Players.Count > 0)
            {
                var current = _room.Players[_room.CurrentPlayerIndex];
                lblCurrentTurn.Text = $"Lượt: {current.Name}";
            }
        }
        private void FormGame_Load(object sender, EventArgs e)
        {
            _room = new UnoRoom(_roomName);
            _logic = new UnoGameLogic();

            foreach (var name in _players)
                _room.Players.Add(new Player(name));

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            // Khởi động timer chờ đủ người chơi
            _waitForPlayersTimer = new System.Windows.Forms.Timer();
            _waitForPlayersTimer.Interval = 3000; // kiểm tra mỗi 3 giây
            _waitForPlayersTimer.Tick += async (s, ev) => await CheckPlayerCount();
            _waitForPlayersTimer.Start();

        }
        private async Task StartGame()
        {
            _room = new UnoRoom(_roomName);
            _logic = new UnoGameLogic();

            foreach (var name in _players)
                _room.Players.Add(new Player(name));

            if (_players.Count == 4)
            {
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;

                _room.StartGame();
                RenderHands();
                UpdateTurnLabel();

                // Đồng bộ TopCard lên Firebase
                var topCardObj = new { color = _room.TopCard.Color.ToString(), value = _room.TopCard.Value.ToString() };
                string jsonTop = JsonConvert.SerializeObject(topCardObj);
                var contentTop = new StringContent(jsonTop, Encoding.UTF8, "application/json");
                await client.PutAsync($"{firebaseUrl}/rooms/{_roomName}/topCard.json?auth={Session.IdToken}", contentTop);

                // Đồng bộ currentTurn
                var turnObj = new { currentTurn = _room.CurrentPlayerIndex };
                string jsonTurn = JsonConvert.SerializeObject(turnObj);
                var contentTurn = new StringContent(jsonTurn, Encoding.UTF8, "application/json");
                await client.PutAsync($"{firebaseUrl}/rooms/{_roomName}/currentTurn.json?auth={Session.IdToken}", contentTurn);

                // Bắt đầu polling Firebase
                _pollTimer = new System.Windows.Forms.Timer();
                _pollTimer.Interval = 3000;
                _pollTimer.Tick += async (s, ev) => await PollFirebase();
                _pollTimer.Start();
            }
            else
            {
                lblCurrentTurn.Text = "Đang chờ người chơi...";
                _waitForPlayersTimer = new System.Windows.Forms.Timer();
                _waitForPlayersTimer.Interval = 3000;
                _waitForPlayersTimer.Tick += async (s, ev) => await CheckPlayerCount();
                _waitForPlayersTimer.Start();
            }
        }

        private void RenderHands()
        {
            if (_room?.Players == null || _room.Players.Count == 0) return;

            // Xác định mình bằng email
            int myIndex = _room.Players.FindIndex(p => p.Name == Session.UserEmail);
            if (myIndex < 0) return; // không fallback về 0 nữa

            Player GetPlayerByOffset(int offset)
            {
                int idx = (myIndex + offset) % _room.Players.Count;
                return _room.Players[idx];
            }

            // Bottom: chính mình
            var me = GetPlayerByOffset(0);
            lstPlayerBottom.Items.Clear();
            foreach (var c in me.Hand)
                lstPlayerBottom.Items.Add($"{c.Color} {c.Value}");
            bot.Text = me.Name;

            // Right
            if (_room.Players.Count > 1)
            {
                var rightPlayer = GetPlayerByOffset(1);
                lstPlayerRight.Items.Clear();
                lstPlayerRight.Items.Add($"Số lá: {rightPlayer.Hand.Count}");
                right.Text = rightPlayer.Name;
            }

            // Top
            if (_room.Players.Count > 2)
            {
                var topPlayer = GetPlayerByOffset(2);
                lstPlayerTop.Items.Clear();
                lstPlayerTop.Items.Add($"Số lá: {topPlayer.Hand.Count}");
                top.Text = topPlayer.Name;
            }

            // Left
            if (_room.Players.Count > 3)
            {
                var leftPlayer = GetPlayerByOffset(3);
                lstPlayerLeft.Items.Clear();
                lstPlayerLeft.Items.Add($"Số lá: {leftPlayer.Hand.Count}");
                left.Text = leftPlayer.Name;
            }

            // Top card
            if (_room.TopCard != null)
                lblTopCard.Text = $"Top Card: {_room.TopCard.Color} {_room.TopCard.Value}";
        }



    }


    public enum UnoColor
    {
        Red,
        Blue,
        Green,
        Yellow,
        Wild
    }

    public enum UnoValue
    {
        Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine,
        Skip, Reverse, DrawTwo,
        Wild, WildDrawFour
    }

    //Class lá bài
    public class UnoCard
    {
        public UnoColor Color { get; set; }
        public UnoValue Value { get; set; }

        public UnoCard(UnoColor color, UnoValue value)
        {
            Color = color;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Color} {Value}";
        }
    }

    //Class cho bộ bài
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

        public UnoCard DrawCard()
        {
            return _cards.Count > 0 ? _cards.Pop() : null;
        }
    }

    //Class dựng cho người chơi
    public class Player
    {
        public string Name { get; set; }
        public List<UnoCard> Hand { get; set; }

        public Player(string name)
        {
            Name = name;
            Hand = new List<UnoCard>();
        }

        public void DrawCard(UnoDeck deck)
        {
            var card = deck.DrawCard();
            if (card != null)
                Hand.Add(card);
        }

        public void PlayCard(UnoCard card)
        {
            Hand.Remove(card);
        }
        public bool CalledUno { get; set; } = false; //(VQ) Đánh dấu player đã nhấn nút uno hay chưa.
    }



    //Class gamelogic(luật chơi)
    public class UnoGameLogic
    {
        public bool CanPlay(UnoCard card, UnoCard topCard)
        {
            return card.Color == topCard.Color ||
                   card.Value == topCard.Value ||
                   card.Color == UnoColor.Wild;
        }

        public void ApplySpecialEffect(UnoCard card, List<Player> players, ref int currentIndex, ref bool isClockwise)
        {
            switch (card.Value)
            {
                case UnoValue.Skip:
                    currentIndex = GetNextPlayerIndex(players.Count, currentIndex, isClockwise);
                    break;
                case UnoValue.Reverse:
                    isClockwise = !isClockwise;
                    break;
                case UnoValue.DrawTwo:
                    int nextIndex = GetNextPlayerIndex(players.Count, currentIndex, isClockwise);
                    players[nextIndex].DrawCard(new UnoDeck());
                    players[nextIndex].DrawCard(new UnoDeck());
                    break;
                case UnoValue.WildDrawFour:
                    int next = GetNextPlayerIndex(players.Count, currentIndex, isClockwise);
                    for (int i = 0; i < 4; i++)
                        players[next].DrawCard(new UnoDeck());
                    break;
            }
        }

        public int GetNextPlayerIndex(int totalPlayers, int currentIndex, bool isClockwise)
        {
            return isClockwise
                ? (currentIndex + 1) % totalPlayers
                : (currentIndex - 1 + totalPlayers) % totalPlayers;
        }
        public bool HandleDrawOne(Player player, UnoRoom room)
        {
            foreach (var card in player.Hand)
            {
                if (CanPlay(card, room.TopCard)) // (TP) Kiểm tra xem trên tay có card nào có thể đánh được hay không và trả về true có thể đánh hoặc rút thêm bài)
                    return true; 
            }

            var drawn = room.Deck.DrawCard();
            if (drawn != null)
                player.Hand.Add(drawn);        //(TP) Nếu không có lá phù hợp để đánh thì rút 1 lá sau đó kiểm tra có đánh được hay không

            if (CanPlay(drawn, room.TopCard))
            {
                return true;
            }

            return false;
        }

    }


    public enum RoomState
    {
        Waiting,
        Playing,
        Finished
    }

    //Class phòng chơi
    public class UnoRoom
    {
        public string RoomName { get; set; }
        public List<Player> Players { get; set; }
        public UnoDeck Deck { get; set; }
        public UnoCard TopCard { get; set; }
        public RoomState State { get; set; }

        public int CurrentPlayerIndex = 0;

        public bool IsClockwise = true;

        public UnoRoom(string roomName)
        {
            RoomName = roomName;
            Players = new List<Player>();
            Deck = new UnoDeck();
            State = RoomState.Waiting;
        }

        public void StartGame() //Hàm chia bài
        {
            State = RoomState.Playing;
            TopCard = Deck.DrawCard();

            foreach (var player in Players)
            {
                for (int i = 0; i < 7; i++)
                    player.DrawCard(Deck);
            }
            
        }
        public bool LastCardCallUno(Player player)
        {
            int cardCount = player.Hand.Count;
            
            if (player.Hand.Count == 1)
            {
                if (!player.CalledUno)  // (VQ) Người này còn 1 lá nhưng chưa bấm UNO.
                {
                   
                    // Nhiệm vụ phạt code vào đây.

                    MessageBox.Show($"{player.Name} còn 1 lá bài nhưng chưa bấm UNO!");
                    player.Hand.Add(Deck.DrawCard());
                    player.Hand.Add(Deck.DrawCard()); // (TP) phạt rút 2 lá 
                }
                else
                {
                    MessageBox.Show($"{player.Name} hợp lệ vì đã bấm UNO.");//(Tp) thông báo hợp lệ
                }
            }
            else if (player.Hand.Count == 0) // (VQ) Người này đánh hết bài. Kết thúc game.
            {
                
                MessageBox.Show($"{player.Name} đã đánh hết bài!");
                State = RoomState.Finished; //(TP) Gọi hàm kết thúc
                return true; // (VQ) Trả về true để người khác biết cần kết thúc game.
            }

            
            player.CalledUno = false; // (VQ) Reset trạng thái UNO sau khi đánh bài.

            

            return false;
        }
        public void PlayerTurn(UnoGameLogic logic)
        {
            var player = Players[CurrentPlayerIndex];

            bool canPlay = logic.HandleDrawOne(player, this);

            if (!canPlay)
            {
                // (TP) Không thể đánh -> qua lượt
                CurrentPlayerIndex = logic.GetNextPlayerIndex(
                    Players.Count,
                    CurrentPlayerIndex,
                    IsClockwise
                );
            }
        }

        public void EndGame()
        {
            State = RoomState.Finished;
        }
        // Nút Đánh bài
        private void RenderHands()
        {

        }

    }





}
