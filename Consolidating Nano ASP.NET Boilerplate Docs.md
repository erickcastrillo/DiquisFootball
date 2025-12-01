# Nano ASP.NET Boilerplate Documentation

## 1. Solution Overview

### Overview
The Nano ASP.NET Boilerplate represents a specialized, architectural foundation designed specifically for the rapid development of Software as a Service (SaaS) applications within the .NET ecosystem.

Unlike comprehensive, highly opinionated frameworks that often abstract away the underlying .NET primitives—forcing developers to learn a proprietary "framework within a framework"—the Nano solution is engineered to be **"unopinionated"** and **"customizable"**. It leverages standard ASP.NET Core patterns and libraries, ensuring that the generated code remains accessible, maintainable, and free from dependencies on proprietary NuGet packages that could introduce vendor lock-in or versioning conflicts.

The architectural philosophy governing the solution is the **Clean Architecture pattern** (also known as Onion Architecture). This concentric design strategy prioritizes the separation of concerns, ensuring that the core business logic (the Domain) remains insulated from external dependencies such as databases, user interfaces, or third-party services. By enforcing strict dependency rules where inner layers know nothing of the outer layers, the boilerplate enhances testability and modularity, allowing individual components—such as the database provider or the frontend technology—to be swapped or upgraded with minimal impact on the core business rules.

The solution is delivered as a comprehensive .NET solution template, installable via the CLI, which scaffolds a production-ready infrastructure including essential SaaS features: **multi-tenancy, JWT-based authentication, role-based authorization, and a pre-configured persistence layer.**

### Implementation: Architecture and Layers
The solution is physically structured into four distinct layers, represented as separate projects within the Visual Studio solution. This physical separation enforces the logical boundaries of the Clean Architecture pattern.

#### 1. Presentation Layer (WebApi / RazorApp)
The outermost layer of the application is the entry point, responsible for handling HTTP requests and orchestrating interactions with the application layer. Depending on the user interface strategy selected during project generation, this layer manifests in one of two forms:

* **WebApi:** This project is generated when the solution is configured for a Single Page Application (SPA) frontend (React or Vue). It serves as a pure RESTful API, exposing endpoints that return JSON data.
    * **Role:** It acts as the gateway for external clients, handling authentication (JWT), request validation, and response formatting.
    * **Components:** It contains the `Program.cs` entry point, `appsettings.json` configuration, and API Controllers for core domains like Authorization, Identity, Tenants, and Products.
    * **Static File Serving:** In production, it is configured to serve the static assets (HTML, JS, CSS) of the compiled SPA client.

* **RazorApp:** This project is generated when the solution is configured for a Server-Side Rendering (SSR) approach.
    * **Role:** It functions as a traditional Multi-Page Application (MPA) using ASP.NET Core Razor Pages.
    * **Authentication Difference:** Unlike the stateless JWT approach of the WebApi, the RazorApp utilizes Cookie-based authentication, which is standard for server-rendered scenarios.
    * **Structure:** It includes both Razor Views (`.cshtml`) for the UI and API Controllers for specific dynamic interactions.

#### 2. Application Layer
Directly beneath the presentation layer lies the Application Layer. This library defines the behavior of the software—the specific use cases and business operations the system supports.

* **Responsibility:** It orchestrates the flow of data between the presentation layer and the domain/infrastructure layers. It *does not* contain business rules (which belong in the Domain) or database logic (which belongs in Infrastructure).
* **Components:**
    * **Application Services:** Classes (e.g., `ProductService`) that implement the `IProductService` interface, executing logic like "Create Product" or "Register Tenant".
    * **DTOs (Data Transfer Objects):** Simple classes used to transport data between processes. They ensure that internal Domain Entities are not exposed directly to the API, preventing over-posting attacks and decoupling the API contract from the database schema.
    * **Interfaces:** It defines the contracts for infrastructure services (e.g., `IRepositoryAsync`, `IEmailService`), adhering to the Dependency Inversion Principle.

#### 3. Infrastructure Layer
The Infrastructure Layer serves as the support system for the application, providing the concrete implementations for the interfaces defined in the Application Layer.

* **Responsibility:** It handles all external concerns, specifically persistence (database access) and integration with third-party services.
* **Components:**
    * **Persistence:** Implements the Entity Framework Core `DbContext` and Repository patterns. It manages database migrations and connection strings.
    * **Identity:** Contains the concrete implementation of ASP.NET Core Identity services for user management.
    * **Services:** Implementations for file storage (e.g., local disk or cloud), email sending, and tenant resolution reside here.

