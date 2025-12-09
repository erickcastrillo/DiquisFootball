# Diquis.BackgroundJobs - Authentication Flow

## Overview
The Diquis.BackgroundJobs application is a dedicated Hangfire dashboard server that requires authentication and role-based authorization.

## Authentication & Authorization

### Required Role
- Users must have the **"root"** role to access both the landing page and Hangfire dashboard
- This is configured in `Program.cs` via the `HangfireAuthorizationFilter` and `HangfireAuthenticationMiddleware`

### Login Flow

1. **Unauthenticated Access**
   - When an unauthenticated user accesses any protected path (`/` or `/hangfire`)
   - They are automatically redirected to: `/Identity/Account/Login?returnUrl={requested-path}`
   - After successful login, they are returned to **the exact URL they originally requested**
   - **Example**: 
     - Request `/` ? Login ? Return to `/`
     - Request `/hangfire` ? Login ? Return to `/hangfire`

2. **Authenticated but Unauthorized**
   - If a user is logged in but doesn't have the "root" role
   - They are redirected to: `/Identity/Account/AccessDenied`

3. **Authorized Access**
   - Users with the "root" role can access both the landing page and Hangfire dashboard
   - The landing page (`/`) provides a link to the Hangfire dashboard
   - User information and logout button are displayed in the top-right corner

4. **Logout**
   - Click the "Logout" button in the top-right corner
   - POST request sent to `/Identity/Account/Logout` with anti-forgery token
   - User is logged out and redirected to `/Identity/Account/Login`
   - All authentication cookies are cleared

### Routes

| Route | Description | Authentication Required |
|-------|-------------|------------------------|
| `/` | Landing page with link to Hangfire | Yes - "root" role |
| `/hangfire` | Hangfire Dashboard | Yes - "root" role |
| `/Identity/Account/Login` | Login page (ASP.NET Identity UI) | No |
| `/Identity/Account/Logout` | Logout endpoint (POST only) | Yes (authenticated only) |
| `/Identity/Account/AccessDenied` | Access denied page | Yes (authenticated only) |

## Implementation Details

### Two-Layer Authentication Approach

The authentication uses a two-layer approach:

#### 1. HangfireAuthenticationMiddleware
Located: `Diquis.BackgroundJobs/Middleware/HangfireAuthenticationMiddleware.cs`

This middleware runs **after** ASP.NET Core authentication/authorization and:

**For Any Protected Path (`/` or `/hangfire/*`):**
- Checks if the user is authenticated
- If not authenticated ? redirects to `/Identity/Account/Login?returnUrl={current-path-with-querystring}`
- Checks if the user has the "root" role
- If authenticated but no role ? redirects to `/Identity/Account/AccessDenied`
- If both checks pass ? allows the request to proceed to the endpoint

**Key Feature**: The middleware preserves the **exact URL** (including query strings) that the user requested, ensuring they return to the same location after login.

**Middleware Order in Program.cs:**
```csharp
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireAuthentication(); // Custom middleware handles authentication checks
```

#### 2. HangfireAuthorizationFilter
Located: `Diquis.BackgroundJobs/Middleware/HangfireCustomAuthorizationFilter.cs`

This filter implements `IDashboardAuthorizationFilter` and provides a second layer of security:
- Checks if the user is authenticated
- Checks if the user has the required role
- Returns `true` or `false` (no redirects handled here)

**Why Two Layers?**
- The middleware handles redirects and provides a consistent entry point
- The filter provides defense-in-depth security at the Hangfire level
- Ensures no endpoint can bypass authentication

### Authentication Flow Diagram

```
User visits "/" or "/hangfire"
    ?
[HangfireAuthenticationMiddleware]
    ?
Is authenticated? ? No ? Redirect to: /Identity/Account/Login?returnUrl={original-url}
    ? Yes                     ?
Has "root" role? ? No ? Redirect to: /Identity/Account/AccessDenied
    ? Yes
Allow request to proceed
    ?
If path = "/" ? Display landing page with user info & logout button
If path = "/hangfire" ? [HangfireAuthorizationFilter] ? Display Dashboard
```

### Return URL Preservation

