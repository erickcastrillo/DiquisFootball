# Diquis - ASP.NET Core Development Guide

## Table of Contents

1. [Development Workflow](#development-workflow)
2. [CI/CD Pipeline](#cicd-pipeline)
3. [Code Organization](#code-organization)
4. [Application Layer Development](#application-layer-development)
5. [Controller/Endpoint Development](#controllerendpoint-development)
6. [Domain Model Development](#domain-model-development)
7. [Testing Guidelines](#testing-guidelines)
8. [Code Style Guidelines](#code-style-guidelines)

---

## Development Workflow

### Starting Development

```bash
# 1. Pull the latest changes from the main branch
git pull origin main

# 2. Install/restore .NET dependencies
dotnet restore

# 3. Apply any pending database migrations
dotnet ef database update --project Diquis.Infrastructure

# 4. Run the application
dotnet run --project Diquis.WebApi

# 5. Run the test suite to ensure everything is stable
dotnet test
```

### Daily Development Cycle

1.  **Create a Feature Branch**
    ```bash
    git checkout -b feature/player-skill-assessment
    ```

2.  **Write a Failing Test (TDD)**
    Create a new test file in the appropriate test project (e.g., `Diquis.Application.Tests`) that defines the expected behavior of your new feature.
    ```csharp
    [Fact]
    public async Task AssignSkill_WithValidData_ShouldSucceed()
    {
        // Arrange
        var service = new PlayerSkillService(...);
        var command = new AssignSkillCommand(...);

        // Act
        var result = await service.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
    }
    ```

3.  **Implement the Feature**
    -   Add or modify entities in `Diquis.Domain`.
    -   Define interfaces and DTOs in `Diquis.Application`.
    -   Implement the business logic in the `Diquis.Application` layer (e.g., in a service or a command handler).
    -   Implement repository or service interfaces in `Diquis.Infrastructure`.
    -   Expose the feature via a controller endpoint in `Diquis.WebApi`.

4.  **Run Tests**
    Run the tests to ensure your new feature works and doesn't break existing functionality.
    ```bash
    dotnet test
    ```

5.  **Commit Changes**
    Use conventional commit messages.
    ```bash
    git add .
    git commit -m "feat(player): Add skill assessment feature"
    ```

6.  **Push and Create a Pull Request**
    ```bash
    git push origin feature/player-skill-assessment
    ```
    Then, open a pull request in GitHub for review.

---

## CI/CD Pipeline

The project uses GitHub Actions for Continuous Integration. The workflow (`.github/workflows/dotnet-build.yml`) is triggered on every push and pull request to the `main` branch.

### CI Pipeline Jobs

1.  **Build**: Compiles the entire solution to ensure there are no build errors.
2.  **Test**: Runs the complete xUnit test suite for all test projects.
3.  **Code Quality (Optional)**: Can be configured to run `dotnet format --verify-no-changes` to ensure code style is consistent.

All jobs must pass before a pull request can be merged.

---

## Scaffolding with CLI Tools

The ASPNano template includes custom CLI tools to accelerate development by scaffolding boilerplate code for services and controllers.

### Scaffolding a New Service
To generate a new service with full CRUD (Create, Read, Update, Delete) operations, use the `nano-service` command. This will create the service, interface, DTOs, and validation rules in the `Application` layer.

**Command:**
```bash
# Usage: dotnet new nano-service -n <ServiceName>
dotnet new nano-service -n Product
```

This command will generate:
- `Diquis.Application/Services/ProductService.cs`
- `Diquis.Application/Interfaces/IProductService.cs`
- `Diquis.Application/DTOs/ProductDto.cs`
- `Diquis.Application/Validators/ProductValidator.cs`

### Scaffolding a New API Controller
To generate a new API controller that is pre-wired to use a service, use the `nano-controller` command.

**Command:**
```bash
# Usage: dotnet new nano-controller -n <ControllerName>
dotnet new nano-controller -n Products
```

This command will generate:
- `Diquis.WebApi/Controllers/ProductsController.cs`

The generated controller will already have the corresponding service (`IProductService`) injected and will include endpoints for all the CRUD operations.

---

## Code Organization

The solution follows **Clean Architecture**, separating concerns into distinct layers (projects).

-   **`Diquis.Domain`**: Core business models and logic.
-   **`Diquis.Application`**: Application-specific business rules and use cases.
-   **`Diquis.Infrastructure`**: Data access, external services, and other implementation details.
-   **`Diquis.WebApi`**: API endpoints, middleware, and presentation concerns.

When adding a new feature, you will likely add files to several of these projects, keeping the concerns for each layer separate.

---

## Application Layer Development

### Creating a New Service or Feature

Business logic is typically encapsulated in services or feature handlers within the `Diquis.Application` project.

**1. Define the Interface (Contract):**
```csharp
// In Diquis.Application/Interfaces
public interface IPlayerRegistrationService
{
    Task<Result<PlayerDto>> RegisterPlayerAsync(RegisterPlayerCommand command);
}
```

**2. Implement the Service:**
```csharp
// In Diquis.Application/Services
public class PlayerRegistrationService : IPlayerRegistrationService
{
    private readonly IRepository<Player> _playerRepository;
    private readonly IRepository<Academy> _academyRepository;

    public PlayerRegistrationService(IRepository<Player> playerRepository, IRepository<Academy> academyRepository)
    {
        _playerRepository = playerRepository;
        _academyRepository = academyRepository;
    }

    public async Task<Result<PlayerDto>> RegisterPlayerAsync(RegisterPlayerCommand command)
    {
        // 1. Validation
        var academy = await _academyRepository.GetByIdAsync(command.AcademyId);
        if (academy == null)
        {
            return Result.Fail<PlayerDto>("Academy not found.");
        }

        // 2. Create Domain Entity
        var player = new Player(command.FirstName, command.LastName, command.DateOfBirth, command.AcademyId);

        // 3. Persist
        await _playerRepository.AddAsync(player);

        // 4. Return DTO
        var playerDto = new PlayerDto(player.Id, player.FullName, player.Age);
        return Result.Ok(playerDto);
    }
}
```

---

## Controller/Endpoint Development

Endpoints in `Diquis.WebApi` are the entry point to the application. They should be lean and primarily responsible for:
1.  Receiving the HTTP request.
2.  Invoking the appropriate application service or handler.
3.  Returning an HTTP response.

```csharp
[ApiController]
[Route("api/v1/{tenantId}/players")]
[Authorize(Policy = "TenantAccess")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerRegistrationService _registrationService;

    public PlayersController(IPlayerRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpPost]
    [Authorize(Policy = "CanCreatePlayers")]
    public async Task<IActionResult> RegisterPlayer(string tenantId, [FromBody] RegisterPlayerCommand command)
    {
        // Ensure the command uses the tenant from the URL
        command.AcademyId = tenantId;

        var result = await _registrationService.RegisterPlayerAsync(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetPlayerById), new { id = result.Value.Id }, result.Value);
        }

        return BadRequest(result.Error);
    }
}
```

---

## Domain Model Development

Entities in `Diquis.Domain` should represent the core concepts of your application and contain logic that is intrinsic to that entity.

```csharp
public class Player : BaseEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Guid AcademyId { get; private set; }

    // Private constructor for EF Core
    private Player() { }

    public Player(string firstName, string lastName, DateTime dateOfBirth, Guid academyId)
    {
        // Enforce invariants
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentNullException(nameof(firstName));
        if (dateOfBirth > DateTime.UtcNow) throw new InvalidOperationException("Date of birth cannot be in the future.");

        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        AcademyId = academyId;
    }

    public string FullName => $"{FirstName} {LastName}";

    public int Age => DateTime.UtcNow.Year - DateOfBirth.Year;

    public void UpdateName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
```

---

## Testing Guidelines

-   **Unit Tests**: Reside in projects like `Diquis.Domain.Tests` and `Diquis.Application.Tests`. They test a single class in isolation, with dependencies mocked.
-   **Integration Tests**: Reside in `Diquis.Infrastructure.Tests` and `Diquis.WebApi.Tests`. They test how components work together, often involving a real database or an in-memory provider.

**Example Unit Test (xUnit, Moq, FluentAssertions):**
```csharp
public class PlayerRegistrationServiceTests
{
    private readonly Mock<IRepository<Player>> _playerRepoMock;
    private readonly Mock<IRepository<Academy>> _academyRepoMock;
    private readonly PlayerRegistrationService _service;

    public PlayerRegistrationServiceTests()
    {
        _playerRepoMock = new Mock<IRepository<Player>>();
        _academyRepoMock = new Mock<IRepository<Academy>>();
        _service = new PlayerRegistrationService(_playerRepoMock.Object, _academyRepoMock.Object);
    }

    [Fact]
    public async Task RegisterPlayerAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var academyId = Guid.NewGuid();
        var command = new RegisterPlayerCommand("John", "Doe", new DateTime(2010, 1, 1), academyId);
        _academyRepoMock.Setup(r => r.GetByIdAsync(academyId)).ReturnsAsync(new Academy());

        // Act
        var result = await _service.RegisterPlayerAsync(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        _playerRepoMock.Verify(r => r.AddAsync(It.IsAny<Player>()), Times.Once);
    }
}
```

---

## Code Style Guidelines

-   **Follow `.editorconfig`**: The project includes an `.editorconfig` file to enforce consistent coding styles across different editors.
-   **Use C# 12 Features**: Leverage modern C# features like primary constructors, file-scoped namespaces, and record types where appropriate.
-   **Asynchronous Programming**: Use `async` and `await` for all I/O-bound operations (like database calls and HTTP requests) to keep the application responsive.
-   **Naming Conventions**: Follow the standard .NET naming conventions (e.g., `PascalCase` for methods and properties, `_camelCase` for private fields).

### Running Formatters

Ensure your code is formatted correctly before committing:
```bash
dotnet format
```
