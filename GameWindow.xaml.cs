using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Windows.Media; 
using System.Windows.Media.Animation;

namespace UNO_Client_WPF
{
    public partial class GameWindow : Window
    {
        private readonly string _roomName;
        private readonly string _playerName;
        private readonly NetworkClient _client;

        private ToggleButton? _selectedCard = null;

        private readonly Dictionary<string, BitmapImage> _imageCache = new();

        
        private bool _isAnimatingPlay = false;  //(VQ) Biến cờ để ngăn Server cập nhật bài khi đang chạy hiệu ứng đánh bài
        private dynamic? _pendingState = null; // 🔥 Buffer state mới nhất

        public GameWindow(string roomName, string playerName)
        {
            InitializeComponent();
            _roomName = roomName;
            _playerName = playerName;

            _client = new NetworkClient();
            _client.OnStateUpdate += HandleStateUpdate; //(TP) nơi khởi tạo các biến cần thiết và bắt đầu kết nối tới server
            _client.OnStart += HandleStart;

            _client.OnLog += HandleLog; //Khởi động biến sự kiện báo lí do phạt logic
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
            if (value == "WildDrawFour") return "WildDrawFour.png";
            if (value == "Wild") return "Wild.png";
            // (VQ) Xử lý các lá Wild 
           

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
            _isAnimatingPlay = true; // 🔥 FIX: Khóa cập nhật NGAY LẬP TỨC để tránh xung đột với gói tin State đến sau đó

            await Dispatcher.InvokeAsync(() =>
            {
                string topColor = (string)msg.topCard.color;
                string topValue = (string)msg.topCard.value;
                imgCurrentCard.Source = LoadAssetImage(MapCardToAsset(topColor, topValue));
                HandPanel.Children.Clear();
            });

            // 🔒 Khóa cập nhật trong suốt quá trình chia bài
            _isAnimatingPlay = true;

            foreach (var c in msg.yourHand)
            {
                string color = (string)c.color;
                string value = (string)c.value;
                BitmapImage img = LoadAssetImage(MapCardToAsset(color, value));

                await Dispatcher.InvokeAsync(() =>
                {
                    AnimateCard(btnDrawPile, HandPanel, img, () =>
                    {
                        AddCardToHand(color, value);
                        CheckUnoStatus();
                    });
                });

                await Task.Delay(500); // độ trễ giữa các lá
            }

            // 🔓 Mở khóa sau khi chia xong toàn bộ
            _isAnimatingPlay = false;

            // Nếu có state pending thì apply ngay
            if (_pendingState != null)
            {
                ApplyState(_pendingState);
                _pendingState = null;
            }
        }



        // Hàm đồng bộ UI từ state server
        private void ApplyState(dynamic msg)
        {
            // 1) Lá trên bàn
            string topColor = (string)msg.topCard.color;
            string topValue = (string)msg.topCard.value;
            imgCurrentCard.Source = LoadAssetImage(MapCardToAsset(topColor, topValue));

            var effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                BlurRadius = 40,
                ShadowDepth = 0,
                Opacity = 1
            };
            switch (topColor)
            {
                case "Red": effect.Color = Colors.Red; break;
                case "Green": effect.Color = Colors.LimeGreen; break;
                case "Blue": effect.Color = Colors.DodgerBlue; break;
                case "Yellow": effect.Color = Colors.Yellow; break;
                default: effect.Opacity = 0; break;

            }
            imgCurrentCard.Effect = effect;

            // 2) Danh sách players
            var players = (IEnumerable<dynamic>)msg.players;

            // 3) Chính mình
            var me = players.FirstOrDefault(p => p.name == _playerName);
            if (me != null)
            {
                var serverHand = (IEnumerable<dynamic>)me.hand;
                HandPanel.Children.Clear();
                foreach (var c in serverHand)
                {
                    AddCardToHand((string)c.color, (string)c.value);
                }
            }

            // 4) Các đối thủ
            var others = players.Where(p => p.name != _playerName).ToList();
            if (others.Count >= 1) UpdateOpponentHand(BotTopHand, txtBotTopName, others[0]);
            if (others.Count >= 2) UpdateOpponentHand(BotLeftHand, txtBotLeftName, others[1]);
            if (others.Count >= 3) UpdateOpponentHand(BotRightHand, txtBotRightName, others[2]);

