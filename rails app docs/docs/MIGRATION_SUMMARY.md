# Documentation Migration Summary

This document tracks the migration of Diquis documentation from Ruby on Rails to ASP.NET Core and Django REST Framework.

## Migration Status

### Completed ✅

| Document | ASP.NET | Django | Notes |
|----------|---------|--------|-------|
| API_AUTHENTICATION.md | ✅ | ✅ | Migrated to Identity/JWT (ASP.NET) and Simple JWT (Django) |
| AUTHORIZATION.md | ✅ | ✅ | Migrated to Policies (ASP.NET) and Permissions (Django) |

### In Progress 🔄

| Document | ASP.NET | Django | Notes |
|----------|---------|--------|-------|
| PROJECT_OVERVIEW.md | 🔄 | 🔄 | Framework-specific overview needed |
| ARCHITECTURE.md | 🔄 | 🔄 | Architecture patterns differ by framework |
| SETUP_GUIDE.md | 🔄 | 🔄 | Installation steps are framework-specific |
| API_DOCUMENTATION.md | 🔄 | 🔄 | Endpoint implementations differ |

### Pending ⏳

| Document | ASP.NET | Django | Notes |
|----------|---------|--------|-------|
| DEVELOPMENT_GUIDE.md | ⏳ | ⏳ | Development workflows |
| SIDEKIQ_SETUP.md | ⏳ | ⏳ | Replace with Hangfire (ASP.NET) / Celery (Django) |
| UUID_AND_MULTITENANCY.md | ⏳ | ⏳ | Multi-tenancy implementation |
| FEATURE_*.md | ⏳ | ⏳ | Feature-specific documentation |

## Framework Equivalents

### Ruby on Rails → ASP.NET Core

| Rails Concept | ASP.NET Equivalent | Notes |
|---------------|-------------------|-------|
| Devise | ASP.NET Core Identity | User authentication and management |
| Devise-JWT | JwtBearer Authentication | JWT token handling |
| Pundit | Authorization Policies | Policy-based authorization |
| ActsAsTenant | Custom Middleware | Multi-tenancy implementation |
| ActiveRecord | Entity Framework Core | ORM and database access |
| Sidekiq | Hangfire / Azure Functions | Background job processing |
| RSpec | xUnit / NUnit | Testing framework |
| FactoryBot | Bogus / AutoFixture | Test data generation |
| RuboCop | StyleCop / Roslyn Analyzers | Code analysis |
| Kaminari/Pagy | PagedList | Pagination |
| ActiveModelSerializers | AutoMapper | Object mapping |
| Rack::Cors | CORS Middleware | Cross-origin requests |

### Ruby on Rails → Django

| Rails Concept | Django Equivalent | Notes |
|---------------|------------------|-------|
| Devise | Django Allauth / dj-rest-auth | User authentication |
| Devise-JWT | Simple JWT | JWT token handling |
| Pundit | Django Permissions / Guardian | Object-level permissions |
| ActsAsTenant | django-tenant-schemas | Multi-tenancy |
| ActiveRecord | Django ORM | Database models |
| Sidekiq | Celery + Redis/RabbitMQ | Background tasks |
| RSpec | pytest / unittest | Testing framework |
| FactoryBot | Factory Boy | Test fixtures |
| RuboCop | pylint / flake8 / black | Code linting |
| Kaminari/Pagy | Django Pagination | Built-in pagination |
| ActiveModelSerializers | Django REST Framework Serializers | API serialization |
| Rack::Cors | django-cors-headers | CORS handling |

## Migration Guidelines

### Authentication

**Rails (Devise + JWT):**

```ruby
# config/initializers/devise.rb
config.jwt do |jwt|
  jwt.secret = Rails.application.credentials.devise_jwt_secret_key
  jwt.expiration_time = 24.hours.to_i
end
```

**ASP.NET Core:**

```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(key),
      ValidateLifetime = true
    };
  });
```

**Django:**

```python
# settings.py
SIMPLE_JWT = {
    'ACCESS_TOKEN_LIFETIME': timedelta(hours=24),
    'REFRESH_TOKEN_LIFETIME': timedelta(days=7),
    'ROTATE_REFRESH_TOKENS': True,
}
```

### Authorization

**Rails (Pundit):**

```ruby
class PlayerPolicy < ApplicationPolicy
  def create?
    system_admin? || has_permission?(:create)
  end
end
```

**ASP.NET Core:**

```csharp
builder.Services.AddAuthorization(options => {
  options.AddPolicy("CanCreate", policy =>
    policy.Requirements.Add(new PermissionRequirement("create")));
});

[Authorize(Policy = "CanCreate")]
public async Task<IActionResult> CreatePlayer() { }
```

**Django:**

```python
class HasCreatePermission(permissions.BasePermission):
    def has_permission(self, request, view):
        return request.user.is_system_admin or \
               self._has_permission('create')

@permission_classes([HasCreatePermission])
def create_player(request): pass
```

### Multi-Tenancy

**Rails (ActsAsTenant):**

```ruby
class Player < ApplicationRecord
  acts_as_tenant(:academy)
end

ActsAsTenant.current_tenant = @academy
```

**ASP.NET Core:**

