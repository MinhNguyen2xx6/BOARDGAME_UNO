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
    public partial class Lobby : Form
    {
        public Lobby()
        {
            InitializeComponent();
        }

        private void btn_dangxuat_Click(object sender, EventArgs e)
        {
            Session.IdToken = null;
            Session.RefreshToken = null;
            Session.UserEmail = null;
            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Close();
        }
    }
}