#### 4. Domain Layer
At the center of the architecture is the Domain Layer. This is the most stable part of the system, containing the enterprise-wide logic and rules.

* **Responsibility:** It represents the business concepts and their relationships, independent of any application logic or technology choices.
* **Components:**
    * **Entities:** Classes representing database tables (e.g., `Tenant`, `Product`, `ApplicationUser`). All entities derive from a common `BaseEntity` (often using GUIDs for IDs).
    * **Interfaces:** Marker interfaces such as `IMustHaveTenant` are defined here to enforce cross-cutting concerns like multi-tenancy on specific entities.

### Configuration: UI Options Comparison
The Nano Boilerplate offers flexibility in frontend architecture. The choice of UI determines the project structure and authentication mechanisms generated by the CLI.

| Feature | React / Vue UI (SPA) | Razor Pages (MPA) |
| :--- | :--- | :--- |
| **Project Type** | WebApi (REST API) | RazorApp (MVC/Razor) |
| **Authentication** | JSON Web Tokens (JWT) | Secure HTTP Cookies |
| **Client Rendering** | Client-side (Browser) | Server-side (.NET) |
| **API Interaction** | AJAX / Fetch via REST | Direct Service Injection / AJAX |
| **Development** | Separate Node.js Server | Single IIS/Kestrel Server |
| **Deployment** | Static Files + API Backend | Single .NET Assembly |

---

## 2. Requirements and Environment Setup

### Overview
The effective utilization of the Nano ASP.NET Boilerplate requires a correctly configured development environment. As a modern .NET solution, it relies on the latest Long Term Support (LTS) versions of the framework and standard enterprise-grade database engines.

### Implementation: System Requirements

#### 1. Backend Development (.NET Solution)
To compile, build, and run the core .NET solution, the host machine must meet the following specifications:

