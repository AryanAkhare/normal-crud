# User Management API

A small ASP.NET Core Web API for managing users. It demonstrates controller-based CRUD endpoints, request validation, in-memory storage, and centralized error handling. Data is held in memory and is cleared when the application stops.

## Prerequisites

- .NET 10 SDK

## Run locally

From the repository root:

```powershell
dotnet restore UserManagementApi.slnx
dotnet run --project UserManagementApi\UserManagementApi.csproj
```

The application prints its local listening URL when it starts. No database or external service is required.

## API

All endpoints are under `/api/users`. User IDs are GUIDs.

| Method | Route | Success response |
| --- | --- | --- |
| `GET` | `/api/users` | `200 OK` with the user collection |
| `GET` | `/api/users/{id}` | `200 OK` with the user, or `404 Not Found` |
| `POST` | `/api/users` | `201 Created` with the user and a `Location` header |
| `PUT` | `/api/users/{id}` | `200 OK` with the updated user, or `404 Not Found` |
| `DELETE` | `/api/users/{id}` | `204 No Content`, or `404 Not Found` |

Create a user:

```powershell
$baseUrl = "http://localhost:5000" # Replace with the URL printed by dotnet run.
$user = Invoke-RestMethod -Method Post -Uri "$baseUrl/api/users" `
  -ContentType "application/json" `
  -Body '{"name":"Ada Lovelace","email":"ada@example.com"}'

Invoke-RestMethod -Uri "$baseUrl/api/users/$($user.id)"

Invoke-RestMethod -Method Put -Uri "$baseUrl/api/users/$($user.id)" `
  -ContentType "application/json" `
  -Body '{"name":"Ada Byron","email":"ada.byron@example.com"}'

Invoke-RestMethod -Method Delete -Uri "$baseUrl/api/users/$($user.id)"
```

The `GET /api/users` route returns the collection. Update request bodies have the same `name` and `email` properties as create requests.

## Validation and errors

The API requires a non-empty name of up to 100 characters and a valid email address of up to 254 characters. ASP.NET Core's `[ApiController]` behavior returns `400 Bad Request` with validation details for invalid request bodies. Requests for a user that does not exist return `404 Not Found`.

Unhandled exceptions are logged by the server and returned as generic `500 Internal Server Error` Problem Details; internal exception details are not sent to API clients.

## Middleware

`ExceptionHandlingMiddleware` is registered before controller endpoints. It logs unhandled exceptions and converts them to generic Problem Details responses. The API does not add authentication, authorization, or HTTPS-redirection middleware.

## Tests

Run the API and middleware tests from the repository root:

```powershell
dotnet test UserManagementApi.slnx
```

The tests exercise HTTP CRUD behavior, validation and missing-user responses, and exception middleware output.
