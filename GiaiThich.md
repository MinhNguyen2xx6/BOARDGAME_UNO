Những tính năng hiện có
Kết nối TCP

Server lắng nghe trên port 5000.

Cho phép tối đa 4 client kết nối.

Sau khi đủ người chơi thì bắt đầu game.

Khởi tạo game

Mỗi người chơi được chia 7 lá.

Rút một lá làm TopCard.

Hành động từ client

"join": thêm người chơi vào phòng.

"play": đánh lá bài.

"draw": rút một lá.

"uno": gọi UNO.

Luật chơi cơ bản

Kiểm tra lá hợp lệ (CanPlay).

Áp dụng hiệu ứng đặc biệt: Skip, Reverse, DrawTwo, WildDrawFour.

Kiểm tra UNO và thắng.

🔧 Các chỉnh sửa bạn đã thêm
Wild/Wild+4 chọn màu

Client gửi thêm chosenColor.

Server cập nhật TopCard.Color theo màu được chọn.

✅ Hoạt động đúng.

DrawTwo/Wild+4

Người kế tiếp rút 2 hoặc 4 lá.

Không mất lượt ngay, vẫn có thể đánh tiếp.

✅ Đã sửa trong ApplySpecialEffect.

Rút bài thường (draw)

Người chơi rút 1 lá.

Không chuyển lượt ngay, có thể đánh tiếp nếu hợp lệ.

✅ Đã sửa trong HandleDraw.
