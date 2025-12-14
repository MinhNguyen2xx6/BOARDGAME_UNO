using System.Windows;
using UNO_Client_WPF.Helpers;

namespace UNO_Client_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (Session.UserEmail == null)
            {
                // Nếu chưa đăng nhập thì hiện LoginWindow
                LoginWindow login = new LoginWindow();
                login.Show();
                this.Close();
            }
            else
            {
               
            }
        }

        private void btn_play_Click(object sender, RoutedEventArgs e)
        {
            LobbyWindow lobby = new LobbyWindow();
            lobby.Show();
            this.Hide();
        }

        private void btn_logout_Click(object sender, RoutedEventArgs e)
        {
            Session.UserEmail = null;
            TokenStorage.Clear();
            new LoginWindow().Show();
            this.Close();
        }
    }
}