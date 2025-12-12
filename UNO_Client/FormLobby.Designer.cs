namespace UNO_Client
{
    partial class FormLobby
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
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(components);
            btn_createroom = new Guna.UI2.WinForms.Guna2Button();
            tbRoomName = new Guna.UI2.WinForms.Guna2TextBox();
            btn_create = new Guna.UI2.WinForms.Guna2Button();
            btn_refresh = new Guna.UI2.WinForms.Guna2Button();
            btn_back = new Guna.UI2.WinForms.Guna2Button();
            flowRooms = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // guna2Elipse1
            // 
            guna2Elipse1.BorderRadius = 10;
            guna2Elipse1.TargetControl = this;
            // 
            // btn_createroom
            // 
            btn_createroom.CustomizableEdges = customizableEdges9;
            btn_createroom.DisabledState.BorderColor = Color.DarkGray;
            btn_createroom.DisabledState.CustomBorderColor = Color.DarkGray;
            btn_createroom.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btn_createroom.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btn_createroom.Font = new Font("Segoe UI", 9F);
            btn_createroom.ForeColor = Color.White;
            btn_createroom.Location = new Point(12, 23);
            btn_createroom.Name = "btn_createroom";
            btn_createroom.ShadowDecoration.CustomizableEdges = customizableEdges10;
            btn_createroom.Size = new Size(193, 43);
            btn_createroom.TabIndex = 1;
            btn_createroom.Text = "Tạo Phòng";
            btn_createroom.Click += btn_createroom_Click;
            // 
            // tbRoomName
            // 
            tbRoomName.CustomizableEdges = customizableEdges7;
            tbRoomName.DefaultText = "";
            tbRoomName.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            tbRoomName.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            tbRoomName.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            tbRoomName.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            tbRoomName.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            tbRoomName.Font = new Font("Segoe UI", 9F);
            tbRoomName.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            tbRoomName.Location = new Point(231, 23);
            tbRoomName.Margin = new Padding(3, 4, 3, 4);
            tbRoomName.Name = "tbRoomName";
            tbRoomName.PlaceholderText = "";
            tbRoomName.SelectedText = "";
            tbRoomName.ShadowDecoration.CustomizableEdges = customizableEdges8;
            tbRoomName.Size = new Size(200, 43);
            tbRoomName.TabIndex = 2;
            tbRoomName.Visible = false;
            // 
            // btn_create
            // 
            btn_create.CustomizableEdges = customizableEdges5;
            btn_create.DisabledState.BorderColor = Color.DarkGray;
            btn_create.DisabledState.CustomBorderColor = Color.DarkGray;
            btn_create.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btn_create.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btn_create.Font = new Font("Segoe UI", 9F);
            btn_create.ForeColor = Color.White;
            btn_create.Location = new Point(12, 23);
            btn_create.Name = "btn_create";
            btn_create.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btn_create.Size = new Size(193, 43);
            btn_create.TabIndex = 3;
            btn_create.Text = "Tạo Phòng";
            btn_create.Visible = false;
            btn_create.Click += btn_create_Click;
            // 
            // btn_refresh
            // 
            btn_refresh.CustomizableEdges = customizableEdges3;
            btn_refresh.DisabledState.BorderColor = Color.DarkGray;
            btn_refresh.DisabledState.CustomBorderColor = Color.DarkGray;
            btn_refresh.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btn_refresh.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btn_refresh.Font = new Font("Segoe UI", 9F);
            btn_refresh.ForeColor = Color.White;
            btn_refresh.Location = new Point(12, 405);
            btn_refresh.Name = "btn_refresh";
            btn_refresh.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btn_refresh.Size = new Size(134, 34);
            btn_refresh.TabIndex = 4;
            btn_refresh.Text = "Refresh";
            btn_refresh.Click += btn_refresh_Click;
            // 
            // btn_back
            // 
            btn_back.CustomizableEdges = customizableEdges1;
            btn_back.DisabledState.BorderColor = Color.DarkGray;
            btn_back.DisabledState.CustomBorderColor = Color.DarkGray;
            btn_back.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btn_back.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btn_back.Font = new Font("Segoe UI", 9F);
            btn_back.ForeColor = Color.White;
            btn_back.Location = new Point(654, 405);
            btn_back.Name = "btn_back";
            btn_back.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btn_back.Size = new Size(134, 34);
            btn_back.TabIndex = 5;
            btn_back.Text = "Quay về";
            btn_back.Click += btn_back_Click;
            // 
            // flowRooms
            // 
            flowRooms.Location = new Point(12, 99);
            flowRooms.Name = "flowRooms";
            flowRooms.Size = new Size(776, 277);
            flowRooms.TabIndex = 6;
            flowRooms.Paint += flowRooms_Paint_1;
            flowRooms.Dock = DockStyle.None;
            flowRooms.Size = new Size(776, 277);
            flowRooms.Location = new Point(12, 99);
            flowRooms.Padding=new Padding(30);
            flowRooms.AutoScroll = true;
            flowRooms.FlowDirection = FlowDirection.TopDown;
            flowRooms.WrapContents = false;
            // 
            // FormLobby
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(34, 33, 74);
            ClientSize = new Size(800, 450);
            Controls.Add(flowRooms);
            Controls.Add(btn_back);
            Controls.Add(btn_refresh);
            Controls.Add(btn_create);
            Controls.Add(tbRoomName);
            Controls.Add(btn_createroom);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormLobby";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FormLobby";
            Load += FormLobby_Load;
            ResumeLayout(false);
        }

        #endregion
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2TextBox tbRoomName;
        private Guna.UI2.WinForms.Guna2Button btn_createroom;
        private Guna.UI2.WinForms.Guna2Button btn_create;
        private Guna.UI2.WinForms.Guna2Button btn_refresh;
        private Guna.UI2.WinForms.Guna2Button btn_back;
        private FlowLayoutPanel flowRooms;
    }
}