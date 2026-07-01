# MiniJiraWeb — Module Map

This document lists the modules present in MiniJiraWeb, showing the controllers, services, repositories, entities, and key related files.

---

## 1. Authentication & Identity Module
Responsible for user registration, multi-step email OTP verification, logins, device management, password hashing, and token issuance.

*   **Status**: 🛑 Disabled / Commented Out
*   **Controllers**:
    *   `AuthController` (commented out) in [AuthController.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.API/Controllers/AuthController.cs)
*   **Services**:
    *   `DeviceManagementService` (commented out) in [DeviceManagementService.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Infrastructure/Services/DeviceManagementService.cs)
    *   `PasswordHasher` in [PasswordHasher.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Infrastructure/Services/PasswordHasher.cs)
    *   `JwtTokenGenerator` in [JwtTokenGenerator.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Infrastructure/Services/JwtTokenGenerator.cs)
    *   `EmailService` in [EmailService.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Infrastructure/Services/EmailService.cs)
*   **Repositories (Data Access)**:
    *   Direct `IApplicationDbContext` queries inside handlers.
*   **Entities**:
    *   `User` in [User.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Domain/Entities/User.cs)
    *   `Device` in [Device.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Domain/Entities/Device.cs)
    *   `RefreshToken` in [RefreshToken.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Domain/Entities/RefreshToken.cs)
    *   `Role` in [Role.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Domain/Entities/Role.cs)
    *   `Permission` in [Permission.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Domain/Entities/Permission.cs)
*   **Related Files**:
    *   Commands: [LoginCommand.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Auth/Commands/LoginCommand.cs), [RegisterCommand.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Auth/Commands/RegisterCommand.cs), [VerifyEmailCommand.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Auth/Commands/VerifyEmailCommand.cs), [VerifyOtpCommand.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Auth/Commands/VerifyOtpCommand.cs)
    *   Models/DTOs: [AuthResponse.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Auth/DTOs/AuthResponse.cs), [PendingRegistration.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Auth/Models/PendingRegistration.cs)

---

## 2. Issues Module
Responsible for task/story/bug lifecycle, Kanban board columns mapping, project backlog retrieval, and sprint board retrieval.

*   **Status**: 🚧 Active (Build Failing due to compilation error in `CreateIssueCommand.cs`)
*   **Controllers**:
    *   `IssuesController` in [IssuesController.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.API/Controllers/IssuesController.cs)
*   **Services**: None (direct use-cases)
*   **Repositories (Data Access)**:
    *   Direct `IApplicationDbContext` queries inside MediatR handlers.
*   **Entities**:
    *   `Issue` in [Issue.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Domain/Entities/Issue.cs)
    *   `Comment` in [Comment.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Domain/Entities/Comment.cs)
    *   `Attachment` in [Attachment.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Domain/Entities/Attachment.cs)
*   **Related Files**:
    *   Commands/Handlers: [CreateIssueCommand.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Issues/Commands/CreateIssueCommand.cs), [DeleteIssueCommand.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Issues/Commands/DeleteIssueCommand.cs), [UpdateIssueStatusCommand.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Issues/Commands/UpdateIssueStatusCommand.cs)
    *   Queries: [GetProjectBacklogQuery.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Issues/Queries/GetProjectBacklog/GetProjectBacklogQuery.cs), [GetSprintBoardQuery.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Issues/Queries/GetSprintBoard/GetSprintBoardQuery.cs)
    *   Frontend Component: [Board.tsx](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/frontend/src/components/board/Board.tsx)

---

## 3. Projects & Sprints Module
Responsible for managing projects, sprints, owner boundaries, and team members.

*   **Status**: 🛑 Skeleton only (stubs/placeholder commands and queries, no REST Controller)
*   **Controllers**: None (missing `ProjectsController.cs`)
*   **Services**: None
*   **Repositories (Data Access)**:
    *   Direct `IApplicationDbContext` queries.
*   **Entities**:
    *   `Project` in [Project.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Domain/Entities/Project.cs)
    *   `Sprint` in [Sprint.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Domain/Entities/Sprint.cs)
*   **Related Files**:
    *   Commands: [CreateProjectCommandHandler.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Projects/Commands/CreateProjectCommandHandler.cs), [UpdateProjectCommandhandler.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Projects/Commands/UpdateProjectCommandhandler.cs), [DeleteProjectCommandHandler.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Projects/Commands/DeleteProjectCommandHandler.cs)
    *   Queries: [GetProjectQueriesHandler.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Projects/Queries/GetProjectQueriesHandler.cs), [GetProjectQueryByIdHandler.cs](file:///d:/CPROJECT/MiniJiraWeb/MiniJiraWebSolution/Web.Application/Projects/Queries/GetProjectQueryByIdHandler.cs)
