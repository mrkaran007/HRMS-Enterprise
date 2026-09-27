# HRMS Enterprise

An ASP.NET Core Web API for managing employees and departments, with employee transfers, transfer history, and JWT-based access control. The project uses a layered service/repository structure with Entity Framework Core and SQL Server.

## Features

- Employee and department create, read, update, and delete operations.
- Employee listing with text search, department and salary filters, sorting, and pagination.
- Employee transfers between departments, with a recorded transfer history.
- DTO-based request and response models, Data Annotations, and service-level validation.
- Custom exceptions translated to consistent JSON error responses by global middleware.
- JWT login, hashed passwords, and role-based authorization for `admin`, `hr`, and `employee` roles.
- Resource-based employee access: admins and HR users can access any employee record; employees can access their own record and transfer history.
- Repository Pattern and Unit of Work for data access and persistence.
- An explicit SQL transaction around the employee-transfer and transfer-history update.

## Technology

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server (the checked-in development connection string uses SQL Server LocalDB)
- JWT Bearer authentication
- Swagger / OpenAPI in the Development environment

## Architecture

HTTP requests enter controllers, which call application services. Services hold business rules and use repository interfaces for persistence. Repositories use `HRMSDbContext`; `UnitOfWork` provides the shared `SaveChangesAsync` operation. Employee transfer is the special multi-step operation: it starts an EF Core database transaction, adds a history record, updates the employee's department, saves, and commits (or rolls back on failure).

```text
HTTP request
    └── Controllers
          └── Services (business rules, DTO mapping)
                ├── Repository interfaces → Repositories
                └── UnitOfWork → HRMSDbContext → SQL Server
```

## Project layout

```text
HRMS.slnx
HRMS.API/
├── Authorization/    Employee resource authorization requirement and handler
├── Controllers/      Auth, Department, and Employee HTTP endpoints
├── Data/             EF Core DbContext and entity configuration
├── DTOs/              Request, response, search, and paging models
├── Exceptions/        Application exception types
├── Middleware/       Global exception-to-JSON handling
├── Migrations/       EF Core schema migrations and model snapshot
├── Models/            User, Employee, Department, and transfer-history entities
├── Repositories/     Repository interfaces/implementations and UnitOfWork
└── Services/          Authentication, department, and employee logic
```

## API overview

All routes use the `api` prefix. Routes marked **Authenticated** require a valid bearer token. Role names are the lowercase values validated by the user-creation service.

