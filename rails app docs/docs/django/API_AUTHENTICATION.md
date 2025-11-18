# API Authentication Documentation (Django REST Framework)

This document describes the authentication system for the Diquis Football API built with Django REST Framework.

## Overview

The API uses **Django REST Framework (DRF)** with **Simple JWT** for user authentication combined with **JWT (JSON Web Tokens)** for API token handling.
This provides a secure, stateless authentication mechanism suitable for API-only applications.

## Authentication Endpoints

### Sign Up (Register)

Create a new user account.

**Endpoint:** `POST /api/auth/register/`

**Request Headers:**

```http
Content-Type: application/json
```

**Request Body:**

```json
{
  "email": "user@example.com",
  "password": "password123",
  "password2": "password123",
  "first_name": "John",
  "last_name": "Doe"
}
```

**Success Response (201 Created):**

```json
{
  "user": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "user@example.com",
    "first_name": "John",
    "last_name": "Doe"
  },
  "refresh": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9...",
  "access": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9...",
  "message": "User registered successfully"
}
```

**Error Response (400 Bad Request):**

```json
{
  "email": [
    "This email address is already in use."
  ],
  "password": [
    "This password is too short. It must contain at least 8 characters."
  ]
}
```

---

### Sign In (Login)

Authenticate an existing user and receive JWT tokens.

**Endpoint:** `POST /api/auth/login/`

**Request Headers:**

```http
Content-Type: application/json
```

**Request Body:**

```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Success Response (200 OK):**

```json
{
  "user": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "user@example.com",
    "first_name": "John",
    "last_name": "Doe",
    "is_system_admin": false
  },
  "refresh": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9...",
  "access": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9...",
  "message": "Logged in successfully"
}
```

**Error Response (401 Unauthorized):**

```json
{
  "detail": "No active account found with the given credentials"
}
```

---

### Refresh Token

Get a new access token using a refresh token.

**Endpoint:** `POST /api/auth/token/refresh/`

**Request Headers:**

```http
Content-Type: application/json
```

**Request Body:**

```json
{
  "refresh": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9..."
}
```

**Success Response (200 OK):**

```json
{
  "access": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9...",
  "access_token_expiration": "2025-10-27T10:30:00Z"
}
```

---

### Sign Out (Logout)

Blacklist the refresh token to log out the user.

**Endpoint:** `POST /api/auth/logout/`

**Request Headers:**

```http
Content-Type: application/json
Authorization: Bearer <access_token>
```

**Request Body:**

```json
{
  "refresh": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9..."
}
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
  "detail": "Authentication credentials were not provided."
}
```

---

## Making Authenticated Requests

To access protected API endpoints, include the JWT access token in the Authorization header:

```http
Authorization: Bearer <access_token>
```

**Example using Python requests:**

```python
import requests

headers = {
    'Authorization': f'Bearer {access_token}',
    'Content-Type': 'application/json'
}

response = requests.get('https://api.example.com/api/v1/players/', headers=headers)
```

---

## Token Management

### Token Lifetime

- **Access Token**: Expires after **15 minutes** by default
- **Refresh Token**: Expires after **7 days** by default

Users can use the refresh token to obtain a new access token without re-authenticating.

Configuration in `settings.py`:

```python
from datetime import timedelta

SIMPLE_JWT = {
    'ACCESS_TOKEN_LIFETIME': timedelta(minutes=15),
    'REFRESH_TOKEN_LIFETIME': timedelta(days=7),
    'ROTATE_REFRESH_TOKENS': True,
    'BLACKLIST_AFTER_ROTATION': True,
    'UPDATE_LAST_LOGIN': True,
    
    'ALGORITHM': 'HS256',
    'SIGNING_KEY': SECRET_KEY,
    'VERIFYING_KEY': None,
    'AUDIENCE': None,
    'ISSUER': None,
    
    'AUTH_HEADER_TYPES': ('Bearer',),
    'AUTH_HEADER_NAME': 'HTTP_AUTHORIZATION',
    'USER_ID_FIELD': 'id',
    'USER_ID_CLAIM': 'user_id',
    
    'AUTH_TOKEN_CLASSES': ('rest_framework_simplejwt.tokens.AccessToken',),
    'TOKEN_TYPE_CLAIM': 'token_type',
}
```

### Token Blacklisting

When a user logs out or refresh tokens rotate, old tokens are added to a blacklist using `rest_framework_simplejwt.token_blacklist`.
This ensures that revoked tokens cannot be reused.

---

## Security Features

1. **Password Hashing**: User passwords are hashed using Django's default PBKDF2 algorithm with:
   - PBKDF2-SHA256
   - 390,000 iterations (Django 4.2+)
   - Automatic salt generation

2. **JWT Secret Key**: Tokens are signed using Django's `SECRET_KEY` setting:
   - Store in environment variables (production)
   - Use `.env` file (development)
   - Never commit to version control

3. **Token Blacklist**: Revoked refresh tokens are stored in the database to prevent reuse.

4. **HTTPS Required**: In production, all authentication requests should be made over HTTPS.

5. **UUID Identifiers**: Each user has a unique UUID for additional security and to avoid exposing sequential IDs.

6. **Password Validators**: Django's built-in validators ensure strong passwords:
   - UserAttributeSimilarityValidator
   - MinimumLengthValidator (8 characters)
   - CommonPasswordValidator
   - NumericPasswordValidator

---

## Implementation Details

### User Model (Custom User)

```python
from django.contrib.auth.models import AbstractBaseUser, PermissionsMixin
from django.db import models
import uuid

