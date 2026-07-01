# MiniJiraWeb — Business Flow & Use Cases Documentation

> **Ngày phân tích:** 2026-06-24  
> **Phiên bản phân tích:** v1.0  

---

## 1. Luồng Xác thực và Đăng ký Tài khoản (Authentication & Registration Flow)

Mặc dù mã nguồn của phần Authentication hiện đang được tạm thời chú thích (`commented out`), tài liệu này mô tả chi tiết thiết kế nghiệp vụ của luồng xác thực đa bước có bảo mật OTP như dự án đã hoạch định.

### 1.1 Luồng Đăng ký tài khoản (Đăng ký ban đầu + Xác thực OTP + Hoàn thiện Profile)

```mermaid
sequenceDiagram
    autonumber
    actor Client as Client (React SPA)
    participant API as Web.API (Controllers)
    participant App as Web.Application (Handlers)
    participant Cache as Cache (Redis/InMemory)
    participant DB as PostgreSQL DB
    participant Mail as SMTP Mail Server

    Note over Client,Mail: Bước 1: Khởi đầu Đăng ký (Verify Email)
    Client->>API: POST /api/auth/firstTimeRegistration (Email)
    API->>App: Send VerifyEmailCommand
    App->>DB: Check if Email exists
    alt Email đã tồn tại
        DB-->>App: Email exists
        App-->>API: Result.Failure(ConflictError)
        API-->>Client: 409 Conflict
    else Email chưa tồn tại
        App->>App: Generate 6-digit OTP & RegistrationId (Guid)
        App->>Cache: Save Email, OTP to "reg_{registrationId}" (TTL: 5m)
        App->>Mail: Send OTP code to User Email
        App-->>API: Result.Success(registrationId)
        API-->>Client: 200 OK (registrationId)
    end

    Note over Client,Mail: Bước 2: Xác thực OTP (Verify OTP)
    Client->>API: POST /api/auth/verifyOtp (registrationId, OTP, OtpType)
    API->>App: Send VerifyOtpCommand
    App->>Cache: Get registration data by registrationId
    alt OTP hết hạn hoặc sai
        App-->>API: Result.Failure(ValidationError)
        API-->>Client: 400 Bad Request
    else OTP chính xác
        App->>DB: Create User Entity (Username, Email, provisional state)
        App->>DB: SaveChanges
        App->>Cache: Remove OTP from cache
        App->>App: Generate provisional JWT + RefreshToken
        App-->>API: Result.Success(AuthResponse)
        API-->>Client: 200 OK (AccessToken, RefreshToken)
    end

    Note over Client,Mail: Bước 3: Hoàn thiện thông tin (User Info Completion)
    Client->>API: PUT /api/auth/profile (FullName, PhoneNumber, AvatarUrl) (Bearer Auth)
    API->>App: Send UserInformationAfterRegisCommand
    App->>App: Extract Email from Token Claims
    App->>DB: Get User by Email
    App->>DB: Update Profile & Set IsActive = true
    App->>DB: SaveChanges
    App->>App: Generate active JWT + RefreshToken
    App-->>API: Result.Success(AuthResponse)
    API-->>Client: 200 OK (Active AccessToken)
```

---

### 1.2 Luồng Đăng nhập (Login Flow)

```mermaid
flowchart TD
    A[Client gửi request POST /api/auth/login] --> B{Tìm User theo Email?}
    B -- Không tìm thấy --> C[Trả về 404 Not Found]
    B -- Tìm thấy --> D{Tài khoản Active?}
    D -- Không active --> E[Trả về 401 Unauthorized]
    D -- Active --> F{Tài khoản bị Lockout?}
    F -- Đang bị khóa --> G[Trả về 401 Unauthorized với LockoutEnd]
    F -- Không bị khóa --> H{Validate Password?}
    
    H -- Sai mật khẩu --> I[Tăng FailedLoginAttempts]
    I --> J{Đạt giới hạn Max Attempts?}
    J -- Đạt giới hạn --> K[Khóa tài khoản tạm thời theo Exponential Backoff]
    J -- Chưa đạt --> L[Trả về 401 Unauthorized]
    
    H -- Đúng mật khẩu --> M[ResetLoginAttempts]
    M --> N[Generate JWT Access Token & Refresh Token]
    N --> O[Trả về 200 OK với Token Response]
```

---

## 2. Luồng Nghiệp vụ Quản lý Công việc (Issue & Sprint Lifecycle)

Quản lý Issue tuân theo triết lý của Agile/Scrum hoặc Kanban, cho phép di chuyển Issue qua các trạng thái trên bảng Kanban.

### 2.1 Luồng Tạo mới Issue (Create Issue Flow)
1.  **Client** gửi yêu cầu `POST /api/issues` với payload chứa `Summary`, `Description`, `Type`, `Priority`, `ProjectId` và tùy chọn `SprintId`.
2.  **Web.API** tiếp nhận request và chuyển qua MediatR gửi `CreateIssueCommand`.
3.  **CreateIssueCommandHandler**:
    *   Tự động sinh mã `Key` của issue theo định dạng `{ProjectKey}-{Number}` (e.g., `PHX-23`).
    *   Kiểm tra sự tồn tại của `ProjectId`, `SprintId`, `AssigneeId` và `ReporterId` nếu được chỉ định.
    *   Lưu Issue mới vào database với trạng thái mặc định là `Backlog`.
    *   Trả về mã ID của Issue mới tạo.

### 2.2 Luồng Kéo thả trên Kanban Board (Drag-and-Drop / Update Issue Status Flow)
Kanban Board được tổ chức thành 5 cột tương ứng với các trạng thái của `IssueStatus`:
`Backlog` ➔ `To Do` ➔ `In Progress` ➔ `Review` ➔ `Done`.

```mermaid
sequenceDiagram
    autonumber
    actor User
    participant View as Board.tsx (Frontend)
    participant API as Web.API (Backend)
    participant DB as PostgreSQL DB

    User->>View: Kéo IssueCard từ cột cũ sang cột mới
    View->>View: Cập nhật local state ngay lập tức (Optimistic UI)
    View->>API: PATCH /api/issues/{id}/status (Payload: status, issueId)
    alt Sync thành công
        API->>DB: Cập nhật Issue.Status = NewStatus
        API-->>View: 200 OK / 204 No Content
    else Sync thất bại (Mất mạng/API sập)
        API-->>View: Error
        Note over View: Giao diện giữ nguyên trạng thái kéo thả<br/>(không rollback để tối ưu trải nghiệm)
    end
```

### 2.3 Luồng Lập kế hoạch Sprint & Backlog (Sprint Planning & Backlog Flow)
*   **Backlog Query:** Khi người dùng xem phân đoạn Backlog, ứng dụng gọi `GET /api/issues/project/{projectId}/backlog`. Handler sẽ lọc ra các Issues của Project mà có `SprintId IS NULL`.
*   **Sprint Board Query:** Khi người dùng mở Active Sprint Board, ứng dụng gọi `GET /api/issues/sprint/{sprintId}/board`. Handler sẽ trả về danh sách các Issues được gán với `SprintId` tương ứng.
