using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace UNO_Client_WPF
{
    public partial class GameWindow : Window
    {
        private string? _roomName;
        private List<string>? _players;

        // Biến lưu lá bài đang được chọn
        private Button? _selectedCard = null;

        public GameWindow(string? roomName, List<string>? players)
        {
            InitializeComponent();
            _roomName = roomName;
            _players = players;

            // Test phát bài
            AddCardToHand("Blue_1");
            AddCardToHand("Red_Skip");
            AddCardToHand("Green_7");
            AddCardToHand("Yellow_Reverse");
        }

        // --- 1. THÊM BÀI VÀO TAY ---
        private void AddCardToHand(string cardName)
        {
            Button btnCard = new Button();
            var style = TryFindResource("CardStyle") as Style;
            if (style != null) btnCard.Style = style;

            btnCard.Tag = cardName; // Lưu tên bài
            btnCard.RenderTransform = new TranslateTransform(0, 0); // Khởi tạo Transform

            Image img = new Image();
            try
            {
                img.Source = new BitmapImage(new Uri($"/Assets/{cardName}.png", UriKind.Relative));
            }
            catch
            {
                img.Source = new BitmapImage(new Uri("/Assets/uno_classic_logo.jpg", UriKind.Relative));
            }
            img.Stretch = Stretch.Uniform;
            btnCard.Content = img;

            // KHI CLICK VÀO LÁ BÀI -> CHỈ CHỌN (SELECT), KHÔNG ĐÁNH NGAY
            btnCard.Click += (s, e) => SelectCard(s as Button);

            HandPanel.Children.Add(btnCard);
            CheckUnoStatus();
        }

        // --- 2. LOGIC CHỌN BÀI (SELECT) ---
        private void SelectCard(Button? clickedBtn)
        {
            if (clickedBtn == null) return;

            // A. Reset lá bài cũ (nếu có) xuống vị trí bình thường
            if (_selectedCard != null)
            {
                var transform = _selectedCard.RenderTransform as TranslateTransform;
                if (transform != null) transform.Y = 0;

                // Trả lại viền trắng mặc định
                if (_selectedCard.Template.FindName("border", _selectedCard) is Border oldBorder)
                {
                    oldBorder.BorderBrush = Brushes.White;
                }
            }

            // B. Nếu click lại lá đang chọn -> Hủy chọn (Bỏ comment nếu muốn tính năng này)
            /*
            if (_selectedCard == clickedBtn) {
                _selectedCard = null;
                return;
            }
            */

            // C. Nổi lá bài mới lên
            _selectedCard = clickedBtn;

            // Dịch chuyển lên cao (-50px)
            var newTransform = _selectedCard.RenderTransform as TranslateTransform;
            if (newTransform == null)
            {
                newTransform = new TranslateTransform();
                _selectedCard.RenderTransform = newTransform;
            }
            newTransform.Y = -50;

            // Đổi màu viền sang xanh lá để biết đang chọn
            if (_selectedCard.Template.FindName("border", _selectedCard) is Border newBorder)
            {
                newBorder.BorderBrush = Brushes.LimeGreen;
            }
        }

        // --- 3. LOGIC NÚT ĐÁNH BÀI (PLAY BUTTON) ---
        private async void btnPlayCard_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCard == null)
            {
                MessageBox.Show("Vui lòng chọn một lá bài trước!");
                return;
            }

            // Khóa nút để tránh spam
            btnPlayCard.IsEnabled = false;

            // Gọi hàm Animation bài bay
            await AnimateCardToTable(_selectedCard);

            // Xử lý sau khi đánh xong
            HandPanel.Children.Remove(_selectedCard);
            _selectedCard = null;
            btnSkip.Visibility = Visibility.Collapsed;

            CheckUnoStatus();
            btnPlayCard.IsEnabled = true;
        }

        // --- 4. ANIMATION BÀI BAY TỪ TAY RA BÀN ---
        private async Task AnimateCardToTable(Button cardBtn)
        {
            // A. Tạo ảnh giả để bay
            Image flyingCard = new Image();
            if (cardBtn.Content is Image srcImg)
                flyingCard.Source = srcImg.Source;

            flyingCard.Width = 110;
            flyingCard.Height = 160;

            // B. Lấy tọa độ hiện tại của lá bài trên tay (Start)
            Point startPoint = cardBtn.TranslatePoint(new Point(0, 0), this);

            // C. Lấy tọa độ bàn chơi (End) - Vị trí imgCurrentCard
            Point endPoint = imgCurrentCard.TranslatePoint(new Point(0, 0), this);

            // D. Đặt ảnh giả vào Canvas
            Canvas.SetLeft(flyingCard, startPoint.X);
            Canvas.SetTop(flyingCard, startPoint.Y);
            AnimationLayer.Children.Add(flyingCard);

            // Ẩn lá bài thật trên tay đi (để cảm giác nó đã bay đi)
            cardBtn.Visibility = Visibility.Hidden;

            // E. Animation Bay
            DoubleAnimation animX = new DoubleAnimation(startPoint.X, endPoint.X, TimeSpan.FromSeconds(0.4));
            DoubleAnimation animY = new DoubleAnimation(startPoint.Y, endPoint.Y, TimeSpan.FromSeconds(0.4));

            // Hiệu ứng bay nhanh dần
            QuadraticEase ease = new QuadraticEase { EasingMode = EasingMode.EaseIn };
            animX.EasingFunction = ease;
            animY.EasingFunction = ease;

            flyingCard.BeginAnimation(Canvas.LeftProperty, animX);
            flyingCard.BeginAnimation(Canvas.TopProperty, animY);

            // F. Đợi bay xong
            await Task.Delay(400);

            // G. Cập nhật bài trên bàn và xóa ảnh giả
            imgCurrentCard.Source = flyingCard.Source;
            AnimationLayer.Children.Remove(flyingCard);
        }

        // --- 5. LOGIC RÚT BÀI ---
        private async void btnDrawPile_Click(object sender, RoutedEventArgs e)
        {
            btnDrawPile.IsEnabled = false;

            Image flyingCard = new Image();
            try { flyingCard.Source = new BitmapImage(new Uri("/Assets/Deck.png", UriKind.Relative)); } catch { }
            flyingCard.Width = 100; flyingCard.Height = 150;
            flyingCard.RenderTransformOrigin = new Point(0.5, 0.5);

            TransformGroup group = new TransformGroup();
            group.Children.Add(new ScaleTransform());
            group.Children.Add(new TranslateTransform());
            flyingCard.RenderTransform = group;

            Point startPoint = btnDrawPile.TranslatePoint(new Point(0, 0), this);
            Point endPoint = HandPanel.TranslatePoint(new Point(HandPanel.ActualWidth / 2, 0), this);

            Canvas.SetLeft(flyingCard, startPoint.X);
            Canvas.SetTop(flyingCard, startPoint.Y);
            AnimationLayer.Children.Add(flyingCard);

            // ANIMATION LẬT BÀI
            Storyboard sb = new Storyboard();

            // 1. Co lại (0 -> 0.2s)
            DoubleAnimation flip1 = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.2));
            Storyboard.SetTarget(flip1, flyingCard);
            Storyboard.SetTargetProperty(flip1, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            sb.Children.Add(flip1);

            // 2. Đổi ảnh (tại 0.2s)
            ObjectAnimationUsingKeyFrames changeImg = new ObjectAnimationUsingKeyFrames();
            changeImg.BeginTime = TimeSpan.FromSeconds(0.2);
            string drawnCard = "Wild"; // Random bài ở đây
            changeImg.KeyFrames.Add(new DiscreteObjectKeyFrame(new BitmapImage(new Uri($"/Assets/{drawnCard}.png", UriKind.Relative))));
            Storyboard.SetTarget(changeImg, flyingCard);
            Storyboard.SetTargetProperty(changeImg, new PropertyPath(Image.SourceProperty));
            sb.Children.Add(changeImg);

            // 3. Phình ra (0.2s -> 0.4s)
            DoubleAnimation flip2 = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            flip2.BeginTime = TimeSpan.FromSeconds(0.2);
            Storyboard.SetTarget(flip2, flyingCard);
            Storyboard.SetTargetProperty(flip2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            sb.Children.Add(flip2);

            // 4. Bay về tay (0.4s -> 0.8s)
            DoubleAnimation moveX = new DoubleAnimation(0, endPoint.X - startPoint.X, TimeSpan.FromSeconds(0.4));
            moveX.BeginTime = TimeSpan.FromSeconds(0.4);
            Storyboard.SetTarget(moveX, flyingCard);
            Storyboard.SetTargetProperty(moveX, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)"));
            sb.Children.Add(moveX);

            DoubleAnimation moveY = new DoubleAnimation(0, endPoint.Y - startPoint.Y + 100, TimeSpan.FromSeconds(0.4));
            moveY.BeginTime = TimeSpan.FromSeconds(0.4);
            Storyboard.SetTarget(moveY, flyingCard);
            Storyboard.SetTargetProperty(moveY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)"));
            sb.Children.Add(moveY);

            sb.Completed += (s, ev) =>
            {
                AnimationLayer.Children.Remove(flyingCard);
                AddCardToHand(drawnCard);
                btnSkip.Visibility = Visibility.Visible;
                btnDrawPile.IsEnabled = true;
            };

            sb.Begin();
        }

        private void btnSkip_Click(object sender, RoutedEventArgs e)
        {
            btnSkip.Visibility = Visibility.Collapsed;
            MessageBox.Show("Bỏ lượt");
        }

        private void btnUno_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("UNO!");
            btnUno.Visibility = Visibility.Collapsed;
        }

        private void CheckUnoStatus()
        {
            if (HandPanel.Children.Count == 1) btnUno.Visibility = Visibility.Visible;
            else btnUno.Visibility = Visibility.Collapsed;
        }

        private void btn_quit_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}