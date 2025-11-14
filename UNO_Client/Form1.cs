using System.Text;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
namespace UNO_Client
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string apiKey = "AIzaSyDX-dOg8L8LSML3hlyy14IlHOKidTKr5vw"; //key api
        private Giaodienchinh _mainForm;
        public Form1(Giaodienchinh mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
        }


        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                var saved = TokenStorage.LoadToken();

                if (saved != null && !string.IsNullOrEmpty(saved.RefreshToken))
                {
                    // Thử auto login
                    var success = await AutoLogin(saved.RefreshToken);

                    // Thông báo kết quả cho form chính
                    _mainForm.HandleAutoLoginResult(success);

                    if (success)
                    {
                        this.Hide(); // Ẩn form login
                        return;
                    }
                    else
                    {
                        // Auto login thất bại, hiển thị thông báo
                        MessageBox.Show("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi auto login: {ex.Message}");
                // Tiếp tục hiển thị form login bình thường
            }
        }

        private async void btn_dk_Click(object sender, EventArgs e)
        {
            string email = tb_taikhoan.Text;
            string password = tb_matkhau.Text; // đưa tk mk từ text box vào thành chuỗi
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }
            // kiểm tra xem chuỗi có rỗng hay không
            try
            {
                var user = new { email = email, password = password, returnSecureToken = true }; // tạo một biến user mang thông tin
                string json = JsonConvert.SerializeObject(user); // chuyển hóa thành file json
                var content = new StringContent(json, Encoding.UTF8, "application/json"); // tạo biến content để gửi đi

                var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}", content); //gửi yêu cầu đăng ký tới fire base
                var data = await response.Content.ReadAsStringAsync(); // dữ liệu phản hồi của fire base

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Tạo tài khoản thành công!");
                }
                else
                {
                    dynamic error = JsonConvert.DeserializeObject(data);
                    MessageBox.Show("Lỗi: " + error["error"]["message"]);
                }
                //kiểm tra kết quả trả về
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
            //trường hợp lõi kết nối
        }

        private async void btn_dangnhap_Click(object sender, EventArgs e)
        {
            string email = tb_taikhoan.Text;
            string password = tb_matkhau.Text; // đưa tk mk từ text box vào thành chuỗi
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }
            // kiểm tra xem chuỗi có rỗng hay không
            try
            {
                var user = new { email = email, password = password, returnSecureToken = true }; // tạo một biến user mang thông tin
                string json = JsonConvert.SerializeObject(user); // chuyển hóa thành file json
                var content = new StringContent(json, Encoding.UTF8, "application/json"); // tạo biến content để gửi đi
                var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}", content); // gửi yêu cầu đăng nhập tới firebase
                var data = await response.Content.ReadAsStringAsync(); // dữ liệu phản hồi của fire base
                if (response.IsSuccessStatusCode)
                {
                    dynamic result = JsonConvert.DeserializeObject(data);

                    //  Lưu token vào Session
                    Session.IdToken = result.idToken;
                    Session.RefreshToken = result.refreshToken;
                    Session.UserEmail = result.email;

                    //Lưu token vào storage
                    TokenStorage.SaveToken(new LocalToken
                    {
                        IdToken = result.idToken,
                        RefreshToken = result.refreshToken,
                        Email = result.email
                    });
                    MessageBox.Show("Đăng nhập thành công!,Chuyển sang giao diện lobby ");
                    _mainForm.UpdateUI(); // Cập nhật UI form chính
                    _mainForm.Show();     // Hiển thị form chính
                    this.Hide();          // Ẩn form login
                }
                else
                {
                    dynamic error = JsonConvert.DeserializeObject(data);
                    MessageBox.Show("Sai mật khẩu, tài khoản không chính xác");           //đưa ra lỗi nếu không đăng nhập thành công
                }
                //kiểm tra kết quả trả về
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
            //trường hợp lõi kết nối
        }

        private async void btn_quenmk_Click(object sender, EventArgs e)
        {
            string email = tb_taikhoan.Text;
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập email để khôi phục!");       // kiểm tra chuỗi có trống hay không 
                return;
            }
            try
            {
                var user = new { requestType = "PASSWORD_RESET", email = email }; // tạo dữ liệu gửi lên fire base trong đó requestType là yêu cầu 
                string json = JsonConvert.SerializeObject(user);// chuyển biến user thành 1 file json để gửi đi
                var content = new StringContent(json, Encoding.UTF8, "application/json"); // chuyển thành biến chính thức để gửi 
                var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}", content); // gửi yêu cầu đến fire base

                if (response.IsSuccessStatusCode)
                    MessageBox.Show("Đã gửi email khôi phục mật khẩu!");
                else
                    MessageBox.Show("Không thể gửi yêu cầu khôi phục!");
                // kết quả trả về sau khi thực hiện
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            // lỗi tổng quát ( do mất mạng hoặc fire base không phản hồi;
        }
        public async Task<bool> AutoLogin(string refreshToken)//Hàm nhận vào một chuỗi refreshToken để dùng khi gọi API refresh token của Firebase
        {
            var content = new StringContent(
                $"grant_type=refresh_token&refresh_token={refreshToken}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded"
            );

            var response = await client.PostAsync(
                $"https://securetoken.googleapis.com/v1/token?key={apiKey}",
                content
            );

            if (!response.IsSuccessStatusCode) return false;

            var data = JsonConvert.DeserializeObject<dynamic>(
                await response.Content.ReadAsStringAsync()
            );

            Session.IdToken = data.id_token;
            Session.RefreshToken = data.refresh_token;
            Session.UserEmail = data.email;

            // cập nhật token mới vào file
            TokenStorage.SaveToken(new LocalToken
            {
                IdToken = data.id_token,
                RefreshToken = data.refresh_token,
                Email = data.email
            });

            return true;
        }
    }
}
