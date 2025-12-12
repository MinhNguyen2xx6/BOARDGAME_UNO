using Newtonsoft.Json;
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
    public partial class FormLobby : Form
    {

        private static readonly HttpClient client = new HttpClient();
        private static readonly string firebaseUrl = "https://doan-36be7-default-rtdb.asia-southeast1.firebasedatabase.app";

        private Giaodienchinh _mainForm;

        // Cache phòng để hiển thị và xử lý JOIN
        private Dictionary<string, RoomInfo> _roomsCache = new Dictionary<string, RoomInfo>();

        // Timer tự động refresh danh sách phòng
        private System.Windows.Forms.Timer _refreshTimer;

        // Biến lưu phòng hiện tại mà user đang ở
        private string _currentRoomKey = null;

        public FormLobby(Giaodienchinh mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
        }
        private async Task LoadRooms()
        {
            var response = await client.GetAsync($"{firebaseUrl}/rooms.json?auth={Session.IdToken}");
            if (!response.IsSuccessStatusCode) return;

            string data = await response.Content.ReadAsStringAsync();

            Dictionary<string, RoomInfo> rooms;

            // Nếu Firebase trả về dạng ARRAY → xử lý đặc biệt
            if (data.TrimStart().StartsWith("["))
            {
                var arr = JsonConvert.DeserializeObject<List<RoomInfo>>(data);

                rooms = arr
                    .Where(r => r != null && !string.IsNullOrWhiteSpace(r.name))
                    .ToDictionary(r => r.name, r => r);
            }
            else
            {
                // Nếu là OBJECT → deserialize như cũ
                rooms = JsonConvert.DeserializeObject<Dictionary<string, RoomInfo>>(data);
            }


            flowRooms.Controls.Clear();
            _roomsCache.Clear();

            if (rooms != null)
            {
                foreach (var kvp in rooms)
                {
                    var room = kvp.Value;
                    if (room == null) continue;

                    int currentPlayers = room.Players?.Count ?? 0;

                    // Panel phòng
                    var roomPanel = new Guna.UI2.WinForms.Guna2ShadowPanel
                    {
                        Width = 300,
                        Height = 100,
                        Radius = 10,
                        ShadowColor = Color.Black,
                        ShadowDepth = 10,
                        BackColor = ColorTranslator.FromHtml("#1B263B"),
                        Margin = new Padding(10)
                    };

                    // Label tên phòng
                    var lblName = new Label
                    {
                        Text = $"{room.name} ({currentPlayers}/4)",
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        ForeColor = Color.White,
                        Location = new Point(15, 15),
                        AutoSize = true
                    };


                    // Nút Join
                    var btnJoin = new Guna.UI2.WinForms.Guna2Button
                    {
                        Text = "Join",
                        Width = 80,
                        Height = 35,
                        Location = new Point(15, 50),
                        BorderRadius = 8,
                        FillColor = Color.MediumSlateBlue,
                        ForeColor = Color.White
                    };

                    btnJoin.Click += async (s, e) => await JoinRoom(room);

                    roomPanel.Controls.Add(lblName);
                    roomPanel.Controls.Add(btnJoin);

                    flowRooms.Controls.Add(roomPanel);

                    _roomsCache[room.name] = room;
                }
            }
        }

        private async Task JoinRoom(RoomInfo room)
        {
            if (!string.IsNullOrEmpty(_currentRoomKey))
            {
                MessageBox.Show("Bạn đang ở trong một phòng, hãy thoát trước khi tham gia phòng khác!");
                return;
            }

            room.Players ??= new List<string>();

            if (room.Players.Contains(Session.UserEmail))
            {
                MessageBox.Show("Bạn đã ở trong phòng này!");
                return;
            }

            if (room.Players.Count >= 4)
            {
                MessageBox.Show("Phòng đã đầy (4/4)!");
                return;
            }

            room.Players.Add(Session.UserEmail);

            try
            {
                string json = JsonConvert.SerializeObject(room);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(
                    $"{firebaseUrl}/rooms/{room.name}.json?auth={Session.IdToken}",
                    content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Tham gia phòng thành công!");
                    _currentRoomKey = room.name;
                    await LoadRooms();

                    FormGame gameForm = new FormGame(_currentRoomKey, room.Players);
                    gameForm.Show();
                    this.Hide();
                }
                else
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Không thể tham gia phòng!\n{response.StatusCode}\n{responseBody}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi join phòng: {ex.Message}");
            }
        }
        private async void FormLobby_Load(object sender, EventArgs e)
        {
            await LoadRooms();

            // Khởi tạo timer tự động refresh mỗi 5 giây
            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 5000; // 5 giây
            _refreshTimer.Tick += async (s, ev) => await LoadRooms();
            _refreshTimer.Start();
        }

        private void btn_createroom_Click(object sender, EventArgs e)
        {
            tbRoomName.Visible = true;
            btn_create.Visible = true;
            btn_createroom.Visible = false;
        }
        private async void btn_create_Click(object sender, EventArgs e)
        {
            // Nếu đang ở trong phòng thì không được tạo thêm
            if (_currentRoomKey != null)
            {
                MessageBox.Show("Bạn đang ở trong một phòng, hãy thoát trước khi tạo phòng mới!");
                return;
            }

            string name = tbRoomName.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng nhập tên phòng!");
                return;
            }

            tbRoomName.Text = "";

            var room = new RoomInfo
            {
                name = name,
                ThoiGianTao = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                Players = new List<string> { Session.UserEmail }
            };

            string json = JsonConvert.SerializeObject(room);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(
                $"{firebaseUrl}/rooms/{name}.json?auth={Session.IdToken}",
                content);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Tạo phòng thành công!");
                _currentRoomKey = name; // lưu phòng hiện tại
                await LoadRooms();
                FormGame gameForm = new FormGame(_currentRoomKey, room.Players);
                gameForm.Show();
                this.Hide();

            }
            else
            {
                MessageBox.Show("Không thể tạo phòng!");
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            MessageBox.Show($"Firebase phản hồi:\n{response.StatusCode}\n{responseBody}");
        }

        private async void btn_refresh_Click(object sender, EventArgs e)
        {
            await LoadRooms();
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            if (_refreshTimer != null)
            {
                _refreshTimer.Stop();
                _refreshTimer.Dispose();
                _refreshTimer = null;
            }

            this.Close();
            _mainForm?.Show();
        }

        private void flowRooms_Paint_1(object sender, PaintEventArgs e)
        {

        }
        public class RoomInfo
        {
            public string name { get; set; }
            public string ThoiGianTao { get; set; }
            public List<string> Players { get; set; }
        }

    }
}