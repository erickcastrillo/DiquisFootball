# Diquis Football Academy SaaS Platform - GitHub Copilot Instructions

## Project Overview

**Project Name:** Diquis Football Academy Management Platform  
**Architecture:** Clean Architecture with Multi-Tenancy  
**Tech Stack:** .NET 10, ASP.NET Core WebAPI, React (TypeScript/Vite), PostgreSQL, Hangfire, OpenTelemetry  
**Domain:** Youth Football Academy Management  

## Architectural Principles

### Clean Architecture Layers

```
???????????????????????????????????????????
?  Presentation Layer (Diquis.WebApi)   ?  ? Controllers, Middleware, Program.cs
???????????????????????????????????????????
?  Application Layer (Diquis.Application)?  ? Services, DTOs, Interfaces
???????????????????????????????????????????
?  Infrastructure (Diquis.Infrastructure) ?  ? EF Core, External Services
???????????????????????????????????????????
?  Domain Layer (Diquis.Domain)          ?  ? Entities, Value Objects, Enums
???????????????????????????????????????????
```

**CRITICAL RULES:**
- Domain layer NEVER references other layers
- Application layer defines interfaces, Infrastructure implements them
- All background job interfaces belong in `Diquis.Application.Common.BackgroundJobs`
- All background job implementations belong in `Diquis.Infrastructure.BackgroundJobs`
- Controllers are thin - delegate to Application Services

### Multi-Tenancy Architecture

**Tenancy Model:** Hybrid (Multi-DB for Professional/Enterprise, Single-DB for Grassroots)

**Key Components:**
1. **TenantResolver Middleware** - Extracts tenant context from JWT or HTTP headers
2. **BaseDbContext** - Manages system-level data (Tenants, Identity)
3. **ApplicationDbContext** - Manages business data with tenant isolation
4. **IMustHaveTenant Interface** - Marker for tenant-scoped entities

**Tenant Resolution Order:**
1. JWT Claims (`tenant` claim)
2. Subdomain (e.g., `academy1.diquis.com`)
3. HTTP Header (`X-Tenant-ID`)
4. Fallback to `"root"` for initial load

**Example Entity:**
```csharp
public class Product : BaseEntity, IMustHaveTenant
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public required string TenantId { get; set; } // Enforced by global filter
}
```

## Code Conventions

### Naming & File Organization

**Entities:** PascalCase, singular (e.g., `Product.cs`, `TrainingSessionPlan.cs`)  
**Services:** `I{Entity}Service` (interface) and `{Entity}Service` (implementation)  
**DTOs:** `{Action}{Entity}Request/Response` (e.g., `CreateProductRequest`, `ProductDTO`)  
**Controllers:** `{Entity}Controller` with `[Route("api/[controller]")]`

### Service Pattern

```csharp
// Interface in Diquis.Application/Services/{Domain}/{Entity}Service.cs
public interface IProductService
{
    Task<Response<ProductDTO>> GetProductAsync(Guid id);
    Task<PaginatedResponse<ProductDTO>> GetProductsPaginatedAsync(ProductTableFilter filter);
    Task<Response<Guid>> CreateProductAsync(CreateProductRequest request);
}

// Implementation pattern
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ProductService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response<Guid>> CreateProductAsync(CreateProductRequest request)
    {
        var product = _mapper.Map<Product>(request);
        // TenantId is auto-set by SaveChanges override
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return Response<Guid>.Success(product.Id);
    }
}
```

### Controller Pattern

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    [Authorize(Roles = "root, admin, editor")]
    [HttpPost]
    public async Task<IActionResult> CreateProductAsync(CreateProductRequest request)
    {
        Response<Guid> result = await _productService.CreateProductAsync(request);
        return Ok(result);
    }
}
```

## Background Jobs (Hangfire)

### Architecture

- **Job Queue:** Diquis.WebApi enqueues jobs using `IBackgroundJobService`
- **Job Processor:** Diquis.BackgroundJobs executes jobs in a separate process
- **Job Storage:** PostgreSQL (shared with application database)

### Job Implementation Pattern

```csharp
// Job class in Diquis.Infrastructure/BackgroundJobs/
public class EmailNotificationJob
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailNotificationJob> _logger;

    public EmailNotificationJob(IEmailService emailService, ILogger<EmailNotificationJob> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task ExecuteAsync(string recipientEmail, string subject, string body)
    {
        _logger.LogInformation("Sending email to {Email}", recipientEmail);
        await _emailService.SendAsync(recipientEmail, subject, body);
        _logger.LogInformation("Email sent successfully");
    }
}

// Enqueuing from a service
public class NotificationService
{
    private readonly IBackgroundJobService _jobService;

