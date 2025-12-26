using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions; // Dùng để kiểm tra định dạng email
using System.Windows;
using System.Windows.Input; // Dùng cho sự kiện MouseLeftButtonDown
using UNO_Client_WPF.Helpers;

namespace UNO_Client_WPF
{
    public partial class LoginWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();

        // API Key Firebase của bạn
        private const string ApiKey = "AIzaSyDX-dOg8L8LSML3hlyy14IlHOKidTKr5vw";

        // Đường dẫn file lưu cấu hình
        private string PrefsPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "login_prefs.json");

        public LoginWindow()
        {
            InitializeComponent();
            LoadRememberedUser();
        }

        // --- 1. HỆ THỐNG REMEMBER ME ---
        private void LoadRememberedUser()
        {
            try
            {
                if (File.Exists(PrefsPath))
                {
                    string json = File.ReadAllText(PrefsPath);
                    var prefs = JsonConvert.DeserializeObject<dynamic>(json);
                    if (prefs != null)
                    {
                        tb_email.Text = prefs.Email;
                        if (prefs.IsRemember != null) cb_remember.IsChecked = (bool)prefs.IsRemember;
                    }
                }
            }
            catch { }
        }

        private void SaveRememberMe(string email, bool isRemember)
        {
            try
            {
                if (isRemember)
                {
                    var prefs = new { Email = email, IsRemember = true };
                    File.WriteAllText(PrefsPath, JsonConvert.SerializeObject(prefs));
                }
                else if (File.Exists(PrefsPath))
                {
                    File.Delete(PrefsPath);
                }
            }
            catch { }
        }

        // --- 2. CHỨC NĂNG ĐĂNG NHẬP ---
        private async void btn_login_Click(object sender, RoutedEventArgs e)
        {
            string email = tb_email.Text.Trim();
            string password = pb_password.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var user = new { email = email, password = password, returnSecureToken = true };
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                // API: signInWithPassword
                var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}", content);
                var data = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic result = JsonConvert.DeserializeObject(data);

                    // Lưu Session
                    Session.IdToken = (string)result.idToken;
                    Session.RefreshToken = (string)result.refreshToken;
                    Session.UserEmail = (string)result.email;

                    // Lưu TokenStorage
                    TokenStorage.SaveToken(new LocalToken
                    {
                        IdToken = (string)result.idToken,
                        RefreshToken = (string)result.refreshToken,
                        Email = (string)result.email
                    });

                    // Lưu Remember Me
                    SaveRememberMe(email, cb_remember.IsChecked == true);

                    MessageBox.Show("Đăng nhập thành công!", "Thông báo");

                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();
                }
                else
                {
                    ShowFirebaseError(data, "Đăng nhập");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
        }

        // --- 3. CHỨC NĂNG ĐĂNG KÝ ---
        private async void btn_register_Click(object sender, RoutedEventArgs e)
        {
            string email = tb_email.Text.Trim();
            string password = pb_password.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập Email và Mật khẩu để đăng ký!", "Thông báo");
                return;
            }

            // Kiểm tra định dạng email
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không hợp lệ!", "Lỗi");
                return;
            }

            // Kiểm tra độ dài mật khẩu (Firebase yêu cầu tối thiểu 6 ký tự)
            if (password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi");
                return;
            }

            try
            {
                var user = new { email = email, password = password, returnSecureToken = true };
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                // API: signUp
                var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={ApiKey}", content);
                var data = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Tạo tài khoản thành công! Bạn có thể đăng nhập ngay.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ShowFirebaseError(data, "Đăng ký");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
        }

        // --- 4. CHỨC NĂNG QUÊN MẬT KHẨU ---
        private async void btn_forgot_pass_Click(object sender, MouseButtonEventArgs e)
        {
            string email = tb_email.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập Email của bạn vào ô bên trên để lấy lại mật khẩu!", "Quên mật khẩu", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                // Request Type: PASSWORD_RESET
                var request = new { requestType = "PASSWORD_RESET", email = email };
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                // API: sendOobCode
                var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={ApiKey}", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Đã gửi email khôi phục mật khẩu đến: {email}\nVui lòng kiểm tra hộp thư (cả mục Spam).", "Đã gửi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ShowFirebaseError(data, "Khôi phục mật khẩu");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
        }

        // --- 5. HÀM XỬ LÝ LỖI CHUNG ---
        private void ShowFirebaseError(string jsonResponse, string actionName)
        {
            try
            {
                dynamic errorJson = JsonConvert.DeserializeObject(jsonResponse);
                string errorCode = errorJson.error.message;
                string msg = $"{actionName} thất bại: {errorCode}";

                // Dịch lỗi sang tiếng Việt
                if (errorCode.Contains("EMAIL_NOT_FOUND")) msg = "Email không tồn tại.";
                else if (errorCode.Contains("INVALID_PASSWORD")) msg = "Mật khẩu không đúng.";
                else if (errorCode.Contains("EMAIL_EXISTS")) msg = "Email này đã được đăng ký rồi.";
                else if (errorCode.Contains("WEAK_PASSWORD")) msg = "Mật khẩu quá yếu (cần > 6 ký tự).";
                else if (errorCode.Contains("INVALID_EMAIL")) msg = "Định dạng email không hợp lệ.";
                else if (errorCode.Contains("USER_DISABLED")) msg = "Tài khoản đã bị khóa.";
                else if (errorCode.Contains("TOO_MANY_ATTEMPTS")) msg = "Thử lại quá nhiều lần. Vui lòng đợi.";

                MessageBox.Show(msg, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("Có lỗi xảy ra: " + jsonResponse);
            }
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}