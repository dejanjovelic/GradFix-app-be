# GradFix Backend

GradFix is an ASP.NET Core Web API that enables citizens to report infrastructure issues within their city. The API provides authentication, report management, image uploads and role-based authorization.

This project was developed as part of the Pierre Enterprises Full Stack Test Task.

---

## Architecture

The application follows a layered architecture with clear separation of responsibilities.

- Controllers handle HTTP requests and responses.
- Services contain business logic.
- Repositories encapsulate data access.
- DTOs isolate the API contract from domain entities.
- Dependency Injection is used throughout the application.

----

## Features

### Authentication

- User registration
- User login
- Google Sign-In
- JWT authentication
- Role-based authorization
- User profile endpoint

### Reports

- Create new reports
- Upload up to three images
- Assign categories
- Store GPS coordinates or manual address
- Retrieve report details
- Automatic initial report status

### Images

- Local file storage
- Image validation
- Unique file names
- Automatic directory creation by year and month

### General

- RESTful API
- Repository pattern
- AutoMapper
- Global exception handling
- DTO separation
- Dependency Injection

---

## Technologies

- ASP.NET Core 9
- Entity Framework Core
- PostgreSQL
- ASP.NET Core Identity
- JWT Bearer Authentication
- AutoMapper
- Google.Apis.Auth
- Swagger (OpenAPI)

---

## Project Structure

```
GradFix-app-be

├── Controllers
├── Domain
├── Infrastructure
├── Mappings
├── Middleware
├── Persistence
├── Services
│   ├── DTOs
│   ├── Interfaces
│   └── Implementations
├── wwwroot
│   └── uploads
└── Program.cs
```

---

## Prerequisites

Before running the application make sure you have installed:

- .NET 9 SDK
- PostgreSQL
- Visual Studio 2022/2026 or Visual Studio Code

---

## Configuration

Create a `.env` file (or configure environment variables) with values similar to the following:

```env
ConnectionStrings__DefaultConnection=YOUR_CONNECTION_STRING

Jwt__Key=YOUR_SECRET_KEY
Jwt__Issuer=GradFix
Jwt__Audience=GradFixUsers
Jwt__ExpiresInMinutes=60

Google__ClientId=YOUR_GOOGLE_CLIENT_ID
```

---

## Database

Apply migrations:

```bash
dotnet ef database update
```

---

## Running the application

Restore packages

```bash
dotnet restore
```

Run the application

```bash
dotnet run
```

or from Visual Studio press

```
F5
```

Swagger will be available at

```
https://localhost:7001/swagger
```

---

## Authentication

The API uses JWT Bearer authentication.

Protected endpoints require

```
Authorization: Bearer {token}
```

JWT contains

- User Id
- Email
- Name
- Surname
- Roles

---

## Image Storage

Uploaded images are stored locally inside

```
wwwroot/uploads/reports/{year}/{month}
```

Images are served as static files.

---

## Error Handling

The API returns appropriate HTTP status codes, including:

| Status | Description |
|---------|-------------|
| 200 | Success |
| 201 | Resource created |
| 400 | Validation error |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Resource not found |
| 500 | Internal server error |

---

## API Documentation

Swagger/OpenAPI is enabled in development.

Example endpoints:

```
POST   /api/auth/register
POST   /api/auth/login
POST   /api/auth/google
GET    /api/auth/me

POST   /api/reports
GET    /api/reports/{id}
```

---

## Testing

Business logic is covered using unit tests with mocked repositories and services.

---

## AI Usage

AI tools (ChatGPT) were used during the development process as a programming assistant for:

- discussing software architecture
- reviewing ASP.NET Core best practices
- generating boilerplate code
- explaining Entity Framework Core behavior
- suggesting improvements for clean architecture
- reviewing unit tests
- identifying implementation issues

All AI-generated code and suggestions were manually reviewed, adapted, tested and integrated before being included in the final solution.

---

## Future Improvements

- Report update
- Report deletion
- Report status workflow
- Pagination
- Filtering
- Admin dashboard endpoints
- Image resizing
- Cloud storage support (Azure Blob Storage / AWS S3)
- Integration tests

---

## Author

Dejan Jovelić