class User(AbstractBaseUser, PermissionsMixin):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    email = models.EmailField(unique=True)
    first_name = models.CharField(max_length=50)
    last_name = models.CharField(max_length=50)
    is_system_admin = models.BooleanField(default=False)
    is_active = models.BooleanField(default=True)
    is_staff = models.BooleanField(default=False)
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
    
    USERNAME_FIELD = 'email'
    REQUIRED_FIELDS = ['first_name', 'last_name']
    
    objects = UserManager()
    
    class Meta:
        db_table = 'users'
        
    def __str__(self):
        return self.email
    
    @property
    def full_name(self):
        return f"{self.first_name} {self.last_name}"
```

### JWT Configuration (settings.py)

```python
# settings.py

INSTALLED_APPS = [
    # ...
    'rest_framework',
    'rest_framework_simplejwt',
    'rest_framework_simplejwt.token_blacklist',
    # ...
]

REST_FRAMEWORK = {
    'DEFAULT_AUTHENTICATION_CLASSES': (
        'rest_framework_simplejwt.authentication.JWTAuthentication',
    ),
    'DEFAULT_PERMISSION_CLASSES': (
        'rest_framework.permissions.IsAuthenticated',
    ),
    'DEFAULT_PAGINATION_CLASS': 'rest_framework.pagination.PageNumberPagination',
    'PAGE_SIZE': 25,
}

# Custom user model
AUTH_USER_MODEL = 'users.User'

# Password validation
AUTH_PASSWORD_VALIDATORS = [
    {
        'NAME': 'django.contrib.auth.password_validation.UserAttributeSimilarityValidator',
    },
    {
        'NAME': 'django.contrib.auth.password_validation.MinimumLengthValidator',
        'OPTIONS': {
            'min_length': 8,
        }
    },
    {
        'NAME': 'django.contrib.auth.password_validation.CommonPasswordValidator',
    },
    {
        'NAME': 'django.contrib.auth.password_validation.NumericPasswordValidator',
    },
]
```

### Authentication Views

```python
# users/views.py
from rest_framework import status
from rest_framework.decorators import api_view, permission_classes
from rest_framework.permissions import AllowAny, IsAuthenticated
from rest_framework.response import Response
from rest_framework_simplejwt.tokens import RefreshToken
from rest_framework_simplejwt.exceptions import TokenError
from django.contrib.auth import get_user_model

User = get_user_model()

@api_view(['POST'])
@permission_classes([AllowAny])
def register(request):
    """Register a new user and return JWT tokens."""
    serializer = UserRegistrationSerializer(data=request.data)
    
    if serializer.is_valid():
        user = serializer.save()
        
        # Generate JWT tokens
        refresh = RefreshToken.for_user(user)
        
        return Response({
            'user': {
                'id': str(user.id),
                'email': user.email,
                'first_name': user.first_name,
                'last_name': user.last_name,
            },
            'refresh': str(refresh),
            'access': str(refresh.access_token),
            'message': 'User registered successfully'
        }, status=status.HTTP_201_CREATED)
    
    return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)


@api_view(['POST'])
@permission_classes([AllowAny])
def login(request):
    """Authenticate user and return JWT tokens."""
    serializer = LoginSerializer(data=request.data)
    
    if serializer.is_valid():
        user = serializer.validated_data['user']
        
        # Generate JWT tokens
        refresh = RefreshToken.for_user(user)
        
        return Response({
            'user': {
                'id': str(user.id),
                'email': user.email,
                'first_name': user.first_name,
                'last_name': user.last_name,
                'is_system_admin': user.is_system_admin,
            },
            'refresh': str(refresh),
            'access': str(refresh.access_token),
            'message': 'Logged in successfully'
        }, status=status.HTTP_200_OK)
    
    return Response(serializer.errors, status=status.HTTP_401_UNAUTHORIZED)


@api_view(['POST'])
@permission_classes([IsAuthenticated])
def logout(request):
    """Blacklist the refresh token to logout."""
    try:
        refresh_token = request.data.get('refresh')
        token = RefreshToken(refresh_token)
        token.blacklist()
        
        return Response({
            'message': 'Logged out successfully'
        }, status=status.HTTP_200_OK)
    except TokenError:
        return Response({
            'error': 'Invalid token'
        }, status=status.HTTP_400_BAD_REQUEST)
