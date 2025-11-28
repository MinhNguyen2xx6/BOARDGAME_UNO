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
    public partial class ColorPickerForm : Form
    {
        public ColorPickerForm()
        {
            InitializeComponent();
        }
        public UnoColor SelectedColor { get; private set; }

        private void btnRed_Click(object sender, EventArgs e)
        {
            SelectedColor = UnoColor.Red;
            this.DialogResult = DialogResult.OK;
        }

        private void btnBlue_Click(object sender, EventArgs e)
        {
            SelectedColor = UnoColor.Blue;
            this.DialogResult = DialogResult.OK;
        }

        private void btnGreen_Click(object sender, EventArgs e)
        {
            SelectedColor = UnoColor.Green;
            this.DialogResult = DialogResult.OK;
        }

        private void btnYellow_Click(object sender, EventArgs e)
        {
            SelectedColor = UnoColor.Yellow;
            this.DialogResult = DialogResult.OK;
        }
    }
}
