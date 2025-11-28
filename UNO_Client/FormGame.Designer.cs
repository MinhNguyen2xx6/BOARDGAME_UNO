using System.Windows.Forms;
using System.Drawing;

namespace UNO_Client
{
    partial class FormGame
    {
        private System.ComponentModel.IContainer components = null;

        // 4 listbox cho 4 cạnh
        private ListBox lstPlayerTop;
        private ListBox lstPlayerBottom;
        private ListBox lstPlayerLeft;
        private ListBox lstPlayerRight;

        // 2 label trung tâm
        private Label lblTopCard;
        private Label lblCurrentTurn;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lstPlayerTop = new ListBox();
            lstPlayerBottom = new ListBox();
            lstPlayerLeft = new ListBox();
            lstPlayerRight = new ListBox();
            lblTopCard = new Label();
            lblCurrentTurn = new Label();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            left = new Label();
            top = new Label();
            right = new Label();
            bot = new Label();
            SuspendLayout();
            // 
            // lstPlayerTop
            // 
            lstPlayerTop.FormattingEnabled = true;
            lstPlayerTop.Location = new Point(171, 27);
            lstPlayerTop.Margin = new Padding(3, 4, 3, 4);
            lstPlayerTop.Name = "lstPlayerTop";
            lstPlayerTop.Size = new Size(571, 84);
            lstPlayerTop.TabIndex = 0;
            // 
            // lstPlayerBottom
            // 
            lstPlayerBottom.FormattingEnabled = true;
            lstPlayerBottom.Location = new Point(171, 488);
            lstPlayerBottom.Margin = new Padding(3, 4, 3, 4);
            lstPlayerBottom.Name = "lstPlayerBottom";
            lstPlayerBottom.Size = new Size(571, 84);
            lstPlayerBottom.TabIndex = 1;
            // 
            // lstPlayerLeft
            // 
            lstPlayerLeft.FormattingEnabled = true;
            lstPlayerLeft.Location = new Point(23, 133);
            lstPlayerLeft.Margin = new Padding(3, 4, 3, 4);
            lstPlayerLeft.Name = "lstPlayerLeft";
            lstPlayerLeft.Size = new Size(114, 344);
            lstPlayerLeft.TabIndex = 2;
            // 
            // lstPlayerRight
            // 
            lstPlayerRight.FormattingEnabled = true;
            lstPlayerRight.Location = new Point(777, 133);
            lstPlayerRight.Margin = new Padding(3, 4, 3, 4);
            lstPlayerRight.Name = "lstPlayerRight";
            lstPlayerRight.Size = new Size(114, 344);
            lstPlayerRight.TabIndex = 3;
            // 
            // lblTopCard
            // 
            lblTopCard.AutoSize = true;
            lblTopCard.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTopCard.Location = new Point(411, 267);
            lblTopCard.Name = "lblTopCard";
            lblTopCard.Size = new Size(130, 28);
            lblTopCard.TabIndex = 4;
            lblTopCard.Text = "Top Card: ---";
            // 
            // lblCurrentTurn
            // 
            lblCurrentTurn.AutoSize = true;
            lblCurrentTurn.Font = new Font("Segoe UI", 10F);
            lblCurrentTurn.Location = new Point(411, 307);
            lblCurrentTurn.Name = "lblCurrentTurn";
            lblCurrentTurn.Size = new Size(74, 23);
            lblCurrentTurn.TabIndex = 5;
            lblCurrentTurn.Text = "Lượt: ---";
            // 
            // button1
            // 
            button1.Location = new Point(664, 368);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 6;
            button1.Text = "draw";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btnDraw_Click;
            // 
            // button2
            // 
            button2.Location = new Point(664, 317);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 7;
            button2.Text = "uno";
            button2.UseVisualStyleBackColor = true;
            button2.Click += btnUno_Click;
            // 
            // button3
            // 
            button3.Location = new Point(664, 266);
            button3.Name = "button3";
            button3.Size = new Size(94, 29);
            button3.TabIndex = 8;
            button3.Text = "play";
            button3.UseVisualStyleBackColor = true;
            button3.Click += btnPlay_Click;
            // 
            // left
            // 
            left.AutoSize = true;
            left.Location = new Point(52, 91);
            left.Name = "left";
            left.Size = new Size(50, 20);
            left.TabIndex = 9;
            left.Text = "label1";
            // 
            // top
            // 
            top.AutoSize = true;
            top.Location = new Point(401, 3);
            top.Name = "top";
            top.Size = new Size(50, 20);
            top.TabIndex = 10;
            top.Text = "label2";
            // 
            // right
            // 
            right.AutoSize = true;
            right.Location = new Point(806, 100);
            right.Name = "right";
            right.Size = new Size(50, 20);
            right.TabIndex = 11;
            right.Text = "label3";
            // 
            // bot
            // 
            bot.AutoSize = true;
            bot.Location = new Point(435, 445);
            bot.Name = "bot";
            bot.Size = new Size(50, 20);
            bot.TabIndex = 12;
            bot.Text = "label4";
            // 
            // FormGame
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(bot);
            Controls.Add(right);
            Controls.Add(top);
            Controls.Add(left);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(lstPlayerTop);
            Controls.Add(lstPlayerBottom);
            Controls.Add(lstPlayerLeft);
            Controls.Add(lstPlayerRight);
            Controls.Add(lblTopCard);
            Controls.Add(lblCurrentTurn);
            Margin = new Padding(3, 4, 3, 4);
            Name = "FormGame";
            Text = "UNO Game";
            Load += FormGame_Load;
            ResumeLayout(false);
            PerformLayout();
        }
        private Button button1;
        private Button button2;
        private Button button3;
        private Label left;
        private Label top;
        private Label right;
        private Label bot;
    }
}