```

### Serializers

```python
# users/serializers.py
from rest_framework import serializers
from django.contrib.auth import get_user_model, authenticate
from django.contrib.auth.password_validation import validate_password

User = get_user_model()

class UserRegistrationSerializer(serializers.ModelSerializer):
    password2 = serializers.CharField(write_only=True, required=True)
    
    class Meta:
        model = User
        fields = ('email', 'password', 'password2', 'first_name', 'last_name')
        extra_kwargs = {
            'password': {'write_only': True}
        }
    
    def validate(self, attrs):
        if attrs['password'] != attrs['password2']:
            raise serializers.ValidationError({
                "password": "Password fields didn't match."
            })
        
        # Validate password strength
        validate_password(attrs['password'])
        
        return attrs
    
    def create(self, validated_data):
        validated_data.pop('password2')
        user = User.objects.create_user(**validated_data)
        return user


class LoginSerializer(serializers.Serializer):
    email = serializers.EmailField(required=True)
    password = serializers.CharField(required=True, write_only=True)
    
    def validate(self, attrs):
        email = attrs.get('email')
        password = attrs.get('password')
        
        user = authenticate(email=email, password=password)
        
        if not user:
            raise serializers.ValidationError(
                'No active account found with the given credentials'
            )
        
        if not user.is_active:
            raise serializers.ValidationError('User account is disabled.')
        
        attrs['user'] = user
        return attrs
```

### URL Configuration

```python
# urls.py
from django.urls import path
from rest_framework_simplejwt.views import TokenRefreshView
from users import views

urlpatterns = [
    path('api/auth/register/', views.register, name='register'),
    path('api/auth/login/', views.login, name='login'),
    path('api/auth/logout/', views.logout, name='logout'),
    path('api/auth/token/refresh/', TokenRefreshView.as_view(), name='token_refresh'),
]
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

### Environment Variables

Create a `.env` file in your project root:

```bash
# .env
SECRET_KEY=your-secret-key-here-min-50-characters-long
DEBUG=False
ALLOWED_HOSTS=localhost,127.0.0.1,yourdomain.com

# Database
DATABASE_URL=postgresql://user:password@localhost:5432/diquis

# JWT Settings (optional overrides)
JWT_ACCESS_TOKEN_LIFETIME=15  # minutes
JWT_REFRESH_TOKEN_LIFETIME=7  # days
```

Load in `settings.py`:

```python
from pathlib import Path
import environ

env = environ.Env()
environ.Env.read_env()

SECRET_KEY = env('SECRET_KEY')
DEBUG = env.bool('DEBUG', default=False)
ALLOWED_HOSTS = env.list('ALLOWED_HOSTS', default=[])
```

---

## Testing Authentication

### Using curl

**Register:**

```bash
curl -X POST "http://localhost:8000/api/auth/register/" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "password123",
    "password2": "password123",
    "first_name": "Test",
    "last_name": "User"
  }'
```

**Login:**

```bash
curl -X POST "http://localhost:8000/api/auth/login/" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "password123"
  }'
```

**Refresh Token:**

```bash
curl -X POST "http://localhost:8000/api/auth/token/refresh/" \
  -H "Content-Type: application/json" \
  -d '{
    "refresh": "<your_refresh_token_here>"
  }'
```

**Logout:**

```bash
curl -X POST "http://localhost:8000/api/auth/logout/" \
  -H "Authorization: Bearer <your_access_token_here>" \
  -H "Content-Type: application/json" \
  -d '{
    "refresh": "<your_refresh_token_here>"
  }'
```

### Using Python requests

```python
import requests

# Register
response = requests.post('http://localhost:8000/api/auth/register/', json={
    'email': 'test@example.com',
    'password': 'password123',
    'password2': 'password123',
    'first_name': 'Test',
    'last_name': 'User'
})
data = response.json()
access_token = data['access']
refresh_token = data['refresh']

# Use access token for authenticated requests
headers = {'Authorization': f'Bearer {access_token}'}
response = requests.get('http://localhost:8000/api/v1/players/', headers=headers)
```

---

## Next Steps

- Implement password reset endpoints
- Add email confirmation (optional)
- Add two-factor authentication (2FA) with `django-otp`
- Implement social authentication (Google, Facebook) with `dj-rest-auth` and `django-allauth`
- Add rate limiting with `django-ratelimit`
- Implement role-based authorization with Django permissions

---

## Related Documentation

- [Django REST Framework Documentation](https://www.django-rest-framework.org/)
- [Simple JWT Documentation](https://django-rest-framework-simplejwt.readthedocs.io/)
- [Django Authentication System](https://docs.djangoproject.com/en/stable/topics/auth/)
- [JWT.io](https://jwt.io/) - JWT introduction and debugger
- [AUTHORIZATION.md](./AUTHORIZATION.md) - Authorization with Django permissions
