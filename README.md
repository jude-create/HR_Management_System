# HR Management System

A full-featured HR management REST API built with **ASP.NET Core Web API**, **Entity Framework Core**, and **PostgreSQL** — covering employees, departments, recruitment, payroll, attendance, holidays, notifications, and role-based authentication.

## Features

- **Employee management** — CRUD operations with department assignment, type/status tracking
- **Department management** — organization structure with duplicate-name protection
- **Recruitment** — job postings and candidate pipeline tracking
- **Payroll** — automated payroll generation for active employees, export placeholder
- **Attendance** — attendance records with a correction-request workflow
- **Holidays** — company holiday calendar (public/company/optional types)
- **Notifications** — in-app notification records
- **Dashboard** — aggregated summary stats (headcount, departments, attendance rate, pending payroll)
- **Authentication & Authorization** — JWT-based login with role-based access control (Admin / HrManager)

## Tech Stack

- **ASP.NET Core Web API** (.NET 8)
- **Entity Framework Core** with **Npgsql** (PostgreSQL provider)
- **PostgreSQL** for data persistence
- **AutoMapper** for entity–DTO mapping
- **JWT Bearer Authentication** for securing endpoints
- **Swagger / Swashbuckle** for API documentation and testing

## Architecture

The project follows a layered service architecture:

```
Controllers/   → HTTP endpoints, request/response mapping, status codes
Services/      → Business logic, validation, database operations
Entities/      → EF Core database models
Dtos/          → Request/response contracts (never expose entities directly)
Mappings/      → AutoMapper profiles (entity ↔ DTO)
Data/          → DbContext and demo seed data
```

Every business service returns a typed **Result** object (e.g. `EmployeeResult`, `JobResult`) instead of a bare `null` or `bool`, so controllers can distinguish between "not found," "invalid input," and "conflict" — and return the correct HTTP status code (`400`, `404`, `409`) for each case.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) (running locally, default port `5432`)
- Visual Studio 2022 (recommended) or any .NET-compatible IDE

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/<your-username>/HR_Management_System.git
   cd HR_Management_System
   ```

2. **Create a local PostgreSQL database** (e.g. `hrmanagement`) using pgAdmin or the `psql` CLI.

3. **Configure your connection string and JWT secret**

   This project keeps secrets out of source control. Set up **User Secrets**:
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=hrmanagement;Username=postgres;Password=YOUR_PASSWORD"
   dotnet user-secrets set "Jwt:Key" "YOUR_RANDOM_64_BYTE_BASE64_KEY"
   dotnet user-secrets set "Jwt:Issuer" "HR_Management_System"
   dotnet user-secrets set "Jwt:Audience" "HR_Management_System"
   dotnet user-secrets set "Jwt:ExpiryMinutes" "120"
   ```
   Generate a secure JWT key with:
   ```bash
   dotnet run --project . -- # or use any cryptographically random 64-byte Base64 string
   ```

4. **Apply migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the API**
   ```bash
   dotnet run
   ```
   Swagger UI will be available at `https://localhost:<port>/swagger`.

## Authentication

Login via `POST /api/Auth/login` to receive a JWT access token:

```json
{
  "email": "admin@hr.local",
  "password": "Password123!"
}
```

Use the returned `accessToken` in Swagger's **Authorize** button (padlock icon, top right) to test protected endpoints. Most endpoints require a valid token; a few (department/payroll/employee deletion, role changes) require the **Admin** role specifically.

### Seeded demo accounts

| Email | Password | Role |
|---|---|---|
| admin@hr.local | Password123! | Admin |
| hr@hr.local | Password123! | HrManager |

## Known Limitations & Roadmap

This project currently models attendance and HR records as **admin-managed data**, not a self-service employee portal:

- `User` (login-capable accounts: Admin, HrManager) and `Employee` (HR records) are intentionally separate entities. Employees do not currently have login access.
- Attendance records and correction requests are created and reviewed by HR staff on behalf of employees, rather than employees clocking in or filing corrections themselves.

### Planned enhancement: Employee self-service

To support employees logging in directly (clock-in/out, viewing their own payslips, submitting their own attendance corrections), the following would be added:

- `Employee.UserId` (nullable) — links an employee record to a login account
- A new `Employee` role in `UserRole`
- Self-service endpoints scoped to "the current logged-in user" rather than requiring an HR admin to specify an `EmployeeId`
- Ownership checks so employees can only view/modify their own records

This is a deliberate scope boundary for the current version, not an oversight — the system is built first as an HR-admin tool, with self-service as a natural next phase.

## License

This project is for portfolio/demonstration purposes.