using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace UNO_Client
{
    public partial class Lobby : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string firebaseUrl = "https://doan-36be7-default-rtdb.asia-southeast1.firebasedatabase.app";

        private Giaodienchinh _mainForm;
        private Dictionary<string, RoomInfo> _roomsCache = new Dictionary<string, RoomInfo>();
        private System.Windows.Forms.Timer _refreshTimer;
        private string _currentRoomKey = null;

        public Lobby(Giaodienchinh mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
        }

        private async void Lobby_Load(object sender, EventArgs e)
        {
            await LoadRooms();

            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 5000;
            _refreshTimer.Tick += async (s, ev) => await LoadRooms();
            _refreshTimer.Start();
        }

        // ================== Firebase helper ==================
        private async Task<RoomInfo> GetRoom(string roomName)
        {
            var response = await client.GetAsync($"{firebaseUrl}/rooms/{roomName}.json?auth={Session.IdToken}");
            if (!response.IsSuccessStatusCode) return null;
            string data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RoomInfo>(data);
        }

        private async Task<bool> SaveRoom(RoomInfo room)
        {
            string json = JsonConvert.SerializeObject(room);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{firebaseUrl}/rooms/{room.name}.json?auth={Session.IdToken}", content);

            string body = await response.Content.ReadAsStringAsync();
            MessageBox.Show($"Status: {response.StatusCode}\nBody: {body}");
            MessageBox.Show($"IdToken: {Session.IdToken}");
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> DeleteRoom(string roomName)
        {
            var response = await client.DeleteAsync($"{firebaseUrl}/rooms/{roomName}.json?auth={Session.IdToken}");
            return response.IsSuccessStatusCode;
        }

        // ================== Load danh sách phòng ==================
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

                if (rooms != null)
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

        // ================== Nút Create ==================
        private async void btnCreateRoom_Click(object sender, EventArgs e)
        {
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

            // ✅ Lưu email vào danh sách Players
            var room = new RoomInfo
            {
                name = name,
                ThoiGianTao = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                Players = new List<string> { Session.UserEmail }
            };

            if (await SaveRoom(room))
            {
                MessageBox.Show("Tạo phòng thành công!");
                _currentRoomKey = name;

                FormGame gameForm = new FormGame(_currentRoomKey, room.Players);
                gameForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Không thể tạo phòng!");
            }
        }

        // ================== Nút Join ==================
        private async void btnJoin_Click(object sender, EventArgs e)
        {

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

            if (room.Players == null) room.Players = new List<string>();

            // ✅ Luôn dùng email để join
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

            if (await SaveRoom(room))
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
                MessageBox.Show("Không thể tham gia phòng!");
            }
        }

        // ================== Nút Exit ==================
        private async void btnExitRoom_Click(object sender, EventArgs e)
        {
            if (_currentRoomKey == null)
            {
                MessageBox.Show("Bạn chưa ở trong phòng nào!");
                return;
            }

            var room = await GetRoom(_currentRoomKey);
            if (room == null || room.Players == null)
            {
                _currentRoomKey = null;
                return;
            }

            room.Players.Remove(Session.UserEmail);

            if (room.Players.Count == 0)
            {
                await DeleteRoom(_currentRoomKey);
                MessageBox.Show("Phòng đã bị xóa vì không còn người chơi!");
            }
            else
            {
                await SaveRoom(room);
            }

            _currentRoomKey = null;
            await LoadRooms();
        }

        // ================== Nút Delete ==================
        private async void btnDeleteRoom_Click(object sender, EventArgs e)
        {
            if (lstRooms.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn phòng để xóa!");
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

            if (room.Players == null || room.Players.Count == 0 || room.Players[0] != Session.UserEmail)
            {
                MessageBox.Show("Chỉ người tạo phòng mới có quyền xóa!");
                return;
            }

            if (await DeleteRoom(room.name))
            {
                MessageBox.Show($"Phòng {room.name} đã được xóa!");
                await LoadRooms();
            }
            else
            {
                MessageBox.Show("Không thể xóa phòng!");
            }
        }

        // ================== Nút Back ==================
        private void btnBack_Click(object sender, EventArgs e)
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
            _refreshTimer = null;

            this.Close();
            _mainForm?.Show();
        }
        private void btnBreset(object sender, EventArgs e)
        {

        }
    }

    public class RoomInfo
    {
        public string name { get; set; }
        public string ThoiGianTao { get; set; }
        public List<string> Players { get; set; }
    }
}
