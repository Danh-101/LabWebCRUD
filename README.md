# Bài thực hành Lập trình Web: ASP.NET Core + CRUD + Axios + EF Core

Project quản lý sản phẩm với Frontend HTML/CSS/JavaScript và Backend ASP.NET Core 10. Dùng **SQLite** để không phải cài SQL Server, **Axios** để gọi REST API và **Entity Framework Core** để truy cập database.

## 1. Cấu trúc project

```text
LabWebCRUD/
├── .devcontainer/devcontainer.json        # Cho GitHub Codespaces (không cài SDK trên máy)
├── .config/dotnet-tools.json              # dotnet-ef phiên bản 10.0.13
├── backend/LabWebCrud.Api/
│   ├── Controllers/ProductsController.cs   # API GET/POST/PUT/DELETE
│   ├── Models/Product.cs                   # Entity Product
│   ├── DTOs/ProductRequest.cs              # Dữ liệu nhập POST/PUT + validation
│   ├── Data/AppDbContext.cs                # DbContext EF Core
│   ├── Migrations/                        # Migration tạo bảng và 3 dòng mẫu
│   ├── wwwroot/index.html                 # Giao diện Frontend
│   ├── wwwroot/css/styles.css
│   ├── wwwroot/js/api.js                  # 5 hàm Axios
│   ├── wwwroot/js/app.js                  # UI và thao tác CRUD
│   ├── appsettings.json                   # Connection string SQLite
│   └── LabWebCrud.Api.csproj              # Packages NuGet
└── README.md
```

## 2. Cách A: chạy bằng GitHub Codespaces, KHÔNG CÀI .NET vào máy

1. Tạo tài khoản hoặc đăng nhập https://github.com.
2. Vào https://github.com/new, đặt repository `LabWebCRUD`, để **Private**, chọn tạo repository.
3. Tải file ZIP project, giải nén bằng Windows (Extract All). Vào repository > **Add file** > **Upload files** > kéo **toàn bộ file/thư mục BÊN TRONG `LabWebCRUD`** lên, chú ý cả `.devcontainer` và `.config` > Commit changes.
4. Chọn **Code** > **Codespaces** > **Create codespace on main**. File `.devcontainer` chỉ dẫn máy cloud dùng .NET 10. Codespaces có hạn mức sử dụng miễn phí theo tài khoản và có thể tính phí nếu vượt quota.
5. Trong terminal của Codespaces (Linux), nhập lần lượt:

```bash
cd backend/LabWebCrud.Api
dotnet --version
dotnet restore
dotnet tool restore
dotnet ef database update
dotnet run --urls http://0.0.0.0:5080
```

6. Khi terminal báo `Now listening on: http://0.0.0.0:5080`, chọn tab **PORTS**, ở cổng **5080** nhấn biểu tượng mở trong trình duyệt (**Open in Browser**). Không dùng `localhost:5080` trên máy Windows khi code đang chạy trên Codespaces.
7. Mở trang web, thử **Thêm, Sửa, Xóa, Làm mới**, thử URL `/api/products` và `/api/products/1` ở cùng tên miền (domain) với trang web.

**Nếu `dotnet --version` không thấy .NET 10**: trong Codespaces nhấn Command Palette (Ctrl+Shift+P) > `Codespaces: Rebuild Container`, bởi config sẽ đặt .NET 10 vào container cloud. Kiểm tra `.devcontainer/devcontainer.json` đã được upload.

## 3. Cách B: chạy trên máy Windows (nếu đã cài .NET 10 SDK)

Mở terminal ngay bên trong thư mục project đã giải nén:

```powershell
cd backend\LabWebCrud.Api
dotnet --version
dotnet restore
dotnet tool restore
dotnet ef database update
dotnet run --urls http://localhost:5080
```

Mở http://localhost:5080. Nếu PowerShell báo `dotnet is not recognized`, nghĩa là máy chưa có SDK hoặc chưa nhận PATH: quay lại **Cách A** để khỏi cài phần mềm nặng. Nếu máy đã có SDK nhưng chỉ thiếu PATH, mở terminal lại sau khi kiểm tra cài đặt.

## 4. Tạo migration như thế nào?

EF Core migration là lịch sử thay đổi cấu trúc database. Project **đã kèm migration `InitialCreate`** để chạy ngay. Lệnh `dotnet ef database update` sẽ tạo file `products.db`, tạo bảng `Products`, chèn 3 dữ liệu mẫu và tạo bảng `__EFMigrationsHistory`.

Để tự thực hành tạo lại migration đầu tiên trong **BẢN SAO MỚI CỦA PROJECT** (làm trước khi có dữ liệu cần giữ): xóa thư mục `Migrations` trong bản sao đó, rồi từ thư mục `backend/LabWebCrud.Api` chạy:

