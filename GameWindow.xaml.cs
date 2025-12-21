using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media; 
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using UNO_Client_WPF.Helpers;
using UNO_Client_WPF.Models;

namespace UNO_Client_WPF
{
    public partial class GameWindow : Window
    {
        private readonly string _roomName;
        private readonly string _playerName;
        private readonly NetworkClient _client;

        private ToggleButton? _selectedCard = null;

        private readonly Dictionary<string, BitmapImage> _imageCache = new();
        private bool _hasLeftRoom = false;
        private static readonly HttpClient http = new HttpClient();
        private const string FirebaseUrl = "https://doan-36be7-default-rtdb.asia-southeast1.firebasedatabase.app";

        private bool _isAnimatingPlay = false;  //(VQ) Biến cờ để ngăn Server cập nhật bài khi đang chạy hiệu ứng đánh bài

        public GameWindow(string roomName, string playerName)
        {
            InitializeComponent();
            _roomName = roomName;
            _playerName = playerName;

            _client = new NetworkClient();
            _client.OnStateUpdate += HandleStateUpdate; //(TP) nơi khởi tạo các biến cần thiết và bắt đầu kết nối tới server
            _client.OnStart += HandleStart;
            this.Closing += GameWindow_Closing;
            ConnectToServer();
        }

        private async void ConnectToServer()
        {
            try
            {
                await _client.ConnectAsync("127.0.0.1", 5000, _playerName);
                MessageBox.Show("Đã kết nối tới server!");
            }
            catch (Exception ex)                         // (TP) hàm kết nối
            {
                MessageBox.Show("Không thể kết nối: " + ex.Message);
            }
        }

        // Map enum -> filename
        private string MapCardToAsset(string color, string value)
        {
            // (VQ) Xử lý các lá Wild
            if (value.Contains("WildDrawFour")) return "WildDrawFour.png";
            if (value.Contains("Wild")) return "Wild.png";

            var digits = new Dictionary<string, string>
            {
                { "Zero", "0" }, { "One", "1" }, { "Two", "2" }, { "Three", "3" },
                { "Four", "4" }, { "Five", "5" }, { "Six", "6" }, { "Seven", "7" },  //(TP) tại vì server gửi các tín hiệu lá bài tên khác với lưu trong file nên phải chuyển đổi về
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
                var fallback = new BitmapImage(new Uri("pack://application:,,,/Assets/uno-classic-logo.jpg", UriKind.Absolute));     // (TP) hàm này là để load ảnh 1 cách an toàn
                _imageCache[fileName] = fallback;
                return fallback;
            }
        }
       
