# 📋 TaskProject

*TaskProject* is a clean, scalable, and secure Task Management API built using modern .NET 8 architectural patterns. It empowers authenticated users to manage personal tasks, while providing administrators with tools for user oversight, application logs, and analytics.

---

## 🚀 Features

### 👤 Authentication & Authorization
- JWT-based authentication
- Role-based access control (Admin, User)
- API rate limiting per user
- API versioning (v1)

### 🧑‍💼 User Functionality
- Register/Login via JWT
- Create, update, delete personal tasks
- View completed and pending tasks with pagination
- Receive real-time notifications via SignalR
- View personal notifications

### 🔐 Admin Functionality
- View all registered users (paginated)
- View application logs (from database and file)
- Role-restricted access via [Authorize(Roles = "Admin")]

### ⚙ Technical Stack
- *.NET 8 Web API*
- *PostgreSQL* with EF Core
- *CQRS + MediatR*
- *Hexagonal (Ports & Adapters) Architecture*
- *FluentValidation, **AutoMapper, **Unit of Work*
- *Serilog* (File + DB sink)
- *SignalR* for real-time notifications
- *In-memory caching*
- *Swagger/OpenAPI documentation*
- *Rate limiting (Fixed Window)*

---

## 🧱 Architecture Overview

This solution follows *Clean Architecture* principles with:
- *Domain Layer*: Core entities and logic
- *Application Layer*: Use-cases (CQRS), DTOs, validation, interfaces
- *Infrastructure Layer*: EF Core, Serilog, services (email, cache, logging)
- *API Layer*: Controllers, middlewares, SignalR hubs

Also follows:
- SOLID principles
- DRY (Don’t Repeat Yourself)
- Loosely coupled design

---

## ✅ Functional Endpoints

| Area        | Endpoint                        | Role      |
|-------------|----------------------------------|-----------|
| Auth        | /api/v1/auth/register          | Public    |
|             | /api/v1/auth/login             | Public    |
| User        | /api/v1/user/me                | User/Admin |
|             | /api/v1/user/tasks             | User      |
|             | /api/v1/user/tasks/completed   | User      |
|             | /api/v1/user/notifications     | User      |
| Admin       | /api/v1/user/all               | Admin     |
|             | /api/v1/user/logs              | Admin     |

---

## 🧪 Testing

### ✅ Unit Tests
- Handlers (e.g. CreateTaskCommandHandler, DeleteTaskHandler)
- Validation logic
- Business rules

### 🔄 Integration Tests
- Controller endpoints via TestServer
- Full request → handler → DB → response cycle
- Test coverage includes success, invalid input, and unauthorized access

Run tests:
```bash
dotnet test


---

### 🧠 Design Principles

This project is built using best practices and principles:

- *Clean Architecture* – separation of concerns at every layer  
- *Hexagonal Architecture (Ports & Adapters)* – decouples domain from infrastructure  
- *SOLID Principles* – Single Responsibility, Open/Closed, Dependency Inversion, etc.  
- *CQRS Pattern* – clear separation between reads (queries) and writes (commands)  
- *DRY & KISS* – clean, minimal, non-repetitive code  
- *Loose Coupling & High Cohesion* – all services are easily testable and replaceable

---

### 📦 Packages & Libraries Used

| Purpose               | Library                                    |
|-----------------------|--------------------------------------------|
| CQRS / Mediator       | MediatR                                   |
| Validation            | FluentValidation                          |
| Database ORM          | EF Core + Npgsql                        |
| Logging               | Serilog (File + PostgreSQL sinks)        |
| API Documentation     | Swashbuckle.AspNetCore (Swagger)         |
| JWT Authentication    | Microsoft.AspNetCore.Authentication.JwtBearer |
| Real-time Notifications | SignalR                                |
| Caching               | IMemoryCache                             |
| API Versioning        | Microsoft.AspNetCore.Mvc.Versioning      |
| Rate Limiting         | .NET Rate Limiting (built-in or via AspNetCoreRateLimit) |
| Testing               | xUnit, Moq, TestServer               |

---

### 📁 Folder Structure
TaskProject/
├── API/                → Web entry point, controllers, hubs, middleware
├── Application/        → CQRS (Commands, Queries), DTOs, Abstractions, Validators
├── Domain/             → Core entities, enums, interfaces
├── Infrastructure/     → EF Core DbContext, caching, logging, services
├── Tests/              → Unit & integration tests
├── README.md           → Project documentation
├── TaskProject.sln     → Solution file



---

### 📘 API Documentation

Once running locally, access Swagger UI at:
https://localhost:7123/swagger



Swagger includes:
- Endpoint paths and summaries
- HTTP method descriptions
- Request/response schemas
- Authorization setup (JWT bearer input)

---

### 👨‍💻 Author

*Ohio Shakirullah*  
📧 Email: shakirullahohio@gmail.com  
🔗 GitHub: [github.com/Shaqoo](https://github.com/Shaqoo)

---

### 📄 License

This project is licensed under the *MIT License*.  
You are free to use, modify, and distribute this software for personal or commercial purposes.