* **.NET SDK:** The solution targets **.NET 10.0**. This SDK must be installed globally to access the `dotnet` CLI and build tools.
* **Database:** A SQL Server instance is required. For development, **SQL Server Express LocalDB** is the default configuration, as it requires zero configuration. However, a full SQL Server instance (Developer or Enterprise edition) running locally or in a container is also supported.
* **IDE:** **Visual Studio 2026** is the recommended Integrated Development Environment for its robust IntelliSense, refactoring tools, and deep integration with MSBuild. Alternatively, JetBrains Rider or Visual Studio Code (with the C# Dev Kit) can be used.

#### 2. Frontend Development (SPA)
If the project utilizes the React or Vue user interface options, the following additional tools are necessary to manage the JavaScript build pipeline:

* **Node.js:** The runtime environment required to execute the build scripts and development server.
* **NPM (Node Package Manager):** Required for resolving client-side dependencies (packages listed in `package.json`).
* **Editor:** Visual Studio Code is highly recommended for frontend development due to its superior support for JavaScript/TypeScript ecosystems, though Visual Studio 2026 can also handle these file types.

#### 3. Razor Pages Development
For projects selecting the Razor Pages option, the requirements are identical to the Backend Development requirements. However, if custom styling (Sass/CSS) compilation is required, Node.js and NPM may still be needed to run the frontend asset pipeline.

### Implementation: Installation and First Run
The installation process is streamlined through the use of NuGet package distribution, allowing developers to treat the boilerplate as a standard project template.

#### Step 1: Template Installation
The Nano Boilerplate is distributed as a NuGet package file (`.nupkg`). The installation involves registering this package with the .NET CLI.

1.  **Locate Package:** Navigate to the `dotnet` folder in the downloaded distribution to find `Nano.Boilerplate.2.3.0.nupkg`.
2.  **Open Terminal:** Launch a PowerShell or Command Prompt window in this directory.
3.  **Execute Install Command:**

```bash
dotnet new install .\Nano.Boilerplate.2.3.0.nupkg
```

> **Context:** This command installs the templates into the global .NET template cache. Once executed, `dotnet new nano` becomes a valid command anywhere on the machine. To remove the templates later, use `dotnet new uninstall Nano.Boilerplate`.

#### Step 2: Running the Application
Upon generating a project, the application is designed to be "runnable" immediately without manual database script execution.

* **Automatic Migrations:** The `Program.cs` entry point is configured to check the database status on startup. If the configured database does not exist, Entity Framework Core will create it. If pending migrations exist, they are applied automatically.
* **Database Seeding:** The system includes a seeding mechanism (`DbInitializer`) that populates the database with initial required data if it is empty.
* **Root Tenant:** A default tenant is created to host the initial users.
* **Admin User:** A root administrator account is generated with the following credentials:
    * **Username:** admin@email.com
    * **Password:** Password123!

#### Step 3: Accessing the Application
By default, the application is configured to listen on specific ports.

* **API / Razor App:** `https://localhost:7250/`
* **SPA Client (Development):** `http://localhost:3000/` (Typically managed by Vite/Webpack dev server).
* **SPA Client (Production/Build):** When the SPA is built, the static files are served directly by the WebApi project at `https://localhost:7250/`.

---

## 3. CLI Tool Usage and Project Scaffolding

### Overview
A defining feature of the Nano ASP.NET Boilerplate is its integration with the .NET CLI via the `dotnet new` templating engine. The Nano CLI tool serves as a productivity accelerator, automating the generation of complex architectural structures.

### Implementation: Scaffolding a New Solution
The `nano` template is used to bootstrap a completely new solution. This process generates the `.sln` file and all associated projects (WebApi, Application, Infrastructure, Domain).

**Command Syntax:**
```bash
dotnet new nano –n <ProjectName> [options]
```

**Parameters:**
* **Name (-n):** The name of the solution (e.g., MySaaS). This namespace will be applied to all generated code.
* **Multi-Tenancy (-m):** Configures the database architecture.
    * `multidb`: Multi-Database Multi-Tenancy. Each tenant gets a dedicated database. This is the default and most robust option for data isolation.
    * `singledb`: Single-Database Multi-Tenancy. All tenants share a database, isolated by a `TenantId` column.
    * `singletenant`: Single Tenant. Removes multi-tenancy logic entirely for traditional applications.
* **User Interface (-ui):** Configures the presentation layer.
    * `spa`: Generates the WebApi project optimized for React/Vue clients. (Default).
    * `razor`: Generates the RazorApp project for server-side rendering.

**Example: Creating a React-based SaaS with Multi-Database support**
```bash
dotnet new nano –n MySaaS –m multidb –ui spa
```

### Implementation: Scaffolding Services (CRUD)
The `nano-service` template automates the creation of a "Vertical Slice" of functionality within the Application layer. It generates the logic required to perform Create, Read, Update, and Delete operations on a new entity.

**Command Syntax:**
```bash
dotnet new nano-service –s <SingularName> –p <PluralName> –ap <AppName>
```

**Parameters:**
* `-s`: Singular name of the entity (e.g., Supplier). Used for class names (`Supplier.cs`).
* `-p`: Plural name (e.g., Suppliers). Used for collection names and endpoints.
* `-ap`: The root namespace of the application (e.g., MySaaS).

**Generated Artifacts:**
Executing this command in the `Application/Services` folder creates:
* **Entity:** `Domain/Entities/Supplier.cs`
* **DTOs:** `Application/Services/Suppliers/DTOs/` (Requests and Responses).
* **Interface:** `Application/Services/Suppliers/ISupplierService.cs`
* **Implementation:** `Application/Services/Suppliers/SupplierService.cs`
* **Specifications:** Filters and query logic for the repository.

**Example Command:**
```bash
dotnet new nano-service –s Supplier –p Suppliers –ap MySaaS
```

### Implementation: Scaffolding Controllers
The `nano-controller` template generates the API surface area. It creates a controller that injects the corresponding Application Service and exposes its methods as HTTP endpoints.

**Command Syntax:**
```bash
dotnet new nano-controller –s <SingularName> –p <PluralName> –ap <AppName>
```

**Special Configuration for Razor Pages:**
If the solution was generated with `-ui razor`, the flag `-ui razor` must be appended to both the service and controller generation commands. This ensures the `GetPaginatedResultsAsync` method generates the correct DTO structure for JQuery Datatables rather than TanStack Table.

---

## 4. Web API Implementation and Configuration

### Overview
The `AspNano.WebApi` project serves as the operational nucleus of the backend solution. Designed for .NET 10, it adopts the **Minimal Hosting Model**, which consolidates the application startup logic into a single `Program.cs` file.

### Implementation: Program.cs Entry Point
To maintain maintainability and strictly adhere to the Clean Architecture principle, the Nano Boilerplate heavily utilizes Extension Methods for service registration.

#### Service Registration Strategy
Instead of listing dozens of service registrations directly in `Program.cs`, the code delegates this to `ServiceCollectionExtensions.cs`.

* **Static Configuration (`ConfigureApplicationServices`):** Handles registration of known, infrastructure-heavy services (CORS, Database Contexts, Identity, MailKit, Cloudinary).
* **Dynamic Configuration (`DynamicServiceRegistrationExtensions`):** A scanner iterates through assemblies and automatically registers any class implementing specific marker interfaces (`ITransientService`, `IScopedService`).

#### Middleware Pipeline
A unique and critical component in this pipeline is the **Tenant Resolver**.

```csharp
var app = builder.Build();

// Standard Pipeline
app.UseHttpsRedirection();
app.UseAuthorization(); // ASP.NET Core Authorization

// Custom Multi-Tenancy Middleware
app.UseMiddleware<TenantResolver>();

app.MapControllers();
app.Run();
```
> **Context:** The `app.UseMiddleware<TenantResolver>()` line is strategically placed. It must execute after Authorization (so the user Identity is known) but before the Controllers (so the Controller knows which Tenant context to use).

### Configuration: appsettings.json
The `appsettings.json` file serves as the centralized repository for application configuration.

**Key Configuration Sections:**

**Connection Strings:**
```json
"ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\mssqllocaldb;Database=MyApplicationDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

**JWT Settings:**
```json
"JwtSettings": {
    "Key": "Your-256-bit-Secret-Key-Must-Be-Long",
    "Issuer": "NanoBoilerplate",
    "Audience": "NanoBoilerplate",
    "DurationInMinutes": 30,
    "RefreshTokenDurationInDays": 14
}
```

---

## 5. Persistence Infrastructure

### Overview
The Persistence Layer utilizes **Entity Framework Core (EF Core)**. The architecture is sophisticated, designed to handle the complexities of multi-tenancy without exposing them to the Application layer.

### Implementation: Database Context Architecture
To support flexible multi-tenancy requirements, the solution employs a specialized multi-context strategy.

#### 1. BaseDbContext (The Management Context)
* **Scope:** Manages shared, system-level data.
* **Entities:** Houses the `Tenant` entity and core ASP.NET Identity tables.
* **Isolation:** In a multi-db setup, these tables exist only in the Root Database.

#### 2. ApplicationDbContext (The Business Context)
* **Scope:** Manages the actual domain entities (e.g., Products, Orders).
* **Multi-DB Behavior:** A separate instance of this context exists for each tenant.
* **Single-DB Behavior:** Houses all tables, including Tenants and Identity.

**Code Example: ApplicationDbContext & Global Filters**
This code demonstrates how the context enforces tenant isolation automatically.

```csharp
public class ApplicationDbContext : DbContext
{
    private readonly ICurrentTenantService _currentTenantService;
    public string CurrentTenantId { get; set; }

    // Constructor injecting the Tenant Service
    public ApplicationDbContext(ICurrentTenantService currentTenantService, DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        _currentTenantService = currentTenantService;
        CurrentTenantId = _currentTenantService.TenantId;
    }

    // DbSets for Domain Entities
    public DbSet<Product> Products { get; set; }
    public DbSet<Tenant> Tenants { get; set; }

    // Override SaveChanges to enforce Tenant Isolation on writes
    public override int SaveChanges()
    {
        // Detect all added/modified entities that implement IMustHaveTenant
        foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    // Force the TenantId to match the current request context
                    entry.Entity.TenantId = CurrentTenantId;
                    break;
            }
        }
        return base.SaveChanges();
    }
}
```

#### 3. TenantDbContext (The Lookup Context)
A lightweight auxiliary context used specifically during the Tenant Resolution phase to look up connection strings.

### Configuration: Migrations
Database schema changes are managed via EF Core Migrations. Because of the multi-context architecture, migrations must be explicitly targeted.

**Creating a Migration (Multi-DB Example):**
```bash
add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb App-NewMigration
```

---

## 6. Multi-Tenancy Engine

### Overview
Multi-tenancy allows a single deployment to serve multiple distinct customer organizations. The system provides strict data isolation vertically through the stack.

### Implementation: Tenant Resolution Middleware
The **Tenant Resolver Middleware** intercepts every incoming HTTP request to identify the "Current Tenant".

**Resolution Strategy:**
1.  **Authenticated Requests:** TenantId is extracted from the JWT Claims.
2.  **Unauthenticated Requests:** Client must provide the Tenant ID via a custom HTTP Header.

### Implementation: Database Isolation Strategies

| Strategy | multidb (Multi-Database) | singledb (Single-Database) |
| :--- | :--- | :--- |
| **Data Storage** | Separate Database per Tenant | Shared Database for all Tenants |
| **Isolation** | Physical (Separate MDF/LDF files) | Logical (TenantId column) |
| **Security** | Highest (Data physically separated) | High (Relies on Query Filters) |
| **Cost** | Higher (More DB resources) | Lower (Shared resources) |
| **Maintenance** | Complex (Migrate N databases) | Simple (Migrate 1 database) |

**Logical Isolation (Code Level):**
regardless of the physical strategy, logical isolation is enforced using the `IMustHaveTenant` interface.

```csharp
public interface IMustHaveTenant {
    public string TenantId { get; set; }
}

