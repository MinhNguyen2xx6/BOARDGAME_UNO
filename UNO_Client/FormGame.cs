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
    public partial class FormGame : Form

    {

        private string _roomName;
        private List<string> _players;
        private NetworkClient _net;
        private string _me;
        public FormGame(string roomName, List<string> players)
        {
            InitializeComponent();
            _roomName = roomName;
            _players = players;
            _me = Session.UserEmail; // hoặc tên người chơi
        }

        private async void FormGame_Load(object sender, EventArgs e)
        {
            _net = new NetworkClient();
            _net.OnStateUpdate += UpdateUIFromState;

            // kết nối tới server (ví dụ localhost:5000)
            await _net.ConnectAsync("127.0.0.1", 5000, _me);

            // nếu là host, gửi start
            if (_players.Count == 4) // hoặc 4 nếu chơi 4 người
                _net.Send(new { type = "start" });
        }

        private void UpdateUIFromState(dynamic state)
        {
            // cập nhật UI: top card, danh sách người chơi, tay bài của mình
            lblTopCard.Text = $"Top Card: {state.topCard.color} {state.topCard.value}";

            lstMyHand.Items.Clear();
            foreach (var p in state.players)
            {
                string line = $"{p.name} - {((IEnumerable<dynamic>)p.hand).Count()} lá";
                lstMyHand.Items.Add(line);
            }

            var meState = ((IEnumerable<dynamic>)state.players)
                          .FirstOrDefault(p => (string)p.name == _me);
            if (meState != null)
            {
                lstMyHand.Items.Clear();
                foreach (var c in meState.hand)
                    lstMyHand.Items.Add($"{c.color} {c.value}");
            }

            int currentIndex = (int)state.currentIndex;
            var players = ((IEnumerable<dynamic>)state.players).ToList();
            string currentName = (string)players[currentIndex].name;
            lblTurn.Text = currentName == _me ? "Lượt của bạn" : $"Lượt của {currentName}";
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {

            if (lstMyHand.SelectedItem == null) return;
            var parts = lstMyHand.SelectedItem.ToString().Split(' ');
            _net.Send(new { type = "play", player = _me, card = new { color = parts[0], value = parts[1] } });
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            _net.Send(new { type = "draw", player = _me });
        }

        private void btnUno_Click(object sender, EventArgs e)
        {
            _net.Send(new { type = "uno", player = _me });
        }
    }

}
