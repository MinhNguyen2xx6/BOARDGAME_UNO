using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNO_Client
{
    public partial class Giaodienchinh : Form
    {
        public Giaodienchinh()
        {
            InitializeComponent();
        }
        private void Giaodienchinh_Load(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            if (Session.UserEmail == null)
            {
                //Chưa đăng nhập → mở form login
                Form1 dangnhap = new Form1(this);
                dangnhap.Show();
                Hide();
            }
            else
            {
                //Đã đăng nhập → hỏi có muốn đăng xuất không
                ShowProfileMenu(sender);
            }
        }
        //update lai giao dien luc dang nhap
        public void UpdateUI()
        {
            if (Session.UserEmail != null)
            {
                btn_login.Text = "Profile";
                lb_username.Text = Session.UserEmail;
                this.Show();
            }
            else
            {
                btn_login.Text = "Đăng nhập";
                lb_username.Text = "";
            }
        }

        //Dieu khien viec login khi dang nhap vao
        public void HandleAutoLoginResult(bool success)
        {
            if (success)
            {
                UpdateUI();
                this.Show();
                this.BringToFront();
            }
            else
            {
                // Auto login thất bại, quay lại login
            }
        }
        //Menu hien thi khi bam vao lai ten
        private void ShowProfileMenu(object sender)
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add($"Tài khoản: {Session.UserEmail}").Enabled = false;
            menu.Items.Add("-"); // Separator
            menu.Items.Add("Đăng xuất", null, (s, ev) => Logout());

            // Hiển thị menu dưới nút
            var button = (Button)sender;
            menu.Show(button, new Point(0, button.Height));
        }
        //Dang xuat
        private void Logout()
        {
            var result = MessageBox.Show(
                "Bạn có muốn đăng xuất không?",
                "Đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Session.IdToken = null;
                Session.RefreshToken = null;
                Session.UserEmail = null;
                TokenStorage.Clear();
                UpdateUI();
            }
        }
        private void btn_Play_Click(object sender, EventArgs e)
        {
            Lobby sanh = new Lobby(this);                                                  
            sanh.Show();
            Hide();
        }
    }
}