        private async void HandleStart(dynamic msg)
        {
            // (VQ) Sử dụng async/await
            await Dispatcher.InvokeAsync(() =>
            {
                string topColor = (string)msg.topCard.color;
                string topValue = (string)msg.topCard.value;
                imgCurrentCard.Source = LoadAssetImage(MapCardToAsset(topColor, topValue)); //(TP) Hàm này dùng để xử lý lần phát bài ban đầu và hiện lá bài ở giauwx sân
                
                HandPanel.Children.Clear();
            });

            foreach (var c in msg.yourHand)
            {
                string color = (string)c.color;
                string value = (string)c.value;
                BitmapImage img = LoadAssetImage(MapCardToAsset(color, value));  // (VQ) Chuẩn bị sẵn ảnh

                await Dispatcher.InvokeAsync(() =>
                {
                    // (VQ) Chạy hiệu ứng bay quân bài
                    AnimateCard(btnDrawPile, HandPanel, img, () =>
                    {
                        // (VQ) Sau khi xong hiệu ứng add vào tay
                        AddCardToHand(color, value);
                        CheckUnoStatus();
                    });
                });
                await Task.Delay(500); // (VQ) Có độ trễ giữa các lần chia bài
            }
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
                var me = players.FirstOrDefault(p => p.name == _playerName);    // (TP) Hàm này là để liên tục update khi nhận được tín hiệu của server

                if (me != null && !_isAnimatingPlay)
                {
                    // (VQ) Kiểm tra số lượng bài sever gửi về so với đang hiện trên màn hình
                    var serverHand = (IEnumerable<dynamic>)me.hand;
                    int serverCount = serverHand.Count();
                    int currentUICount = HandPanel.Children.Count;

                    if (serverCount > currentUICount) // (VQ) Trường hợp rút bài
                    {
                        // (VQ) Lấy thông tin lá bài mới nhất 
                        var newCardData = serverHand.Last();
                        string color = (string)newCardData.color;
                        string value = (string)newCardData.value;
                        BitmapImage img = LoadAssetImage(MapCardToAsset(color, value));

                        _isAnimatingPlay = true; // (VQ) Khóa để không bị cập nhật chồng chéo

                        // (VQ) Chạy animation
                        AnimateCard(btnDrawPile, HandPanel, img, () =>
                        {
                            // (VQ) Sau khi xong hiệu ứng mới đồng bộ
                            HandPanel.Children.Clear();
                            foreach (var c in serverHand)
                            {
                                AddCardToHand((string)c.color, (string)c.value);
                            }
                            _isAnimatingPlay = false;
                            CheckUnoStatus();
                        });
                    }
                    else if (serverCount < currentUICount) //(VQ) Trường hợp đánh bài hoặc mất bài
                    {
                        HandPanel.Children.Clear();
                        foreach (var c in serverHand)
                        {
                            AddCardToHand((string)c.color, (string)c.value);
                        }
                    }
                }

                // (VQ) Các đối thủ (Dùng UpdateOpponent)
                var others = players.Where(p => p.name != _playerName).ToList();
                if (others.Count >= 1) UpdateOpponentHand(BotTopHand, txtBotTopName, others[0]);
                if (others.Count >= 2) UpdateOpponentHand(BotLeftHand, txtBotLeftName, others[1]);
                if (others.Count >= 3) UpdateOpponentHand(BotRightHand, txtBotRightName, others[2]);

                // Hiển thị lượt hiện tại
                int currentIndex = (int)msg.currentIndex;
                var currentPlayer = players.ElementAt(currentIndex);
                txtTurnInfo.Text = $"Lượt: {(currentPlayer.name == _playerName ? "BẠN" : currentPlayer.name)}";
                // Kiểm tra trạng thái UNO
                CheckUnoStatus();
            }));
        }

        // (VQ) Hàm UpdateOpponentHand
        private void UpdateOpponentHand(Panel panel, TextBlock nameLabel, dynamic playerData)
        {
            nameLabel.Text = playerData.name;
            panel.Children.Clear();
            int count = ((IEnumerable<dynamic>)playerData.hand).Count();
            for (int i = 0; i < count; i++)
            {
                var img = new Image
                {
                    Width = 60,
                    Height = 90,
                    Source = LoadAssetImage("Deck.png"),
                    Margin = new Thickness(3, 0, 3, 0) // (VQ) Hiệu ứng xếp chồng
                };
                panel.Children.Add(img);
                var border = new Border
                {
                    Width = img.Width,
                    Height = img.Height,
                    BorderBrush = Brushes.White,
                    BorderThickness = new Thickness(2),
                    Child = img,
                    CornerRadius = new CornerRadius(5)
                };
                panel.Children.Add(border);
            }
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

        private void AnimateCard(FrameworkElement sourceUI, FrameworkElement targetUI, BitmapImage cardImg, Action onComplete)
        {
            this.UpdateLayout();

            // Lấy tâm của source và target
            Point startPos = sourceUI.TranslatePoint(
                new Point(sourceUI.ActualWidth / 2, sourceUI.ActualHeight / 2), this);
            Point endPos = targetUI.TranslatePoint(
                new Point(targetUI.ActualWidth / 2, targetUI.ActualHeight / 2), this);

            // Nếu target là HandPanel thì dịch thêm chút để không đè lên nhau
            if (targetUI == HandPanel)
            {
                endPos.X += 30; // dịch sang phải một chút
                endPos.Y -= 20; // dịch lên một chút
            }

            Image animatedCard = new Image
            {
                Source = cardImg,
                Width = 110,
                Height = 160,
                RenderTransformOrigin = new Point(0.5, 0.5),
                IsHitTestVisible = false
            };

            Canvas.SetLeft(animatedCard, startPos.X - animatedCard.Width / 2);
            Canvas.SetTop(animatedCard, startPos.Y - animatedCard.Height / 2);
            AnimationLayer.Children.Add(animatedCard);

            Duration duration = new Duration(TimeSpan.FromMilliseconds(700));// (VQ) Tốc độ bay khi rút bài
            DoubleAnimation animX = new DoubleAnimation(endPos.X - animatedCard.Width / 2, duration)
            { EasingFunction = new PowerEase { Power = 2 } };
            DoubleAnimation animY = new DoubleAnimation(endPos.Y - animatedCard.Height / 2, duration)
            { EasingFunction = new PowerEase { Power = 2 } };

            // (VQ) Bài xoay 360 độ khi bay về tay
            DoubleAnimation animRotate = new DoubleAnimation(0, 360, duration);

            animatedCard.RenderTransform = new RotateTransform();

            // (VQ) Làm các animation hoạt động cùng 1 lúc
            Storyboard sb = new Storyboard();
            Storyboard.SetTarget(animX, animatedCard);
            Storyboard.SetTargetProperty(animX, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTarget(animY, animatedCard);
            Storyboard.SetTargetProperty(animY, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTarget(animRotate, animatedCard);
            Storyboard.SetTargetProperty(animRotate, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));

            sb.Children.Add(animX);
            sb.Children.Add(animY);
            sb.Children.Add(animRotate);

            sb.Completed += (s, e) =>
            {
                AnimationLayer.Children.Remove(animatedCard);// (VQ) Xóa lá bài ảo
                onComplete?.Invoke();// (VQ) Gọi hành động thêm bài thật vào tay
            };
            sb.Begin();
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

            _isAnimatingPlay = true; // (VQ) Khóa cập nhật từ server

            // (VQ) Thực hiện Animation
            var imgControl = _selectedCard.Content as Image;
            BitmapImage imgSource = (BitmapImage)imgControl.Source;

            var cardToAnimate = _selectedCard; // (VQ) Lưu biến tạm

            cardToAnimate.Opacity = 0;
            cardToAnimate.IsHitTestVisible = false; //(VQ) Ngăn người dùng click vào khoảng trống

            AnimateCard(cardToAnimate, imgCurrentCard, imgSource, () =>
            {
                HandPanel.Children.Remove(cardToAnimate);
                imgCurrentCard.Source = imgSource;
                _selectedCard = null;
                _isAnimatingPlay = false;


                _client.Send(new
                {
                    type = "play",
                    player = _playerName,
                    card = new { color, value }
                });
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

        private async void btn_quit_Click(object sender, RoutedEventArgs e)
        {
            btn_quit.IsEnabled = false;
            await LeaveRoomAsync();//Out phong
            LobbyWindow lobby = new LobbyWindow();
            lobby.Show();//Quay ve Lobby
            this.Close();
        }

        private async Task LeaveRoomAsync()
        {
            if (_hasLeftRoom) return;
            _hasLeftRoom = true;

            try
            {
                var res = await http.GetAsync(
                    $"{FirebaseUrl}/rooms/{_roomName}.json?auth={Session.IdToken}"
                );

                if (!res.IsSuccessStatusCode) return;

                var json = await res.Content.ReadAsStringAsync();
                var room = JsonConvert.DeserializeObject<RoomInfo>(json);

                if (room == null || room.Players == null) return;

                room.Players.Remove(_playerName);

                if (room.Players.Count == 0)
                {
                    await http.DeleteAsync(
                        $"{FirebaseUrl}/rooms/{_roomName}.json?auth={Session.IdToken}"
                    );
                }
                else
                {
                    var updated = JsonConvert.SerializeObject(room);
                    await http.PutAsync(
                        $"{FirebaseUrl}/rooms/{_roomName}.json?auth={Session.IdToken}",
                        new StringContent(updated, Encoding.UTF8, "application/json")
                    );
                }
            }
            catch { }
        }

        private async void GameWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await LeaveRoomAsync();
        }

    }
}