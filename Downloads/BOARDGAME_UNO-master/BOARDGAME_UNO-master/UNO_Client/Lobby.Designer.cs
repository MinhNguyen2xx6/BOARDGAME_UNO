namespace UNO_Client
{
    partial class Lobby
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
            btn_dangxuat = new Button();
            SuspendLayout();
            // 
            // btn_dangxuat
            // 
            btn_dangxuat.Location = new Point(32, 505);
            btn_dangxuat.Name = "btn_dangxuat";
            btn_dangxuat.Size = new Size(94, 34);
            btn_dangxuat.TabIndex = 0;
            btn_dangxuat.Text = "Đăng xuất";
            btn_dangxuat.UseVisualStyleBackColor = true;
            btn_dangxuat.Click += btn_dangxuat_Click;
            // 
            // Lobby
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1080, 576);
            Controls.Add(btn_dangxuat);
            Name = "Lobby";
            Text = "Lobby";
            ResumeLayout(false);
        }

        #endregion

        private Button btn_dangxuat;
    }
}