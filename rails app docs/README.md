# Diquis - Football Academy Management System

> A comprehensive Rails 8.0+ API for managing football academies, players, teams, and training sessions
> with multi-tenant architecture.

[![CI Status](https://github.com/erickcastrillo/Diquis/workflows/CI/badge.svg)](https://github.com/erickcastrillo/Diquis/actions/workflows/ci.yml)
[![Ruby Version](https://img.shields.io/badge/ruby-3.4.5-red.svg)](https://www.ruby-lang.org/)
[![Rails Version](https://img.shields.io/badge/rails-8.0.3-red.svg)](https://rubyonrails.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15+-blue.svg)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7.0-red.svg)](https://redis.io/)

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Quick Start](#quick-start)
- [Docker Development](#docker-development)
- [Documentation](#documentation)
- [Technology Stack](#technology-stack)
- [Project Status](#project-status)

## 🎯 Overview

Diquis is a modern football academy management system built with Rails 8.0+ API-only architecture.
It implements **Vertical Slice Architecture** with complete multi-tenant isolation, allowing multiple
academies to operate independently within a single application.

The system manages:

- 🏫 **Academy Operations** - Complete academy administration
- ⚽ **Player Management** - Player registration, profiles, and skill tracking
- 👥 **Team Organization** - Team rosters and memberships
- 📅 **Training Sessions** - Scheduling and attendance tracking
- 📊 **Analytics & Reporting** - Performance metrics, financial reports, and business intelligence
- 🎽 **Asset Management** - Equipment, uniforms, and inventory tracking
- 💬 **Communication System** - Multi-channel messaging and parent portal
- 🏥 **Health Management** - Medical records and injury tracking

## ✨ Features

### Multi-Tenant Architecture

- **Complete Data Isolation** - Each academy's data is fully isolated
- **Hybrid Context Management** - URL-based, header-based, and user-default academy context
- **ActsAsTenant Integration** - Automatic tenant scoping for all queries
- **Cross-Academy Administration** - System admin access across all academies

### Advanced Player Management

- Player registration with parent/guardian information
- Age-category validation (U-8, U-10, U-12, etc.)
- Position and skill assignments
- Player search and filtering
- Image upload support (Active Storage)
- Encrypted sensitive data (Rails 8.0 encryption)

### Training & Attendance

- Training session scheduling with conflict detection
- Bulk attendance tracking for entire teams
- Training types: Technical, Tactical, Physical, Fitness, Scrimmage
- Automated reminder notifications (24h and 2h before)
- Real-time updates via WebSocket (Solid Cable)
- Attendance reports and analytics

### API Features

- **RESTful Design** - Clean, predictable API endpoints
- **JWT Authentication** - Secure token-based auth with Devise
- **Role-Based Authorization** - Pundit policies for fine-grained access control
- **Comprehensive Serialization** - Active Model Serializers with relationship inclusion
- **Pagination** - Efficient data pagination with metadata
- **OpenAPI Documentation** - Interactive Swagger UI at `/api-docs`
- **API Versioning** - Future-proof with v1, v2 namespaces

### Developer Experience

- **CI/CD Pipeline** - Automated testing, linting, and security scanning on every PR
- **Overmind Process Management** - Single command to start all services
- **Comprehensive Testing** - RSpec with FactoryBot and Faker
- **Code Quality Tools** - Rubocop, Brakeman, Bullet
- **Background Jobs** - Sidekiq with Redis
- **Development Documentation** - Complete guides and examples

## 🏗️ Architecture

Diquis implements **Vertical Slice Architecture** where features are organized by business capability:

```text
app/slices/
├── academy_management/     # Academy CRUD and settings
├── player_management/      # Player registration and profiles
├── team_management/        # Team organization and rosters
├── training_management/    # Training scheduling and attendance
└── shared_resources/       # Positions, skills, categories
```text

Each slice contains:

- **Controllers** - HTTP interface
- **Services** - Business logic (Service Layer Pattern)
- **Models** - Domain models
- **Serializers** - JSON representation
- **Policies** - Authorization rules
- **Jobs** - Background processing

### Key Design Patterns

1. **Service Layer Pattern** - All business logic in service classes
2. **Multi-Tenant Pattern** - Academy-based data isolation
3. **Repository Pattern** - Services handle all data access
4. **Policy Pattern** - Pundit for authorization
5. **Serializer Pattern** - Active Model Serializers for consistent API responses

## 🚀 Quick Start

### Prerequisites

- Ruby 3.3.0+
- Rails 8.0.3+
- PostgreSQL 15+
- Redis 7.0+
- Overmind or Foreman

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/yourusername/diquis.git
cd diquis

# 2. Configure environment (optional - has sensible defaults)
cp .env.example .env
# Edit .env if you need custom configuration

# 3. Install dependencies
bundle install

# 4. Setup database
rails db:create db:migrate db:seed

# 5. Start development servers
./bin/dev
```text

### Access Points

- **API:** http://localhost:3000
- **API Documentation:** http://localhost:3000/api-docs
- **Sidekiq Dashboard:** http://localhost:3000/sidekiq (Job monitoring, management, and cron jobs)

### VS Code Debugging

For full debugging support in VS Code:

1. Press `F5` to open debug configurations
2. Select "🚀 Debug bin/dev with Remote Attach" to start with debugging
3. Or select "🔧 Debug Rails Server" for Rails-only debugging
4. Set breakpoints and debug your code

See [🐛 VS Code Debug Setup](./.vscode/DEBUG_SETUP.md) for complete setup guide.

### Environment Configuration

The application uses a **single `.env` file** for Docker development:

```bash
# Copy the example file
cp .env.example .env
```

**How it works:**

- Docker Compose automatically loads `.env` file
- All services (web, sidekiq) share the same environment
- For local (non-Docker) development, set variables in your shell

**What you can configure:**

- `SEED_DEFAULT_PASSWORD` - Default password for development seed data
- OpenTelemetry/Honeycomb.io observability (optional)
- External services (SMTP, etc.)
- Database/Redis settings (usually not needed, Docker defaults work)

**Files:**

- `.env` - Your settings (gitignored, **never commit**)
- `.env.example` - Template with all options (committed)

See [📝 Seed Data Guide](./docs/SEED_DATA.md) for details.

### Quick API Test

```bash
# Register a user
curl -X POST http://localhost:3000/auth/sign_up \
  -H "Content-Type: application/json" \
  -d '{"user":{"email":"test@example.com","password":"password123","password_confirmation":"password123"}}'

# Login and get token
curl -X POST http://localhost:3000/auth/sign_in \
  -H "Content-Type: application/json" \
  -d '{"user":{"email":"test@example.com","password":"password123"}}'

# Use token to access API
curl http://localhost:3000/api/v1/academies \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## 🐳 Docker Development

For a complete containerized development environment with PostgreSQL and Redis:

### Quick Start

```bash
# Start all services
./docker-dev

# Reset everything (useful after installing new gems)
./docker-dev reset
```

### All Docker Commands

```bash
# Start all services (default)
./docker-dev
./docker-dev up

# Stop all services
./docker-dev down

# Restart all services
./docker-dev restart

# Reset environment (rebuild from scratch)
./docker-dev reset

# Check service status
./docker-dev status

# Follow logs for all services
./docker-dev logs

# Show available commands
./docker-dev help
```

### Manual Docker Setup

```bash
# 1. Copy environment configuration
cp .env.example .env

# 2. Start all services
./docker-dev

# 3. Setup database (if needed after reset)
docker compose exec web bundle exec rails db:migrate
docker compose exec web bundle exec rails db:seed
```

### When to Use Reset

Use `./docker-dev reset` when:

- You've installed new gems (Gemfile.lock changed)
- Docker containers are in a broken state
- You need a completely fresh environment
- Database schema changes aren't applying

**⚠️ Warning:** Reset will delete all database data and require confirmation!

### Docker Services

- **Rails App:** http://localhost:3000 (includes Vite dev server via bin/dev)
- **Sidekiq Web UI:** http://localhost:4567
- **PostgreSQL:** localhost:5432
- **Redis:** localhost:6379

### Additional Docker Commands

```bash
# Rails console
docker compose exec web bundle exec rails console

# Database operations
docker compose exec web bundle exec rails db:migrate

# Individual service management
docker compose stop web    # Stop specific service
docker compose start web   # Start specific service
docker compose logs web    # View specific service logs
```

📚 **Complete Docker Guide:** [docs/DOCKER_SETUP.md](docs/DOCKER_SETUP.md)

## 📚 Documentation

Comprehensive documentation is available in the `/docs` directory:

### Core Documentation

| Document | Description |
|----------|-------------|
| [📖 PROJECT_OVERVIEW.md](./docs/PROJECT_OVERVIEW.md) | High-level project overview, technology stack, and architecture principles |
| [🏛️ ARCHITECTURE.md](./docs/ARCHITECTURE.md) | Detailed architecture documentation including vertical slices, multi-tenancy, service layer, and data models |
| [🏗️ INFRASTRUCTURE_SETUP.md](./docs/INFRASTRUCTURE_SETUP.md) | **NEW** - Comprehensive infrastructure setup guide (authentication, authorization, jobs, CI/CD, deployment) |
| [🔧 SETUP_GUIDE.md](./docs/SETUP_GUIDE.md) | Complete installation and configuration guide for development environment |
| [🌐 API_DOCUMENTATION.md](./docs/API_DOCUMENTATION.md) | Comprehensive API reference with endpoints, request/response examples, and error handling |
| [👨‍💻 DEVELOPMENT_GUIDE.md](./docs/DEVELOPMENT_GUIDE.md) | Development workflows, coding standards, testing guidelines, and best practices |
| [🆔 UUID_AND_MULTITENANCY.md](./docs/UUID_AND_MULTITENANCY.md) | UUID primary keys and ActsAsTenant multi-tenancy implementation guide |
| [🧱 SLICE_ARCHITECTURE.md](./docs/SLICE_ARCHITECTURE.md) | **NEW** - Complete slice-based architecture guide with generators, best practices, and examples |
| [⚡ SLICE_GENERATORS.md](./docs/SLICE_GENERATORS.md) | **NEW** - Quick reference for slice generator commands and usage |
| [📏 CODING_STANDARDS.md](./docs/CODING_STANDARDS.md) | **NEW** - Comprehensive coding standards and linting guidelines for all languages |
| [🤖 Copilot Configuration](./.copilot/README.md) | **NEW** - GitHub Copilot optimization for project-specific code suggestions |
| [🐛 VS Code Debug Setup](./.vscode/DEBUG_SETUP.md) | **NEW** - Complete debugging configuration for Rails and React |
| [🔄 Sidekiq Setup](./docs/SIDEKIQ_SETUP.md) | **NEW** - Background job processing with Sidekiq and cron jobs |
| [🧪 E2E Testing Plan](./docs/E2E_TESTING_IMPLEMENTATION_PLAN.md) | **NEW** - Complete Playwright E2E testing guide and implementation |
| [🤖 AI Assistant Context](./GEMINI.md) | **NEW** - Comprehensive context for AI assistants (Copilot, Claude, ChatGPT, Gemini) |

### Quick Links

**Getting Started:**

- [Installation Prerequisites](./docs/SETUP_GUIDE.md#prerequisites)
- [Project Initialization](./docs/SETUP_GUIDE.md#project-initialization)
- [Environment Configuration](./docs/SETUP_GUIDE.md#environment-variables)

**Architecture Deep Dive:**

- [Vertical Slice Architecture](./docs/ARCHITECTURE.md#vertical-slice-architecture)
- [Multi-Tenancy Implementation](./docs/ARCHITECTURE.md#multi-tenancy-architecture)
- [Service Layer Pattern](./docs/ARCHITECTURE.md#service-layer-pattern)
- [Data Model Overview](./docs/ARCHITECTURE.md#data-model)

**Infrastructure:**

- [Authentication Setup (Devise + JWT)](./docs/INFRASTRUCTURE_SETUP.md#authentication-setup)
- [Authorization Setup (Pundit)](./docs/INFRASTRUCTURE_SETUP.md#authorization-setup)
- [Background Jobs (Sidekiq)](./docs/INFRASTRUCTURE_SETUP.md#background-jobs-setup)
- [Multi-Tenancy (ActsAsTenant)](./docs/INFRASTRUCTURE_SETUP.md#multi-tenancy-setup)
- [CI/CD Pipeline](./docs/INFRASTRUCTURE_SETUP.md#cicd-pipeline)
- [Deployment (Kamal)](./docs/INFRASTRUCTURE_SETUP.md#deployment)

**API Usage:**

- [Authentication](./docs/API_DOCUMENTATION.md#authentication)
- [Academy Management API](./docs/API_DOCUMENTATION.md#academy-management-api)
- [Player Management API](./docs/API_DOCUMENTATION.md#player-management-api)
- [Training Management API](./docs/API_DOCUMENTATION.md#training-management-api)

**Development:**

- [Development Workflow](./docs/DEVELOPMENT_GUIDE.md#development-workflow)
- [Creating Services](./docs/DEVELOPMENT_GUIDE.md#service-layer-development)
- [Writing Tests](./docs/DEVELOPMENT_GUIDE.md#testing-guidelines)
- [Code Style Guide](./docs/DEVELOPMENT_GUIDE.md#code-style-guidelines)
- [Coding Standards](./docs/CODING_STANDARDS.md) - Comprehensive linting and style guidelines
- [Copilot Configuration](./.copilot/README.md) - GitHub Copilot optimization and patterns
- [VS Code Debugging](./.vscode/DEBUG_SETUP.md) - Complete debugging setup for full-stack development
- [Sidekiq Setup](./docs/SIDEKIQ_SETUP.md) - Background job processing configuration and usage
- [Slice Architecture](./docs/SLICE_ARCHITECTURE.md) - Complete guide to slice-based development
- [Slice Generators](./docs/SLICE_GENERATORS.md) - Quick reference for generator commands
- [E2E Testing Guide](./docs/E2E_TESTING_IMPLEMENTATION_PLAN.md) - Playwright testing setup and patterns
- [AI Assistant Context](./GEMINI.md) - Context for AI assistants (Copilot, Claude, ChatGPT, Gemini)

## 🛠️ Technology Stack

### Backend

- **Framework:** Ruby on Rails 8.0.3+ (API-only)
- **Language:** Ruby 3.3.0
- **Database:** PostgreSQL 15+ with UUID primary keys
- **Multi-Tenancy:** ActsAsTenant
- **Authentication:** Devise + JWT
- **Authorization:** Pundit
- **Background Jobs:** Sidekiq
- **Caching:** Redis
- **File Storage:** Active Storage (S3/GCS)
- **Real-time:** Solid Cable (WebSocket)

### Frontend (Separate Repository)

- **Framework:** React 18+
- **State:** Redux Toolkit
- **Data Fetching:** TanStack Query (React Query)
- **Build:** Vite
- **Styling:** Tailwind CSS

### Development Tools

- **Process Manager:** Overmind
- **Testing:** RSpec + FactoryBot + Faker (Backend), Vitest (Frontend), Playwright (E2E)
- **API Docs:** Rswag (Swagger/OpenAPI 3.0)
- **Code Quality:** Rubocop, Brakeman, Bullet
- **Container:** Docker + Kamal 2 (deployment)

## 📁 Project Structure

```text
diquis/
├── app/
│   ├── controllers/
│   │   ├── api/v1/base_controller.rb
│   │   └── application_controller.rb
│   ├── slices/                      # Vertical slices architecture
│   │   ├── academy_management/      # Academy CRUD and settings
│   │   │   ├── controllers/
│   │   │   ├── services/
│   │   │   ├── models/
│   │   │   ├── serializers/
│   │   │   ├── policies/
│   │   │   ├── validators/
│   │   │   └── jobs/
│   │   ├── player_management/       # Player registration and profiles
│   │   ├── team_management/         # Team organization and rosters
│   │   ├── training_management/     # Training scheduling and attendance
│   │   ├── shared_resources/        # Positions, skills, categories
│   │   ├── asset_management/        # Equipment and inventory tracking
│   │   ├── reporting_analytics/     # Business intelligence and reports
│   │   └── communication_notification/ # Multi-channel messaging
│   ├── shared/                      # Cross-cutting concerns
│   │   ├── services/                # BaseService and shared utilities
│   │   ├── concerns/                # Model/Controller concerns
│   │   ├── policies/                # ApplicationPolicy base
│   │   ├── validators/              # Custom validation classes
│   │   └── serializers/             # ApplicationSerializer base
│   ├── controllers/                 # Traditional Rails controllers
│   ├── models/                      # Traditional Rails models
│   ├── jobs/                        # Background jobs
│   ├── mailers/                     # Email templates
│   └── views/                       # View templates (minimal for API)
├── config/
│   ├── application.rb
│   ├── routes.rb
│   └── database.yml
├── db/
│   ├── migrate/
│   └── seeds.rb
├── docs/                            # 📚 Comprehensive documentation
│   ├── PROJECT_OVERVIEW.md
│   ├── ARCHITECTURE.md
│   ├── SETUP_GUIDE.md
│   ├── API_DOCUMENTATION.md
│   └── DEVELOPMENT_GUIDE.md
├── spec/                            # RSpec tests
│   ├── factories/
│   ├── requests/
│   ├── services/
│   └── models/
├── swagger/                         # OpenAPI specs
├── Procfile.dev                     # Overmind configuration
├── Gemfile
└── README.md
```text

## 🧪 Testing

### Backend Tests (RSpec)

```bash
# Run all tests
bundle exec rspec

# Run specific test file
bundle exec rspec spec/slices/player_management/services/player_registration_service_spec.rb

# Run tests with coverage
COVERAGE=true bundle exec rspec

# Run tests in parallel
bundle exec parallel_rspec spec/
```

### Frontend Tests (Vitest)

```bash
# Run all frontend unit tests
npm run test

# Run tests in watch mode
npm run test:ui

# Run tests with coverage
npm run test:coverage
```

### E2E Tests (Playwright)

```bash
# Run all E2E tests
npm run e2e

# Run in interactive UI mode
npm run e2e:ui

# Run in debug mode (step through tests)
npm run e2e:debug

# Run with visible browser
npm run e2e:headed

# View HTML test report
npm run e2e:report

# Run specific test file
npm run e2e e2e/auth/login.spec.ts
```

**E2E Test Status:**

- ✅ 14 tests passing (critical user flows)
- ⏭️ 39 tests skipped (session timing issues in Docker - requires architectural fix)
- ❌ 0 tests failing

See [E2E Testing Implementation Plan](./docs/E2E_TESTING_IMPLEMENTATION_PLAN.md) for complete documentation.text

## 🔒 Security

- **JWT Authentication** - Secure token-based authentication
- **Role-Based Access Control** - Fine-grained permissions with Pundit
- **Data Encryption** - Rails 8.0 Active Record encryption for sensitive data
- **SQL Injection Prevention** - Parameterized queries via ActiveRecord
- **CORS Configuration** - Controlled cross-origin access
- **Security Scanning** - Brakeman for vulnerability detection

## 🚢 Deployment

Diquis uses **Kamal 2** for zero-downtime deployments:

```bash
# Setup deployment
kamal setup

# Deploy application
kamal deploy

# Check status
kamal app logs
```text

## 📊 Project Status

### Current Implementation

✅ Project structure and documentation
✅ Architecture design and patterns
✅ Service layer foundation
✅ Multi-tenancy configuration (ActsAsTenant)
✅ UUID primary keys enabled
✅ API design and routing
✅ Authentication setup (Devise + JWT)
✅ Authorization setup (Pundit)
✅ Testing framework (RSpec + Vitest + Playwright)
✅ Background jobs (Sidekiq + Sidekiq-Cron)
✅ E2E testing with Playwright (14 passing tests)
✅ User management slice
✅ Docker development environment
✅ CI/CD pipeline

### In Progress

🔄 Player management slice
🔄 Team management slice
🔄 Training management slice
🔄 Session isolation for E2E tests (39 tests currently skipped)

### Planned

📋 Frontend React application
📋 WebSocket real-time features
📋 Email notification system
📋 Report generation
📋 Analytics dashboard
📋 Mobile app integration

### Implementation Phases

For detailed implementation guides:

- [Phase 0: Project Foundation](./docs/PHASE_0_SETUP.md) - Initial setup and configuration
- [Phase 1: Core Infrastructure](./docs/PHASE_1_INFRASTRUCTURE.md) - Authentication, authorization, jobs, multi-tenancy
- [Implementation Phases Overview](./docs/IMPLEMENTATION_PHASES.md) - Complete phased development plan

## 🤝 Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Write tests for your changes
4. Commit your changes (`git commit -m 'Add amazing feature'`)
5. Push to the branch (`git push origin feature/amazing-feature`)
6. Open a Pull Request

See [DEVELOPMENT_GUIDE.md](./docs/DEVELOPMENT_GUIDE.md) for coding standards and workflows.

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👥 Team

- **Development Team:** dev@diquis.com
- **Project Lead:** [Your Name]
- **Architecture:** Based on Django Diquis reference implementation

## 🙏 Acknowledgments

- Based on the architectural patterns from the Django Diquis project
- Built with Ruby on Rails 8.0+ modern features
- Inspired by Domain-Driven Design and Vertical Slice Architecture principles

## 📞 Support

- **Documentation:** [docs/](./docs/)
- **Issues:** [GitHub Issues](https://github.com/yourusername/diquis/issues)
- **Email:** dev@diquis.com

---

## Built with ❤️ for football academies worldwide

For detailed information, start with the [Project Overview](./docs/PROJECT_OVERVIEW.md) or jump directly
to the [Setup Guide](./docs/SETUP_GUIDE.md) to begin development.
# Quality control system installed
