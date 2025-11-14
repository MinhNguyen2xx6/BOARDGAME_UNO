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
            SuspendLayout();
            // 
            // btnCreateRoom
            // 
            btnCreateRoom.Location = new Point(107, 115);
            btnCreateRoom.Margin = new Padding(2);
            btnCreateRoom.Name = "btnCreateRoom";
            btnCreateRoom.Size = new Size(94, 34);
            btnCreateRoom.TabIndex = 1;
            btnCreateRoom.Text = "Tạo Phòng";
            btnCreateRoom.UseVisualStyleBackColor = true;
            btnCreateRoom.Click += btnCreateRoom_Click;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(392, 65);
            btnReset.Margin = new Padding(2);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(94, 34);
            btnReset.TabIndex = 2;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnReset_Click;
            // 
            // lstRooms
            // 
            lstRooms.FormattingEnabled = true;
            lstRooms.Location = new Point(370, 140);
            lstRooms.Margin = new Padding(2);
            lstRooms.Name = "lstRooms";
            lstRooms.Size = new Size(145, 104);
            lstRooms.TabIndex = 3;
            // 
            // lblRoomName
            // 
            lblRoomName.AutoSize = true;
            lblRoomName.Location = new Point(38, 81);
            lblRoomName.Margin = new Padding(2, 0, 2, 0);
            lblRoomName.Name = "lblRoomName";
            lblRoomName.Size = new Size(118, 20);
            lblRoomName.TabIndex = 4;
            lblRoomName.Text = "Nhập Tên Phòng";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(38, 129);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(0, 20);
            label2.TabIndex = 5;
            // 
            // tbRoomName
            // 
            tbRoomName.Location = new Point(158, 78);
            tbRoomName.Margin = new Padding(2);
            tbRoomName.Name = "tbRoomName";
            tbRoomName.Size = new Size(121, 27);
            tbRoomName.TabIndex = 6;
            // 
            // btn_back
            // 
            btn_back.Location = new Point(38, 432);
            btn_back.Name = "btn_back";
            btn_back.Size = new Size(94, 29);
            btn_back.TabIndex = 7;
            btn_back.Text = "Quay Về";
            btn_back.UseVisualStyleBackColor = true;
            btn_back.Click += btn_back_Click;
            // 
            // Lobby
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1080, 576);
            Controls.Add(btn_back);
            Controls.Add(tbRoomName);
            Controls.Add(label2);
            Controls.Add(lblRoomName);
            Controls.Add(lstRooms);
            Controls.Add(btnReset);
            Controls.Add(btnCreateRoom);
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
    }
}