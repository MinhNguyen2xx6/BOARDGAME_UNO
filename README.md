# BOARDGAME_UNO
## Hướng dẫn cấu hình, cài đặt và chạy app
### 1. Yêu cầu hệ thống
-Hệ điều hành: Windows 10/11.

-Ngôn ngữ: C# (.NET Framework 4.8).

-IDE: Visual Studio 2022 hoặc mới hơn.

-Thư viện ngoài:

+System.Net.Sockets

+Newtonsoft.Json

-Kết nối mạng: Cùng mạng LAN.

### 2. Clone source code
```
git clone https://github.com/MinhNguyen2xx6/BOARDGAME_UNO.git
cd BOARDGAME_UNO
```
3. Cấu trúc project
+ UNO_Server – chương trình Server (Console App).
+ UNO_Client – chương trình Client (WinForms).
- Mở file BOARDGAME_UNO.sln bằng Visual Studio.

4. Cấu hình mạng
- Ứng dụng sử dụng TCP Socket nên cần thiết lập IP và Port cho kết nối.

5. Cách chạy chương trình
- Bước 1: Chạy Server
+ Trong Visual Studio, chuột phải UNO_Server → Set as Startup Project.
+ Nhấn Ctrl + F5 để chạy.