public class Product : IMustHaveTenant {
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string TenantId { get; set; }
}
```

---

## 7. Authentication and Authorization

### Overview
Security is built upon ASP.NET Core Identity adapted for stateless REST APIs using **JSON Web Tokens (JWT)**.

### Implementation: Authentication Service
The flow centers on the `TokenService`.
1.  **Login Request:** User submits credentials.
2.  **Validation:** `UserManager` validates password hash.
3.  **Token Generation:** Generates a JWT with `TenantId` and `UserId` embedded in Claims.
4.  **Refresh Tokens:** A short-lived JWT is paired with a long-lived Refresh Token for seamless user experience.

### Implementation: Authorization Policies
Access control is enforced using standard attributes.

**Code Example: Secure Controller**
```csharp
// Only Admins can access
[HttpPost]
public async Task<IActionResult> Create(CreateProductRequest request)
{
    return Ok(await _productService.CreateProductAsync(request));
}

[AllowAnonymous] // Public access
[HttpPost("login")]
public async Task<IActionResult> GetTokenAsync(TokenRequest request)
{
    return Ok(await _tokenService.GetTokenAsync(request));
}
```

---

## 8. Application Services

### Overview
Application Services constitute the API of the business layer. They implement the Service-Oriented Architecture (SOA) pattern, encapsulating business logic behind coarse-grained interfaces.

### Implementation: Service Anatomy
A typical service in the Nano Boilerplate follows a rigorous structure to ensure consistency and testability.

#### 1. The Interface (`IProductService`)
Defines the contract. Services typically return DTOs (Data Transfer Objects) rather than Domain Entities.

#### 2. The Implementation (`ProductService`)
The service implements the interface. It utilizes Dependency Injection to access the `IRepositoryAsync` (or `DbContext`) and `IMapper` (AutoMapper).

**Code Walkthrough: ProductService Logic**
```csharp
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context; // Or IRepositoryAsync

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    // CREATE Operation
    public Product CreateProduct(CreateProductRequest request)
    {
        // 1. Map DTO to Entity
        var product = new Product();
        product.Name = request.Name;
        product.Price = request.Price;

        // 2. Persist to Database
        _context.Add(product);
        _context.SaveChanges(); 
        // Note: SaveChanges automatically sets TenantId via Interceptor/Override
        
        return product;
    }

    // READ Operation (List)
    public IEnumerable<Product> GetAllProducts()
    {
        // Global Query Filter automatically applies "WHERE TenantId = X"
        var products = _context.Products.ToList();
        return products;
    }
}
```

#### 3. Pagination and DTOs
The boilerplate includes specialized handling for pagination requests (`GetProductsPaginatedAsync`).

* **SPA Mode:** Returns data formatted for **Tanstack Table v8** (focus on `pageIndex`, `pageSize`).
* **Razor Mode:** Returns data formatted for **JQuery Datatables** (focus on `draw`, `recordsTotal`, `recordsFiltered`).

---

## 9. React UI Architecture

### Overview
The React UI option generates a modern, robust Single Page Application (SPA). Built with TypeScript and bundled with Vite, it represents a fully decoupled frontend that communicates with the .NET backend strictly via REST APIs.

### Implementation: Project Structure
The React project structure (`client` folder) is organized by feature rather than file type, promoting modularity.

* `src/pages`: Contains the top-level View components (e.g., `pages/products/ProductsList.tsx`).
* `src/components`: Contains reusable, atom-level UI widgets.
* `src/layout`: Houses the global application shell.
* `src/assets`: Stores static resources and the Bootstrap 5 SCSS theme files.

### Implementation: Core Libraries

#### 1. Data Tables (TanStack Table v8)
For displaying data grids—a core requirement of SaaS apps—the solution uses TanStack Table v8.
* **Headless Architecture:** It provides the logic for sorting, filtering, and pagination hooks but delegates the rendering to the developer.
* **Integration:** The Nano implementation wraps these hooks with Bootstrap 5 table classes.

#### 2. Forms and Validation (Formik + Yup)
Forms are managed using Formik, the standard for React form state management.
* **Validation:** Validation rules are defined as Yup Schemas (e.g., `yup.object({ name: yup.string().required() })`).

#### 3. Environment Configuration
The React app uses Vite's environment variable system to manage API connections.
* `.env.development`: Sets `VITE_API_URL` to `https://localhost:7250`.
* `.env.production`: Sets `VITE_API_URL` to the production API endpoint.