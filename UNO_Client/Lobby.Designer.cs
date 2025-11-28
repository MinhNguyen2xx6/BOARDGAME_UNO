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
            btnCreateRoom = new Button();
            btnReset = new Button();
            lstRooms = new ListBox();
            lblRoomName = new Label();
            label2 = new Label();
            tbRoomName = new TextBox();
            btn_back = new Button();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            SuspendLayout();
            // 
            // btnCreateRoom
            // 
            btnCreateRoom.Location = new Point(84, 91);
            btnCreateRoom.Margin = new Padding(2);
            btnCreateRoom.Name = "btnCreateRoom";
            btnCreateRoom.Size = new Size(82, 26);
            btnCreateRoom.TabIndex = 1;
            btnCreateRoom.Text = "Tạo Phòng";
            btnCreateRoom.UseVisualStyleBackColor = true;
            btnCreateRoom.Click += btnCreateRoom_Click;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(343, 49);
            btnReset.Margin = new Padding(2);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(82, 26);
            btnReset.TabIndex = 2;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnBreset;
            // 
            // lstRooms
            // 
            lstRooms.FormattingEnabled = true;
            lstRooms.ItemHeight = 15;
            lstRooms.Location = new Point(324, 90);
            lstRooms.Margin = new Padding(2);
            lstRooms.Name = "lstRooms";
            lstRooms.Size = new Size(329, 259);
            lstRooms.TabIndex = 3;
            // 
            // lblRoomName
            // 
            lblRoomName.AutoSize = true;
            lblRoomName.Location = new Point(33, 61);
            lblRoomName.Margin = new Padding(2, 0, 2, 0);
            lblRoomName.Name = "lblRoomName";
            lblRoomName.Size = new Size(96, 15);
            lblRoomName.TabIndex = 4;
            lblRoomName.Text = "Nhập Tên Phòng";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(33, 97);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(0, 15);
            label2.TabIndex = 5;
            // 
            // tbRoomName
            // 
            tbRoomName.Location = new Point(138, 58);
            tbRoomName.Margin = new Padding(2);
            tbRoomName.Name = "tbRoomName";
            tbRoomName.Size = new Size(106, 23);
            tbRoomName.TabIndex = 6;
            // 
            // btn_back
            // 
            btn_back.Location = new Point(33, 324);
            btn_back.Margin = new Padding(3, 2, 3, 2);
            btn_back.Name = "btn_back";
            btn_back.Size = new Size(82, 22);
            btn_back.TabIndex = 7;
            btn_back.Text = "Quay Về";
            btn_back.UseVisualStyleBackColor = true;
            btn_back.Click += btnBack_Click;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ActiveCaption;
            button1.Location = new Point(468, 40);
            button1.Name = "button1";
            button1.Size = new Size(123, 36);
            button1.TabIndex = 8;
            button1.Text = "JOIN";
            button1.UseVisualStyleBackColor = false;
            button1.Click += btnJoin_Click;
            // 
            // button2
            // 
            button2.Location = new Point(197, 91);
            button2.Name = "button2";
            button2.Size = new Size(84, 26);
            button2.TabIndex = 9;
            button2.Text = "Thoát phòng";
            button2.UseVisualStyleBackColor = true;
            button2.Click += btnExitRoom_Click;
            // 
            // button3
            // 
            button3.Location = new Point(197, 144);
            button3.Name = "button3";
            button3.Size = new Size(84, 29);
            button3.TabIndex = 10;
            button3.Text = "Xóa phòng";
            button3.UseVisualStyleBackColor = true;
            button3.Click += btnDeleteRoom_Click;
            // 
            // Lobby
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(945, 432);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(btn_back);
            Controls.Add(tbRoomName);
            Controls.Add(label2);
            Controls.Add(lblRoomName);
            Controls.Add(lstRooms);
            Controls.Add(btnReset);
            Controls.Add(btnCreateRoom);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Lobby";
            Text = "Lobby";
            Load += Lobby_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnCreateRoom;
        private Button btnReset;
        private ListBox lstRooms;
        private Label lblRoomName;
        private Label label2;
        private TextBox tbRoomName;
        private Button btn_back;
        private Button button1;
        private Button button2;
        private Button button3;
    }
}