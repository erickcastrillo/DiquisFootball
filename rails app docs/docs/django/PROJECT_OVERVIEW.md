# Diquis - Django Project Overview

**Project Name:** Diquis Football Management System  
**Framework:** Django 5.0+ with Django REST Framework  
**Architecture:** Django Apps with Service Layer Pattern  
**Database:** PostgreSQL with UUID primary keys  
**Created:** October 13, 2025

## Project Description

Diquis is a comprehensive football (soccer) academy management system designed to manage players, teams, training sessions, and administrative tasks across multiple academies. The system implements multi-tenant architecture where each academy operates as an isolated tenant with its own data and resources.

## Technology Stack

### Backend

- **Framework:** Django 5.0+ with Django REST Framework 3.14+
- **Language:** Python 3.11+
- **Database:** PostgreSQL 15+
- **Multi-Tenancy:** django-tenant-schemas or django-tenants
- **Authentication:** Simple JWT (djangorestframework-simplejwt)
- **Authorization:** Django REST Framework Permissions with custom classes
- **Background Jobs:** Celery with Redis broker
- **Caching:** Redis (django-redis)
- **File Storage:** Django Storage (AWS S3/Google Cloud via django-storages)
- **Image Processing:** Pillow
- **API Documentation:** drf-spectacular (OpenAPI 3.0)

### Frontend (Separate Repository)

- **Framework:** React
- **State Management:** Redux Toolkit
- **Data Fetching:** TanStack Query (React Query)
- **Build Tool:** Vite
- **Styling:** Tailwind CSS

### Development Tools

- **Task Runner:** Django management commands + Invoke tasks
- **Testing:** pytest with pytest-django
- **Code Quality:** pylint, black, mypy, isort
- **Performance:** django-debug-toolbar, django-silk

## Architecture Principles

### 1. Django Apps Organization

Features are organized as Django apps by business capability. Each app contains:

- Models (domain entities)
- Views/ViewSets (API endpoints)
- Serializers (JSON conversion)
- Permissions (authorization rules)
- Services (business logic)
- Tasks (Celery jobs)

### 2. Service Layer Pattern

All business logic and complex database interactions are handled through service classes that:

- Encapsulate business rules
- Handle validations
- Manage transactions with `@transaction.atomic`
- Return consistent result objects

### 3. Multi-Tenant Design

- Academy-based data isolation using django-tenant-schemas
- Automatic tenant scoping for queries
- Tenant context from URL, headers, or user session
- Shared resources (categories, divisions) across tenants

### 4. API-First Design

- RESTful JSON API using Django REST Framework
- Consistent response patterns with custom renderers
- Comprehensive error handling with exception handlers
- OpenAPI/Swagger documentation via drf-spectacular
- Versioned endpoints (v1, v2) using namespaces

### 5. Domain-Driven Design

- Business logic centralized in service classes
- Rich domain models with properties and methods
- Clear boundaries between apps
- Ubiquitous language throughout codebase

## Core Business Domains (Apps)

### 1. Academy Management

- Academy creation and configuration
- User-academy associations
- Academy settings and preferences
- Owner/administrator management

**Key Models:** `Academy`, `AcademyUser`  
**Key Services:** `AcademyCreationService`, `AcademyFinderService`

### 2. Player Management

- Player registration and profiles
- Player search and filtering
- Player-position assignments
- Parent/guardian information
- Player skill assessments

**Key Models:** `Player`, `PlayerSkill`  
**Key Services:** `PlayerRegistrationService`, `PlayerSearchService`

### 3. Team Management

- Team creation and organization
- Team rosters and memberships
- Team schedules
- Category/division assignments

**Key Models:** `Team`, `TeamMembership`  
**Key Services:** `TeamCreationService`, `TeamRosterService`

### 4. Training Management

- Training session scheduling
- Attendance tracking (bulk operations)
- Training types and locations
- Coach assignments
- Real-time updates (Django Channels - WebSocket)

**Key Models:** `Training`, `TrainingAttendance`  
**Key Services:** `TrainingSchedulingService`, `AttendanceTrackingService`  
**Key Tasks:** `send_training_reminder`

### 5. Shared Resources

- Positions (Goalkeeper, Defender, etc.)
- Skills (Passing, Shooting, etc.)
- Categories (U-8, U-10, U-12, etc.)
- Divisions (Primera, Amateur, etc.)

**Key Models:** `Position`, `Skill`, `Category`, `Division`

### 6. Asset Management

- Equipment and uniform tracking
- Inventory management with reorder points
- Asset allocation and check-out system
- Maintenance scheduling and cost tracking
- Depreciation calculations and financial reporting

**Key Models:** `Asset`, `AssetCategory`, `AssetAllocation`  
**Key Services:** `AssetManagementService`, `AssetAllocationService`

### 7. Reporting & Analytics

