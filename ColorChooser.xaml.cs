using System.Windows;
using System.Windows.Controls;

namespace UNO_Client_WPF
{
    public partial class ColorChooser : Window
    {
        public string SelectedColor { get; private set; } = "Red";

        public ColorChooser()
        {
            InitializeComponent();
        }

        // Hàm này phải nằm ở đây (ColorChooser.xaml.cs)
        private void BtnColor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                SelectedColor = btn.Tag.ToString();
                DialogResult = true;
                Close();
            }
        }
    }
}