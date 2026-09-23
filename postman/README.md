# 📮 BookManager Postman Test Suite (Format v3 - YAML Based)

Thư mục này chứa toàn bộ tài nguyên kiểm thử API được cấu trúc theo chuẩn **Postman v3 Format (File-based YAML)** mới nhất của Postman.

---

## 📁 Cấu Trúc Thư Mục Chuẩn v3

```
postman/
├── collections/
│   └── BookManager API/
│       ├── 1. Authentication/
│       │   ├── Register (Đăng ký tài khoản).request.yaml
│       │   ├── Login (Đăng nhập - Auto Save Tokens).request.yaml
│       │   └── Refresh Token (Làm mới Token).request.yaml
│       └── 2. Books Management/
│           ├── Create Book (Tạo sách mới).request.yaml
│           ├── Get All Books (With Search, Sort & Filter).request.yaml
│           ├── Get Book By ID.request.yaml
│           ├── Update Book (Cập nhật sách).request.yaml
│           ├── Delete Book (Xóa sách).request.yaml
│           └── Search Books (Tìm kiếm theo từ khóa).request.yaml
│
├── environments/
│   ├── BookManager - Local Environment.environment.yaml       # Môi trường Local (http://localhost:8080)
│   └── BookManager - Production Environment.environment.yaml  # Môi trường Production
│
├── globals/
│   └── workspace.globals.yaml                                # Các biến toàn cục
└── README.md
```

### 💡 Lợi ích của Postman Format v3 (YAML):
- **Tương thích 100% với Git**: Mỗi Request và Environment được tách thành 1 file YAML riêng biệt, giúp việc commit, diff và giải quyết merge conflict khi làm việc nhóm cực kỳ dễ dàng.
- **Tự động nhận diện (Native Workspace)**: Postman tự động đọc toàn bộ cây thư mục này khi mở workspace.

---

## 🚀 Hướng Dẫn Sử Dụng Postman v3 Từng Bước

### Bước 1: Mở Workspace Trong Postman
- Nếu Postman của bạn đã kết nối thư mục dự án: Toàn bộ Collection **`BookManager API`** và các Environment đã sẵn sàng ở thanh menu bên trái.
- Nếu bạn muốn nạp lại: Bấm **Import** (`Ctrl + O`) ➔ Kéo thả cả thư mục `postman/` vào Postman.

---

### Bước 2: Chọn Môi Trường (Environment)
1. Ở góc trên cùng bên phải màn hình Postman, click vào ô chọn Environment.
2. Chọn: **`BookManager - Local Environment`**.
3. Biến `{{baseUrl}}` sẽ tự động kích hoạt giá trị `http://localhost:8080`.

---

### Bước 3: Thứ Tự Thực Hiện Kiểm Thử (Test Flow)

#### 1. Đăng ký tài khoản (`Register`)
- Mở `1. Authentication` ➔ Chọn **`Register (Đăng ký tài khoản)`** ➔ Bấm **Send**.
- Test Script tự động kiểm tra status `200 OK`.

#### 2. Đăng nhập (`Login`) - *Tự động lưu Token*
- Chọn **`Login (Đăng nhập - Auto Save Tokens)`** ➔ Bấm **Send**.
- ✨ **Tự động hóa**: Đoạn script `afterResponse` trong file `Login...request.yaml` sẽ tự động bắt `accessToken` & `refreshToken` và gán vào Environment. Bạn **không cần copy token thủ công**.

#### 3. Quản lý Sách (`2. Books Management`)
- **`Create Book (Tạo sách mới)`**: Bấm **Send** ➔ Tự động lưu ID sách vào biến `{{createdBookId}}`.
- **`Get All Books`**: Bấm **Send** ➔ Lấy danh sách sách (kèm bộ lọc giá, tìm kiếm).
- **`Get Book By ID`**: Bấm **Send** ➔ Lấy đúng sách theo `{{createdBookId}}`.
- **`Update Book`**: Bấm **Send** ➔ Cập nhật thông tin sách theo `{{createdBookId}}`.
- **`Delete Book`**: Bấm **Send** ➔ Xóa sách theo `{{createdBookId}}`.

#### 4. Làm mới Token (`Refresh Token`)
- Khi token 15 phút hết hạn, chọn **`Refresh Token`** ➔ Bấm **Send** để cấp mới token.

---

## 🤖 Chạy Tự Động Toàn Bộ (Collection Runner)

1. Click vào Collection **`BookManager API`** ở menu bên trái.
2. Bấm nút **Run** (hoặc chuột phải chọn **Run collection**).
3. Đảm bảo đã chọn Environment: **`BookManager - Local Environment`**.
4. Bấm **Run BookManager API** ➔ Toàn bộ các test case sẽ tự động chạy và trả về kết quả **100% Pass**.
