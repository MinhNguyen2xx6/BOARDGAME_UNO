namespace UNO_Client
{
    partial class FormGame
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.ListBox lstMyHand;
        private System.Windows.Forms.Label lblTopCard;
        private System.Windows.Forms.Label lblTurn;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnDraw;
        private System.Windows.Forms.Button btnUno;

        private System.Windows.Forms.Label lblPlayer1;
        private System.Windows.Forms.Label lblPlayer2;
        private System.Windows.Forms.Label lblPlayer3;
        private System.Windows.Forms.Label lblPlayer4;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            lstMyHand = new ListBox();
            lblTopCard = new Label();
            lblTurn = new Label();
            btnPlay = new Button();
            btnDraw = new Button();
            btnUno = new Button();
            lblPlayer1 = new Label();
            lblPlayer2 = new Label();
            lblPlayer3 = new Label();
            lblPlayer4 = new Label();
            guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(components);
            SuspendLayout();
            // 
            // lstMyHand
            // 
            lstMyHand.FormattingEnabled = true;
            lstMyHand.Location = new Point(169, 439);
            lstMyHand.Name = "lstMyHand";
            lstMyHand.Size = new Size(600, 164);
            lstMyHand.TabIndex = 0;
            // 
            // lblTopCard
            // 
            lblTopCard.AutoSize = true;
            lblTopCard.Location = new Point(350, 200);
            lblTopCard.Name = "lblTopCard";
            lblTopCard.Size = new Size(72, 20);
            lblTopCard.TabIndex = 8;
            lblTopCard.Text = "Top Card:";
            // 
            // lblTurn
            // 
            lblTurn.AutoSize = true;
            lblTurn.Location = new Point(350, 230);
            lblTurn.Name = "lblTurn";
            lblTurn.Size = new Size(41, 20);
            lblTurn.TabIndex = 7;
            lblTurn.Text = "Turn:";
            // 
            // btnPlay
            // 
            btnPlay.Location = new Point(454, 380);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(75, 23);
            btnPlay.TabIndex = 6;
            btnPlay.Text = "Play";
            btnPlay.Click += btnPlay_Click;
            // 
            // btnDraw
            // 
            btnDraw.Location = new Point(169, 127);
            btnDraw.Name = "btnDraw";
            btnDraw.Size = new Size(75, 23);
            btnDraw.TabIndex = 5;
            btnDraw.Text = "Draw";
            btnDraw.Click += btnDraw_Click;
            // 
            // btnUno
            // 
            btnUno.Location = new Point(796, 478);
            btnUno.Name = "btnUno";
            btnUno.Size = new Size(75, 23);
            btnUno.TabIndex = 4;
            btnUno.Text = "UNO!";
            btnUno.Click += btnUno_Click;
            // 
            // lblPlayer1
            // 
            lblPlayer1.AutoSize = true;
            lblPlayer1.Location = new Point(93, 465);
            lblPlayer1.Name = "lblPlayer1";
            lblPlayer1.Size = new Size(61, 20);
            lblPlayer1.TabIndex = 3;
            lblPlayer1.Text = "Player 1";
            // 
            // lblPlayer2
            // 
            lblPlayer2.AutoSize = true;
            lblPlayer2.Location = new Point(650, 30);
            lblPlayer2.Name = "lblPlayer2";
            lblPlayer2.Size = new Size(61, 20);
            lblPlayer2.TabIndex = 2;
            lblPlayer2.Text = "Player 2";
            // 
            // lblPlayer3
            // 
            lblPlayer3.AutoSize = true;
            lblPlayer3.Location = new Point(30, 200);
            lblPlayer3.Name = "lblPlayer3";
            lblPlayer3.Size = new Size(61, 20);
            lblPlayer3.TabIndex = 1;
            lblPlayer3.Text = "Player 3";
            // 
            // lblPlayer4
            // 
            lblPlayer4.AutoSize = true;
            lblPlayer4.Location = new Point(650, 200);
            lblPlayer4.Name = "lblPlayer4";
            lblPlayer4.Size = new Size(61, 20);
            lblPlayer4.TabIndex = 0;
            lblPlayer4.Text = "Player 4";
            // 
            // guna2Elipse1
            // 
            guna2Elipse1.BorderRadius = 10;
            guna2Elipse1.TargetControl = this;
            // 
            // FormGame
            // 
            ClientSize = new Size(914, 600);
            Controls.Add(lblPlayer4);
            Controls.Add(lblPlayer3);
            Controls.Add(lblPlayer2);
            Controls.Add(lblPlayer1);
            Controls.Add(btnUno);
            Controls.Add(btnDraw);
            Controls.Add(btnPlay);
            Controls.Add(lblTurn);
            Controls.Add(lblTopCard);
            Controls.Add(lstMyHand);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormGame";
            Text = "UNO Game - 4 Players";
            Load += FormGame_Load;
            ResumeLayout(false);
            PerformLayout();
        }
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
    }
}