            // 5) Hiển thị lượt hiện tại
            int currentIndex = (int)msg.currentIndex;
            var currentPlayer = players.ElementAt(currentIndex);
            txtTurnInfo.Text = $"Lượt: {(currentPlayer.name == _playerName ? "BẠN" : currentPlayer.name)}";

            // 6) Kiểm tra trạng thái UNO
            CheckUnoStatus();
        }


        private void HandleStateUpdate(dynamic msg)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Nếu đang animation thì lưu state mới nhất vào buffer
                if (_isAnimatingPlay)
                {
                    _pendingState = msg;
                    return;
                }

                // Nếu không animation thì apply ngay
                ApplyState(msg);
            }));
        }


        //Hàm sử lí sự kiện log
        private void HandleLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                // Thêm tin nhắn vào ListBox (lbLog là tên bạn đặt trong XAML)
                lbLog.Items.Add(message);

                // Tự động cuộn xuống dòng cuối
                if (lbLog.Items.Count > 0)
                {
                    lbLog.ScrollIntoView(lbLog.Items[lbLog.Items.Count - 1]);
                }
            });
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
                    Width = 40,
                    Height = 60,
                    Source = LoadAssetImage("Deck.png"),
                    Margin = new Thickness(2, 0, 2, 0) // (VQ) Hiệu ứng xếp chồng
                };
                panel.Children.Add(img);
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

            // (VQ) Tính vị trí bắt đầu và kết thúc
            Point startPos = sourceUI.TranslatePoint(new Point(0, 0), this);
            Point endPos = targetUI.TranslatePoint(new Point(0, 0), this);

            if (targetUI == HandPanel)
            {
                endPos.X += (targetUI.ActualWidth > 100 ? targetUI.ActualWidth - 100 : 0);
            }

            // (VQ) Tạo lá bài ảo để làm animation, ko đụng vào bài thật
            Image animatedCard = new Image
            {
                Source = cardImg,
                Height = 150,
                RenderTransformOrigin = new Point(0.5, 0.5),
                IsHitTestVisible = false
            };

            Canvas.SetLeft(animatedCard, startPos.X);
            Canvas.SetTop(animatedCard, startPos.Y);
            AnimationLayer.Children.Add(animatedCard);

            Duration duration = new Duration(TimeSpan.FromMilliseconds(700)); // (VQ) Tốc độ bay khi rút bài
            DoubleAnimation animX = new DoubleAnimation(endPos.X, duration) { EasingFunction = new PowerEase { Power = 2 } };
            DoubleAnimation animY = new DoubleAnimation(endPos.Y, duration) { EasingFunction = new PowerEase { Power = 2 } };

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
                AnimationLayer.Children.Remove(animatedCard); // (VQ) Xóa lá bài ảo
                onComplete?.Invoke(); // (VQ) Gọi hành động thêm bài thật vào tay
                _isAnimatingPlay = false; // 🔓 Mở khóa animation
                if (_pendingState != null)
                {
                    ApplyState(_pendingState); 
                    _pendingState = null; // 🔥 Nếu có state pending thì apply ngay
                }    
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

            _isAnimatingPlay = true; // 🔒 Khóa cập nhật từ server

            // Thực hiện Animation
            var imgControl = _selectedCard.Content as Image;
            BitmapImage imgSource = (BitmapImage)imgControl.Source;

            var cardToAnimate = _selectedCard;

            cardToAnimate.Opacity = 0;
            cardToAnimate.IsHitTestVisible = false;

            AnimateCard(cardToAnimate, imgCurrentCard, imgSource, () =>
            {
                // Xóa lá bài khỏi tay sau animation
                HandPanel.Children.Remove(cardToAnimate);
                _selectedCard = null;

                _isAnimatingPlay = false; // 🔓 Mở khóa sau animation

                string? nextColor = null;

                // Nếu là Wild hoặc WildDrawFour thì hiện bảng chọn màu
                if (color == "Wild" || value == "WildDrawFour")
                {
                    var dialog = new ColorChooser();
                    dialog.Owner = this;
                    if (dialog.ShowDialog() == true)
                    {
                        nextColor = dialog.SelectedColor;
                    }
                    else
                    {
                        // Nếu người chơi không chọn màu, mặc định là Red
                        nextColor = "Red";
                    }
                }

                // Gửi lệnh play lên server
                _client.Send(new
                {
                    type = "play",
                    player = _playerName,
                    card = new { color, value },
                    nextColor = nextColor
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

        private void btn_quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
       
    }
}