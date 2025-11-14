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
            var token = TokenStorage.LoadToken();
            if (token != null)
            {
                // Có token ? t?o form chính tr??c
                Giaodienchinh main = new Giaodienchinh();
                Form1 login = new Form1(main);

                // Hi?n th? form chính ngay l?p t?c (s? ???c c?p nh?t sau khi auto login)
                main.Show();
                Application.Run(); // Ch?y application context thay vì form c? th?
            }
            else
            {
                // Không có token ? ch?y login bình th??ng
                Giaodienchinh main = new Giaodienchinh();
                Form1 login = new Form1(main);
                Application.Run(login);
            }
        }
    }
}