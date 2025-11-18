# Diquis - Framework Documentation Index

This directory contains documentation for implementing Diquis Football Management System across different frameworks.

## Documentation Structure

```txt
docs/
├── README.md (this file)
├── MIGRATION_SUMMARY.md         # Migration guide and framework equivalents
├── MIGRATION_COMPLETION.md      # Migration status and next steps
├── QUICK_REFERENCE.md           # Side-by-side code comparisons
├── FEATURE_*.md                 # Business logic & feature specifications (framework-agnostic)
├── aspnet/                      # ASP.NET Core implementation docs
│   ├── API_AUTHENTICATION.md    # ASP.NET Core Identity + JWT
│   └── AUTHORIZATION.md         # Policy-based authorization
├── django/                      # Django REST Framework implementation docs
│   ├── API_AUTHENTICATION.md    # Simple JWT authentication
│   └── AUTHORIZATION.md         # DRF Permissions
└── rails/                       # Ruby on Rails implementation docs (reference)
    ├── API_AUTHENTICATION.md    # Devise + JWT authentication
    ├── API_DOCUMENTATION.md     # Rails API reference
    ├── ARCHITECTURE.md          # Rails architecture patterns
    ├── AUTHENTICATION_*.md      # Rails auth guides
    ├── AUTHORIZATION.md         # Pundit authorization
    ├── DEVELOPMENT_GUIDE.md     # Rails development workflow
    ├── GEMFILE_DOCUMENTATION.md # Gem dependencies
    ├── INFRASTRUCTURE_*.md      # Rails infrastructure setup
    ├── PROJECT_OVERVIEW.md      # Rails project overview
    ├── SETUP_GUIDE.md           # Rails environment setup
    ├── SIDEKIQ_SETUP.md         # Background jobs setup
    └── [Other Rails-specific docs]
```

## Quick Start by Framework

### Ruby on Rails (Reference Implementation)

**Key Technologies:**

- Rails 8.0.3+ (API-only)
- Devise + JWT for authentication
- Pundit for authorization
- ActsAsTenant for multi-tenancy
- Sidekiq for background jobs

**Documentation:**

- [rails/API_AUTHENTICATION.md](./rails/API_AUTHENTICATION.md)
- [rails/AUTHORIZATION.md](./rails/AUTHORIZATION.md)
- [rails/PROJECT_OVERVIEW.md](./rails/PROJECT_OVERVIEW.md)
- [rails/ARCHITECTURE.md](./rails/ARCHITECTURE.md)
- [rails/SETUP_GUIDE.md](./rails/SETUP_GUIDE.md)

---

### ASP.NET Core

**Key Technologies:**

- ASP.NET Core 10.0+
- ASP.NET Core Identity + JWT Bearer
- Authorization Policies
- Custom multi-tenancy middleware
- Hangfire for background jobs

**Documentation:**

- [aspnet/API_AUTHENTICATION.md](./aspnet/API_AUTHENTICATION.md) - JWT authentication with Identity
- [aspnet/AUTHORIZATION.md](./aspnet/AUTHORIZATION.md) - Policy-based authorization
- aspnet/PROJECT_OVERVIEW.md (Coming soon)
- aspnet/ARCHITECTURE.md (Coming soon)
- aspnet/SETUP_GUIDE.md (Coming soon)

**Quick Links:**