    public void SendWelcomeEmail(string email)
    {
        _jobService.Enqueue(() => new EmailNotificationJob(default!, default!)
            .ExecuteAsync(email, "Welcome!", "Welcome to Diquis!"));
    }
}
```

### Registration

```csharp
// In Diquis.WebApi/Program.cs
builder.Services.AddScoped<EmailNotificationJob>();

// In Diquis.BackgroundJobs/Extensions/ServiceCollectionExtensions.cs
services.AddScoped<EmailNotificationJob>();
```

## React Frontend (TypeScript + Vite)

### Project Structure

```
src/
??? api/                    # Axios client & API agent
??? components/             # Reusable UI components
?   ??? PageLayout/        # Layout wrapper with title prop
?   ??? AccountLayout/     # Auth pages layout
??? features/              # Feature-based organization
?   ??? products/          # Example feature
?       ??? ProductList.tsx
?       ??? ProductForm.tsx
?       ??? types.ts
??? pages/                 # Route-level components
??? stores/                # MobX stores
??? assets/
?   ??? scss/             # Bootstrap 5 theming
??? lib/
    ??? types.ts          # Shared TypeScript types
```

### Key Libraries

- **State Management:** MobX
- **Routing:** React Router v6
- **Forms:** Formik + Yup validation
- **Tables:** TanStack Table v8
- **HTTP Client:** Axios with auth interceptors
- **UI Framework:** Bootstrap 5 + React-Bootstrap

### API Client Pattern

```typescript
// src/api/agent.ts
import axios from 'axios';

axios.defaults.baseURL = import.meta.env.VITE_API_URL;

