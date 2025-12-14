using Newtonsoft.Json;
using System.IO;

namespace UNO_Client_WPF.Helpers
{
    public class LocalToken
    {
        // Thêm dấu ? để sửa lỗi Non-nullable
        public string? IdToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? Email { get; set; }
    }

    public static class TokenStorage
    {
        private static string path = "token.json";

        public static void SaveToken(LocalToken token)
        {
            string json = JsonConvert.SerializeObject(token);
            File.WriteAllText(path, json);
        }

        public static LocalToken? LoadToken() // Trả về LocalToken? (có thể null)
        {
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<LocalToken>(json);
        }

        public static void Clear()
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}