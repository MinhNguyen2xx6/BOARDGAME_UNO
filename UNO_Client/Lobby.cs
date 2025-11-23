using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace UNO_Client
{
    public partial class Lobby : Form
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

        public Lobby(Giaodienchinh mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
        }

        private async void Lobby_Load(object sender, EventArgs e)
        {
            await LoadRooms();

            // Khởi tạo timer tự động refresh mỗi 5 giây
            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 5000; // 5 giây
            _refreshTimer.Tick += async (s, ev) => await LoadRooms();
            _refreshTimer.Start();
        }

        private async void btnCreateRoom_Click(object sender, EventArgs e)
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
            }
            else
            {
                MessageBox.Show("Không thể tạo phòng!");
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            MessageBox.Show($"Firebase phản hồi:\n{response.StatusCode}\n{responseBody}");

        }

        private async void btnReset_Click(object sender, EventArgs e)
        {
            await LoadRooms();
        }

        private async Task LoadRooms()
        {
            try
            {
                var response = await client.GetAsync($"{firebaseUrl}/rooms.json?auth={Session.IdToken}");
                if (!response.IsSuccessStatusCode)
                {
                    string err = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Không thể tải danh sách phòng!\n{err}");
                    return;
                }

                string data = await response.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<Dictionary<string, RoomInfo>>(data);

                lstRooms.Items.Clear();
                _roomsCache.Clear();

                if (rooms != null && rooms.Count > 0)
                {
                    foreach (var kvp in rooms)
                    {
                        var key = kvp.Key;
                        var room = kvp.Value;
                        if (room == null) continue;

                        if (string.IsNullOrWhiteSpace(room.name))
                            room.name = key;

                        int currentPlayers = room.Players != null ? room.Players.Count : 0;
                        string display = $"{room.name} ({currentPlayers}/4)";

                        lstRooms.Items.Add(display);
                        _roomsCache[room.name] = room;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách phòng! " + ex.Message);
            }
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

        // Nút JOIN
        private async void button1_Click(object sender, EventArgs e)
        {
            // Nếu đang ở trong phòng thì không được join thêm
            if (_currentRoomKey != null)
            {
                MessageBox.Show("Bạn đang ở trong một phòng, hãy thoát trước khi tham gia phòng khác!");
                return;
            }

            if (lstRooms.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn phòng để tham gia!");
                return;
            }

            string selectedText = lstRooms.SelectedItem.ToString();
            int idx = selectedText.LastIndexOf('(');
            string roomName = idx > 0 ? selectedText.Substring(0, idx).Trim() : selectedText.Trim();

            if (!_roomsCache.TryGetValue(roomName, out var room))
            {
                MessageBox.Show("Không tìm thấy phòng!");
                return;
            }

            if (room.Players == null)
                room.Players = new List<string>();

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

            string json = JsonConvert.SerializeObject(room);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(
                $"{firebaseUrl}/rooms/{room.name}.json?auth={Session.IdToken}",
                content);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Tham gia phòng thành công!");
                _currentRoomKey = room.name; // lưu phòng hiện tại
                await LoadRooms();
                FormGame gameForm = new FormGame(_currentRoomKey, room.Players);
                gameForm.Show(); //Mở form trò chơi mới
                this.Hide();
            }
            else
            {
                MessageBox.Show("Không thể tham gia phòng!");
            }
        

        }

        // Nút Thoát phòng
        private async void button2_Click(object sender, EventArgs e)
        {
            if (_currentRoomKey == null)
            {
                MessageBox.Show("Bạn chưa ở trong phòng nào!");
                return;
            }

            var response = await client.GetAsync($"{firebaseUrl}/rooms/{_currentRoomKey}.json?auth={Session.IdToken}");
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Không thể tải dữ liệu phòng!");
                return;
            }

            string data = await response.Content.ReadAsStringAsync();
            var room = JsonConvert.DeserializeObject<RoomInfo>(data);

            if (room == null || room.Players == null)
            {
                _currentRoomKey = null;
                return;
            }

            room.Players.Remove(Session.UserEmail);

            if (room.Players.Count == 0)
            {
                var deleteResponse = await client.DeleteAsync($"{firebaseUrl}/rooms/{_currentRoomKey}.json?auth={Session.IdToken}");
                if (deleteResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("Phòng đã bị xóa vì không còn người chơi!");
                }
            }
            else
            {
                string json = JsonConvert.SerializeObject(room);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PutAsync($"{firebaseUrl}/rooms/{_currentRoomKey}.json?auth={Session.IdToken}", content);
            }

            _currentRoomKey = null;
            await LoadRooms();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (lstRooms.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn phòng để xóa!");
                return;
            }

            // Lấy tên phòng từ chuỗi hiển thị "TênPhòng (x/4)"
            string selectedText = lstRooms.SelectedItem.ToString();
            int idx = selectedText.LastIndexOf('(');
            string roomName = idx > 0 ? selectedText.Substring(0, idx).Trim() : selectedText.Trim();

            if (!_roomsCache.TryGetValue(roomName, out var room))
            {
                MessageBox.Show("Không tìm thấy phòng!");
                return;
            }

            // Kiểm tra quyền xóa: chỉ người tạo phòng (Players[0]) mới được xóa
            if (room.Players == null || room.Players.Count == 0 || room.Players[0] != Session.UserEmail)
            {
                MessageBox.Show("Chỉ người tạo phòng mới có quyền xóa!");
                return;
            }

            // Xóa phòng khỏi Firebase
            var response = await client.DeleteAsync($"{firebaseUrl}/rooms/{room.name}.json?auth={Session.IdToken}");
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show($"Phòng {room.name} đã được xóa!");
                await LoadRooms();
            }
            else
            {
                string err = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Không thể xóa phòng!\n{err}");
            }
        }
    }

    public class RoomInfo
    {
        public string name { get; set; }
        public string ThoiGianTao { get; set; }
        public List<string> Players { get; set; }
    }
}
