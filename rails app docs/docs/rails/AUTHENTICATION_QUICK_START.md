# Authentication Quick Start Guide

This guide will help you quickly get started with the Devise + JWT authentication system.

## 🚀 Quick Setup (5 minutes)

### 1. Run Migrations

```bash
rails db:migrate
```

### 2. Generate and Set JWT Secret

```bash
# Generate a secret key
rake secret

# Open credentials editor
EDITOR="nano" rails credentials:edit

# Add this line (replace with your generated secret):
devise_jwt_secret_key: your_secret_key_here_from_rake_secret

# Save and exit (Ctrl+X, then Y, then Enter in nano)
```

**Alternative:** Use environment variable:

```bash
export DEVISE_JWT_SECRET_KEY="your_secret_key_here"
```

### 3. Start Server

```bash
rails server
```

## 📝 Test Authentication (curl)

### Sign Up (Register)

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
```

**Expected Response:**

- Status: `201 Created`
- Header: `Authorization: Bearer eyJhbGciOiJIUzI1NiJ9...`
- Body contains user data

**Save the token from the Authorization header for next steps!**

### Sign In (Login)

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
```

**Expected Response:**

- Status: `200 OK`
- Header: `Authorization: Bearer <new_token>`
- Body: `{"data": {...}, "message": "Logged in successfully"}`

### Sign Out (Logout)

```bash
# Replace <TOKEN> with your actual token
curl -X DELETE "http://localhost:3000/auth/sign_out" \
  -H "Authorization: Bearer <TOKEN>" \
  -H "Content-Type: application/json"
```

**Expected Response:**

- Status: `200 OK`
- Body: `{"message": "Logged out successfully"}`

### Making Authenticated API Calls

```bash
# Use the token from sign in to access protected endpoints
curl -X GET "http://localhost:3000/api/v1/protected_endpoint" \
  -H "Authorization: Bearer <TOKEN>" \
  -H "Content-Type: application/json"
```

## 🧪 Test with Rails Console

```bash
rails console
```

```ruby
# Create a user
user = User.create!(
  email: "admin@example.com",
  password: "password123",
  password_confirmation: "password123",
  first_name: "Admin",
  last_name: "User",
  is_system_admin: true
)

# Check user
user.full_name  # => "Admin User"
user.system_admin?  # => true

# List all users
User.all

# Exit console
exit
```

## 🎭 Testing with Postman/Insomnia

### 1. Sign Up

- **Method:** POST
- **URL:** `http://localhost:3000/auth/sign_up`
- **Headers:** `Content-Type: application/json`
- **Body (JSON):**

  ```json
  {
    "user": {
      "email": "test@example.com",
      "password": "password123",
      "password_confirmation": "password123",
      "first_name": "Test",
      "last_name": "User"
    }
  }
  ```

### 2. Save Token

Copy the `Authorization` header value from the response (the part after "Bearer ").

### 3. Use Token

For subsequent requests, add this header:

- **Key:** `Authorization`
- **Value:** `Bearer <your_token_here>`

## 🔍 Common Issues

### Issue: "JWT secret key is nil"

**Solution:** Make sure you've set the secret key:

```bash
# Check if it's set
rails credentials:show | grep devise_jwt_secret_key

# If not found, add it
EDITOR="nano" rails credentials:edit
```

### Issue: "Couldn't find User"

**Solution:** Run migrations:

```bash
rails db:migrate
```

### Issue: "401 Unauthorized" on protected endpoints

**Solutions:**

1. Check token is in the request: `Authorization: Bearer <token>`
2. Token might be expired (24 hours) - sign in again
3. Token might be revoked - sign in again

### Issue: Cannot sign up duplicate email

This is expected behavior. Use a different email or delete the existing user:

```bash
rails console
User.find_by(email: "test@example.com")&.destroy
exit
```

## 📖 Full Documentation

- [Complete API Documentation](./API_AUTHENTICATION.md)
- [Detailed Setup Guide](./SETUP_AUTHENTICATION.md)
- [Phase 1 Infrastructure](./PHASE_1_INFRASTRUCTURE.md)

## 🎉 You're Ready

You now have a working authentication system. Next steps:

1. ✅ Run the test suite: `bundle exec rspec spec/requests/auth`
2. 🏗️ Build your API endpoints in `app/controllers/api/v1/`
3. 🔐 Add authorization with Pundit (next task)
4. 📚 Read the full [API Authentication Documentation](./API_AUTHENTICATION.md)

## 💡 Tips

- Tokens expire after 24 hours
- Logged out tokens are blacklisted and can't be reused
- Store tokens securely on the client side (not in localStorage for XSS protection)
- Use HTTPS in production
- Keep your JWT secret key safe and never commit it to git

Happy coding! 🚀
