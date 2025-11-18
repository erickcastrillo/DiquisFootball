# Authorization with ASP.NET Core Policies

This document describes how authorization is implemented in Diquis using ASP.NET Core Authorization Policies.

## Overview

Diquis uses **ASP.NET Core Authorization** with custom policy-based authorization. This provides a flexible and powerful way to implement authorization logic based on claims, roles, and custom requirements.

## Architecture

### Authorization Policies

Policies are defined in `Program.cs` and can be applied to controllers or actions using the `[Authorize(Policy = "PolicyName")]` attribute.

```csharp
builder.Services.AddAuthorization(options =>
{
    // System admin policy
    options.AddPolicy("RequireSystemAdmin", policy =>
        policy.RequireClaim("IsSystemAdmin", "True"));
    
    // Academy access policy
    options.AddPolicy("RequireAcademyAccess", policy =>
        policy.Requirements.Add(new AcademyAccessRequirement()));
    
    // CRUD policies
    options.AddPolicy("CanCreate", policy =>
        policy.Requirements.Add(new PermissionRequirement("create")));
    
    options.AddPolicy("CanUpdate", policy =>
        policy.Requirements.Add(new PermissionRequirement("update")));
    
    options.AddPolicy("CanDelete", policy =>
        policy.Requirements.Add(new PermissionRequirement("delete")));
});
```

### Custom Requirements

#### AcademyAccessRequirement

```csharp
public class AcademyAccessRequirement : IAuthorizationRequirement
{
    public Guid? RequiredAcademyId { get; set; }
}

public class AcademyAccessHandler : AuthorizationHandler<AcademyAccessRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AcademyAccessRequirement requirement)
    {
        var userId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        
        // System admins have access to all academies
        if (context.User.HasClaim("IsSystemAdmin", "True"))
        {
            context.Succeed(requirement);
            return;
        }
        
        // Get academy ID from route or header
        var academyId = GetAcademyIdFromRequest();
        if (academyId == null)
        {
            context.Fail();
            return;
        }
        
        // Check if user has access to this academy
        var hasAccess = await _context.AcademyUsers
            .AnyAsync(au => au.UserId == userId && 
                           au.AcademyId == academyId && 
                           au.IsActive);
        
        if (hasAccess)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
    
    private Guid? GetAcademyIdFromRequest()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        // Try to get from route
        if (httpContext.Request.RouteValues.TryGetValue("academyId", out var routeValue))
        {
            if (Guid.TryParse(routeValue?.ToString(), out var academyId))
                return academyId;
        }
        
        // Try to get from header
        if (httpContext.Request.Headers.TryGetValue("X-Academy-Context", out var headerValue))
        {
            if (Guid.TryParse(headerValue, out var academyId))
                return academyId;
        }
        
        return null;
    }
}
```

#### PermissionRequirement

```csharp
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        
        // System admins have all permissions
        if (context.User.HasClaim("IsSystemAdmin", "True"))
        {
            context.Succeed(requirement);
            return;
        }
        
        var academyId = GetAcademyIdFromRequest();
        if (academyId == null)
        {
            context.Fail();
            return;
        }
        
        // Get user's role for this academy
        var academyUser = await _context.AcademyUsers
            .Include(au => au.Role)
            .FirstOrDefaultAsync(au => au.UserId == userId && 
                                      au.AcademyId == academyId && 
                                      au.IsActive);
        
        if (academyUser == null)
        {
            context.Fail();
            return;
        }
        
        // Check if role has the required permission
        var hasPermission = await _context.RolePermissions
            .AnyAsync(rp => rp.RoleId == academyUser.RoleId && 
                           rp.Permission == requirement.Permission);
        
        if (hasPermission)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
```

## Usage

### In Controllers

Apply authorization policies to controllers or specific actions:

```csharp
[ApiController]
[Route("api/v1/{academyId}/players")]
[Authorize(Policy = "RequireAcademyAccess")]
public class PlayersController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPlayers(Guid academyId)
    {
        // User has already been authorized for academy access
        var players = await _playerService.GetPlayersByAcademyAsync(academyId);
        return Ok(players);
    }
    
    [HttpPost]
    [Authorize(Policy = "CanCreate")]
    public async Task<IActionResult> CreatePlayer(Guid academyId, [FromBody] CreatePlayerRequest request)
    {
        var player = await _playerService.CreatePlayerAsync(academyId, request);
        return Created($"/api/v1/{academyId}/players/{player.Id}", player);
    }
    
    [HttpPut("{id}")]
    [Authorize(Policy = "CanUpdate")]
    public async Task<IActionResult> UpdatePlayer(Guid academyId, Guid id, [FromBody] UpdatePlayerRequest request)
    {
        var player = await _playerService.UpdatePlayerAsync(academyId, id, request);
        return Ok(player);
    }
    
    [HttpDelete("{id}")]
    [Authorize(Policy = "CanDelete")]
    public async Task<IActionResult> DeletePlayer(Guid academyId, Guid id)
    {
        await _playerService.DeletePlayerAsync(academyId, id);
        return NoContent();
    }
}
```

### Resource-Based Authorization

