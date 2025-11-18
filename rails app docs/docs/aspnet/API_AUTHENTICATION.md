# API Authentication Documentation (ASP.NET Core)

This document describes the authentication system for the Diquis Football API built with ASP.NET Core.

## Overview

The API uses **ASP.NET Core Identity** for user authentication combined with **JWT (JSON Web Tokens)** for API token handling.
This provides a secure, stateless authentication mechanism suitable for API-only applications.

## Authentication Endpoints

### Sign Up (Register)

Create a new user account.

**Endpoint:** `POST /api/auth/register`

**Request Headers:**

```http
Content-Type: application/json
```

**Request Body:**

```json
{
  "email": "user@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Success Response (201 Created):**

```json
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe"
  },
  "message": "User registered successfully"
}
```

**Response Headers:**

```http
Authorization: Bearer <jwt_token>
```

**Error Response (400 Bad Request):**

```json
{
  "errors": {
    "Email": [
      "Email is already in use"
    ],
    "Password": [
      "Password must be at least 6 characters"
    ]
  }
}
```

---

### Sign In (Login)

Authenticate an existing user and receive a JWT token.

**Endpoint:** `POST /api/auth/login`

**Request Headers:**

```http
Content-Type: application/json
```

**Request Body:**

```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Success Response (200 OK):**

```json
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "isSystemAdmin": false
  },
  "token": "eyJhbGciOiJIUzI1NiJ9...",
  "expiresAt": "2025-10-27T10:30:00Z",
  "message": "Logged in successfully"
}
```

**Error Response (401 Unauthorized):**

```json
{
  "error": "Invalid email or password"
}
```

---

### Sign Out (Logout)

Invalidate the current JWT token and log out the user.

**Endpoint:** `POST /api/auth/logout`

**Request Headers:**

```http
Content-Type: application/json
Authorization: Bearer <jwt_token>
```

**Success Response (200 OK):**

```json
{
  "message": "Logged out successfully"
}
```

**Error Response (401 Unauthorized):**

```json
{
  "message": "No active session"
}
```

---

## Making Authenticated Requests

To access protected API endpoints, include the JWT token in the Authorization header:

```http
Authorization: Bearer <jwt_token>
```

**Example using HttpClient:**

```csharp
var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);

var response = await client.GetAsync("https://api.example.com/api/v1/players");
```

---

## Token Management

### Token Lifetime

JWT tokens expire after **24 hours** by default. After expiration, users must sign in again to obtain a new token.

Configuration in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-32-characters-long",
    "Issuer": "DiquisAPI",
    "Audience": "DiquisClient",
    "ExpiryMinutes": 1440
  }
}
```

### Token Revocation

When a user signs out, their JWT token is added to a blacklist (`TokenBlacklist` table) to prevent further use.
This ensures that logged-out tokens cannot be reused even if they haven't expired yet.

---

## Security Features

1. **Password Hashing**: User passwords are hashed using ASP.NET Core Identity's default PBKDF2 algorithm with:
   - HMAC-SHA256
   - 10,000 iterations (configurable)
   - 128-bit salt
   - 256-bit subkey

2. **JWT Secret Key**: Tokens are signed using a secret key stored in:
   - `appsettings.json` (development)
   - User Secrets (development)
   - Azure Key Vault (production)
   - Environment variables

3. **Token Blacklist**: Revoked tokens are stored in the database to prevent reuse.

4. **HTTPS Required**: In production, all authentication requests should be made over HTTPS.

5. **GUID Identifiers**: Each user has a unique GUID for additional security and to avoid exposing sequential IDs.

6. **Email Confirmation**: Optional email verification before login (configurable).

7. **Account Lockout**: Automatic lockout after failed login attempts (configurable).

---

## Implementation Details

### User Model (ApplicationUser)

```csharp
public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsSystemAdmin { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<AcademyUser> AcademyUsers { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
}
```

### JWT Configuration (Program.cs)

```csharp
// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
    };
});

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    
    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
```

### Authentication Controller

```csharp
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }
        
        var token = await _tokenService.GenerateTokenAsync(user);
        
        return Created("", new
        {
            data = new
            {
                id = user.Id,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName
            },
            token = token,
            message = "User registered successfully"
        });
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user == null)
        {
            return Unauthorized(new { error = "Invalid email or password" });
        }
        
        var result = await _signInManager.CheckPasswordSignInAsync(
            user, request.Password, lockoutOnFailure: true);
        
        if (!result.Succeeded)
        {
            return Unauthorized(new { error = "Invalid email or password" });
        }
        
        var token = await _tokenService.GenerateTokenAsync(user);
        
        return Ok(new
        {
            data = new
            {
                id = user.Id,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                isSystemAdmin = user.IsSystemAdmin
            },
            token = token,
            expiresAt = DateTime.UtcNow.AddMinutes(1440),
            message = "Logged in successfully"
        });
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _tokenService.RevokeTokenAsync(userId);
        
        return Ok(new { message = "Logged out successfully" });
    }
}
```

### Token Service

```csharp
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    
    public async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("IsSystemAdmin", user.IsSystemAdmin.ToString())
        };
        
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToInt32(_configuration["JwtSettings:ExpiryMinutes"])),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public async Task RevokeTokenAsync(string userId)
    {
        var blacklistEntry = new TokenBlacklist
        {
            UserId = Guid.Parse(userId),
            RevokedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
        
        _context.TokenBlacklists.Add(blacklistEntry);
        await _context.SaveChangesAsync();
    }
}
```

---

## Error Handling

The API returns standardized error responses:

### 401 Unauthorized

Returned when authentication fails or token is invalid/expired.

### 400 Bad Request

Returned when request validation fails (e.g., missing required fields, invalid email format).

### 500 Internal Server Error

Returned when an unexpected server error occurs.

---

## Configuration

### User Secrets (Development)

```bash
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key-min-32-characters-long"
```

### Environment Variables (Production)

```bash
export JWT_SECRET_KEY="your-secret-key-min-32-characters-long"
export JWT_ISSUER="DiquisAPI"
export JWT_AUDIENCE="DiquisClient"
export JWT_EXPIRY_MINUTES="1440"
```

---

## Testing Authentication

### Using curl

**Register:**

```bash
curl -X POST "http://localhost:5000/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Password123!",
    "confirmPassword": "Password123!",
    "firstName": "Test",
    "lastName": "User"
  }'
```

**Login:**

```bash
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Password123!"
  }'
```

**Logout:**

```bash
curl -X POST "http://localhost:5000/api/auth/logout" \
  -H "Authorization: Bearer <your_token_here>" \
  -H "Content-Type: application/json"
```

### Using C# HttpClient

```csharp
// Login
var loginRequest = new
{
    email = "test@example.com",
    password = "Password123!"
};

var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
var token = result.Token;

// Use token for authenticated requests
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);
```

---

## Next Steps

- Implement password reset endpoints
- Add email confirmation (optional)
- Add refresh token mechanism for long-lived sessions
- Implement two-factor authentication (2FA)
- Add OAuth2/OpenID Connect support (Google, Microsoft)
- Implement role-based authorization with ASP.NET Core Authorization Policies

---

## Related Documentation

- [ASP.NET Core Identity Documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [JWT Authentication in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
- [JWT.io](https://jwt.io/) - JWT introduction and debugger
- [AUTHORIZATION.md](./AUTHORIZATION.md) - Authorization with ASP.NET Core policies
