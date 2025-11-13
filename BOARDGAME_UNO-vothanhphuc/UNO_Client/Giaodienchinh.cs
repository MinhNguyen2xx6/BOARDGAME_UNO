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

        private void btn_login_Click(object sender, EventArgs e)
        {
            Form1 dangnhap = new Form1();
            dangnhap.Show();
        }

        private void btn_Play_Click(object sender, EventArgs e)
        {
            Lobby sanh = new Lobby();                                                  
            sanh.Show();
            Hide();
        }
    }
}