For more fine-grained control, use resource-based authorization:

```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdatePlayer(Guid academyId, Guid id, [FromBody] UpdatePlayerRequest request)
{
    var player = await _context.Players.FindAsync(id);
    
    if (player == null)
        return NotFound();
    
    // Check if user is authorized to update this specific player
    var authResult = await _authorizationService.AuthorizeAsync(
        User, player, "CanUpdatePlayer");
    
    if (!authResult.Succeeded)
        return Forbid();
    
    // Update player
    await _playerService.UpdatePlayerAsync(player, request);
    return Ok(player);
}
```

## Authorization Models

### AcademyUser

```csharp
public class AcademyUser
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AcademyId { get; set; }
    public Guid RoleId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public ApplicationUser User { get; set; }
    public Academy Academy { get; set; }
    public Role Role { get; set; }
}
```

### Role

```csharp
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } // Admin, Coach, Assistant, Viewer
    public string Description { get; set; }
    
    // Navigation properties
    public ICollection<AcademyUser> AcademyUsers { get; set; }
    public ICollection<RolePermission> Permissions { get; set; }
}
```

### RolePermission

```csharp
public class RolePermission
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public string Permission { get; set; } // create, read, update, delete
    public string Resource { get; set; } // player, team, training, etc.
    
    // Navigation properties
    public Role Role { get; set; }
}
```

## Permission Levels

### System Admin

- Full access to all academies and resources
- Can create new academies
- Can manage system-wide settings

### Academy Admin

- Full CRUD access within their academy
- Can manage academy users and roles
- Can configure academy settings

### Coach

- Read/write access to players, teams, trainings
- Cannot delete critical resources
- Limited access to academy settings

### Assistant Coach

- Read access to players and teams
- Can record training attendance
- Cannot modify rosters or schedules

### Viewer

- Read-only access to academy data
- No modification rights

## Error Handling

When authorization fails, return a 403 Forbidden response:

```csharp
// Global exception handler in Program.cs
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";
        
        await context.Response.WriteAsJsonAsync(new
        {
            error = "PERMISSION_DENIED",
            message = "You are not authorized to perform this action"
        });
    });
});
```

## Testing Authorization

### Unit Tests

```csharp
[Fact]
public async Task CreatePlayer_WithoutPermission_ReturnsForbidden()
{
    // Arrange
    var user = CreateUser(isSystemAdmin: false);
    var academyId = Guid.NewGuid();
    
    // Mock authorization to fail
    _authorizationService
        .Setup(x => x.AuthorizeAsync(user, It.IsAny<object>(), "CanCreate"))
        .ReturnsAsync(AuthorizationResult.Failed());
    
    // Act
    var result = await _controller.CreatePlayer(academyId, new CreatePlayerRequest());
    
    // Assert
    Assert.IsType<ForbidResult>(result);
}

[Fact]
public async Task CreatePlayer_AsSystemAdmin_Succeeds()
{
    // Arrange
    var user = CreateUser(isSystemAdmin: true);
    var academyId = Guid.NewGuid();
    
    // Act
    var result = await _controller.CreatePlayer(academyId, new CreatePlayerRequest());
    
    // Assert
    Assert.IsType<CreatedResult>(result);
}
```

### Integration Tests

```csharp
[Fact]
public async Task GetPlayers_WithValidAcademyAccess_ReturnsPlayers()
{
    // Arrange
    var client = _factory.CreateClient();
    var token = await GetAuthTokenAsync("user@example.com", "password");
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", token);
    
    // Act
    var response = await client.GetAsync("/api/v1/academy-id/players");
    
    // Assert
    response.EnsureSuccessStatusCode();
    var players = await response.Content.ReadFromJsonAsync<List<PlayerDto>>();
    Assert.NotEmpty(players);
}
```

## Best Practices

### 1. Use Policies Over Roles

```csharp
// Good - Policy-based
[Authorize(Policy = "CanManagePlayers")]
public async Task<IActionResult> CreatePlayer() { }

// Bad - Role-based
[Authorize(Roles = "Admin,Coach")]
public async Task<IActionResult> CreatePlayer() { }
```

### 2. Centralize Authorization Logic

Define all policies in one place (Program.cs or separate configuration class).

### 3. Use Resource-Based Authorization for Fine-Grained Control

Check authorization against specific resources when needed.

### 4. Cache Authorization Results

Use in-memory caching for frequently checked permissions to improve performance.

### 5. Test Authorization Thoroughly

Test all permission combinations and edge cases.

## Security Considerations

### 1. Deny by Default

Always start with denied access and explicitly grant permissions.

### 2. Validate Academy Context

Always verify the academy context matches the user's permissions.

### 3. Check Multi-Tenancy

Ensure resources belong to the correct academy before allowing operations.

### 4. Audit Authorization Failures

Log failed authorization attempts for security monitoring.

## Related Documentation

- [ASP.NET Core Authorization Documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/)
- [Policy-Based Authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies)
- [Resource-Based Authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased)
- [API_AUTHENTICATION.md](./API_AUTHENTICATION.md) - Authentication setup
