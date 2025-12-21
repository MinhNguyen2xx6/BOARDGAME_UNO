using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class NetworkClient
{
    private TcpClient _client;
    private NetworkStream _stream;

    // Thêm sự kiện cho cả start và state
    public event Action<dynamic> OnStart;
    public event Action<dynamic> OnStateUpdate;

    public async Task ConnectAsync(string host, int port, string playerName)
    {
        _client = new TcpClient();
        await _client.ConnectAsync(host, port);
        _stream = _client.GetStream();

        // gửi join
        Send(new { type = "join", player = playerName });

        // lắng nghe
        _ = ListenLoop();
    }

    public void Send(object message)
    {
        string json = JsonConvert.SerializeObject(message);
        byte[] data = Encoding.UTF8.GetBytes(json);
        _stream.Write(data, 0, data.Length);
    }

    private async Task ListenLoop()
    {
        byte[] buffer = new byte[8192];
        while (_client.Connected)
        {
            int read = await _stream.ReadAsync(buffer, 0, buffer.Length);
            if (read <= 0) break;

            string json = Encoding.UTF8.GetString(buffer, 0, read);
            dynamic msg = JsonConvert.DeserializeObject(json);
            string type = msg.type;

            if (type == "state")
                OnStateUpdate?.Invoke(msg);
            else if (type == "start")
                OnStart?.Invoke(msg);
        }
    }
}