axios.interceptors.request.use((config) => {
  const token = store.authStore.token;
  config.headers = {
    ...config.headers,
    Tenant: store.authStore.tenant ?? "",
    'Accept-Language': i18n.language,
  };
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

const Products = {
  search: (params: SearchParams) => 
    requests.post<PaginatedResult<Product>>(`/products/products-paginated`, params),
  create: (product: AddProductRequest) => 
    requests.post<Result<Product>>("/products", product),
  // ...
};
```

### Component Pattern with PageLayout

```tsx
import { PageLayout } from 'components';
import { Card } from 'react-bootstrap';

const ProductList = () => {
  return (
    <PageLayout title="Products">
      <Card>
        <Card.Body>
          {/* Table/List content */}
        </Card.Body>
      </Card>
    </PageLayout>
  );
};
```

## Database & Migrations

### Entity Framework Core Setup

**Connection Strings:**
- `DefaultConnection` - BaseDbContext (system/tenant metadata)
- `ApplicationConnection` - ApplicationDbContext (business data)
- `HangfireConnection` - Hangfire job storage

### Migration Commands

```bash
# Application database migration
Add-Migration -Context ApplicationDbContext -OutputDir Persistence/Migrations/AppDb MigrationName

# Update database
Update-Database -Context ApplicationDbContext

# Hangfire/Base migrations
Add-Migration -Context BaseDbContext -OutputDir Persistence/Migrations/BaseDb MigrationName
```

### Global Query Filters (Tenant Isolation)

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Auto-apply tenant filter to all IMustHaveTenant entities
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        if (typeof(IMustHaveTenant).IsAssignableFrom(entityType.ClrType))
        {
            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(LambdaExpressionBuilder.BuildTenantFilter(entityType.ClrType, CurrentTenantId));
        }
    }
}
```

## Authentication & Authorization

### JWT Authentication

**Token Structure:**
```json
{
  "sub": "user-guid",
  "email": "user@academy.com",
  "tenant": "tenant-guid",
  "role": ["admin", "coach"],
  "exp": 1234567890
}
```

### Role-Based Authorization

**Built-in Roles:**
- `root` - Platform administrator (cross-tenant access)
- `admin` - Academy administrator
- `coach` - Coaching staff
- `editor` - Content editor
- `basic` - Basic user (parents, etc.)

**Controller Authorization:**
```csharp
[Authorize(Roles = "root, admin")]
[HttpGet]
public async Task<IActionResult> GetUsersAsync() { }

[Authorize(Roles = "root, admin, coach")]
[HttpPost]
public async Task<IActionResult> CreateTrainingSessionAsync() { }
```

## Testing Strategy

### Project Structure

```
Diquis.Application.Tests/    # Unit tests for services
Diquis.Domain.Tests/         # Domain entity tests
Diquis.Infrastructure.Tests/ # Integration tests
Diquis.WebApi.Tests/         # API endpoint tests
```

### Test Patterns

```csharp
public class ProductServiceTests
{
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductService _service;

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new CreateProductRequest { Name = "Test", Price = 10 };
        
        // Act
        var result = await _service.CreateProductAsync(request);
        
        // Assert
        Assert.True(result.Succeeded);
        Assert.NotEqual(Guid.Empty, result.Data);
    }
}
```

## AI Integration Guidelines

### AI-Generated Content Pattern

**Key Principles:**
1. **Human-in-the-Loop** - Always require manual review/approval
2. **Audit Trail** - Store AI prompts and responses
3. **Disclaimers** - Append legal disclaimers to all AI content
4. **Context Enrichment** - Pull data from multiple services for prompts

### AI Service Pattern

```csharp
public class AICoachingService
{
    private readonly IOpenAIService _aiService;
    private readonly ITeamService _teamService;

    public async Task<string> GenerateSessionPlanAsync(GenerateSessionPlanRequest request)
    {
        // 1. Gather context
        var team = await _teamService.GetByIdAsync(request.TeamId);
        var equipment = await _inventoryService.GetAvailableEquipmentAsync();
        
        // 2. Build prompt
        string prompt = $@"You are a youth football coach.
        Generate a {request.Duration}-minute training plan for {team.AgeGroup}.
        Available equipment: {string.Join(", ", equipment.Select(e => e.Name))}.
        Focus: {request.FocusArea}";
        
        // 3. Call AI
        var response = await _aiService.GetCompletionAsync(prompt);
        
        // 4. Add disclaimer
        var result = response + "\n\n**AI Disclaimer:** This is AI-generated content...";
        
        // 5. Save for audit
        var plan = new TrainingSessionPlan 
        { 
            GeneratedContent = result,
            AiPromptContext = JsonSerializer.Serialize(new { team, equipment })
        };
        await _context.TrainingSessionPlans.AddAsync(plan);
        await _context.SaveChangesAsync();
        
        return result;
    }
}
```

### Frontend Review Component Pattern

```tsx
export const AIContentReview = ({ content, onApprove }) => {
  const [canApprove, setCanApprove] = useState(false);
  const scrollRef = useRef<HTMLDivElement>(null);

  const handleScroll = () => {
    const el = scrollRef.current;
    if (el && el.scrollHeight - el.scrollTop <= el.clientHeight + 20) {
      setCanApprove(true);
    }
  };

  return (
    <>
      <div ref={scrollRef} onScroll={handleScroll} 
           style={{ maxHeight: '60vh', overflowY: 'auto' }}>
        <div dangerouslySetInnerHTML={{ __html: content }} />
      </div>
      <Button onClick={onApprove} disabled={!canApprove}>
        Approve & Save
      </Button>
    </>
  );
};
```

## OpenTelemetry & Observability

### Configuration

```csharp
builder.Services.AddOpenTelemetry().WithTracing(tracingBuilder =>
{
    tracingBuilder
        .AddSource("Diquis.Hangfire")
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter();
});
```

### Custom Activity Source

```csharp
public class HangfireActivityFilter : IClientFilter, IServerFilter
{
    private static readonly ActivitySource ActivitySource = new("Diquis.Hangfire");

    public void OnPerforming(PerformingContext context)
    {
        var activity = ActivitySource.StartActivity($"Job: {context.BackgroundJob.Job.Method.Name}");
        activity?.SetTag("job.id", context.BackgroundJob.Id);
    }
}
```

## Common Patterns & Best Practices

### Response Wrapper Pattern

```csharp
// All API responses use this structure
public class Response<T>
{
    public bool Succeeded { get; set; }
    public T Data { get; set; }
    public List<string> Messages { get; set; }
    
    public static Response<T> Success(T data) => new() 
    { 
        Succeeded = true, 
        Data = data 
    };
    
    public static Response<T> Fail(string message) => new() 
    { 
        Succeeded = false, 
        Messages = new List<string> { message } 
    };
}
```

### Pagination Pattern

```csharp
public class PaginatedResponse<T> : Response<List<T>>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
```

### Error Handling

```csharp
try
{
    var result = await _service.CreateAsync(request);
    return Ok(result);
}
catch (ValidationException ex)
{
    return BadRequest(Response<string>.Fail(ex.Message));
}
catch (NotFoundException ex)
{
    return NotFound(Response<string>.Fail(ex.Message));
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error");
    return StatusCode(500, Response<string>.Fail("Internal server error"));
}
```

## Environment Configuration

### Development

- **API URL:** `https://localhost:7250`
- **Hangfire Dashboard:** `https://localhost:7298/hangfire`
- **React Dev Server:** `http://localhost:3000`
- **Database:** PostgreSQL LocalDB

### Environment Variables (.env)

```bash
# React Frontend
VITE_API_URL=https://localhost:7250/api

# .NET Backend (appsettings.Development.json)
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=DiquisBase;...",
    "ApplicationConnection": "Host=localhost;Database=DiquisApp;...",
    "HangfireConnection": "Host=localhost;Database=DiquisHangfire;..."
  },
  "JwtSettings": {
    "Key": "your-256-bit-secret-key",
    "Issuer": "Diquis",
    "Audience": "Diquis",
    "DurationInMinutes": 30
  }
}
```

## Project-Specific Terminology

**Entities to be aware of:**
- `Tenant` - Football academy organization
- `ApplicationUser` - Users (coaches, parents, players)
- `Team` - Football team within an academy
- `Player` - Player entity (linked to ApplicationUser)
- `TrainingSession` - Scheduled training session
- `Match` - Match/game record
- `Product` - Sample business entity (from boilerplate)

**Custom Services:**
- `ICurrentTenantUserService` - Manages current tenant/user context
- `IBackgroundJobService` - Hangfire job enqueueing abstraction
- `IJobClientWrapper` - Hangfire client wrapper for DI
- `IEmailService` - Email sending abstraction
- `IFileStorageService` - File upload/storage abstraction

## When to Use Each Layer

### Domain Layer
- Creating new entities
- Adding value objects
- Defining domain events
- Enum types

### Application Layer
- Business logic orchestration
- Service interfaces
- DTOs for API contracts
- AutoMapper profiles
- FluentValidation validators
- Background job interfaces

### Infrastructure Layer
- EF Core DbContext
- Repository implementations
- External API clients (Email, SMS, AI)
- Background job implementations
- File storage implementations

### WebApi/Presentation Layer
- Controllers (thin - delegate to services)
- Middleware (TenantResolver, Error handling)
- Filters
- Program.cs configuration

## Quick Reference Commands

```bash
# Build solution
dotnet build Diquis.sln

# Run API
dotnet run --project Diquis.WebApi

# Run Hangfire worker
dotnet run --project Diquis.BackgroundJobs

# Run tests
dotnet test Diquis.sln

# Create migration
Add-Migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb MigrationName

# Frontend build
cd Diquis.WebApi/Frontend
npm run build

# Frontend dev server
npm run dev
```

## Nano Boilerplate CLI Scaffolding

### Scaffolding New Services (CRUD)

The project uses the Nano boilerplate's CLI scaffolding tools for rapid vertical slice generation:

```bash
# Navigate to Application/Services directory first
cd Diquis.Application/Services

# Generate a new service vertical slice
dotnet new nano-service -s <SingularName> -p <PluralName> -ap Diquis

# Example: Create a Player service
dotnet new nano-service -s Player -p Players -ap Diquis
```

**What This Generates:**
- `Diquis.Domain/Entities/Player.cs` - Domain entity
- `Diquis.Application/Services/Players/IPlayerService.cs` - Service interface
- `Diquis.Application/Services/Players/PlayerService.cs` - Service implementation
- `Diquis.Application/Services/Players/DTOs/` - Request/Response DTOs
- Specifications for filtering and querying

### Scaffolding Controllers

```bash
# Navigate to Controllers directory
cd Diquis.WebApi/Controllers

# Generate controller
dotnet new nano-controller -s <SingularName> -p <PluralName> -ap Diquis

# Example: Create Players controller
dotnet new nano-controller -s Player -p Players -ap Diquis
```

**Important Notes:**
- After scaffolding, **manually review and customize** the generated code
- Ensure entities implement `IMustHaveTenant` if they need tenant isolation
- Add proper `[Authorize(Roles = "...")]` attributes to controller actions
- Register services in DI container if using manual registration

## Anti-Patterns to Avoid

? **Don't:** Reference Infrastructure from Application layer  
? **Do:** Define interfaces in Application, implement in Infrastructure

? **Don't:** Put business logic in Controllers  
? **Do:** Delegate to Application Services

? **Don't:** Directly reference DbContext in Controllers  
? **Do:** Use Service layer abstraction

? **Don't:** Hard-code tenant IDs  
? **Do:** Use `ICurrentTenantUserService` or middleware

? **Don't:** Create background jobs in WebApi project  
? **Do:** Create in Infrastructure, register in both projects

? **Don't:** Skip the disclaimer on AI-generated content  
? **Do:** Always append legal disclaimers

? **Don't:** Auto-send AI content  
? **Do:** Require human review (Human-in-the-Loop pattern)

## Additional Context

This is a **production SaaS application** for managing youth football academies. Code quality, security, and tenant isolation are critical. When in doubt:

1. Follow Clean Architecture principles strictly
2. Ensure tenant isolation on all entities
3. Use the existing patterns (Response wrapper, service pattern, etc.)
4. Add proper authorization checks
5. Include comprehensive logging
6. Write tests for critical paths
