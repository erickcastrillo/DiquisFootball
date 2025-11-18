# Quick Reference: Rails → ASP.NET Core & Django

## Authentication Implementation

### User Registration

#### Rails (Devise)

```ruby
# POST /auth/sign_up
{
  "user": {
    "email": "user@example.com",
    "password": "password123",
    "password_confirmation": "password123"
  }
}
```

#### ASP.NET Core

```csharp
// POST /api/auth/register
{
  "email": "user@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!"
}

// Controller
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    var user = new ApplicationUser { Email = request.Email };
    var result = await _userManager.CreateAsync(user, request.Password);
    if (result.Succeeded) {
        var token = await _tokenService.GenerateTokenAsync(user);
        return Created("", new { token });
    }
    return BadRequest(result.Errors);
}
```

#### Django

```python
# POST /api/auth/register/
{
  "email": "user@example.com",
  "password": "password123",
  "password2": "password123"
}

# View
@api_view(['POST'])
def register(request):
    serializer = UserRegistrationSerializer(data=request.data)
    if serializer.is_valid():
        user = serializer.save()
        refresh = RefreshToken.for_user(user)
        return Response({
            'refresh': str(refresh),
            'access': str(refresh.access_token)
        }, status=201)
    return Response(serializer.errors, status=400)
```

---

### User Login

#### Rails (Devise)

```ruby
# app/controllers/auth/sessions_controller.rb
def respond_with(resource, _opts = {})
  render json: {
    data: UserSerializer.new(resource),
    message: 'Logged in successfully'
  }
end
```

#### ASP.NET Core

```csharp
// Controllers/AuthController.cs
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    var user = await _userManager.FindByEmailAsync(request.Email);
    var result = await _signInManager.CheckPasswordSignInAsync(
        user, request.Password, lockoutOnFailure: true);
    
    if (result.Succeeded)
    {
        var token = await _tokenService.GenerateTokenAsync(user);
        return Ok(new { token, user = new UserDto(user) });
    }
    
    return Unauthorized(new { error = "Invalid credentials" });
}
```

#### Django

```python
# users/views.py
@api_view(['POST'])
def login(request):
    serializer = LoginSerializer(data=request.data)
    if serializer.is_valid():
        user = serializer.validated_data['user']
        refresh = RefreshToken.for_user(user)
        return Response({
            'refresh': str(refresh),
            'access': str(refresh.access_token),
            'user': UserSerializer(user).data
        })
    return Response(serializer.errors, status=401)
```

---

## Authorization Implementation

### Policy/Permission Definition

#### Rails (Pundit)

```ruby
# app/policies/player_policy.rb
class PlayerPolicy < ApplicationPolicy
  def create?
    system_admin? || has_permission?(:create)
  end
  
  def update?
    system_admin? || (same_academy? && has_permission?(:update))
  end
  
  private
  
  def same_academy?
    record.academy == ActsAsTenant.current_tenant
  end
end
```

#### ASP.NET Core

```csharp
// Requirements/PermissionRequirement.cs
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    public PermissionRequirement(string permission) 
        => Permission = permission;
}

// Handlers/PermissionHandler.cs
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userId = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        
        if (context.User.HasClaim("IsSystemAdmin", "True"))
        {
            context.Succeed(requirement);
            return;
        }
        
        var hasPermission = await _context.RolePermissions
            .AnyAsync(rp => rp.UserId == userId && 
                           rp.Permission == requirement.Permission);
        
        if (hasPermission)
            context.Succeed(requirement);
    }
}

// Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanCreate", policy =>
        policy.Requirements.Add(new PermissionRequirement("create")));
});
```

#### Django

```python
# permissions.py
class HasCreatePermission(permissions.BasePermission):
    def has_permission(self, request, view):
        if request.user.is_system_admin:
            return True
        
        academy_id = view.kwargs.get('academy_id')
        academy_user = AcademyUser.objects.filter(
            user=request.user,
            academy_id=academy_id,
            is_active=True
        ).first()
        
        if not academy_user:
            return False
        
        return RolePermission.objects.filter(
            role=academy_user.role,
            permission='create'
        ).exists()

class HasUpdatePermission(permissions.BasePermission):
    def has_object_permission(self, request, view, obj):
        if request.user.is_system_admin:
            return True
        
        # Check if user has access to this academy
        if obj.academy_id != request.academy_id:
            return False
        
        return self._has_permission(request.user, obj.academy_id, 'update')
```

---

### Controller/ViewSet Usage

#### Rails

```ruby
class PlayersController < Api::V1::BaseController
  def create
    @player = Player.new(player_params)
    authorize @player  # Calls PlayerPolicy#create?
    
    if @player.save
      render json: @player, status: :created
    else
      render json: @player.errors, status: :unprocessable_entity
    end
  end
end
```

#### ASP.NET Core