- [Authentication Setup](./aspnet/API_AUTHENTICATION.md#implementation-details)
- [Authorization Policies](./aspnet/AUTHORIZATION.md#authorization-policies)
- [Multi-Tenancy](./aspnet/AUTHORIZATION.md#academyaccessrequirement)

---

### Django REST Framework

**Key Technologies:**

- Django 5.0+
- Django REST Framework
- Simple JWT for authentication
- DRF Permissions for authorization
- django-tenant-schemas for multi-tenancy
- Celery for background jobs

**Documentation:**

- [django/API_AUTHENTICATION.md](./django/API_AUTHENTICATION.md) - JWT authentication with Simple JWT
- [django/AUTHORIZATION.md](./django/AUTHORIZATION.md) - Permission classes
- django/PROJECT_OVERVIEW.md (Coming soon)
- django/ARCHITECTURE.md (Coming soon)
- django/SETUP_GUIDE.md (Coming soon)

**Quick Links:**

- [Authentication Setup](./django/API_AUTHENTICATION.md#implementation-details)
- [Permission Classes](./django/AUTHORIZATION.md#custom-permission-classes)
- [Multi-Tenancy](./django/AUTHORIZATION.md#hasacademyaccess)

---

## Framework Comparison

### Authentication

| Feature | Rails (Devise) | ASP.NET (Identity) | Django (Simple JWT) |
|---------|---------------|-------------------|---------------------|
| User Model | Built-in | Built-in | Custom required |
| Password Hashing | bcrypt | PBKDF2 | PBKDF2 |
| JWT Support | devise-jwt gem | JwtBearer | djangorestframework-simplejwt |
| Token Refresh | ✅ | ✅ | ✅ |
| Token Blacklist | ✅ | Custom | ✅ |

### Authorization

| Feature | Rails (Pundit) | ASP.NET (Policies) | Django (DRF Permissions) |
|---------|---------------|-------------------|------------------------|
| Role-Based | ✅ | ✅ | ✅ |
| Policy-Based | ✅ | ✅ | ✅ |
| Resource-Based | ✅ | ✅ | ✅ (Object-level) |
| Multi-Tenancy | ActsAsTenant | Custom middleware | django-tenant-schemas |

### Background Jobs

| Feature | Rails (Sidekiq) | ASP.NET (Hangfire) | Django (Celery) |
|---------|----------------|-------------------|-----------------|
| Queue Support | ✅ | ✅ | ✅ |
| Scheduled Jobs | ✅ | ✅ | ✅ |
| Retries | ✅ | ✅ | ✅ |
| Dashboard | ✅ | ✅ | ✅ (Flower) |
| Backend | Redis | SQL/Redis/Memory | Redis/RabbitMQ |

## Common Concepts Across Frameworks

### Multi-Tenancy (Academy-Based)

All implementations use academy-based multi-tenancy where:

- Each academy is an isolated tenant
- Users can belong to multiple academies
- Data is automatically scoped to the current academy
- System admins can access all academies

**Implementation:**

- **Rails**: ActsAsTenant gem
- **ASP.NET**: Custom middleware + query filters
- **Django**: django-tenant-schemas or custom middleware

### API Design

All implementations follow RESTful principles:

```txt
GET    /api/v1/{academy_id}/players           # List players
POST   /api/v1/{academy_id}/players           # Create player
GET    /api/v1/{academy_id}/players/{id}      # Get player
PUT    /api/v1/{academy_id}/players/{id}      # Update player
DELETE /api/v1/{academy_id}/players/{id}      # Delete player
```

### Response Format

Standardized JSON responses:

```json
{
  "data": { ... },
  "meta": {
    "pagination": { ... }
  }
}
```

Error responses:

```json
{
  "error": "ERROR_CODE",
  "message": "Human-readable message",
  "details": [ ... ]
}
```

## Migration Guide

See [MIGRATION_SUMMARY.md](./MIGRATION_SUMMARY.md) for:

- Framework equivalents table
- Code migration examples
- Database schema mapping
- Testing framework comparison
- Step-by-step migration instructions

## Getting Started

### 1. Choose Your Framework

Select based on your team's expertise and requirements:

- **Rails**: Rapid development, convention over configuration
- **ASP.NET**: Enterprise-grade, strongly-typed, Microsoft ecosystem
- **Django**: Python ecosystem, batteries-included, rapid prototyping

### 2. Review Documentation

Start with these docs in order:

1. rails/PROJECT_OVERVIEW - Understand the business domain
2. rails/ARCHITECTURE - Learn the system design
3. Framework-specific API_AUTHENTICATION - Implement authentication
4. Framework-specific AUTHORIZATION - Implement authorization
5. Framework-specific SETUP_GUIDE - Set up your development environment

### 3. Set Up Development Environment

Follow the framework-specific setup guide:

- [Rails Setup Guide](./rails/SETUP_GUIDE.md)
- ASP.NET Setup Guide (Coming soon)
- Django Setup Guide (Coming soon)

### 4. Implement Core Features

Follow this order:

1. Authentication (Users, JWT)
2. Authorization (Roles, Permissions)
3. Multi-Tenancy (Academy context)
4. Academy Management
5. Player Management
6. Team Management
7. Training Management
8. Additional Features

## API Documentation

### Swagger/OpenAPI

All implementations include interactive API documentation:

- **Rails**: Rswag at `/api-docs`
- **ASP.NET**: Swashbuckle at `/swagger`
- **Django**: drf-spectacular at `/api/schema/swagger-ui/`

### Postman Collections

Postman collections are available for all frameworks:

- `postman/rails-collection.json`
- `postman/aspnet-collection.json`
- `postman/django-collection.json`

## Testing

### Test Coverage Goals

All implementations should maintain:

- ≥80% code coverage
- 100% coverage for authentication/authorization
- Integration tests for all API endpoints
- Unit tests for business logic

### Testing Tools

| Framework | Unit Tests | Integration Tests | Mocking |
|-----------|-----------|-------------------|---------|
| Rails | RSpec | RSpec + Capybara | FactoryBot |
| ASP.NET | xUnit/NUnit | WebApplicationFactory | Moq |
| Django | pytest | Django TestCase | Factory Boy |

## Deployment

### Containerization

All implementations support Docker:

- Rails: Kamal 2
- ASP.NET: Docker + Kubernetes
- Django: Docker + Gunicorn

### Cloud Platforms

Supported platforms:

- AWS (All frameworks)
- Azure (Recommended for ASP.NET)
- Google Cloud (All frameworks)
- Heroku (Rails, Django)
- DigitalOcean (All frameworks)

## Contributing

When adding new features:

1. Implement in Rails first (reference implementation)
2. Document in Rails docs
3. Create equivalent implementations for ASP.NET and Django
4. Update framework-specific documentation
5. Update MIGRATION_SUMMARY.md with new equivalents

## Support

### Documentation Issues

If you find errors or gaps in documentation:

1. Check MIGRATION_SUMMARY.md for framework equivalents
2. Review the Rails reference implementation
3. Consult official framework documentation
4. Open an issue or submit a PR

### Framework-Specific Resources

**Rails:**

- [Rails Guides](https://guides.rubyonrails.org/)
- [Devise Documentation](https://github.com/heartcombo/devise)
- [Pundit Documentation](https://github.com/varvet/pundit)

**ASP.NET:**

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Identity Documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Authorization Documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/)

**Django:**

- [Django Documentation](https://docs.djangoproject.com/)
- [DRF Documentation](https://www.django-rest-framework.org/)
- [Simple JWT Documentation](https://django-rest-framework-simplejwt.readthedocs.io/)

## License

MIT License - See LICENSE file for details

## Contact

For questions or support:

- Email: dev@diquis.com
- GitHub Issues: [Project Issues](https://github.com/erickcastrillo/Diquis/issues)