```csharp
// Custom middleware
public class TenantMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var academyId = context.Request.RouteValues["academyId"];
        context.Items["AcademyId"] = academyId;
        await _next(context);
    }
}

// In queries
var players = _context.Players
    .Where(p => p.AcademyId == currentAcademyId);
```

**Django:**

```python
# Using django-tenant-schemas or custom middleware
class TenantMiddleware:
    def __init__(self, get_response):
        self.get_response = get_response
    
    def __call__(self, request):
        academy_id = request.resolver_match.kwargs.get('academy_id')
        request.academy_id = academy_id
        return self.get_response(request)

# In queries
players = Player.objects.filter(academy_id=request.academy_id)
```

### Background Jobs

**Rails (Sidekiq):**

```ruby
class WelcomeEmailJob < ApplicationJob
  queue_as :mailers
  
  def perform(user_id)
    user = User.find(user_id)
    UserMailer.welcome_email(user).deliver_now
  end
end

WelcomeEmailJob.perform_later(user.id)
```

**ASP.NET Core (Hangfire):**

```csharp
public class EmailService
{
    public void SendWelcomeEmail(Guid userId)
    {
        var user = _userManager.FindByIdAsync(userId.ToString()).Result;
        _emailSender.SendEmailAsync(user.Email, "Welcome", "...");
    }
}

BackgroundJob.Enqueue<EmailService>(x => x.SendWelcomeEmail(userId));
```

**Django (Celery):**

```python
from celery import shared_task

@shared_task
def send_welcome_email(user_id):
    user = User.objects.get(id=user_id)
    send_mail(
        'Welcome',
        'Welcome message...',
        'from@example.com',
        [user.email],
    )

send_welcome_email.delay(user.id)
```

## API Endpoint Mapping

### Rails Routes

```ruby
# config/routes.rb
namespace :api do
  namespace :v1 do
    resources :academies do
      resources :players
    end
  end
end
```

### ASP.NET Core Routes

```csharp
// Program.cs or Controllers
[ApiController]
[Route("api/v1/academies/{academyId}/players")]
public class PlayersController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPlayers(Guid academyId) { }
}
```

### Django Routes

```python
# urls.py
from rest_framework.routers import DefaultRouter

router = DefaultRouter()
router.register(
    r'academies/(?P<academy_id>[^/.]+)/players',
    PlayerViewSet,
    basename='player'
)

urlpatterns = [
    path('api/v1/', include(router.urls)),
]
```

## Database Migration

### Schema Comparison

| Rails (ActiveRecord) | ASP.NET (EF Core) | Django ORM |
|---------------------|-------------------|------------|
| `t.uuid :slug, index: true` | `Guid Id { get; set; }` | `UUIDField(primary_key=True)` |
| `t.string :email, null: false` | `string Email { get; set; }` (Required) | `EmailField()` |
| `t.references :academy` | `Guid AcademyId { get; set; }` | `ForeignKey('Academy')` |
| `t.timestamps` | `DateTime CreatedAt/UpdatedAt` | `DateTimeField(auto_now_add=True)` |
| `t.boolean :is_active, default: true` | `bool IsActive { get; set; } = true` | `BooleanField(default=True)` |

## Testing Migration

### RSpec → xUnit (ASP.NET)

```csharp
[Fact]
public async Task CreatePlayer_WithValidData_ReturnsCreated()
{
    // Arrange
    var request = new CreatePlayerRequest { ... };
    
    // Act
    var result = await _controller.CreatePlayer(academyId, request);
    
    // Assert
    var createdResult = Assert.IsType<CreatedResult>(result);
    Assert.NotNull(createdResult.Value);
}
```

### RSpec → pytest (Django)

```python
def test_create_player_with_valid_data_returns_created(api_client, academy):
    # Arrange
    data = {'first_name': 'John', 'last_name': 'Doe', 'age': 15}
    
    # Act
    response = api_client.post(
        f'/api/v1/{academy.id}/players/',
        data,
        format='json'
    )
    
    # Assert
    assert response.status_code == 201
    assert response.data['first_name'] == 'John'
```

## Next Steps

1. ✅ Complete authentication documentation (ASP.NET & Django)
2. ✅ Complete authorization documentation (ASP.NET & Django)
3. 🔄 Create PROJECT_OVERVIEW for both frameworks
4. 🔄 Create ARCHITECTURE documentation for both frameworks
5. 🔄 Create SETUP_GUIDE for both frameworks
6. ⏳ Migrate feature-specific documentation
7. ⏳ Create deployment guides
8. ⏳ Create API documentation with Swagger (ASP.NET) / drf-spectacular (Django)

## Contributing

When adding new documentation:

1. Create the Rails version first
2. Identify Rails-specific concepts
3. Find ASP.NET and Django equivalents
4. Create parallel documentation structure
5. Update this migration summary

## Notes

- ASP.NET uses Pascal Case conventions vs Ruby's snake_case
- Django maintains snake_case like Rails
- ASP.NET uses strongly-typed models, Django uses dynamic typing like Rails
- All three frameworks support RESTful API design
- JWT authentication is similar across all frameworks
- Multi-tenancy requires custom implementation in ASP.NET, library support in Rails and Django