| Method | Route | Access | Purpose |
|---|---|---|---|
| `POST` | `/api/auth/users` | Public | Create a user account |
| `POST` | `/api/auth/login` | Public | Verify credentials and return a JWT |
| `GET` | `/api/employees` | Authenticated | Search, filter, sort, and page employee records |
| `GET` | `/api/employees/me` | Authenticated | Return claims for the current user |
| `GET` | `/api/employees/{id}` | Authenticated + resource policy | Read an employee (admin/HR, or the employee's own record) |
| `POST` | `/api/employees` | `admin`, `hr` | Create an employee |
| `PUT` | `/api/employees/{id}` | `admin`, `hr` | Update an employee |
| `DELETE` | `/api/employees/{id}` | `admin` | Delete an employee |
| `PUT` | `/api/employees/{id}/transfer` | `admin`, `hr` | Transfer an employee to another department |
| `GET` | `/api/employees/{id}/transfer-history` | Authenticated + resource policy | Read transfer history (admin/HR, or the employee's own history) |
| `GET` | `/api/departments` | Authenticated | List departments |
| `GET` | `/api/departments/{id}` | Authenticated | Read a department |
| `POST` | `/api/departments` | `admin`, `hr` | Create a department |
| `PUT` | `/api/departments/{id}` | `admin`, `hr` | Update a department |
| `DELETE` | `/api/departments/{id}` | `admin` | Delete a department (only when no employees are assigned) |

### Employee search and paging

`GET /api/employees` accepts these query parameters:

| Parameter | Description |
|---|---|
| `search` | Case-insensitive substring match against first name, last name, or email |
| `departmentId` | Filter by department |
| `minimumSalary`, `maximumSalary` | Inclusive salary range |
| `sortBy` | `firstName`, `lastName`, `salary`, or `joiningDate`; otherwise employee ID is used |
| `sortOrder` | Use `desc` for descending order; other values use ascending order |
| `pageNumber` | 1-based page number; defaults to `1` |
| `pageSize` | Page size from 1 to 100; defaults to `10` |

Example:

```text
GET /api/employees?search=alex&departmentId=2&minimumSalary=40000&sortBy=salary&sortOrder=desc&pageNumber=1&pageSize=10
```

## Authentication and authorization

1. Create a user with `POST /api/auth/users`. Supported roles are `admin`, `hr`, and `employee`. HR and employee accounts must link to an existing `employeeId`; admin accounts cannot link to one. Passwords are stored as hashes.
2. Log in with `POST /api/auth/login`. A successful response contains a signed JWT and its expiration time.
3. Send the token on protected requests using `Authorization: Bearer <token>`.
4. Role attributes restrict employee and department writes. Employee detail and transfer-history reads additionally invoke the `EmployeeAccess` resource policy, which checks the token's role and `EmployeeId` claim.

The user-creation endpoint is currently public in the API. Restrict it to an administrator or implement a controlled bootstrap flow before exposing the service beyond a trusted development environment.

Swagger UI is enabled only when `ASPNETCORE_ENVIRONMENT` is `Development`; it includes a bearer-token security scheme.

## Database and migrations

The checked-in EF Core migrations cover the initial schema, a unique department-name index, employee transfer history, and user authentication. `HRMSDbContext` configures restrictive relationships for employees, departments, transfer history, and users, plus a filtered unique index so an employee can have at most one linked account.

The development configuration points to LocalDB (`(localdb)\MSSQLLocalDB`) and a database named `HRMSDB`. With SQL Server LocalDB available, apply the committed migrations from the repository root:

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project HRMS.API/HRMS.API.csproj
```

If `dotnet-ef` is already installed, skip the install command. The API project includes EF Core design and tools package references.

## Run locally

### Prerequisites

- .NET 8 SDK
- SQL Server LocalDB, or another SQL Server instance
- EF Core CLI (`dotnet-ef`) to apply migrations

1. Clone the repository and move into its root directory.
2. Configure `ConnectionStrings:DefaultConnection` for your SQL Server instance and set JWT values as described below.
3. Apply the database migrations using the command above.
4. Start the API:

   ```bash
   dotnet run --project HRMS.API/HRMS.API.csproj
   ```

The checked-in Development launch profile uses `http://localhost:5181` and `https://localhost:7258`, and launches Swagger at `/swagger`. Use the URL printed by `dotnet run` if your launch profile differs.

## Configuration

The API reads `ConnectionStrings:DefaultConnection` and the `Jwt` settings `Key`, `Issuer`, `Audience`, and `ExpiryMinutes`. Configure these with .NET user secrets or environment variables for local work; do not commit real credentials or signing keys.

For example, set development environment variables in PowerShell (replace the sample values locally):

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=(localdb)\MSSQLLocalDB;Database=HRMSDB;Trusted_Connection=True;TrustServerCertificate=True;"
$env:Jwt__Key = "replace-with-a-long-random-local-development-key"
$env:Jwt__Issuer = "HRMS.API"
$env:Jwt__Audience = "HRMS.CLIENT"
$env:Jwt__ExpiryMinutes = "60"
```

The repository's `appsettings.json` contains a clearly named development placeholder JWT key; replace it through a local configuration source. Use a strong secret outside development.

## Current scope

This repository currently contains the backend API described above. No frontend application or deployment configuration is included in the checked-in solution.

