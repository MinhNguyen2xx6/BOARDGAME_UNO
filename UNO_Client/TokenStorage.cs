using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO_Client
{
    public class LocalToken
    {
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
        public string Email { get; set; }
    }

    public static class TokenStorage
    {
        private static string path = "token.json";//Lưu cạnh file exe

        //Sau khi đăng nhập thì lưu lại token
        public static void SaveToken(LocalToken token)
        {
            string json = JsonConvert.SerializeObject(token);
            File.WriteAllText(path, json);
        }

        //Load token 
        public static LocalToken LoadToken()
        {
            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<LocalToken>(json);
        }
        
        //use when logout
        public static void Clear()
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
