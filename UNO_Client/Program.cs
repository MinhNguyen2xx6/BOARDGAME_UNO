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
            Giaodienchinh mainForm = new Giaodienchinh();

            // Tạo form đăng nhập, truyền form chính vào
            Form1 loginForm = new Form1(mainForm);

            // Chạy form đăng nhập trước
            Application.Run(loginForm);

        }
    }
}