# MiniJiraWeb — Unknowns & Risks Documentation

> **Ngày phân tích:** 2026-06-24  
> **Phiên bản phân tích:** v1.0  

---

## 1. Những điểm chưa rõ ràng (Unknowns & Questions)

Dưới đây là danh sách các câu hỏi kiến trúc và nghiệp vụ cần làm rõ với Product Owner hoặc đội ngũ phát triển chính:

### Q1: Trạng thái và kế hoạch tích hợp Authentication?
*   **Chi tiết:** Tại sao toàn bộ tầng Authentication lại bị comment? Đội ngũ đang gặp khó khăn trong việc cài đặt dịch vụ quản lý thiết bị (`DeviceManagementService`), hay đang có kế hoạch dịch chuyển sang một giải pháp Identity Provider bên thứ ba (e.g., Keycloak, Auth0, Microsoft Entra ID) thay vì tự xây dựng luồng băm mật khẩu + OTP bằng JWT nội bộ?
*   **Mức độ ảnh hưởng:** Rất lớn. Quyết định này sẽ định hình lại toàn bộ cơ chế Auth của cả backend và frontend.

### Q2: Cấu hình Email SMTP trong môi trường chạy thử và Prod?
*   **Chi tiết:** Trong cấu hình `appsettings.json`, SMTP đang trỏ tới `localhost:1025` không có user/pass. Liệu dự án có kế hoạch dựng một container MailHog / Mailpit chạy kèm trong Docker Compose để phục vụ test OTP local, hay sẽ tích hợp dịch vụ cloud (e.g., SendGrid, Mailgun, Amazon SES)?
*   **Mức độ ảnh hưởng:** Trung bình. Cần thiết để kích hoạt và kiểm thử luồng đăng ký qua OTP email.

### Q3: Cơ chế phân quyền trong Dự án (Project Membership)?
*   **Chi tiết:** Thực thể `Project` hiện tại chỉ có liên kết `OwnerId` (User tạo dự án). Hệ thống hoàn toàn chưa có bảng liên kết nhiều-nhiều giữa `Users` và `Projects` (e.g., bảng `ProjectMembers`). 
*   **Câu hỏi:** Làm cách nào để xác định một User có quyền xem/sửa các Issues trong một Project? Mọi người dùng trong hệ thống đều có quyền truy cập tất cả dự án (Public Monolith), hay hệ thống phân quyền thành viên theo dự án sẽ được xây dựng sau?
*   **Mức độ ảnh hưởng:** Lớn. Quyết định cấu trúc DB và logic truy vấn lọc dữ liệu (Authorization boundaries).

### Q4: Quy trình chuyển đổi trạng thái của Issue (Workflow Transitions)?
*   **Chi tiết:** API cập nhật status (`UpdateIssueStatusCommand`) hiện cho phép chuyển đổi trạng thái issue sang bất kỳ giá trị nào (`Backlog`, `ToDo`, `InProgress`, `Review`, `Done`) mà không cần check điều kiện.
*   **Câu hỏi:** Hệ thống Jira Clone này có yêu cầu kiểm tra ràng buộc workflow không (e.g., Issue chỉ được sang `Done` khi đã qua cột `Review`, hay chỉ Assignee/Reporter mới được đổi trạng thái)?
*   **Mức độ ảnh hưởng:** Trung bình. Ảnh hưởng trực tiếp đến logic cập nhật của Handlers và UI hiển thị.

---

## 2. Rủi ro tiềm ẩn (Potential Risks)

### R1: Rò rỉ thông tin nhạy cảm (Security / Credential Leaks)
*   Chuỗi `JwtSettings.Secret` dạng text đang được đặt công khai trong file cấu hình `appsettings.json` đẩy lên git. Nếu ứng dụng được triển khai thử nghiệm mà không ghi đè giá trị này, kẻ tấn công có thể giả mạo chữ ký JWT để giả danh bất kì người dùng nào (bao gồm Admin).

### R2: Trải nghiệm bất đồng bộ dữ liệu (Optimistic UI Sync Issues)
*   Tại frontend, trạng thái Kanban Board được thay đổi ngay lập tức trên UI trước khi gọi API lưu trữ. Nếu API trả về lỗi (do mất kết nối mạng hoặc lỗi DB), frontend hiện tại im lặng bỏ qua mà không rollback trạng thái thẻ công việc về cột cũ.
*   Điều này dẫn đến rủi ro "ảo ảnh dữ liệu": người dùng nghĩ rằng công việc đã được chuyển trạng thái thành công, nhưng sau khi reload trang thì issue lại quay trở lại vị trí cũ do DB backend chưa được cập nhật.

### R3: Trình duyệt React 19 tương thích với thư viện kéo thả hello-pangea/dnd
*   Ứng dụng đang dùng React 19 và Vite 8 (rất mới) kết hợp với `@hello-pangea/dnd`. Thư viện kéo thả này thỉnh thoảng gặp lỗi tương thích với chế độ React Strict Mode trên React 19. Cần theo dõi sát sao logs console ở frontend để phát hiện các lỗi crash render.
