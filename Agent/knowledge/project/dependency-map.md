# MiniJiraWeb — Dependency Map

This document outlines the dependencies between core backend classes and components in MiniJiraWeb.

---

## 1. Controllers (Presentation Layer)

### `IssuesController`
*   **Dependencies**:
    *   `IMediator` (MediatR dispatcher)
*   **Consumers**:
    *   React frontend SPA client
*   **Relationships**:
    *   Dispatches issue command and query requests to application handlers.

### `AuthController` (Commented Out)
*   **Dependencies**:
    *   `IMediator`
*   **Consumers**:
    *   React frontend SPA client
*   **Relationships**:
    *   Dispatches register, login, and verify OTP command requests to application handlers.

---

## 2. MediatR Handlers (Application Layer)

### `CreateIssueCommandHandler`
*   **Dependencies**:
    *   `IApplicationDbContext`
*   **Consumers**:
    *   MediatR pipeline (triggered by `CreateIssueCommand`)
*   **Relationships**:
    *   Retrieves project key prefix to generate sequence code (e.g. `JIRA-1`) and adds issue to database context.

### `UpdateIssueStatusCommandHandler`
*   **Dependencies**:
    *   `IApplicationDbContext`
*   **Consumers**:
    *   MediatR pipeline (triggered by `UpdateIssueStatusCommand`)
*   **Relationships**:
    *   Updates the `IssueStatus` of a matching issue in the database.

### `GetProjectBacklogQueryHandler`
*   **Dependencies**:
    *   `IApplicationDbContext`
*   **Consumers**:
    *   MediatR pipeline (triggered by `GetProjectBacklogQuery`)
*   **Relationships**:
    *   Queries `Issues` where `ProjectId` matches and `SprintId` is null.

### `GetSprintBoardQueryHandler`
*   **Dependencies**:
    *   `IApplicationDbContext`
*   **Consumers**:
    *   MediatR pipeline (triggered by `GetSprintBoardQuery`)
*   **Relationships**:
    *   Queries `Issues` where `SprintId` matches the requested sprint.

### `VerifyOtpCommandHandler` (Commented Out / Broken)
*   **Dependencies**:
    *   `ICacheService`
    *   `IApplicationDbContext`
    *   `IPasswordHasher`
    *   `IJwtTokenGenerator`
    *   `IDeviceManagementService`
*   **Consumers**:
    *   MediatR pipeline (triggered by `VerifyOtpCommand`)
*   **Relationships**:
    *   Checks temporary registration data in Redis/Memory Cache, creates `User` and `Device` records in Postgres, generates JWT.

### `LoginCommandHandler` (Commented Out)
*   **Dependencies**:
    *   `IApplicationDbContext`
    *   `IPasswordHasher`
    *   `IJwtTokenGenerator`
*   **Consumers**:
    *   MediatR pipeline (triggered by `LoginCommand`)
*   **Relationships**:
    *   Checks user hash password, manages lockout limits, and returns access/refresh token.

---

## 3. Services (Infrastructure Layer)

### `DeviceManagementService` (Commented Out)
*   **Dependencies**:
    *   `IApplicationDbContext`
*   **Consumers**:
    *   `VerifyOtpCommandHandler`
*   **Relationships**:
    *   Maintains the active sessions count (limit 2 active devices) and trusted devices settings in the database.

### `JwtTokenGenerator`
*   **Dependencies**:
    *   `IOptions<JwtSettings>`
*   **Consumers**:
    *   `LoginCommandHandler`, `VerifyOtpCommandHandler`, `UserInformationCommandHandler`
*   **Relationships**:
    *   Creates JWT payload claims and cryptographically signs with security key.

### `EmailService`
*   **Dependencies**:
    *   `IOptions<EmailSettings>`
*   **Consumers**:
    *   `VerifyEmailCommandHandler`, `RegisterCommandHandler`
*   **Relationships**:
    *   Connects to SMTP host (using MailKit) to send HTML template emails.

### `RedisCacheService`
*   **Dependencies**:
    *   `IConnectionMultiplexer` (StackExchange.Redis)
*   **Consumers**:
    *   Any component needing `ICacheService` (when CacheSettings Provider is "Redis")
*   **Relationships**:
    *   Performs database serialization for Key-Value data in external Redis instance.

### `InMemoryCacheService`
*   **Dependencies**:
    *   `IMemoryCache` (Microsoft.Extensions.Caching.Memory)
*   **Consumers**:
    *   Any component needing `ICacheService` (when CacheSettings Provider is "Memory")
*   **Relationships**:
    *   Maintains key-value data inside the web process RAM.
