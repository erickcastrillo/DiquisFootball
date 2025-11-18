# Diquis - ASP.NET Core Architecture

## 1. Clean Architecture Overview

The Diquis API is built upon the principles of **Clean Architecture**, as implemented by the ASPNano boilerplate. This architectural style emphasizes a separation of concerns, which makes the application more maintainable, scalable, and testable.

### The Dependency Rule
The fundamental rule is that **source code dependencies can only point inwards**. Nothing in an inner layer can know anything about an outer layer. This means the business logic is independent of the database, UI, or any external services.

```
+---------------------------------------------------------+
|  WebApi (Presentation) & Infrastructure (External)      |
| +-----------------------------------------------------+ |
| |         Application (Use Cases)                     | |
| | +-------------------------------------------------+ | |
| | |               Domain (Entities)                 | | |
| | +-------------------------------------------------+ | |
| +-----------------------------------------------------+ |
+---------------------------------------------------------+
      <-- Dependency Flow -->
```

---

## 2. Application Layers

The solution is divided into four main projects, each representing a layer of the architecture.

### `Diquis.Domain` (Core)

This is the innermost layer and the heart of the application. It contains the enterprise-wide business logic and data models.

- **Responsibilities**:
  - Defines the core **Entities** (e.g., `Player`, `Team`) and their intrinsic business rules (invariants).
  - Contains domain-specific **Enums**, **Exceptions**, and **Domain Events**.
- **Dependencies**: This project has **no dependencies** on any other project in the solution. It is a pure C# library.

### `Diquis.Application` (Use Cases)

This layer contains the application-specific business logic and orchestrates the data flow. It defines the application's use cases (features).

- **Responsibilities**:
  - Defines **Interfaces** (contracts) for infrastructure concerns, such as `IRepository<T>` for data access or `IMailService` for sending emails. This inverts the dependency on the infrastructure layer.
  - Contains **DTOs (Data Transfer Objects)** that define the shape of data for API requests and responses.
  - Implements **Business Logic** in **Application Services** (e.g., `ProductService`) or through a mediator pattern (e.g., using MediatR with Commands and Queries).
  - Includes **Validation Logic** for incoming DTOs, typically using **FluentValidation**.
- **Dependencies**: Depends only on `Diquis.Domain`.

### `Diquis.Infrastructure` (Implementation)

This layer contains the concrete implementations for all external concerns and services defined in the `Application` layer.

- **Responsibilities**:
  - **Persistence**: Implements the `IRepository<T>` interface using **Entity Framework Core**. This includes the `DbContext` classes (`ApplicationDbContext`, `TenantDbContext`), database migrations, and the `DbInitializer` for seeding data.
  - **Identity & Security**: Implements authentication using **ASP.NET Core Identity** and services for JWT generation.
  - **External Service Integrations**: Provides concrete implementations for services like:
    - **Mailing**: Using **MailKit** to send emails.
    - **Image Handling**: Using **Cloudinary** for image uploads and transformations.
    - **File Storage**: Using **Azure Blob Storage** for general file uploads.
    - **Excel Exports**: Using **EPPlus** to generate Excel files.
    - **PDF Exports**: Using **QuestPDF** to generate PDF documents.
  - **Object Mapping**: Contains **AutoMapper** profiles to map between Domain entities and Application DTOs.
- **Dependencies**: Depends on `Diquis.Application`.

### `Diquis.WebApi` (Presentation)

This is the outermost layer, responsible for handling HTTP requests and presenting the application's features to the outside world.

- **Responsibilities**:
  - **API Controllers**: Defines the RESTful API endpoints. Controllers should be "thin," meaning they delegate all business logic to the `Application` layer.
  - **Middleware**: Contains the middleware pipeline, including:
    - `TenantResolver`: For identifying the current tenant on each request.
    - Global error handling.
    - Other custom middleware.
  - **Configuration & Startup**: The `Program.cs` file bootstraps the application. It is responsible for:
    - Registering all services for dependency injection (using extension methods like `AddServices` and `ConfigureApplicationServices`).
    - Configuring the middleware pipeline.
    - Loading configuration from `appsettings.json`.
  - **Static Files**: Serves the built React application from the `wwwroot` folder in production.
- **Dependencies**: Depends on `Diquis.Application` and `Diquis.Infrastructure`.

---

## 3. Key Architectural Concepts

### Multi-Tenancy
The architecture is designed for multi-tenancy from the ground up. See the [Multi-Tenancy Guide](./MULTI_TENANCY.md) for a detailed explanation.

### API Design
- **RESTful**: The API follows REST principles with standard HTTP verbs and status codes.
- **DTOs**: Request and response models are used to decouple the API contract from the internal domain models.
- **Validation**: **FluentValidation** is used for validating incoming request DTOs.
- **Documentation**: **Swashbuckle** is used to automatically generate an OpenAPI (Swagger) specification.

### Security
- **Authentication**: Handled by **ASP.NET Core Identity** with **JWT Bearer tokens**.
- **Authorization**: Implemented using **Policy-Based Authorization** for fine-grained control.

---

## 4. Background Jobs

The application uses **Hangfire** for processing background jobs. This allows long-running, asynchronous tasks (like sending emails or generating reports) to be executed outside of the main HTTP request thread, improving API responsiveness and reliability.

### Key Components

- **`IBackgroundJobService` Interface**: Defined in the `Application` layer, this interface provides an abstraction for enqueuing jobs. This ensures the application's business logic is not directly dependent on Hangfire.

- **`HangfireJobService` Implementation**: Located in the `Infrastructure` layer, this class implements `IBackgroundJobService` using Hangfire's `IBackgroundJobClient`.

- **Job Storage**: Hangfire is configured to use the main application database (SQL Server or PostgreSQL) to persist job information. This ensures that jobs are not lost if the application restarts.

- **Hangfire Dashboard**: A monitoring dashboard is available at the `/hangfire` route. Access is restricted to users with the "Admin" role via a custom authorization filter (`HangfireAuthorizationFilter`). This portal allows administrators to view the status of all scheduled, processing, succeeded, and failed jobs.