```csharp
[ApiController]
[Route("api/v1/{academyId}/players")]
public class PlayersController : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "CanCreate")]
    public async Task<IActionResult> CreatePlayer(
        Guid academyId,
        [FromBody] CreatePlayerRequest request)
    {
        var player = new Player { AcademyId = academyId };
        // Map request to player...
        
        await _context.Players.AddAsync(player);
        await _context.SaveChangesAsync();
        
        return Created($"/api/v1/{academyId}/players/{player.Id}", player);
    }
}
```

#### Django

```python
class PlayerViewSet(viewsets.ModelViewSet):
    permission_classes = [HasAcademyAccess]
    
    def create(self, request, academy_id=None):
        # Check create permission
        if not HasCreatePermission().has_permission(request, self):
            raise PermissionDenied()
        
        serializer = PlayerSerializer(data=request.data)
        serializer.is_valid(raise_exception=True)
        serializer.save(academy_id=academy_id)
        
        return Response(serializer.data, status=status.HTTP_201_CREATED)
    
    def update(self, request, academy_id=None, pk=None):
        player = self.get_object()
        
        # Check update permission
        if not HasUpdatePermission().has_object_permission(request, self, player):
            raise PermissionDenied()
        
        serializer = PlayerSerializer(player, data=request.data, partial=True)
        serializer.is_valid(raise_exception=True)
        serializer.save()
        
        return Response(serializer.data)
```

---

## Multi-Tenancy

### Setting Academy Context

#### Rails (ActsAsTenant)

```ruby
# app/controllers/application_controller.rb
before_action :set_tenant

def set_tenant
  academy_slug = params[:academy_slug] || 
                 request.headers['X-Academy-Context']
  
  @current_academy = Academy.find_by!(slug: academy_slug)
  ActsAsTenant.current_tenant = @current_academy
end

# All queries automatically scoped
Player.all  # Only returns players for current academy
```

#### ASP.NET Core

```csharp
// Middleware/TenantMiddleware.cs
public class TenantMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var academyId = context.Request.RouteValues["academyId"]?.ToString() ??
                       context.Request.Headers["X-Academy-Context"].ToString();
        
        if (Guid.TryParse(academyId, out var id))
        {
            context.Items["AcademyId"] = id;
        }
        
        await next(context);
    }
}

// Usage in services
public class PlayerService
{
    public async Task<List<Player>> GetPlayersAsync()
    {
        var academyId = (Guid)_httpContextAccessor.HttpContext.Items["AcademyId"];
        return await _context.Players
            .Where(p => p.AcademyId == academyId)
            .ToListAsync();
    }
}
```

#### Django

```python
# middleware.py
class TenantMiddleware:
    def __init__(self, get_response):
        self.get_response = get_response
    
    def __call__(self, request):
        academy_id = (
            request.resolver_match.kwargs.get('academy_id') or
            request.META.get('HTTP_X_ACADEMY_CONTEXT')
        )
        request.academy_id = academy_id
        return self.get_response(request)

# Usage in views/viewsets
class PlayerViewSet(viewsets.ModelViewSet):
    def get_queryset(self):
        return Player.objects.filter(academy_id=self.request.academy_id)
```

---

## Configuration Files

### Rails

```yaml
# config/database.yml
default: &default
  adapter: postgresql
  encoding: unicode
  pool: <%= ENV.fetch("RAILS_MAX_THREADS") { 5 } %>

development:
  <<: *default
  database: diquis_development

# config/initializers/devise.rb
Devise.setup do |config|
  config.jwt do |jwt|
    jwt.secret = Rails.application.credentials.devise_jwt_secret_key
    jwt.expiration_time = 24.hours.to_i
  end
end
```

### ASP.NET Core

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Diquis;User Id=sa;Password=YourPassword;"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-32-characters",
    "Issuer": "DiquisAPI",
    "Audience": "DiquisClient",
    "ExpiryMinutes": 1440
  }
}

// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        // Configure JWT...
    });
```

### Django

```python
# settings.py
DATABASES = {
    'default': {
        'ENGINE': 'django.db.backends.postgresql',
        'NAME': 'diquis',
        'USER': 'postgres',
        'PASSWORD': env('DATABASE_PASSWORD'),
        'HOST': 'localhost',
        'PORT': '5432',
    }
}

SIMPLE_JWT = {
    'ACCESS_TOKEN_LIFETIME': timedelta(hours=24),
    'REFRESH_TOKEN_LIFETIME': timedelta(days=7),
    'SIGNING_KEY': SECRET_KEY,
    'ALGORITHM': 'HS256',
}

REST_FRAMEWORK = {
    'DEFAULT_AUTHENTICATION_CLASSES': [
        'rest_framework_simplejwt.authentication.JWTAuthentication',
    ],
}
```

---

## For More Details

- **ASP.NET**: See [aspnet/API_AUTHENTICATION.md](aspnet/API_AUTHENTICATION.md) and [aspnet/AUTHORIZATION.md](aspnet/AUTHORIZATION.md)
- **Django**: See [django/API_AUTHENTICATION.md](django/API_AUTHENTICATION.md) and [django/AUTHORIZATION.md](django/AUTHORIZATION.md)
- **Migration Guide**: See [MIGRATION_SUMMARY.md](MIGRATION_SUMMARY.md)