```bash
dotnet tool restore
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Nếu đã có database `products.db` và `InitialCreate`, không chạy `migrations add InitialCreate` thêm lần nữa.** Muốn tạo migration kế tiếp, sửa entity/model và chạy:

```bash
dotnet ef migrations add TenThayDoiMoi
dotnet ef database update
```

Thử trực tiếp: trong `Models/Product.cs`, thêm thuộc tính `public string? Brand { get; set; }`, lưu lại, chạy:

```bash
dotnet ef migrations add AddBrandToProduct
dotnet ef database update
```

Sau đó có thể xem file migration mới trong thư mục `Migrations`. Nếu frontend cần trường `Brand` thì cập nhật thêm DTO, Controller, HTML và JS. Lưu ý EF có thể thông báo thay đổi seed dữ liệu nếu chỉnh model `HasData`.

## 5. Cấu hình kết nối database

File `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=products.db"
  }
}
```

Trong `Program.cs`:

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
```

Trong `.csproj` đã có `Microsoft.EntityFrameworkCore.Sqlite` và `Microsoft.EntityFrameworkCore.Design` bản `10.0.13`. Nếu tự tạo project trắng:

```bash
dotnet new webapi --use-controllers -n LabWebCrud.Api --framework net10.0
cd LabWebCrud.Api
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 10.0.13
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.13
dotnet tool install --global dotnet-ef --version 10.0.13
```

*(Project có sẵn dùng `dotnet tool restore`, không cần cài `dotnet-ef` global.)*

## 6. Danh sách API

| Method | Endpoint | Chức năng | HTTP thành công |
|---|---|---|---|
| GET | `/api/products` | Tất cả sản phẩm | 200 OK |
| GET | `/api/products/1` | Sản phẩm ID = 1 | 200 OK |
| POST | `/api/products` | Thêm sản phẩm | 201 Created |
| PUT | `/api/products/1` | Cập nhật sản phẩm | 204 No Content |
| DELETE | `/api/products/1` | Xóa sản phẩm | 204 No Content |

API trả `404 Not Found` nếu ID không tồn tại. POST/PUT trả `400 Bad Request` khi dữ liệu vi phạm điều kiện validation.

Ví dụ dữ liệu POST và PUT (JSON):

```json
{
  "name": "Webcam Full HD",
  "description": "Camera cho hoc online",
  "price": 820000,
  "quantity": 7
}
```

Giá tính bằng **đồng Việt Nam (số nguyên)**. Frontend gửi dữ liệu thông qua Axios trong `wwwroot/js/api.js`:

```javascript
axios.get('/api/products');
axios.get('/api/products/1');
axios.post('/api/products', { name: 'Webcam', description: '', price: 820000, quantity: 7 });
axios.put('/api/products/1', { name: 'Webcam moi', description: '', price: 790000, quantity: 9 });
axios.delete('/api/products/1');
```

## 7. Bài test thực hành và ảnh chụp màn hình để nộp

1. Chạy app và chụp danh sách 3 sản phẩm ban đầu.
2. Mở DevTools > Network (F12), bấm Làm mới => phải có `GET /api/products` trả 200.
3. Thêm một sản phẩm => `POST /api/products` trả 201, hàng mới xuất hiện trong bảng.
4. Nhấn Sửa => `GET /api/products/{id}` trả 200; bấm Lưu => `PUT /api/products/{id}` trả 204.
5. Nhấn Xóa và xác nhận => `DELETE /api/products/{id}` trả 204.
6. Chụp terminal sau khi chạy `dotnet ef database update` và thư mục `Migrations`.
7. Chạy lại ứng dụng; sản phẩm vẫn tồn tại do lưu trong file SQLite, không phải InMemory.

Bảng `Products` có 5 cột: `Id`, `Name`, `Description`, `Price`, `Quantity`. `DbContext` cho phép truy vấn/cập nhật entity bằng C#; EF Core chuyển thành SQL thao tác SQLite.

## 8. Sự cố thường gặp

- **`dotnet` không nhận diện:** Codespaces chưa có đúng devcontainer hoặc Windows chưa cài SDK. Với người không muốn tải, dùng Cách A.
- **`dotnet ef` không nhận diện:** chạy `dotnet tool restore` từ thư mục con nằm dưới thư mục gốc `LabWebCRUD` (chứa `.config/dotnet-tools.json`).
- **`no such table: Products`:** chưa chạy `dotnet ef database update` ở đúng thư mục backend.
- **Trang trống:** kiểm tra `dotnet run` còn chạy, mở đúng URL được Codespaces forward và CDN Axios có được tải không (DevTools > Network).
- **Đường dẫn frontend không ra:** phải mở trang bằng ASP.NET server, không mở trực tiếp file `index.html` theo giao thức `file://`.
- **Cổng 5080 đã bị dùng:** dừng chương trình cũ bằng Ctrl+C, hoặc đổi cổng trong lệnh run.

## 9. Giải thích luồng CRUD

**Frontend (`app.js`) → Axios (`api.js`) → `ProductsController` → `AppDbContext` → SQLite (`products.db`) → JSON trả về Axios → cập nhật bảng.**

Mục đích bài học: API Controller xử lý request, DTO kiểm tra đầu vào, EF Core giúp thao tác dữ liệu, migration quản lý cấu trúc database, Axios kết nối Frontend - Backend qua HTTP.
