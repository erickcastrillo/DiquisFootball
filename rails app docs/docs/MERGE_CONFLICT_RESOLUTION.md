# Merge Conflict Resolution

This document describes how the merge conflicts between the authentication branch and main were resolved.

## Conflicts Resolved

### 1. `app/controllers/application_controller.rb`

**Conflict:** Main branch added multi-tenancy (ActsAsTenant) logic, while our authentication branch added authentication and error handling.

**Resolution:** Merged both changes:

- Kept authentication logic (`authenticate_user!`, `current_user`, `decoded_token`)
- Kept multi-tenancy logic (`set_current_tenant`, `find_current_academy`)
- Maintained error handling from authentication branch
- Ensured `authenticate_user!` runs before `set_current_tenant` in the filter chain

### 2. `spec/factories.rb`

**Conflict:** Main branch added `academy` and `player` factories, while our authentication branch added `user` and `jwt_denylist` factories.

**Resolution:** Combined all factories in one file:

- User and jwt_denylist factories (from authentication branch)
- Academy and player factories (from main branch)

### 3. Authentication Controllers

**Added:** Skip tenant setup in authentication controllers since they run before authentication:

- `Auth::SessionsController` - Added `skip_before_action :set_current_tenant, raise: false`
- `Auth::RegistrationsController` - Added `skip_before_action :set_current_tenant, raise: false`

This prevents errors when users try to authenticate (since they don't have a tenant context yet).

## Files Changed

- ✅ `app/controllers/application_controller.rb` - Merged authentication + multi-tenancy
- ✅ `spec/factories.rb` - Merged all factories
- ✅ `app/controllers/auth/sessions_controller.rb` - Skip tenant setup
- ✅ `app/controllers/auth/registrations_controller.rb` - Skip tenant setup

## Testing

The merge maintains compatibility with both:

1. **Authentication flow** - Users can sign up, sign in, and sign out without needing tenant context
2. **Multi-tenancy** - Once authenticated, API requests are scoped to the appropriate academy tenant

## Result

The authentication system now works harmoniously with the multi-tenancy system:

- Users authenticate first (without tenant)
- Then API calls are scoped to their academy tenant
- Auth endpoints skip tenant scoping
- All other endpoints require both authentication and tenant context
