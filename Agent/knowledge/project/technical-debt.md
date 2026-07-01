# MiniJiraWeb — Technical Debt & Code Smell Assessment

> **Ngày phân tích:** 2026-06-24  
> **Phiên bản phân tích:** v1.0  

---

## 1. Nợ kỹ thuật Nghiêm trọng (Critical Technical Debts)

### 1.1 Tính năng Authentication & Authorization bị vô hiệu hóa hoàn toàn
*   **Chi tiết:** File `AuthController.cs`, toàn bộ Handlers trong thư mục `Web.Application/Auth/Commands/` (`LoginCommand.cs`, `RegisterCommand.cs`, `VerifyOtpCommand.cs`, etc.) và `DeviceManagementService.cs` đều bị comment bằng `//`.
*   **Hệ quả:** Hệ thống hiện tại hoàn toàn trống trơn về mặt bảo mật. Bất kỳ ai cũng có thể truy cập các endpoint của `IssuesController` mà không cần xác thực.
*   **Mức độ ưu tiên khắc phục:** Cao (Blocker).

### 1.2 Thiếu Controller quản lý Dự án (Missing ProjectsController)
*   **Chi tiết:** Ở tầng Application đã định nghĩa đầy đủ các usecase CRUD cho Project (`CreateProjectCommandHandler`, `UpdateProjectCommandhandler`, `DeleteProjectCommandHandler`, etc.). Tuy nhiên, tầng Web.API hoàn toàn thiếu vắng `ProjectsController`.
*   **Hệ quả:** Client không có cách nào gọi API để thao tác với Projects từ bên ngoài.
*   **Mức độ ưu tiên khắc phục:** Cao.

### 1.3 Lệch pha HTTP Method giữa Frontend và Backend (HTTP Method Inconsistency)
*   **Chi tiết:** 
    *   Backend `IssuesController.cs` khai báo: `[HttpPut("{id}/status")]` (Dùng động từ HTTP `PUT`).
    *   Frontend `frontend/src/services/api.ts` gọi: `api.patch('/issues/.../status')` (Dùng động từ HTTP `PATCH`).
*   **Hệ quả:** Khi chạy thực tế kết nối API, cuộc gọi từ frontend sẽ lập tức lỗi `405 Method Not Allowed`.
*   **Mức độ ưu tiên khắc phục:** Trung bình (Cần sửa đổi ở một trong hai đầu).

---

## 2. Điểm Code Smells & Cú pháp lỗi trong Code

### 2.1 Mã nguồn dở dang gây lỗi biên dịch (Compilation Error if Active)
*   Trong file `Web.Application/Auth/Commands/VerifyOtpCommand.cs`, dòng 88-95 có chứa cú pháp dở dang:
    ```csharp
    var newDevice = Device.Create(
        user!.Id,
        pendingData.Username,

        ); // Thiếu tham số truyền vào hàm Device.Create

    _deviceService.LoginOnNewDevice(user!.Id, ) // Thiếu đối số truyền vào và dấu kết thúc
    ```
    Đồng thời, hàm kết thúc bằng nhiều dấu ngoặc nhọn thừa và hai lệnh `return Result<AuthResponse>.Success(...)` chồng chéo không hợp lệ (dòng 112 và dòng 115).
*   **Hệ quả:** Nếu uncomment file này lên, project `Web.Application` sẽ lập tức gặp lỗi biên dịch (Compilation Error).

### 2.2 Thiếu xử lý lỗi trực quan ở Frontend
*   Trong `Board.tsx` (dòng 46-53):
    ```typescript
    try {
      await issueApi.updateIssueStatus({ ... });
    } catch {
      // API not available, drag still works locally — silently ignore
    }
    ```
    Việc bắt lỗi nhưng im lặng bỏ qua (`silently ignore`) làm mất đi khả năng thông báo cho người dùng biết dữ liệu của họ chưa được lưu trên máy chủ nếu kết nối mạng thất bại.

---

## 3. Khuyến nghị Tái cấu trúc & Cải thiện (Refactoring Suggestions)

1.  **Kích hoạt lại Auth sau khi sửa lỗi cú pháp:**
    *   Hoàn thiện mã nguồn ở `VerifyOtpCommand.cs` bằng cách cung cấp đầy đủ thông tin `Device` từ Request (IP, OS, Browser, Fingerprint) thay vì bỏ trống tham số.
    *   Uncomment hệ thống Auth để kiểm thử.
2.  **Đồng bộ thiết kế RESTful:**
    *   Chuyển API cập nhật status sang `PATCH` ở cả Backend và Frontend để tuân thủ đúng triết lý REST (chỉ cập nhật một phần dữ liệu - `status` - của thực thể Issue).
3.  **Tạo `ProjectsController`:**
    *   Tạo mới controller exposes các endpoint cho phép client truy vấn và quản lý các dự án trong hệ thống.
4.  **Bổ sung Thư viện Validation:**
    *   Dù đã cài đặt package `FluentValidation`, hệ thống chưa định nghĩa bất kì validator class nào cho các command đầu vào (e.g., `CreateIssueCommandValidator` để bắt lỗi nhập trống Summary, sai định dạng Key,...).
5.  **Bổ sung UnitTest:**
    *   Solution hoàn toàn trống rỗng các project unit test/integration test. Cần bổ sung xUnit project để viết test suite kiểm thử business logic của các MediatR Handlers.
6.  **Tách cấu hình JWT Key an toàn:**
    *   Secret key của JWT đang được ghi cứng trong `appsettings.json`. Cần chuyển sang sử dụng Environment Variables hoặc User Secrets trong môi trường local development để tránh rò rỉ mã bảo mật trên git repository.
