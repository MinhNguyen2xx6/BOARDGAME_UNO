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
    public partial class Formgem : Form
    {
        public Formgem()
        {
            InitializeComponent();
            LoadBG();
        }

        private void Formgem_Load(object sender, EventArgs e)
        {

        }
        private void LoadBG()
        {
            this.BackgroundImage = Properties.Resources.Background;
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void pn_user1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
