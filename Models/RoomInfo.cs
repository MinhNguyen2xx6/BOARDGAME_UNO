using System.Collections.Generic;

namespace UNO_Client_WPF.Models // Chú ý namespace
{
    public class RoomInfo
    {
        // Thêm dấu ? vào sau string và List để cho phép null
        public string? name { get; set; }
        public string? ThoiGianTao { get; set; }
        public List<string>? Players { get; set; }

        public string PlayerCountDisplay => $"Người chơi: {Players?.Count ?? 0}/4";
    }
}