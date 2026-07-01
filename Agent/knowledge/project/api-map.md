# MiniJiraWeb — API Map & Endpoints Documentation

> **Ngày phân tích:** 2026-06-24  
> **Phiên bản phân tích:** v1.0  
> **Base URL:** `http://localhost:5252/api`  

---

## 1. Thiết kế API Wrapper & HTTP Responses

Tất cả các API Response chuẩn của hệ thống được bọc bởi lớp `ApiResponse<T>` đặt tại `Web.API/Response/ApiResponse.cs`. Điều này giúp đảm bảo cấu trúc JSON phản hồi đồng nhất:

```json
{
  "isSuccess": true,
  "message": "Thông điệp thành công/thất bại",
  "data": { ... },
  "errors": [ ... ]
}
```

Các kết quả nghiệp vụ trả về từ lớp `Web.Application` dưới dạng `Result<T>` được chuyển đổi thành HTTP Response nhờ class tiện ích `ResultExtensions` ở Presentation Layer (`Web.API`). Các mã trạng thái lỗi được ánh xạ như sau:

*   **Result.Success**: `200 OK` hoặc `204 NoContent`
*   **ErrorType.Validation** (Mã lỗi Validation): `400 Bad Request`
*   **ErrorType.Unauthorized** (Mã lỗi Auth): `401 Unauthorized`
*   **ErrorType.NotFound** (Mã lỗi không tìm thấy thực thể): `404 Not Found`
*   **ErrorType.Conflict** (Mã lỗi xung đột dữ liệu): `409 Conflict`
*   **ErrorType.Failure** (Mã lỗi nghiệp vụ chung): `500 Internal Server Error`

---

## 2. Bản đồ Endpoints (API Map)

### 2.1 Nhóm API Xác thực & Tài khoản (Authentication)
*   **Tình trạng hiện tại:** Tạm thời bị vô hiệu hóa (`commented out` cả Controller và Handlers).

| Endpoint | Method | Authentication | Mô tả | Payload (JSON) / Query | Response (200 OK) |
|---|---|---|---|---|---|
| `/auth/firstTimeRegistration` | `POST` | Public | Đăng ký email ban đầu & gửi OTP | `{ "email": "string" }` | `{ "isSuccess": true, "data": "registrationId" }` |
| `/auth/verifyOtp` | `POST` | Public | Xác thực mã OTP & tạo User tạm | `{ "otpId": "string", "otp": "string", "otpType": 0 }` | `{ "isSuccess": true, "data": { "accessToken": "...", "refreshToken": "..." } }` |
| `/auth/profile` | `PUT` | Bearer Token | Cập nhật thông tin và kích hoạt account | `{ "fullName": "string", "phoneNumber": "string", "avaURL": "string" }` | `{ "isSuccess": true, "data": { "accessToken": "..." } }` |
| `/auth/login` | `POST` | Public | Đăng nhập tài khoản | `{ "email": "string", "password": "string" }` | `{ "isSuccess": true, "data": { "accessToken": "...", "refreshToken": "..." } }` |

---

### 2.2 Nhóm API Quản lý Issue (Issues)
*   **Tình trạng hiện tại:** Hoạt động (`Active`).
*   **Controller:** `IssuesController.cs`

| Endpoint | Method | Authentication | Mô tả | Payload / Query | Response (200 OK / 201 Created) |
|---|---|---|---|---|---|
| `/issues` | `POST` | Public | Tạo mới một Issue (Task, Story, Bug, Epic) | `{ "summary": "string", "description": "string", "type": 0, "priority": 2, "projectId": "GUID", "sprintId": "GUID?" }` | `{ "id": "GUID" }` (Trả về ID của Issue được tạo) |
| `/issues/{id}/status` | `PUT` | Public | Cập nhật trạng thái Issue (kéo thả Kanban) | *Route param:* `id` (GUID)<br/>*Body:* `{ "issueId": "GUID", "status": 2 }` | `204 No Content` |
| `/issues/project/{projectId}/backlog` | `GET` | Public | Lấy danh sách Issue thuộc Backlog của Project | *Route param:* `projectId` (GUID) | `[ { "id": "GUID", "key": "PHX-1", "summary": "...", "status": "Backlog", ... } ]` |
| `/issues/sprint/{sprintId}/board` | `GET` | Public | Lấy danh sách Issue thuộc một Sprint cụ thể | *Route param:* `sprintId` (GUID) | `[ { "id": "GUID", "key": "PHX-2", "summary": "...", "status": "InProgress", ... } ]` |

---

### 2.3 Nhóm API Quản lý Project (Projects)
*   **Tình trạng hiện tại:** 🛑 **Thiếu Controller**!
*   Các Use Case nghiệp vụ (Commands/Queries) đã được xây dựng đầy đủ ở tầng `Web.Application/Projects` nhưng Presentation Layer (`Web.API`) chưa định nghĩa `ProjectsController` để exposing chúng ra ngoài REST API.

| Endpoint (Dự kiến) | Method | Handler tương ứng trong Application | Mô tả |
|---|---|---|---|
| `/projects` | `POST` | `CreateProjectCommandHandler` | Tạo mới một dự án |
| `/projects` | `GET` | `GetProjectQueriesHandler` | Lấy danh sách dự án của user hiện tại |
| `/projects/{id}` | `GET` | `GetProjectQueryByIdHandler` | Lấy thông tin chi tiết một dự án bằng ID |
| `/projects/{id}` | `PUT` | `UpdateProjectCommandhandler` | Cập nhật thông tin dự án |
| `/projects/{id}` | `DELETE` | `DeleteProjectCommandHandler` | Xóa dự án |

---

## 3. Chiến lược Versioning & Quy chuẩn thiết kế

1.  **API Versioning:** Chưa áp dụng cơ chế Versioning cụ thể (e.g., `/api/v1/issues`). Các router hiện tại đang map trực tiếp dạng `/api/[controller]`. Đây là một điểm cần cải thiện khi hệ thống lớn lên.
2.  **Đồng bộ HTTP Method:**
    *   API Backend quy định cập nhật trạng thái Issue bằng `PUT /api/issues/{id}/status`.
    *   Tuy nhiên, frontend `frontend/src/services/api.ts` lại đang gửi request dạng `PATCH /api/issues/${data.issueId}/status` bằng Axios. Sự lệch pha này sẽ dẫn đến lỗi `405 Method Not Allowed` khi frontend kết nối trực tiếp với backend thật.
3.  **Tự động hóa Swagger:**
    *   Tự động quét và hiển thị toàn bộ endpoint thông qua Swashbuckle ở môi trường Development.
    *   Chưa cấu hình JWT Bearer Authorize button trên giao diện Swagger UI để phục vụ test API cần token.
