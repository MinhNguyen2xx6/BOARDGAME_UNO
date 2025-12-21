using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UNO_Client_WPF.Helpers;
using UNO_Client_WPF.Models;

namespace UNO_Client_WPF
{
    public partial class LobbyWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private const string FirebaseUrl = "https://doan-36be7-default-rtdb.asia-southeast1.firebasedatabase.app";
        private DispatcherTimer timer;

        public LobbyWindow()
        {
            InitializeComponent();
            LoadRooms();
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            timer.Tick += (s, e) => LoadRooms();
            timer.Start();
        }

        private async void LoadRooms()
        {
            try
            {
                var res = await client.GetAsync($"{FirebaseUrl}/rooms.json?auth={Session.IdToken}");
                if (!res.IsSuccessStatusCode) return;
                var data = await res.Content.ReadAsStringAsync();

                List<RoomInfo> rooms = new List<RoomInfo>();
                if (data.StartsWith("["))
                {
                    var arr = JsonConvert.DeserializeObject<List<RoomInfo>>(data);
                    if (arr != null) rooms = arr.Where(r => r != null).ToList();
                }
                else
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, RoomInfo>>(data);
                    if (dict != null) rooms = dict.Values.ToList();
                }
                ListRooms.ItemsSource = rooms;
            }
            catch { }
        }

        private async void btn_create_Click(object sender, RoutedEventArgs e)
        {
            var name = tbRoomName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng nhập tên phòng!");
                return;
            }

            //  Kiểm tra phòng đã tồn tại chưa
            var checkRes = await client.GetAsync(
                $"{FirebaseUrl}/rooms/{name}.json?auth={Session.IdToken}"
            );

            if (checkRes.IsSuccessStatusCode)
            {
                var content = await checkRes.Content.ReadAsStringAsync();
                if (content != "null")
                {
                    MessageBox.Show("Tên phòng đã tồn tại!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            //  Tạo phòng mới
            var room = new RoomInfo
            {
                name = name,
                Players = new List<string> { Session.UserEmail }
            };

            var json = JsonConvert.SerializeObject(room);

            await client.PutAsync(
                $"{FirebaseUrl}/rooms/{name}.json?auth={Session.IdToken}",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            OpenGame(name, room.Players);
        }


        private async void btn_join_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var room = btn.Tag as RoomInfo;

            if (room.Players == null)
                room.Players = new List<string>();

            // Kiểm tra trước khi add
            if (room.Players.Count >= 4)
            {
                MessageBox.Show("Phòng đã đủ 4 người!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Nếu chưa có thì mới add
            if (!room.Players.Contains(Session.UserEmail))
                room.Players.Add(Session.UserEmail);

            // Update server
            var json = JsonConvert.SerializeObject(room);
            await client.PutAsync(
                $"{FirebaseUrl}/rooms/{room.name}.json?auth={Session.IdToken}",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            OpenGame(room.name, room.Players);
        }


        private void OpenGame(string roomName, List<string> players)
        {
            timer.Stop();
            // chỉ truyền tên của người chơi hiện tại
            GameWindow game = new GameWindow(roomName, Session.UserEmail);
            game.Show();
            this.Close();
        }


        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            new MainWindow().Show();
            this.Close();
        }
    }
}