The middleware captures the complete URL including query strings:
```csharp
string returnUrl = context.Request.Path + context.Request.QueryString;
```

This means:
- Request: `http://localhost:7298/` ? Return URL: `/`
- Request: `http://localhost:7298/hangfire` ? Return URL: `/hangfire`
- Request: `http://localhost:7298/hangfire/jobs/enqueued` ? Return URL: `/hangfire/jobs/enqueued`
- Request: `http://localhost:7298/hangfire?filter=succeeded` ? Return URL: `/hangfire?filter=succeeded`

### Anti-Forgery Token Implementation

The logout button uses ASP.NET Core's anti-forgery token system to prevent CSRF attacks:

```csharp
app.MapGet("/", (HttpContext context, IAntiforgery antiforgery) =>
{
    // Generate anti-forgery token
    var tokens = antiforgery.GetAndStoreTokens(context);
    var requestToken = tokens.RequestToken!;
    
    // Include token in logout form
    // <input type='hidden' name='__RequestVerificationToken' value='{requestToken}' />
});
```

This ensures that:
1. Only forms generated by the server can log out users
2. CSRF attacks cannot force logout
3. The logout form is secure and validated by ASP.NET Identity

## User Management

### Creating a Root User
To create a user with "root" role access, you need to:

1. Register a user through the Identity UI or seed data
2. Assign the "root" role to that user
3. This is typically done in `JobsDbInitializer.cs` during database initialization

Example seed code:
```csharp
// Create root role if it doesn't exist
if (!await roleManager.RoleExistsAsync("root"))
{
    await roleManager.CreateAsync(new IdentityRole("root"));
}

// Create root user
var rootUser = new DiquisInfrastructureJobsUser
{
    UserName = "admin@job.com",
    Email = "admin@job.com",
    EmailConfirmed = true
};

await userManager.CreateAsync(rootUser, "Password123!");
await userManager.AddToRoleAsync(rootUser, "root");
```

### Default Seeded User
The application automatically seeds a root user on first run:
- **Email**: `admin@job.com`
- **Password**: `Password123!`
- **Role**: `root`

**?? Security**: Change this password immediately in production!

## Landing Page

The root path (`/`) serves a responsive HTML landing page that:
- Displays the application name and purpose
- Shows the currently logged-in user's email in the top-right corner
- Provides a **Logout** button with anti-forgery protection for secure sign-out
- Displays user status and role information
- Provides a direct link to the Hangfire dashboard
- Requires authentication (protected by the middleware)
- Is accessible only to users with the "root" role

### Landing Page Features
- **User Information Display**: Shows logged-in user's email
- **Secure Logout Button**: Form-based POST to `/Identity/Account/Logout` with anti-forgery token
- **Status Badge**: Visual indicator showing server is online
- **Role Display**: Shows "Root Administrator" for authenticated users
- **Responsive Design**: Works on mobile and desktop devices
- **Direct Dashboard Access**: One-click access to Hangfire dashboard

The landing page provides a user-friendly entry point while maintaining security.

## Security Considerations

1. **HTTPS**: Always use HTTPS in production (configured via `UseHttpsRedirection()`)
2. **Strong Passwords**: Ensure Identity password requirements are configured appropriately
3. **Role Management**: Limit the "root" role to trusted administrators only
4. **Session Timeout**: Configure appropriate session timeouts in Identity settings
5. **Two-Factor Authentication**: Consider enabling 2FA for root users in production
6. **Defense in Depth**: The two-layer authentication approach provides redundant security
7. **Change Default Password**: The seeded admin password should be changed immediately
8. **Return URL Validation**: ASP.NET Identity automatically validates return URLs to prevent open redirect attacks
9. **Logout Security**: Logout uses POST with anti-forgery token to prevent CSRF attacks
10. **Anti-Forgery Tokens**: All logout forms include anti-forgery tokens for security

## Troubleshooting

### Issue: Still getting 401 error
- **Cause**: The middleware might not be in the correct order, or you may need to restart the application
- **Solution**: 
  1. Stop the application completely
  2. Rebuild the solution
  3. Start the application again
  4. The middleware must be placed AFTER `UseAuthentication()` and `UseAuthorization()`

