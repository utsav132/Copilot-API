# User Management API - HypeTech Solutions

A secure and robust ASP.NET Core Web API for managing user data, featuring custom middleware for auditing, security, and error resilience.

## 🚀 Features

- **Standardized CRUD**: Endpoints for creating, reading, updating, and deleting users.
- **Custom Middleware Pipeline**:
  - **Global Exception Handling**: Returns consistent JSON error responses for any unhandled failures.
  - **Token-Based Authentication**: Secures endpoints using a Bearer token mechanism.
  - **Request/Response Logging**: Full auditing of HTTP methods, paths, and status codes.
- **In-Memory Storage**: Seeded with sample data for immediate testing.

## 🛠️ Middleware Configuration

To comply with corporate auditing and security policies, the middleware is configured in the following order in `Program.cs`:

1.  **Exception Handling**: Catches errors from all downstream components.
2.  **Authentication**: Validates identity before allowing access to logs or business logic.
3.  **Logging**: Audits successful (and failed) authenticated requests.

## 📝 How to Log Correctly

For this API, logging is handled by the `RequestResponseLoggingMiddleware`. To maintain high-quality audit logs:

1.  **Use Structured Logging**: Always use template strings (e.g., `_logger.LogInformation("HTTP {Method} {Path} received", method, path)`) rather than string interpolation. This allows log providers (like Azure Monitor or ELK) to index the parameters separately.
2.  **Log at the Right Time**: Our logging middleware captures the request *before* execution and the status code *after* execution (`await _next(context)`).
3.  **Audit Key Metadata**: Always include:
    -   **HTTP Method** (GET, POST, etc.)
    -   **Request Path**
    -   **Response Status Code** (200, 401, 500, etc.)
4.  **Sensitivity**: Avoid logging sensitive information like passwords or full tokens. Only log the existence or validity of authentication.

## 🚦 Getting Started

### Prerequisites
- .NET 8.0 SDK or later

### Running the API
1.  Navigate to the project directory:
    ```bash
    cd UserManagementAPI
    ```
2.  Run the application:
    ```bash
    dotnet run
    ```

### Authentication
All requests must include the following header:
- **Header**: `Authorization`
- **Value**: `Bearer HypeTech-Secure-Token-2026`

### Testing with Postman
1.  Set method to `GET` and URL to `http://localhost:5243/users`.
2.  In the **Auth** tab, select **Bearer Token** and enter `HypeTech-Secure-Token-2026`.
3.  Click **Send**.

## 📁 Project Structure
- `/Middleware`: Custom middleware classes.
- `/Models`: Data transfer objects (User.cs).
- `Program.cs`: API configuration and route definitions.
