# Development Guide

This guide provides comprehensive information for developers setting up, working on, and contributing to the **BookManager** codebase.

---

## Table of Contents

- [Development Environment Setup](#development-environment-setup)
- [Quick Start (Run within 15 Minutes)](#quick-start-run-within-15-minutes)
- [Project Structure Reference](#project-structure-reference)
- [Coding Standards & Conventions](#coding-standards--conventions)
- [Database Migrations (EF Core)](#database-migrations-ef-core)
- [Step-by-Step: Adding a New Feature](#step-by-step-adding-a-new-feature)
- [Common Development Tasks](#common-development-tasks)
- [Debugging Techniques](#debugging-techniques)
- [Troubleshooting Common Issues](#troubleshooting-common-issues)

---

## Development Environment Setup

### Required Tools & Runtimes

| Tool | Recommended Version | Purpose |
| :--- | :--- | :--- |
| **.NET SDK** | **.NET 10.0** (or .NET 8.0+) | Compiles and executes the C# backend. |
| **Docker Desktop** | Latest (v24.0+) | Runs the PostgreSQL 17 database container. |
| **IDE** | Visual Studio 2022 / VS Code / Rider | Code editing, debugging, and IntelliSense. |
| **Database Client** | pgAdmin 4 / DBeaver / TablePlus | Direct database querying and inspection. |
| **API Client** | Postman / Insomnia | HTTP request testing. |
| **Git** | Latest (v2.40+) | Version control. |

### Verifying Installations

Open your terminal and verify your environment:

```bash
# Check .NET SDK
dotnet --version
# Expected: 10.0.xxx

# Check Docker
docker --version
docker compose version

# Check Git
git --version
```

---

## Quick Start (Run within 15 Minutes)

### Step 1: Clone Repository & Setup Configuration

```bash
git clone <repository-url>
cd BookManager

# Copy environment template
cp .env.example .env
```

Review your `.env` file to ensure the database port and credentials match your local setup:
```dotenv
POSTGRES_DB=bookmanager_db
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres123
POSTGRES_PORT=5432
API_PORT=8080
```

---

### Step 2: Choose Execution Mode

#### Option A: Run Full Stack with Docker Compose (Recommended)
This starts both PostgreSQL and the API inside optimized containers:

```bash
docker compose up --build -d
```
- **Web UI & API**: `http://localhost:8080`
- **Swagger Documentation**: `http://localhost:8080/swagger`

---

#### Option B: Run Database in Docker + Run API via .NET CLI (For Code Debugging)

1. **Start PostgreSQL only**:
   ```bash
   docker compose up -d postgres
   ```

2. **Restore dependencies & Build**:
   ```bash
   dotnet restore
   dotnet build
   ```

3. **Run the API with Hot Reload**:
   ```bash
   dotnet watch run
   ```
   The API will launch at `http://localhost:5242` (Swagger: `http://localhost:5242/swagger`).

---

## Project Structure Reference

```
BookManager/
├── Controllers/                         # Presentation Layer (Thin Controllers)
│   ├── AuthController.cs                # /api/auth endpoints (Register, Login, Refresh)
│   └── BookController.cs                # /api/books endpoints (CRUD, Search, Filters)
│
├── Service/                             # Business Logic Layer (Fat Services)
│   ├── IAuthService.cs & AuthService.cs # Authentication, password hashing, user registration
│   ├── IBookService.cs & BookService.cs # Book catalog operations, business validation
│   └── IJwtService.cs & JwtService.cs   # JWT token generation, validation, separate keys
│
├── Repositories/                        # Data Access Layer (Repository Pattern)
│   ├── IBookRepository.cs & BookRepository.cs # LINQ queries, filters, sorting, AsNoTracking
│   └── IUserRepository.cs & UserRepository.cs # User retrieval and uniqueness checks
│
├── Models/                              # Domain Entities
│   ├── User.cs                          # User entity (Id, Username, PasswordHash, Role)
│   ├── Book.cs                          # Book entity (Id, Title, Author, Price, Category, Stock)
│   └── Author.cs                        # Author entity (Id, Name, Biography, Books)
│
├── DTOs/                                # Data Transfer Objects
│   ├── Auth/                            # LoginDto, RegisterDto, RefreshRequestDto, AuthResponseDto
│   └── Books/                           # CreateBookDto, UpdateBookDto, BookResponseDto
│
├── Data/                                # Persistence & EF Core
│   ├── AppDbContext.cs                  # EF Core DbContext definition
│   └── AppDbContextFactory.cs           # Design-time factory for EF Core CLI migrations
│
├── Extensions/                          # Dependency Injection & Configuration Modules
│   ├── DatabaseExtensions.cs            # PostgreSQL DbContext & auto-migration setup
│   ├── JwtExtensions.cs                 # JwtBearer authentication middleware setup
│   ├── ServiceExtensions.cs             # Scoped service & repository DI registrations
│   └── SwaggerExtensions.cs             # OpenAPI Swagger specification setup
│
├── Middleware/                          # Custom HTTP Middleware
│   └── GlobalExceptionMiddleware.cs     # Centralized RFC 7807 Problem Details error handler
│
├── Migrations/                          # EF Core Schema Version Snapshots
│
├── wwwroot/                             # Static Web Assets
│   └── index.html                       # Frontend demo & test dashboard
│
├── docker-compose.yaml                  # Multi-container orchestration (App + Postgres)
├── Dockerfile                           # Multi-stage container build definition
├── BookManager.postman_collection.json  # Pre-configured Postman API collection
├── appsettings.json                     # Default application configurations
└── .env.example                         # Environment variables template
```

---

## Coding Standards & Conventions

### 1. Architectural Rules
- **Controllers must remain thin**: No raw LINQ, database access, or complex calculations inside controllers.
- **DTOs for all boundaries**: Entities in `Models/` must never be directly accepted as controller parameters or returned in responses. Always map via `DTOs/`.
- **Interface-driven dependencies**: Always inject interfaces (`IBookService`, `IBookRepository`) rather than concrete classes.

### 2. C# Language Conventions
- **Asynchronous Code**: Always use `async`/`await` for I/O operations and append `Async` to method names (`GetAllAsync`, `GetByIdAsync`).
- **Cancellation Tokens**: Always accept `CancellationToken ct = default` in service/repository methods and pass it down to EF Core calls (`ToListAsync(ct)`).
- **Naming Standards**:
  - `PascalCase`: Classes, interfaces, methods, properties, public constants (`BookService`, `GetAllAsync`).
  - `camelCase`: Method arguments, local variables (`minPrice`, `keyword`).
  - `_camelCase`: Private `readonly` fields (`_bookRepository`, `_configuration`).

### 3. Error Handling Rules
- Do **not** wrap every controller action in `try/catch`.
- In Services, throw semantic exceptions:
  - Resource not found: `throw new KeyNotFoundException("Book not found.");` (translates to `404 Not Found`).
  - Business rule violation: `throw new InvalidOperationException("Username already exists.");` (translates to `400 Bad Request`).
  - The `GlobalExceptionMiddleware` automatically translates these into standardized JSON responses.

---

## Database Migrations (EF Core)

Install the global EF Core CLI tool if you haven't already:

```bash
dotnet tool install --global dotnet-ef
```

### Common Migration Commands

```bash
# 1. Create a new migration after updating an entity in Models/
dotnet ef migrations add AddAuthorRelationship

# 2. Apply pending migrations to the local database
dotnet ef database update

# 3. Remove the last migration (only if not yet applied to production)
dotnet ef migrations remove

# 4. Generate an idempotent SQL script for production deployment
dotnet ef migrations script -i -o migrations.sql
```

> **Automated Migration**: In containerized environments, the application executes `app.ApplyDatabaseMigrations()` on startup, automatically applying all pending migrations.

---

## Step-by-Step: Adding a New Feature

Follow this standard workflow to add a new feature (e.g., **Categories API**):

### 1. Create Domain Entity (`Models/Category.cs`)
```csharp
namespace BookManager.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
}
```

### 2. Register DbSet in `Data/AppDbContext.cs`
```csharp
public DbSet<Category> Categories => Set<Category>();
```

### 3. Create DTOs (`DTOs/Categories/CategoryDto.cs`)
```csharp
namespace BookManager.DTOs.Categories;

public record CategoryDto(int Id, string Name, string Description);
public record CreateCategoryDto(string Name, string Description);
```

### 4. Create Repository Interface & Implementation (`Repositories/`)
```csharp
public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(CancellationToken ct = default);
    Task<Category> AddAsync(Category category, CancellationToken ct = default);
}
```

### 5. Create Service Interface & Implementation (`Service/`)
```csharp
public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default);
}
```

### 6. Create Controller (`Controllers/CategoryController.cs`)
```csharp
[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;
    public CategoryController(ICategoryService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));
}
```

### 7. Register Dependencies in `Extensions/ServiceExtensions.cs`
```csharp
services.AddScoped<ICategoryRepository, CategoryRepository>();
services.AddScoped<ICategoryService, CategoryService>();
```

### 8. Generate & Apply Migration
```bash
dotnet ef migrations add AddCategoryEntity
dotnet ef database update
```

---

## Common Development Tasks

### Formatting & Linting Code
```bash
dotnet format
```

### Running Unit Tests
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Regenerating Swagger OpenAPI Spec
Run the project and navigate to:
👉 `http://localhost:8080/swagger/v1/swagger.json`

---

## Debugging Techniques

### 1. Enabling Detailed SQL Query Logging
To see raw SQL generated by EF Core in your terminal output, update `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### 2. Debugging in Visual Studio Code
Add this configuration to `.vscode/launch.json`:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net10.0/BookManager.dll",
      "args": [],
      "cwd": "${workspaceFolder}",
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openUrl",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      },
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  ]
}
```

---

## Troubleshooting Common Issues

| Symptom | Probable Cause | Resolution |
| :--- | :--- | :--- |
| `28P01: password authentication failed for user "postgres"` | PostgreSQL initialized with an older password stored in the Docker volume. | Run `docker compose down -v` to reset the volume and re-initialize with the password specified in `.env`. |
| `net::ERR_CONNECTION_REFUSED` | The API container crashed on startup or port mapping is incorrect. | Check container logs with `docker compose logs api`. Ensure `API_PORT=8080` in `.env`. |
| `Cannot load library libgssapi_krb5.so.2` | Informational log from Npgsql searching for Kerberos. | Safe to ignore; does not affect application functionality. |
| `Port 5432 is already in use` | A local PostgreSQL instance is already running on port 5432. | Change `POSTGRES_PORT=5433` in `.env` and `docker-compose.yaml`. |
| `Missing required configuration value: JWT_SECRET` | Required JWT key is missing in environment variables. | Ensure `JWT_SECRET` is set in `.env` (at least 32 characters). |
