# Authentication Setup Instructions

This guide will help you set up and configure the Devise + JWT authentication system.

## Prerequisites

1. PostgreSQL database is running
2. Rails dependencies are installed (`bundle install`)
3. pgcrypto extension is enabled (migration already exists)

## Step 1: Run Database Migrations

Run the authentication-related migrations:

```bash
rails db:migrate
```text

This will create the following tables:

- `users` - User accounts with authentication fields
- `jwt_denylists` - Revoked JWT tokens

## Step 2: Generate JWT Secret Key

Generate a secure secret key for signing JWT tokens:

```bash
# Generate a random secret
rake secret
```text

Copy the output and add it to your Rails credentials:

```bash
# Open credentials file
EDITOR="nano" rails credentials:edit

# Add this line (replace with your generated secret):
devise_jwt_secret_key: your_generated_secret_key_here
```text

**Alternative:** Set as environment variable:

```bash
# In .env file or export in shell
export DEVISE_JWT_SECRET_KEY="your_generated_secret_key_here"
```text

## Step 3: Verify Installation

Check that the User model exists:

```bash
rails console
```text

In the console:

```ruby
# Check User model
User
# => User(id: integer, email: string, ...)

# Check JWT Denylist model
JwtDenylist
# => JwtDenylist(id: integer, jti: string, exp: datetime, ...)

# Exit console
exit
```text

## Step 4: Create a Test User (Optional)

Create a test user in the Rails console:

```bash
rails console
```text

```ruby
User.create!(
  email: "admin@example.com",
  password: "password123",
  password_confirmation: "password123",
  first_name: "Admin",
  last_name: "User",
  is_system_admin: true
)
```text

## Step 5: Test Authentication Endpoints

Start the Rails server:

```bash
rails server
```text

### Test Sign Up

```bash
curl -X POST "http://localhost:3000/auth/sign_up" \
  -H "Content-Type: application/json" \
  -d '{
    "user": {
      "email": "test@example.com",
      "password": "password123",
      "password_confirmation": "password123",
      "first_name": "Test",
      "last_name": "User"
    }
  }' \
  -i
```text

Look for the `Authorization` header in the response containing your JWT token.

### Test Sign In

```bash
curl -X POST "http://localhost:3000/auth/sign_in" \
  -H "Content-Type: application/json" \
  -d '{
    "user": {
      "email": "test@example.com",
      "password": "password123"
    }
  }' \
  -i
```text

### Test Sign Out

```bash
# Replace <TOKEN> with the token from sign in response
curl -X DELETE "http://localhost:3000/auth/sign_out" \
  -H "Authorization: Bearer <TOKEN>" \
  -H "Content-Type: application/json"
```text

## Step 6: Verify Routes

Check that authentication routes are configured:

```bash
rails routes | grep auth
```text

Expected output should include:

```text
new_user_session        GET    /auth/sign_in
user_session            POST   /auth/sign_in
destroy_user_session    DELETE /auth/sign_out
new_user_registration   GET    /auth/sign_up
user_registration       POST   /auth/sign_up
...
```text

## Troubleshooting

### Issue: "Couldn't find User"

Make sure you've run the migrations:

```bash
rails db:migrate
```text

### Issue: "uninitialized constant JwtDenylist"

The JwtDenylist model should be created. Check that the file exists:

```bash
cat app/models/jwt_denylist.rb
```text

### Issue: "JWT secret key is missing"

Make sure you've set the JWT secret key in credentials or environment variables:

```bash
rails credentials:show | grep devise_jwt_secret_key
# or
echo $DEVISE_JWT_SECRET_KEY
```text

### Issue: Database connection error

Make sure PostgreSQL is running and the database exists:

```bash
rails db:create
rails db:migrate
```text

## Security Checklist

- [ ] JWT secret key is set and kept secure
- [ ] HTTPS is enabled in production
- [ ] Password requirements meet security standards (minimum 6 characters)
- [ ] Database backups include `jwt_denylists` table
- [ ] Environment variables are not committed to version control
- [ ] Rails credentials master key is stored securely

## Next Steps

1. Run the test suite to verify authentication works:

   ```bash
   bundle exec rspec spec/requests/auth
   ```

1. Review the API documentation: [docs/API_AUTHENTICATION.md](./API_AUTHENTICATION.md)

2. Implement authorization with Pundit (Phase 1, Task 1.8)

3. Configure email settings for password recovery (optional)

4. Set up Sidekiq for background jobs related to authentication (Phase 1, Task 1.6)

## References

- [Devise Documentation](https://github.com/heartcombo/devise)
- [Devise-JWT Documentation](https://github.com/waiting-for-dev/devise-jwt)
- [Phase 1 Infrastructure Documentation](./PHASE_1_INFRASTRUCTURE.md)
