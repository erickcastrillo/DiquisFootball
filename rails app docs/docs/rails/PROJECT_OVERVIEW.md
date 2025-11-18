# Diquis - Rails API Project Overview

**Project Name:** Diquis Football Management System  
**Framework:** Ruby on Rails 8.0.3+ (API-only)  
**Architecture:** Vertical Slice Architecture  
**Database:** PostgreSQL with UUID primary keys  
**Created:** October 13, 2025

## Project Description

Diquis is a comprehensive football (soccer) academy management system designed to manage players, teams, training sessions, and administrative tasks across multiple academies. The system implements multi-tenant architecture where each academy operates as an isolated tenant with its own data and resources.

## Technology Stack

### Backend

- **Framework:** Rails 8.0.3+ API-only
- **Language:** Ruby 3.3.0
- **Database:** PostgreSQL 15+
- **Multi-Tenancy:** ActsAsTenant gem
- **Authentication:** Devise + JWT tokens
- **Authorization:** Pundit (role-based policies)
- **Background Jobs:** Sidekiq with Redis
- **Caching:** Redis
- **File Storage:** Active Storage (AWS S3/Google Cloud)
- **Image Processing:** ImageMagick/Vips
- **API Documentation:** Rswag (Swagger/OpenAPI 3.0)

### Frontend (Separate Repository)

- **Framework:** React
- **State Management:** Redux Toolkit
- **Data Fetching:** TanStack Query (React Query)
- **Build Tool:** Vite
- **Styling:** Tailwind CSS

### Development Tools

- **Process Manager:** Overmind (or Foreman)
- **Testing:** RSpec with FactoryBot
- **Code Quality:** Rubocop, Brakeman
- **Performance:** Bullet (N+1 detection)

## Architecture Principles

### 1. Vertical Slice Architecture

Features are organized by business capability rather than technical layer. Each slice contains:

- Controllers
- Services
- Models
- Serializers
- Policies
- Jobs

### 2. Service Layer Pattern

All business logic and database interactions are handled through service classes (POROs) that:

- Encapsulate business rules
- Handle validations
- Manage transactions
- Return consistent result objects

### 3. Multi-Tenant Design

- Academy-based data isolation using ActsAsTenant
- Automatic tenant scoping for queries
- Tenant context from URL, headers, or user session
- Shared resources (categories, divisions) across tenants

### 4. API-First Design

- RESTful JSON API
- Consistent response patterns
- Comprehensive error handling
- OpenAPI/Swagger documentation
- Versioned endpoints (v1, v2)

### 5. Domain-Driven Design

- Business logic centralized in domain services
- Rich domain models with behavior
- Clear boundaries between slices
- Ubiquitous language throughout codebase

## Core Business Domains (Slices)

### 1. Academy Management

- Academy creation and configuration
- User-academy associations
- Academy settings and preferences
- Owner/administrator management

### 2. Player Management

- Player registration and profiles
- Player search and filtering
- Player-position assignments
- Parent/guardian information
- Player skill assessments

### 3. Team Management

- Team creation and organization
- Team rosters and memberships
- Team schedules
- Category/division assignments

### 4. Training Management

- Training session scheduling
- Attendance tracking (bulk operations)
- Training types and locations
- Coach assignments
- Real-time updates (WebSocket)

### 5. Shared Resources

- Positions (Goalkeeper, Defender, etc.)
- Skills (Passing, Shooting, etc.)
- Categories (U-8, U-10, U-12, etc.)
- Divisions (Primera, Amateur, etc.)

### 6. Asset Management

- Equipment and uniform tracking
- Inventory management with reorder points
- Asset allocation and check-out system
- Maintenance scheduling and cost tracking
- Depreciation calculations and financial reporting

### 7. Reporting & Analytics

- Financial reports (P&L, revenue, expenses)
- Player development analytics
- Team performance metrics
- Operational efficiency reports
- Business intelligence and benchmarking

### 8. Communication System

- Multi-channel messaging (email, SMS, push notifications)
- Parent portal with secure access
- Event notifications and reminders
- Emergency alerts and announcements
- Message delivery tracking and templates

## Key Features

### Multi-Academy Context Management (Hybrid Approach)

1. **URL-Based:** `/api/v1/{academy_slug}/players`
2. **Header-Based:** `X-Academy-Context: academy-slug`
3. **Redux Store:** Frontend state management
4. **React Context:** Deep component tree access
5. **TanStack Query:** Automatic cache invalidation on academy switch

### Authentication & Authorization

- JWT-based authentication
- Role-based access control (RBAC)
- Academy-scoped permissions
- System admin vs academy admin roles

### Real-Time Features (Rails 8.0)

- Solid Cable for WebSocket connections
- Live training attendance updates
- Real-time notifications

### Data Privacy & Security

- Rails 8.0 Active Record encryption
- Sensitive data encryption (emails, phone numbers)
- Blind indexes for encrypted search
- Audit logging for critical operations

### Background Processing

- Sidekiq for async jobs
- Welcome emails on player registration
- Training reminders (24h and 2h before)
- Data exports and reports

### API Features

- Pagination with metadata
- Filtering and search
- Relationship inclusion (`?include=position,category`)
- Field sorting
- Comprehensive error responses

## Development Workflow

### Process Management (Overmind)

```bash
# Start all services
./bin/dev

# Individual services
overmind start web      # Rails server
overmind start worker   # Sidekiq
overmind start frontend # React dev server
overmind connect web    # View Rails logs
```text

### Environment Structure

- **Development:** Local PostgreSQL + Redis
- **Test:** Isolated test database with RSpec
- **Production:** Dockerized with Kamal 2 deployment

## Project Structure

```text
diquis/
├── app/
│   ├── controllers/
│   │   ├── api/v1/base_controller.rb
│   │   └── application_controller.rb
│   ├── slices/                    # Vertical slices
│   │   ├── academy_management/
│   │   ├── player_management/
│   │   ├── team_management/
│   │   ├── training_management/
│   │   └── shared_resources/
│   ├── shared/                    # Cross-cutting concerns
│   │   ├── services/
│   │   ├── policies/
│   │   └── models/concerns/
│   ├── serializers/
│   ├── jobs/
│   └── mailers/
├── config/
├── db/
├── docs/                          # Project documentation
├── lib/
├── spec/                          # RSpec tests
└── swagger/                       # API documentation
```text

## Reference Django Project

This Rails implementation is based on an existing Django "Diquis" project that serves as the architectural reference. The Rails version maintains the same business logic, domain models, and API contracts while leveraging Rails conventions and best practices.

## Getting Started

See the following documentation files:

- [SETUP_GUIDE.md](./SETUP_GUIDE.md) - Initial project setup
- [ARCHITECTURE.md](./ARCHITECTURE.md) - Detailed architecture documentation
- [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) - API design and usage
- [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md) - Development workflows

## License

MIT License

## Contact

Development Team: dev@diquis.com
