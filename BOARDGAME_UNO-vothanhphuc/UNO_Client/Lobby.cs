using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;

namespace UNO_Client
{
    public partial class Lobby : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string firebaseUrl = "https://doan-36be7-default-rtdb.asia-southeast1.firebasedatabase.app";

        public Lobby()
        {
            InitializeComponent();
        }
        private void btn_dangxuat_Click(object sender, EventArgs e)
        {
            Session.IdToken = null;
            Session.RefreshToken = null;
            Session.UserEmail = null;
            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Close();
        }

        private void Lobby_Load(object sender, EventArgs e)
        {

        }

        private async void btnCreateRoom_Click(object sender, EventArgs e)
        {
            string name = tbRoomName.Text.Trim();


            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng nhập tên phòng!");
                
                return;
            }
            tbRoomName.Text = "";



            var rooms = new
            {

                name = name,
                ThoiGianTao = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                Players = new string[] { Session.UserEmail }
            };

            string json = JsonConvert.SerializeObject(rooms);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await client.PutAsync(
                $"{firebaseUrl}/rooms/{name}.json?auth={Session.IdToken}",
                content);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Tạo phòng thành công!");
            }
            else
            {
                MessageBox.Show("Không thể tạo phòng!");
            }
        }


        private async void btnReset_Click(object sender, EventArgs e)
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
                var rooms = JsonConvert.DeserializeObject<Dictionary<string, Newtonsoft.Json.Linq.JObject>>(data);

                lstRooms.Items.Clear();
                if (rooms != null && rooms.Count > 0)
                {
                    foreach (var kvp in rooms)
                    {
                        string code = kvp.Key;
                        string name = kvp.Value["name"]?.ToString() ?? kvp.Key;


                        lstRooms.Items.Add($"{name}");
                    }
                }
                else
                {
                    MessageBox.Show("Hiện chưa có phòng nào!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách phòng!" + ex.Message);
            }
        }



    }
}

