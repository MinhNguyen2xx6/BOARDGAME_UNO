namespace UNO_Client
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            // Ki?m tra token tr??c khi t?o form
            Giaodienchinh main = new Giaodienchinh();
            Form1 login = new Form1(main);

            // Load token
            var saved = TokenStorage.LoadToken();

            if (saved != null && !string.IsNullOrEmpty(saved.RefreshToken))
            {
                // Gán tạm để giao diện chính nhận biết
                Session.RefreshToken = saved.RefreshToken;
                Session.UserEmail = saved.Email;

                // Hiển thị form chính (auto login chạy tại Form1_Load)
                Application.Run(main);
            }
            else
            {
                // Không có token → chạy thẳng form login
                Application.Run(login);
            }
        }
    }
}