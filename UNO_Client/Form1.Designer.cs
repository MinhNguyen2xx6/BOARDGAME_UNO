namespace UNO_Client
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tb_taikhoan = new TextBox();
            tb_matkhau = new TextBox();
            label1 = new Label();
            label2 = new Label();
            btn_dk = new Button();
            btn_dangnhap = new Button();
            btn_quenmk = new Button();
            label3 = new Label();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tb_taikhoan
            // 
            tb_taikhoan.Location = new Point(328, 144);
            tb_taikhoan.Name = "tb_taikhoan";
            tb_taikhoan.Size = new Size(181, 27);
            tb_taikhoan.TabIndex = 0;
            // 
            // tb_matkhau
            // 
            tb_matkhau.Location = new Point(328, 212);
            tb_matkhau.Name = "tb_matkhau";
            tb_matkhau.Size = new Size(181, 27);
            tb_matkhau.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 15F);
            label1.ForeColor = Color.Coral;
            label1.Location = new Point(177, 144);
            label1.Name = "label1";
            label1.Size = new Size(124, 35);
            label1.TabIndex = 3;
            label1.Text = "Tài khoản:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = SystemColors.Control;
            label2.Font = new Font("Segoe UI", 15F);
            label2.ForeColor = Color.Coral;
            label2.Location = new Point(177, 203);
            label2.Name = "label2";
            label2.Size = new Size(118, 35);
            label2.TabIndex = 4;
            label2.Text = "Mật khẩu";
            // 
            // btn_dk
            // 
            btn_dk.Location = new Point(149, 307);
            btn_dk.Name = "btn_dk";
            btn_dk.Size = new Size(94, 29);
            btn_dk.TabIndex = 5;
            btn_dk.Text = "Đăng ký";
            btn_dk.UseVisualStyleBackColor = true;
            btn_dk.Click += btn_dk_Click;
            // 
            // btn_dangnhap
            // 
            btn_dangnhap.Location = new Point(348, 307);
            btn_dangnhap.Name = "btn_dangnhap";
            btn_dangnhap.Size = new Size(94, 29);
            btn_dangnhap.TabIndex = 6;
            btn_dangnhap.Text = "Đăng nhập";
            btn_dangnhap.UseVisualStyleBackColor = true;
            btn_dangnhap.Click += btn_dangnhap_Click;
            // 
            // btn_quenmk
            // 
            btn_quenmk.Location = new Point(538, 307);
            btn_quenmk.Name = "btn_quenmk";
            btn_quenmk.Size = new Size(134, 29);
            btn_quenmk.TabIndex = 7;
            btn_quenmk.Text = "Quên mật khẩu";
            btn_quenmk.UseVisualStyleBackColor = true;
            btn_quenmk.Click += btn_quenmk_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 25F);
            label3.ForeColor = Color.Coral;
            label3.Location = new Point(228, 43);
            label3.Name = "label3";
            label3.Size = new Size(367, 57);
            label3.TabIndex = 8;
            label3.Text = "UNO SIÊU KHỦNG";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.luat_choi_huong_dan_choi_board_game_uno_co_ban_cho_nguoi_moi_choi_2021123013512168071;
            pictureBox1.Location = new Point(-4, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(799, 430);
            pictureBox1.TabIndex = 9;
            pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(791, 428);
            Controls.Add(label3);
            Controls.Add(btn_quenmk);
            Controls.Add(btn_dangnhap);
            Controls.Add(btn_dk);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(tb_matkhau);
            Controls.Add(tb_taikhoan);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tb_taikhoan;
        private TextBox tb_matkhau;
        private Label label1;
        private Label label2;
        private Button btn_dk;
        private Button btn_dangnhap;
        private Button btn_quenmk;
        private Label label3;
        private PictureBox pictureBox1;
    }
}
