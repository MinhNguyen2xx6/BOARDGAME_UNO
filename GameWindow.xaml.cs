using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace UNO_Client_WPF
{
    public partial class GameWindow : Window
    {
        private readonly string _roomName;
        private readonly string _playerName;
        private readonly NetworkClient _client;

        private ToggleButton? _selectedCard = null;

        private readonly Dictionary<string, BitmapImage> _imageCache = new();

        public GameWindow(string roomName, string playerName)
        {
            InitializeComponent();
            _roomName = roomName;
            _playerName = playerName;

            _client = new NetworkClient();
            _client.OnStateUpdate += HandleStateUpdate;  //(TP) nơi khởi tạo các biến cần thiết và bắt đầu kết nối tới server
            _client.OnStart += HandleStart;

            ConnectToServer();
        }


        private async void ConnectToServer()
        {
            try
            {
                await _client.ConnectAsync("127.0.0.1", 5000, _playerName);
                MessageBox.Show("Đã kết nối tới server!");
            }
            catch (Exception ex)                                                // (TP) hàm kết nối
            {
                MessageBox.Show("Không thể kết nối: " + ex.Message);
            }
        }

        // Map enum -> filename
        private string MapCardToAsset(string color, string value)
        {
            if (value == "Wild") return "Wild.png";
            if (value == "WildDrawFour") return "WildDrawFour.png";

            var digits = new Dictionary<string, string>
            {
                { "Zero", "0" }, { "One", "1" }, { "Two", "2" }, { "Three", "3" },
                { "Four", "4" }, { "Five", "5" }, { "Six", "6" }, { "Seven", "7" },         //(TP) tại vì server gửi các tín hiệu lá bài tên khác với lưu trong file nên phải chuyển đổi về
                { "Eight", "8" }, { "Nine", "9" }
            };

            if (digits.TryGetValue(value, out var d))
                return $"{color}_{d}.png";

            if (value == "DrawTwo") return $"{color}_Draw.png"; // sửa theo tên file thật của bạn
            if (value == "Skip") return $"{color}_Skip.png";
            if (value == "Reverse") return $"{color}_Reverse.png";

            return $"{color}_{value}.png";
        }

        private BitmapImage LoadAssetImage(string fileName)
        {
            if (_imageCache.TryGetValue(fileName, out var cached)) return cached;
            try
            {
                var uri = new Uri($"pack://application:,,,/Assets/{fileName}", UriKind.Absolute);
                var bmp = new BitmapImage(uri);
                _imageCache[fileName] = bmp;
                return bmp;
            }
            catch
            {
                var fallback = new BitmapImage(new Uri("pack://application:,,,/Assets/uno-classic-logo.jpg", UriKind.Absolute));        // (TP) hàm này là để load ảnh 1 cách an toàn
                _imageCache[fileName] = fallback;
                return fallback;
            }
        }

        private void HandleStart(dynamic msg)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                string topColor = (string)msg.topCard.color;
                string topValue = (string)msg.topCard.value;
                imgCurrentCard.Source = LoadAssetImage(MapCardToAsset(topColor, topValue));             //(TP) Hàm này dùng để xử lý lần phát bài ban đầu và hiện lá bài ở giauwx sân

                HandPanel.Children.Clear();
                foreach (var c in msg.yourHand)
                {
                    AddCardToHand((string)c.color, (string)c.value);
                }

                CheckUnoStatus();
            }));
        }
        private void HandleStateUpdate(dynamic msg)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Lá trên bàn
                string topColor = (string)msg.topCard.color;
                string topValue = (string)msg.topCard.value;
                imgCurrentCard.Source = LoadAssetImage(MapCardToAsset(topColor, topValue));

                // Danh sách players
                var players = (IEnumerable<dynamic>)msg.players;

                // Chính mình
                var me = players.FirstOrDefault(p => p.name == _playerName);                    // (TP) Hàm này là để liên tục update khi nhận được tín hiệu của server
                if (me != null)
                {
                    HandPanel.Children.Clear();
                    foreach (var c in me.hand)
                    {
                        AddCardToHand((string)c.color, (string)c.value);
                    }

                }

                // Các đối thủ
                var others = players.Where(p => p.name != _playerName).ToList();
                if (others.Count >= 3)
                {
                    dynamic top = others[0];
                    dynamic left = others[1];
                    dynamic right = others[2];

                    // Top
                    txtBotTopName.Text = top.name;
                    BotTopHand.Children.Clear();
                    int topCount = ((IEnumerable<dynamic>)top.hand).Count();
                    for (int i = 0; i < topCount; i++)
                        BotTopHand.Children.Add(CreateBackImage());

                    // Left
                    txtBotLeftName.Text = left.name;
                    BotLeftHand.Children.Clear();
                    int leftCount = ((IEnumerable<dynamic>)left.hand).Count();          //(TP) Phần này là phần hiện số bài của đối thủ bằng bài úp nma bị fail nhớ fix lại
                    for (int i = 0; i < leftCount; i++)
                        BotLeftHand.Children.Add(CreateBackImage());

                    // Right
                    txtBotRightName.Text = right.name;
                    BotRightHand.Children.Clear();
                    int rightCount = ((IEnumerable<dynamic>)right.hand).Count();
                    for (int i = 0; i < rightCount; i++)
                        BotRightHand.Children.Add(CreateBackImage());
                }

                // Hiển thị lượt hiện tại
                int currentIndex = (int)msg.currentIndex;
                var currentPlayer = players.ElementAt(currentIndex);
                string turnName = currentPlayer.name == _playerName ? "Bạn" : currentPlayer.name;
                txtTurnInfo.Text = $"Lượt: {turnName}";
                // Kiểm tra trạng thái UNO
                CheckUnoStatus();
            }));
        }

        // Helper tạo lá úp
        private Image CreateBackImage()
        {
            return new Image
            {
                Width = 40,
                Height = 60,
                Source = new BitmapImage(new Uri("/Assets/Back.png", UriKind.Relative)), //(TP) hàm để tạo bài úp
                Margin = new Thickness(2, 0, 2, 0)
            };
        }




        private void AddCardToHand(string color, string value)
        {
            string fileName = MapCardToAsset(color, value);

            var btnCard = new ToggleButton
            {
                Tag = $"{color}_{value}",
                Width = 110,
                Height = 160,
                Margin = new Thickness(5, 0, 0, 0),
                Style = TryFindResource("CardStyle") as Style
            };

            var img = new System.Windows.Controls.Image
            {
                Width = 110,
                Height = 160,
                Stretch = System.Windows.Media.Stretch.UniformToFill,     //(TP) Hàm thêm bài lên tay bằng cách tạo các button rồi thêm ảnh vào
                Source = LoadAssetImage(fileName)
            };

            btnCard.Content = img;

            btnCard.Checked += (s, e) =>
            {
                if (_selectedCard is ToggleButton old && !ReferenceEquals(old, btnCard))
                    old.IsChecked = false;
                _selectedCard = btnCard;
            };
            btnCard.Unchecked += (s, e) =>
            {
                if (ReferenceEquals(_selectedCard, btnCard))
                    _selectedCard = null;
            };

            HandPanel.Children.Add(btnCard);
        }

        private void btnPlayCard_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCard == null)
            {
                MessageBox.Show("Vui lòng chọn một lá bài trước!");
                return;
            }

            var parts = _selectedCard.Tag.ToString().Split('_');
            if (parts.Length < 2)
            {
                MessageBox.Show("Thẻ bài không hợp lệ.");
                return;
            }

            string color = parts[0];
            string value = parts[1];

            _client.Send(new
            {
                type = "play",
                player = _playerName,
                card = new { color, value }
            });
        }

        private void btnDrawPile_Click(object sender, RoutedEventArgs e)
        {
            _client.Send(new { type = "draw", player = _playerName });
        }

        private void btnUno_Click(object sender, RoutedEventArgs e)
        {
            _client.Send(new { type = "uno", player = _playerName });
        }

        private void CheckUnoStatus()
        {
            btnUno.Visibility = HandPanel.Children.Count == 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btn_quit_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }
}