### Issue: Redirect loop
- **Cause**: Identity pages may not be properly configured or login page is also protected
- **Solution**: Ensure `app.MapRazorPages()` is called and Identity UI pages are not protected by default authorization

### Issue: Not redirected to originally requested URL after login
- **Cause**: Return URL might be malformed or blocked by ASP.NET Identity security
- **Solution**: 
  - Ensure the return URL starts with `/` (local path)
  - Check that no custom logic in Identity pages is overriding the return URL
  - Clear browser cookies and try again

### Issue: Access Denied even with correct role
- **Cause**: Role claim not properly added to user
- **Solution**: Verify user has the role assigned in the database and re-login to refresh claims

### Issue: 404 on login page
- **Cause**: Identity UI not scaffolded or included
- **Solution**: Ensure `Microsoft.AspNetCore.Identity.UI` package is installed and `AddDefaultUI()` is called

### Issue: Middleware not executing
- **Cause**: Middleware order is incorrect or endpoint routing is interfering
- **Solution**: Ensure middleware is registered BEFORE `app.MapGet()` and `app.MapHangfireDashboard()` but AFTER authentication/authorization middleware

### Issue: Logout button not working
- **Cause**: CSRF validation failure or anti-forgery token not generated correctly
- **Solution**: 
  - Ensure `app.MapRazorPages()` is called (required for Identity pages)
  - Check that `IAntiforgery` service is being injected in the endpoint
  - Verify the anti-forgery token is being generated and included in the form
  - Check browser console for JavaScript or network errors
  - Clear browser cache and cookies

### Issue: "The antiforgery token could not be decrypted"
- **Cause**: Anti-forgery token validation failing
- **Solution**: 
  - Ensure data protection keys are properly configured
  - Clear browser cookies and reload the page
  - Restart the application to generate new keys

### Issue: User email not displaying on landing page
- **Cause**: Email claim not present in authentication token
- **Solution**: 
  - Ensure user has `EmailConfirmed = true` in the database
  - Check that email claim is being added during sign-in
  - Re-login to refresh authentication cookie
  - Verify Identity options are configured to include email claim

## Usage Examples

### Example 1: First-time visitor
```
1. User navigates to: http://localhost:7298/
2. Not authenticated ? Redirected to: /Identity/Account/Login?returnUrl=%2F
3. User logs in with admin@job.com
4. After login ? Redirected to: http://localhost:7298/
5. Sees landing page with user info and logout button
```

### Example 2: Direct Hangfire access
```
1. User navigates to: http://localhost:7298/hangfire
2. Not authenticated ? Redirected to: /Identity/Account/Login?returnUrl=%2Fhangfire
3. User logs in
4. After login ? Redirected to: http://localhost:7298/hangfire
5. Sees Hangfire dashboard immediately
```

### Example 3: Deep link with query string
```
1. User navigates to: http://localhost:7298/hangfire/jobs/enqueued?from=100
2. Not authenticated ? Redirected to: /Identity/Account/Login?returnUrl=%2Fhangfire%2Fjobs%2Fenqueued%3Ffrom%3D100
3. User logs in
4. After login ? Redirected to: http://localhost:7298/hangfire/jobs/enqueued?from=100
5. Sees exact page they originally requested
```

### Example 4: Logout flow
```
1. User is on landing page: http://localhost:7298/
2. User clicks "Logout" button in top-right corner
3. POST request sent to: /Identity/Account/Logout (with anti-forgery token)
4. ASP.NET Identity validates token and clears authentication cookies
5. User redirected to: /Identity/Account/Login
6. User sees login page and is fully logged out
```

### Example 5: Complete session lifecycle
```
1. Visit root: http://localhost:7298/
2. Redirect to login: /Identity/Account/Login?returnUrl=%2F
3. Enter credentials: admin@job.com / Password123!
4. Submit login form
5. Redirect back to: http://localhost:7298/
6. See landing page with user info
7. Click "Open Hangfire Dashboard" button
8. See Hangfire dashboard: http://localhost:7298/hangfire
9. Browse jobs and check status
10. Click "Logout" button
11. POST to: /Identity/Account/Logout
12. Redirect to: /Identity/Account/Login
13. Session ended, must login again to access