- Financial reports (P&L, revenue, expenses)
- Player development analytics
- Team performance metrics
- Operational efficiency reports
- Business intelligence and benchmarking

**Key Models:** `Report`, `ReportGeneration`, `FinancialTransaction`  
**Key Services:** `ReportGenerationService`, `FinancialAnalyticsService`

### 8. Communication System

- Multi-channel messaging (email, SMS, push notifications)
- Parent portal with secure access
- Event notifications and reminders
- Emergency alerts and announcements
- Message delivery tracking and templates

**Key Models:** `Message`, `MessageDelivery`, `ParentPortalAccess`  
**Key Services:** `MessageDeliveryService`, `NotificationSchedulingService`

## Key Features

### Multi-Academy Context Management (Hybrid Approach)

1. **URL-Based:** `/api/v1/{academy_slug}/players/`
2. **Header-Based:** `X-Academy-Context: academy-slug`
3. **Redux Store:** Frontend state management
4. **React Context:** Deep component tree access
5. **TanStack Query:** Automatic cache invalidation on academy switch

### Authentication & Authorization

- JWT-based authentication with Simple JWT
- Role-based access control (RBAC) using custom permission classes
- Academy-scoped permissions
- System admin vs academy admin roles

### Real-Time Features (Django Channels)

- WebSocket support for real-time updates
- Live training attendance updates
- Real-time notifications
- Channel layers with Redis backend

### Data Privacy & Security

- Field-level encryption for sensitive data
- django-cryptography for encrypted fields
- Indexing encrypted fields for searchability
- Audit logging for critical operations

### Background Processing

- Celery for async tasks with Redis broker
- Welcome emails on player registration
- Training reminders (24h and 2h before)
- Data exports and reports
- Periodic tasks with Celery Beat

### API Features

- Pagination with Django REST Framework pagination classes
- Filtering and search with django-filter
- Relationship inclusion using `?include=position,category`
- Field sorting with `ordering` parameter
- Comprehensive error responses with custom exception handlers

## Development Workflow

### Process Management

```bash
# Start Django development server
python manage.py runserver

# Start Celery worker
celery -A diquis worker -l info

# Start Celery beat (scheduler)
celery -A diquis beat -l info

# Start Flower (Celery monitoring)
celery -A diquis flower
```

### Environment Structure

- **Development:** Local PostgreSQL + Redis
- **Test:** Isolated test database with pytest
- **Production:** Dockerized with docker-compose or Kubernetes

## Project Structure

```txt
diquis/
├── manage.py
├── diquis/                    # Project settings
│   ├── __init__.py
│   ├── settings/
│   │   ├── base.py
│   │   ├── development.py
│   │   ├── production.py
│   │   └── test.py
│   ├── urls.py
│   ├── wsgi.py
│   └── asgi.py
├── apps/                          # Django apps
│   ├── academy_management/
│   ├── player_management/
│   ├── team_management/
│   ├── training_management/
│   ├── shared_resources/
│   ├── asset_management/
│   ├── reporting_analytics/
│   └── communication_notification/
├── common/                        # Shared utilities
│   ├── services/
│   ├── permissions/
│   ├── mixins/
│   └── utils/
├── tests/                         # Test suite
├── docs/                          # Documentation
├── static/                        # Static files
├── media/                         # User uploads
└── requirements/                  # Dependencies
    ├── base.txt
    ├── development.txt
    ├── production.txt
    └── test.txt
```

## Reference Implementation

This Django implementation is based on the Ruby on Rails "Diquis" project that serves as the architectural reference. The Django version maintains the same business logic, domain models, and API contracts while leveraging Django and DRF conventions and best practices.

## Getting Started

See the following documentation files:

- [SETUP_GUIDE.md](./SETUP_GUIDE.md) - Initial project setup
- [ARCHITECTURE.md](./ARCHITECTURE.md) - Detailed architecture documentation
- [API_AUTHENTICATION.md](./API_AUTHENTICATION.md) - Authentication implementation
- [AUTHORIZATION.md](./AUTHORIZATION.md) - Authorization setup
- [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md) - Development workflows

## Python Package Management

### Virtual Environment

```bash
# Create virtual environment
python -m venv venv

# Activate (Linux/macOS)
source venv/bin/activate

# Activate (Windows)
venv\Scripts\activate

# Install dependencies
pip install -r requirements/development.txt
```

### Key Packages

- **Django:** Web framework
- **djangorestframework:** API framework
- **djangorestframework-simplejwt:** JWT authentication
- **django-filter:** Advanced filtering
- **drf-spectacular:** OpenAPI documentation
- **celery:** Background tasks
- **redis:** Caching and message broker
- **psycopg2-binary:** PostgreSQL adapter
- **pillow:** Image processing
- **pytest-django:** Testing framework
- **black:** Code formatter
- **pylint:** Code linter
- **mypy:** Static type checker

## License

MIT License

## Contact

Development Team: dev@diquis.com
