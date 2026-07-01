# MiniJiraWeb — Project Brain

This document provides a high-level summary of the MiniJiraWeb project, including its core purpose, current state, modules, known issues, and risks.

---

## 1. Project Purpose
MiniJiraWeb is a simple issue-tracking and project management application cloned from Jira's core workflow. It provides:
- A backend REST API built using **ASP.NET Core 8.0** (.NET 8) with **Clean Architecture**.
- A frontend Single Page Application (SPA) built using **React 19**, **TypeScript**, **Vite 8**, and **TailwindCSS 4**.
- A **PostgreSQL 16** database containerized with Docker, using Entity Framework Core 8.0 for data access.
- Caching layer using **Redis** or local in-memory cache.

---

## 2. Current Status
The project is in an **active development / onboarding state** with key functionalities partially implemented and some critical features disabled or incomplete:
- **Backend Build Status**: currently **Failing** due to a compilation error in `CreateIssueCommand.cs` where the `ProjectId` parameter is missing from the record declaration but is referenced in the handler.
- **Issues Module**: Mostly active. Database tables, migrations, commands/queries, and Swagger endpoints are exposed, except for the PATCH/PUT protocol mismatch with frontend.
- **Projects Module**: Partial. Application layer use-cases exist but are stubbed (throwing `NotImplementedException` or empty). The `ProjectsController` is entirely missing from the Presentation (Web.API) layer.
- **Authentication & Authorization**: Inactive. The `AuthController`, JWT Token logic, device fingerprinting, and email OTP verification are drafted but completely commented out.

---

## 3. Main Modules & Components

| Module | Location | Components | Status |
|---|---|---|---|
| **Identity & Auth** | `Web.Application/Auth`, `Web.Infrastructure/Services`, `Web.API/Controllers/AuthController.cs` | Login, OTP Registration, Password Hashing, Device Management | 🛑 Commented Out |
| **Issues** | `Web.Application/Issues`, `Web.API/Controllers/IssuesController.cs` | Issue creation, status changes, Backlog retrieval, Active Sprint retrieval | 🚧 Active (Build Failing) |
| **Projects** | `Web.Application/Projects` | CRUD Projects, Sprint configuration | 🛑 Stubs / Missing Controller |
| **Frontend Board** | `frontend/src/components/board` | Kanban Board, Column sorting, Issue cards | 🚧 Active (Development) |

---

## 4. Development Progress & Key Findings
1. **Clean Architecture Structure**: Very clean isolation of concerns. Domain layer (`Web.Domain`) holds entities, enums, and primitives (like Result pattern) and has zero dependencies.
2. **CQRS & MediatR**: Fully embraced for Command/Query separation, reducing Controller bloat.
3. **Database Integration**: Entity relationships, constraints, and cascading rules are fully configured in `ApplicationDbContext` with Fluent API.
4. **Caching Support**: Ready-to-go dual providers (Redis or MemoryCache) configured via `appsettings.json`.

---

## 5. Known Issues
- **Compile Error**: `CreateIssueCommand` has compilation errors because `ProjectId` is referenced in the handler but missing from the record payload.
- **Project CRUD Placeholder**: `CreateProjectCommandHandler` throws `NotImplementedException`, while `UpdateProjectCommandhandler` and others are empty classes.
- **REST Method Mismatch**: Frontend calls `PATCH /api/issues/{id}/status` for dragging issue cards, but backend only exposes `PUT /api/issues/{id}/status`.
- **VerifyOtp Syntax Error**: If uncommented, `VerifyOtpCommand.cs` will fail compilation due to incomplete `Device.Create()` calls and mismatching return statements.
- **Missing Controller**: No `ProjectsController` exists to expose project endpoints.

---

## 6. Known Risks
- **Security Vulnerability**: JWT Secret key is hardcoded directly inside `appsettings.json`. Needs to be moved to Environment Variables or Secrets Manager.
- **Optimistic UI Data Sync**: Frontend board updates UI columns optimistically, but if the API fails, there is no rollback logic to restore the card to its original column.
- **React 19 Compatibility**: React 19 strict mode might conflict with `@hello-pangea/dnd` leading to double-rendering issues.
- **IsProfileCompleted Side-effect**: `User.IsProfileCompleted` property has a side-effect that changes `IsActive = true` when read, which is a significant design flaw.
