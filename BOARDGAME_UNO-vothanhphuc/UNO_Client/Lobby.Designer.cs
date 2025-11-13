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
            btnCreateRoom = new Button();
            btnReset = new Button();
            lstRooms = new ListBox();
            lblRoomName = new Label();
            label2 = new Label();
            tbRoomName = new TextBox();
            SuspendLayout();
            // 
            // btn_dangxuat
            // 
            btn_dangxuat.Location = new Point(40, 631);
            btn_dangxuat.Margin = new Padding(4);
            btn_dangxuat.Name = "btn_dangxuat";
            btn_dangxuat.Size = new Size(118, 42);
            btn_dangxuat.TabIndex = 0;
            btn_dangxuat.Text = "Đăng xuất";
            btn_dangxuat.UseVisualStyleBackColor = true;
            btn_dangxuat.Click += btn_dangxuat_Click;
            // 
            // btnCreateRoom
            // 
            btnCreateRoom.Location = new Point(134, 144);
            btnCreateRoom.Name = "btnCreateRoom";
            btnCreateRoom.Size = new Size(118, 42);
            btnCreateRoom.TabIndex = 1;
            btnCreateRoom.Text = "Tạo Phòng";
            btnCreateRoom.UseVisualStyleBackColor = true;
            btnCreateRoom.Click += btnCreateRoom_Click;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(490, 81);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(118, 42);
            btnReset.TabIndex = 2;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnReset_Click;
            // 
            // lstRooms
            // 
            lstRooms.FormattingEnabled = true;
            lstRooms.ItemHeight = 25;
            lstRooms.Location = new Point(462, 175);
            lstRooms.Name = "lstRooms";
            lstRooms.Size = new Size(180, 129);
            lstRooms.TabIndex = 3;
            // 
            // lblRoomName
            // 
            lblRoomName.AutoSize = true;
            lblRoomName.Location = new Point(48, 101);
            lblRoomName.Name = "lblRoomName";
            lblRoomName.Size = new Size(143, 25);
            lblRoomName.TabIndex = 4;
            lblRoomName.Text = "Nhập Tên Phòng";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(48, 161);
            label2.Name = "label2";
            label2.Size = new Size(0, 25);
            label2.TabIndex = 5;
            // 
            // tbRoomName
            // 
            tbRoomName.Location = new Point(197, 98);
            tbRoomName.Name = "tbRoomName";
            tbRoomName.Size = new Size(150, 31);
            tbRoomName.TabIndex = 6;
            // 
            // Lobby
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1350, 720);
            Controls.Add(tbRoomName);
            Controls.Add(label2);
            Controls.Add(lblRoomName);
            Controls.Add(lstRooms);
            Controls.Add(btnReset);
            Controls.Add(btnCreateRoom);
            Controls.Add(btn_dangxuat);
            Margin = new Padding(4);
            Name = "Lobby";
            Text = "Lobby";
            Load += Lobby_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_dangxuat;
        private Button btnCreateRoom;
        private Button btnReset;
        private ListBox lstRooms;
        private Label lblRoomName;
        private Label label2;
        private TextBox tbRoomName;
    }
}