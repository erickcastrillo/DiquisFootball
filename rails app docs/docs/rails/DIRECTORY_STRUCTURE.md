# Diquis - Directory Structure Overview

## Purpose

This document provides a comprehensive overview of the Diquis project directory structure and the purpose of each component.

## Project Structure

### Root Level

```text
diquis/
├── .github/                     # GitHub configuration
├── app/                         # Application code
├── bin/                         # Executable scripts
├── config/                      # Application configuration
├── db/                          # Database files
├── docs/                        # Project documentation
├── lib/                         # Library code
├── log/                         # Application logs
├── public/                      # Public assets
├── shared/                      # Cross-cutting concerns
├── spec/                        # RSpec tests
├── storage/                     # Active Storage files
├── test/                        # Minitest files (legacy)
├── tmp/                         # Temporary files
└── vendor/                      # Third-party code
```text

## Core Application Structure

### App Directory (`app/`)

The main application code organized using Vertical Slice Architecture:

```text
app/
├── controllers/                 # Traditional Rails controllers
│   ├── application_controller.rb
│   └── concerns/
├── jobs/                        # Background jobs
│   └── application_job.rb
├── mailers/                     # Email templates
│   └── application_mailer.rb
├── models/                      # Traditional Rails models
│   ├── application_record.rb
│   └── concerns/
├── slices/                      # Vertical slice architecture
│   ├── academy_management/      # Academy operations
│   ├── asset_management/        # Equipment & inventory
│   ├── communication_notification/ # Messaging system
│   ├── player_management/       # Player operations
│   ├── reporting_analytics/     # Business intelligence
│   ├── shared_resources/        # Common resources
│   ├── team_management/         # Team operations
│   └── training_management/     # Training sessions
└── views/                       # View templates (minimal)
    └── layouts/
```text

### Vertical Slices (`app/slices/`)

Each slice contains complete functionality for a business domain:

```text
[slice_name]/
├── controllers/                 # HTTP request handlers
├── services/                    # Business logic
├── models/                      # Domain models
├── serializers/                 # JSON serialization
├── policies/                    # Authorization rules
├── validators/                  # Custom validations
└── jobs/                        # Background jobs
```text

#### Academy Management Slice

**Purpose:** Manage football academies (tenants)

- Create and configure academies
- User-academy associations
- Academy settings and preferences

#### Player Management Slice  

**Purpose:** Player registration, profiles, and data

- Player registration with validations
- Player search and filtering
- Skill tracking and assessments
- Parent/guardian information

#### Team Management Slice

**Purpose:** Organize players into teams

- Team creation and management
- Player-team assignments
- Team rosters and memberships

#### Training Management Slice

**Purpose:** Schedule and track training sessions

- Training session scheduling
- Attendance tracking
- Conflict detection
- Notification system

#### Shared Resources Slice

**Purpose:** Manage shared resources

- Player positions (Goalkeeper, Defender, etc.)
- Skills (Passing, Shooting, etc.)
- Age categories (U-8, U-10, etc.)
- Competition divisions

#### Asset Management Slice

**Purpose:** Equipment and inventory tracking

- Asset registration and barcoding
- Equipment allocation to players/teams
- Maintenance scheduling
- Depreciation tracking

#### Reporting & Analytics Slice

**Purpose:** Business intelligence and reporting

- Financial reports (P&L, cash flow)
- Player development analytics
- Operational efficiency reports
- KPI dashboards

#### Communication & Notification Slice

**Purpose:** Multi-channel communication system

- Email, SMS, push notifications
- Parent portal access
- Automated reminders
- Emergency alerts

### Shared Components (`shared/`)

Cross-cutting concerns used across multiple slices:

```text
shared/
├── services/                    # Base service classes
├── concerns/                    # Reusable modules
├── policies/                    # Base policy classes
├── validators/                  # Shared validation logic
└── serializers/                 # Base serializer classes
```text

## Configuration Structure

### Config Directory (`config/`)

```text
config/
├── environments/                # Environment-specific settings
│   ├── development.rb
│   ├── production.rb
│   └── test.rb
├── initializers/                # Initialization code
├── locales/                     # Internationalization files
├── application.rb               # Main application configuration
├── routes.rb                    # URL routing
├── database.yml                 # Database configuration
├── sidekiq.yml                  # Background job configuration
└── puma.rb                      # Web server configuration
```text

## Documentation Structure

### Docs Directory (`docs/`)

```text
docs/
├── PROJECT_OVERVIEW.md          # High-level project overview
├── ARCHITECTURE.md              # Detailed architecture documentation
├── SETUP_GUIDE.md               # Installation and setup guide
├── API_DOCUMENTATION.md         # API reference and examples
├── DEVELOPMENT_GUIDE.md         # Development workflows and standards
├── FEATURE_ASSET_MANAGEMENT.md  # Asset management feature spec
├── IMPLEMENTATION_PHASES.md     # Development phases and milestones
├── PHASE_0_SETUP.md            # Initial setup phase
├── PHASE_1_INFRASTRUCTURE.md   # Infrastructure implementation
└── GEMFILE_DOCUMENTATION.md    # Gem selections and rationale
```text

## Testing Structure

### Spec Directory (`spec/`)

```text
spec/
├── controllers/                 # Controller tests
├── factories/                   # FactoryBot factories
├── fixtures/                    # Test fixtures
├── integration/                 # Integration tests
├── mailers/                     # Mailer tests
├── models/                      # Model tests
├── requests/                    # Request/API tests
├── services/                    # Service layer tests
├── support/                     # Test support files
│   ├── database_cleaner.rb
│   ├── vcr.rb
│   └── custom_matchers.rb
├── rails_helper.rb              # Rails testing configuration
└── spec_helper.rb               # RSpec configuration
```text

## GitHub Configuration

### .github Directory (`.github/`)

```text
.github/
├── workflows/                   # GitHub Actions
│   └── ci.yml
├── ISSUE_TEMPLATE/              # Issue templates
│   ├── bug_report.md
│   └── feature_request.md
├── PULL_REQUEST_TEMPLATE.md     # PR template
└── dependabot.yml               # Dependency updates
```text

## Development Tools

### Process Management

- **Procfile.dev** - Overmind process definitions
- **.overmind.env** - Environment variables for Overmind

### Code Quality

- **.rubocop.yml** - Ruby code style rules
- **.rspec** - RSpec configuration

### Version Management

- **.ruby-version** - Ruby version specification
- **Gemfile** - Gem dependencies
- **Gemfile.lock** - Locked gem versions

## Design Principles

### Vertical Slice Architecture

- Features organized by business capability
- Each slice contains all necessary components
- Minimal dependencies between slices
- Clear separation of concerns

### Multi-Tenant Architecture

- Academy-based data isolation
- Automatic tenant scoping
- Secure cross-academy operations

### Service Layer Pattern

- Business logic in service classes
- Consistent error handling
- Transaction management
- Clear input/output contracts

### API-First Design

- RESTful endpoints
- JSON API responses
- Comprehensive serialization
- OpenAPI documentation

This structure supports:

- **Scalability** - Clear boundaries and separation
- **Maintainability** - Organized by business domain
- **Testability** - Comprehensive test coverage
- **Developer Experience** - Clear conventions and documentation
