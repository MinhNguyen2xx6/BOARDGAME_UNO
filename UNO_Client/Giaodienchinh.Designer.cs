namespace UNO_Client
{
    partial class Giaodienchinh
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            btn_Play = new Button();
            btn_login = new Button();
            label_tennguoidung = new Label();
            lb_username = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.d841d3fa_a631_4ecb_af3c_5c6cdbea61a4;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1319, 961);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // btn_Play
            // 
            btn_Play.Location = new Point(567, 575);
            btn_Play.Name = "btn_Play";
            btn_Play.Size = new Size(167, 91);
            btn_Play.TabIndex = 1;
            btn_Play.Text = "Play";
            btn_Play.UseVisualStyleBackColor = true;
            btn_Play.Click += btn_Play_Click;
            // 
            // btn_login
            // 
            btn_login.Location = new Point(12, 12);
            btn_login.Name = "btn_login";
            btn_login.Size = new Size(356, 50);
            btn_login.TabIndex = 2;
            btn_login.Text = "Đăng nhập";
            btn_login.UseVisualStyleBackColor = true;
            btn_login.Click += btn_login_Click;
            // 
            // label_tennguoidung
            // 
            label_tennguoidung.AutoSize = true;
            label_tennguoidung.Location = new Point(1166, 42);
            label_tennguoidung.Name = "label_tennguoidung";
            label_tennguoidung.Size = new Size(50, 20);
            label_tennguoidung.TabIndex = 3;
            label_tennguoidung.Text = "label1";
            // 
            // lb_username
            // 
            lb_username.AutoSize = true;
            lb_username.Location = new Point(12, 27);
            lb_username.Name = "lb_username";
            lb_username.Size = new Size(0, 20);
            lb_username.TabIndex = 4;
            // 
            // Giaodienchinh
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1281, 966);
            Controls.Add(lb_username);
            Controls.Add(label_tennguoidung);
            Controls.Add(btn_login);
            Controls.Add(btn_Play);
            Controls.Add(pictureBox1);
            Name = "Giaodienchinh";
            Text = "Giaodienchinh";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Button btn_Play;
        private Button btn_login;
        private Label label_tennguoidung;
        private Label lb_username;
    }
}