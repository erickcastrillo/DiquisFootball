# Diquis - Phased Implementation Plan

## Overview

This document outlines a systematic, phase-by-phase approach to implementing the Diquis Rails API. Each phase builds upon the previous one, with clear deliverables and verification steps.

---

## 📋 Implementation Phases

### Phase 0: Project Foundation (Week 1)

**Goal:** Set up development environment and project structure

### Phase 1: Core Infrastructure (Week 1-2)

**Goal:** Implement base classes, multi-tenancy, and authentication

### Phase 2: Academy Management (Week 2-3)

**Goal:** Complete academy CRUD and user management

### Phase 3: Player Management (Week 3-4)

**Goal:** Player registration, profiles, and search

### Phase 4: Team Management (Week 4-5)

**Goal:** Team organization and player assignments

### Phase 5: Training Management (Week 5-6)

**Goal:** Training scheduling and attendance tracking

### Phase 6: Shared Resources (Week 6)

**Goal:** Positions, skills, categories, and divisions

### Phase 7: Testing & Documentation (Week 7)

**Goal:** Comprehensive test coverage and API documentation

### Phase 8: Production Readiness (Week 8)

**Goal:** Performance optimization and deployment

### Phase 9: Asset Management (Week 9-10)

**Goal:** Implement comprehensive asset and inventory tracking system

### Phase 10: Reporting & Analytics (Week 11-12)

**Goal:** Implement financial reporting and business intelligence features

### Phase 11: Communication System (Week 13-14)

**Goal:** Implement multi-channel communication and parent portal

### Phase 12: Advanced Features (Week 15-16)

**Goal:** Implement health management, events, and additional modules

---

## Phase 0: Project Foundation

### Objectives

- ✅ Install all required dependencies
- ✅ Create Rails application structure
- ✅ Configure database and Redis
- ✅ Set up development tools
- ✅ Create documentation structure

### Tasks

#### 0.1: Environment Setup

```bash
# Install Ruby and Rails
rbenv install 3.3.0
rbenv global 3.3.0
gem install rails -v 8.0.3

# Install system dependencies
# macOS
brew install postgresql@15 redis imagemagick tmux overmind

# Ubuntu/Debian
sudo apt install postgresql-15 redis-server imagemagick tmux
```text

#### 0.2: Create Rails Application

```bash
# Create new Rails API app
rails new diquis --api --database=postgresql --skip-test -T

cd diquis
```text

#### 0.3: Configure Gemfile

Create `Gemfile` with all required gems (see [PHASE_0_SETUP.md](./PHASE_0_SETUP.md))

#### 0.4: Install Dependencies

```bash
bundle install
```text

#### 0.5: Initialize Testing Framework

```bash
# Install RSpec
rails generate rspec:install

# Install other tools
rails generate devise:install
rails generate pundit:install
rails generate rswag:install
```text

#### 0.6: Create Directory Structure

```bash
# Create slices directories
mkdir -p app/slices/{academy_management,player_management,team_management,training_management,shared_resources}/{controllers,services,models,serializers,policies,validators}

# Create shared directory
mkdir -p app/shared/{services,policies,models/concerns}

# Create jobs by slice
mkdir -p app/jobs/{academy_management,player_management,training_management}

# Create documentation
mkdir -p docs

# Create swagger directory
mkdir -p swagger/v1
```text

#### 0.7: Configure Application

- Edit `config/application.rb` with autoload paths and CORS
- Configure `config/database.yml`
- Set up `config/routes.rb` basic structure
- Create `.env` file with environment variables

#### 0.8: Set up Process Management

- Create `Procfile.dev`
- Create `bin/dev` script
- Create `.overmind.env`

#### 0.9: Initialize Git Repository

```bash
git init
git add .
git commit -m "Initial commit: Project foundation setup"
```text

### Deliverables

- [ ] Rails application created and running
- [ ] All gems installed
- [ ] Directory structure in place
- [ ] Development processes can start with `./bin/dev`
- [ ] Documentation structure created
- [ ] Git repository initialized

### Verification

```bash
# Start development servers
./bin/dev

# In another terminal, verify:
curl http://localhost:3000/health  # Should return OK
```text

---

## Phase 1: Core Infrastructure

### Objectives

- ✅ Implement base service class
- ✅ Implement base controller
- ✅ Set up multi-tenancy (ActsAsTenant)
- ✅ Configure authentication (Devise + JWT)
- ✅ Configure authorization (Pundit)
- ✅ Create model concerns (Sluggable, Auditable)
- ✅ Set up background jobs (Sidekiq)

### Tasks

#### 1.1: Database Setup

```bash
# Create databases
rails db:create

# Create initial migration for pgcrypto (UUID support)
rails generate migration EnablePgcrypto
```text

Edit migration:

```ruby
class EnablePgcrypto < ActiveRecord::Migration[8.0]
  def change
    enable_extension 'pgcrypto'
  end
end
```text

```bash
rails db:migrate
```text

#### 1.2: Create User Model (Devise)

```bash
# Generate Devise User model
rails generate devise User

# Add additional fields to User migration
rails generate migration AddFieldsToUsers \
  slug:uuid:uniq \
  first_name:string \
  last_name:string \
  is_system_admin:boolean
```text

#### 1.3: Implement Base Service Class

Create `app/shared/services/base_service.rb` (see [PHASE_1_INFRASTRUCTURE.md](./PHASE_1_INFRASTRUCTURE.md))

#### 1.4: Implement Base Controllers

- Create `app/controllers/application_controller.rb`
- Create `app/controllers/api/v1/base_controller.rb`

#### 1.5: Implement Model Concerns

- Create `app/models/concerns/sluggable.rb`
- Create `app/models/concerns/auditable.rb`

#### 1.6: Configure Multi-Tenancy

- Add ActsAsTenant configuration to `config/application.rb`
- Create tenant resolution logic in ApplicationController

#### 1.7: Configure JWT Authentication

- Install and configure devise-jwt
- Create authentication controllers
- Set up token generation and validation

#### 1.8: Configure Authorization

- Create `app/shared/policies/application_policy.rb`
- Set up Pundit in ApplicationController

#### 1.9: Configure Sidekiq

- Create `config/initializers/sidekiq.rb`
- Create `app/jobs/application_job.rb` with tenant support

#### 1.10: Create Base Serializer

- Create `app/serializers/application_serializer.rb`

### Deliverables

- [ ] Base service class with error handling
- [ ] Base controllers with tenant context
- [ ] User authentication working
- [ ] Authorization policies configured
- [ ] Model concerns (Sluggable, Auditable)
- [ ] Sidekiq running and configured
- [ ] Base serializer implemented

### Verification

```bash
# Run tests
bundle exec rspec spec/shared/

# Test authentication
curl -X POST http://localhost:3000/auth/sign_up \
  -H "Content-Type: application/json" \
  -d '{"user":{"email":"test@test.com","password":"password123"}}'
```text

---

## Phase 2: Academy Management

### Objectives

- ✅ Create Academy model (tenant)
- ✅ Implement academy CRUD operations
- ✅ Create academy services
- ✅ Implement academy serializers
- ✅ Add academy policies
- ✅ Create academy-user association
- ✅ Implement academy context switching

### Tasks

#### 2.1: Create Academy Model

```bash
rails generate model Academy \
  slug:uuid:uniq \
  name:string \
  description:text \
  owner_name:string \
  owner_email:string \
  owner_phone:string \
  address_line_1:string \
  address_line_2:string \
  city:string \
  state_province:string \
  postal_code:string \
  country:string \
  founded_date:date \
  website:string \
  is_active:boolean
```text

#### 2.2: Create AcademyUser Join Model

```bash
rails generate model AcademyUser \
  academy:references \
  user:references \
  role:string \
  is_active:boolean
```text

#### 2.3: Implement Academy Model

- Add validations
- Add associations
- Include concerns (Sluggable, Auditable)
- Add Active Storage for logo

#### 2.4: Create Academy Services

- `AcademyCreationService` - Create academy with defaults
- `AcademyFinderService` - Find and authorize academy access
- `AcademyUpdateService` - Update academy details

#### 2.5: Create Academy Controller

- `app/slices/academy_management/controllers/academies_controller.rb`
- Implement index, show, create, update actions

#### 2.6: Create Academy Serializer

- `app/slices/academy_management/serializers/academy_serializer.rb`
- Include logo URL, formatted dates

#### 2.7: Create Academy Policy

- `app/slices/academy_management/policies/academy_policy.rb`
- Define permissions for CRUD operations

#### 2.8: Add Routes

Update `config/routes.rb` with academy routes

#### 2.9: Create Tests

- Model specs
- Service specs
- Controller request specs
- Factory definitions

### Deliverables

- [ ] Academy model with validations
- [ ] Academy CRUD services
- [ ] Academy API endpoints
- [ ] Academy serializers
- [ ] Academy policies
- [ ] Comprehensive test suite
- [ ] Factory definitions

### Verification

```bash
# Run academy tests
bundle exec rspec spec/slices/academy_management/

# Test API
curl http://localhost:3000/api/v1/academies \
  -H "Authorization: Bearer $TOKEN"
```text

---

## Phase 3: Player Management

### Objectives

- ✅ Create Player model (tenant-scoped)
- ✅ Implement player registration
- ✅ Create player search functionality
- ✅ Implement player CRUD services
- ✅ Add player serializers
- ✅ Create player policies
- ✅ Implement image upload

### Tasks

#### 3.1: Create Player Model

```bash
rails generate model Player \
  slug:uuid:uniq \
  academy:references \
  first_name:string \
  last_name:string \
  age:integer \
  gender:string \
  foot:string \
  parent_name:string \
  parent_email:string \
  phone_number:string \
  position_id:bigint \
  category_id:bigint \
  is_active:boolean
```text

#### 3.2: Implement Player Model

- Add `acts_as_tenant :academy`
- Add validations
- Add associations
- Add scopes (by_age_range, by_gender, etc.)
- Add Active Storage for picture
- Add encrypted attributes (parent_email, phone_number)

#### 3.3: Create Player Services

- `PlayerRegistrationService` - Register new player with validations
- `PlayerFinderService` - Find player with authorization
- `PlayerSearchService` - Advanced search with filters
- `PlayerUpdateService` - Update player information

#### 3.4: Create Player Controller

- `app/slices/player_management/controllers/players_controller.rb`
- Implement all CRUD actions
- Add search action
- Add statistics action

#### 3.5: Create Player Serializer

- `app/slices/player_management/serializers/player_serializer.rb`
- Include relationships (position, category, teams)
- Add computed attributes (full_name, picture_url)

#### 3.6: Create Player Policy

- `app/slices/player_management/policies/player_policy.rb`
- Define academy-scoped permissions

#### 3.7: Create Background Jobs

- `PlayerRegistrationJob` - Send welcome email, create audit log

#### 3.8: Add Routes

Update routes with player endpoints (academy-scoped)

#### 3.9: Create Tests

- Model specs with tenant context
- Service specs for all operations
- Controller request specs
- Factory definitions

### Deliverables

- [ ] Player model with tenant scoping
- [ ] Player registration service
- [ ] Player search functionality
- [ ] Player API endpoints
- [ ] Player serializers
- [ ] Player policies
- [ ] Background jobs
- [ ] Comprehensive test suite

### Verification

```bash
# Run player tests
bundle exec rspec spec/slices/player_management/

# Test player registration
curl -X POST http://localhost:3000/api/v1/$ACADEMY_SLUG/players \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"player":{"first_name":"Test","last_name":"Player",...}}'
```text

---

## Phase 4: Team Management

### Objectives

- ✅ Create Team model (tenant-scoped)
- ✅ Create TeamMembership model
- ✅ Implement team CRUD operations
- ✅ Implement player assignment to teams
- ✅ Create team services
- ✅ Add team serializers

### Tasks

#### 4.1: Create Team Model

```bash
rails generate model Team \
  slug:uuid:uniq \
  academy:references \
  name:string \
  category_id:bigint \
  division_id:bigint \
  coach:string \
  is_active:boolean
```text

#### 4.2: Create TeamMembership Model

```bash
rails generate model TeamMembership \
  team:references \
  player:references \
  joined_at:datetime \
  is_active:boolean
```text

#### 4.3: Implement Team Models

- Add validations and associations
- Add scopes
- Include tenant scoping

#### 4.4: Create Team Services

- `TeamCreationService` - Create team with category/division
- `TeamRosterService` - Manage team membership
- `TeamFinderService` - Find team with authorization

#### 4.5: Create Team Controller

- Implement CRUD actions
- Add add_player action
- Add remove_player action

#### 4.6: Create Team Serializer

- Include player count
- Include category and division
- Optional player list inclusion

#### 4.7: Create Team Policy

- Define team management permissions

#### 4.8: Add Routes

Update routes with team endpoints

#### 4.9: Create Tests

- Model specs
- Service specs
- Controller specs
- Factory definitions

### Deliverables

- [ ] Team model with associations
- [ ] Team roster management
- [ ] Team API endpoints
- [ ] Team serializers
- [ ] Team policies
- [ ] Comprehensive test suite

### Verification

```bash
# Run team tests
bundle exec rspec spec/slices/team_management/

# Test team creation
curl -X POST http://localhost:3000/api/v1/$ACADEMY_SLUG/teams \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"team":{"name":"U-16 Team A",...}}'
```text

---

## Phase 5: Training Management

### Objectives

- ✅ Create Training model
- ✅ Create TrainingAttendance model
- ✅ Implement training scheduling
- ✅ Implement attendance tracking
- ✅ Create training services
- ✅ Add real-time updates (Solid Cable)
- ✅ Implement reminder notifications

### Tasks

#### 5.1: Create Training Model

```bash
rails generate model Training \
  slug:uuid:uniq \
  team:references \
  place:string \
  date:date \
  time:time \
  duration:interval \
  training_type:string \
  coach:string \
  description:text
```text

#### 5.2: Create TrainingAttendance Model

```bash
rails generate model TrainingAttendance \
  training:references \
  player:references \
  status:string \
  notes:text
```text

#### 5.3: Implement Training Models

- Add validations (conflict detection)
- Add associations
- Add scopes

#### 5.4: Create Training Services

- `TrainingSchedulingService` - Schedule with validations
- `AttendanceTrackingService` - Bulk attendance update
- `TrainingFinderService` - Find training with authorization

#### 5.5: Create Training Controller

- Implement CRUD actions
- Add bulk_attendance action
- Add attendance_report action
- Add calendar action

#### 5.6: Create Training Serializer

- Include attendance summary
- Include player list

#### 5.7: Create Training Channel (WebSocket)

- Implement real-time attendance updates
- Create TrainingChannel

#### 5.8: Create Background Jobs

- `TrainingReminderJob` - Send notifications

#### 5.9: Create Training Policy

- Define training management permissions

#### 5.10: Add Routes

Update routes with training endpoints (nested under teams)

#### 5.11: Create Tests

- Model specs with conflict detection
- Service specs
- Controller specs
- Channel specs
- Job specs

### Deliverables

- [ ] Training model with scheduling logic
- [ ] Attendance tracking system
- [ ] Training API endpoints
- [ ] Real-time updates via WebSocket
- [ ] Background reminder jobs
- [ ] Comprehensive test suite

### Verification

```bash
# Run training tests
bundle exec rspec spec/slices/training_management/

# Test training scheduling
curl -X POST http://localhost:3000/api/v1/$ACADEMY_SLUG/teams/$TEAM_SLUG/trainings \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"training":{...}}'

# Test bulk attendance
curl -X POST http://localhost:3000/api/v1/$ACADEMY_SLUG/teams/$TEAM_SLUG/trainings/$TRAINING_SLUG/bulk_attendance \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"attendances":[...]}'
```text

---

## Phase 6: Shared Resources

### Objectives

- ✅ Create Position model (global and academy-specific)
- ✅ Create Skill model
- ✅ Create Category model (global)
- ✅ Create Division model (global)
- ✅ Create PlayerSkill model
- ✅ Implement shared resource APIs

### Tasks

#### 6.1: Create Shared Models

```bash
# Position (can be academy-specific)
rails generate model Position \
  slug:uuid:uniq \
  academy_id:bigint \
  name:string \
  abbreviation:string \
  category:string \
  description:text \
  language:string

# Skill (can be academy-specific)
rails generate model Skill \
  slug:uuid:uniq \
  academy_id:bigint \
  name:string \
  description:text \
  category:string \
  language:string

# Category (global)
rails generate model Category \
  slug:uuid:uniq \
  name:string \
  language:string

# Division (global)
rails generate model Division \
  slug:uuid:uniq \
  name:string \
  language:string

# PlayerSkill (assessment)
rails generate model PlayerSkill \
  player:references \
  skill:references \
  level:integer \
  notes:text
```text

#### 6.2: Implement Models

- Add validations
- Add associations
- Add scopes for language filtering

#### 6.3: Create Shared Resource Controllers

- PositionsController
- SkillsController
- CategoriesController
- DivisionsController

#### 6.4: Create Serializers

- Include player counts
- Add conditional attributes

#### 6.5: Create Seed Data

- Default positions
- Default skills
- Standard categories (U-8, U-10, etc.)
- Common divisions

#### 6.6: Add Routes

Update routes with shared resource endpoints

#### 6.7: Create Tests

- Model specs
- Controller specs
- Factory definitions

### Deliverables

- [ ] Shared resource models
- [ ] Shared resource APIs
- [ ] Seed data for defaults
- [ ] Serializers
- [ ] Test suite

### Verification

```bash
# Run shared resource tests
bundle exec rspec spec/slices/shared_resources/

# Test categories endpoint
curl http://localhost:3000/api/v1/categories \
  -H "Authorization: Bearer $TOKEN"
```text

---

## Phase 7: Testing & Documentation

### Objectives

- ✅ Achieve 90%+ test coverage
- ✅ Generate API documentation (Swagger)
- ✅ Create integration tests
- ✅ Perform security audit
- ✅ Add performance tests

### Tasks

#### 7.1: Complete Test Coverage

- Write missing model specs
- Write missing service specs
- Write missing controller specs
- Add integration tests
- Add API documentation tests (rswag)

#### 7.2: Generate Swagger Documentation

```bash
# Generate Swagger specs
bundle exec rake rswag:specs:swaggerize

# Verify at http://localhost:3000/api-docs
```text

#### 7.3: Security Audit

```bash
# Run Brakeman security scanner
bundle exec brakeman

# Fix any security issues
```text

#### 7.4: Performance Testing

- Identify N+1 queries with Bullet
- Add database indexes
- Optimize slow queries
- Add query result caching

#### 7.5: Update Documentation

- Review and update all docs files
- Add code examples
- Update API documentation
- Create troubleshooting guide

### Deliverables

- [ ] 90%+ test coverage
- [ ] Complete Swagger documentation
- [ ] Security audit passed
- [ ] Performance optimizations
- [ ] Updated documentation

### Verification

```bash
# Run full test suite with coverage
COVERAGE=true bundle exec rspec

# Check coverage report
open coverage/index.html

# Run security scan
bundle exec brakeman

# Check API docs
open http://localhost:3000/api-docs
```text

---

## Phase 8: Production Readiness

### Objectives

- ✅ Configure production environment
- ✅ Set up error monitoring
- ✅ Configure logging
- ✅ Set up CI/CD pipeline
- ✅ Create deployment scripts
- ✅ Perform load testing

### Tasks

#### 8.1: Production Configuration

- Configure production database
- Set up Redis production instance
- Configure Active Storage for S3/GCS
- Set up environment variables

#### 8.2: Error Monitoring

- Install and configure error tracking (Sentry/Honeybadger)
- Set up alerting

#### 8.3: Logging

- Configure structured logging
- Set up log aggregation
- Configure log rotation

#### 8.4: CI/CD Pipeline

Create `.github/workflows/ci.yml`:

```yaml
name: CI

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    services:
      postgres:
        image: postgres:15
        env:
          POSTGRES_PASSWORD: postgres
        ports:
          - 5432:5432
      
      redis:
        image: redis:7
        ports:
          - 6379:6379
    
    steps:
      - uses: actions/checkout@v3
      - uses: ruby/setup-ruby@v1
        with:
          ruby-version: 3.3.0
          bundler-cache: true
      
      - name: Setup database
        run: |
          bundle exec rails db:create db:migrate
        env:
          DATABASE_URL: postgresql://postgres:postgres@localhost:5432/test
          RAILS_ENV: test
      
      - name: Run tests
        run: bundle exec rspec
        env:
          DATABASE_URL: postgresql://postgres:postgres@localhost:5432/test
          REDIS_URL: redis://localhost:6379/0
          RAILS_ENV: test
      
      - name: Security audit
        run: bundle exec brakeman --no-pager
      
      - name: Code style
        run: bundle exec rubocop
```text

#### 8.5: Deployment Configuration

- Set up Kamal 2 configuration
- Create deployment scripts
- Configure SSL/TLS
- Set up domain and DNS

#### 8.6: Load Testing

- Create load test scenarios
- Run performance tests
- Optimize bottlenecks

#### 8.7: Monitoring

- Set up application monitoring (New Relic/DataDog)
- Configure uptime monitoring
- Set up database monitoring

#### 8.8: Backup Strategy

- Configure database backups
- Test restore procedures
- Document disaster recovery plan

### Deliverables

- [ ] Production environment configured
- [ ] Error monitoring active
- [ ] CI/CD pipeline working
- [ ] Deployment process documented
- [ ] Load testing completed
- [ ] Monitoring dashboards set up
- [ ] Backup strategy in place

### Verification

```bash
# Deploy to staging
kamal deploy -d staging

# Run smoke tests
curl https://staging.diquis.com/health

# Deploy to production
kamal deploy -d production

# Verify production
curl https://api.diquis.com/health
```text

---

## Phase 9: Asset Management

### Objectives

- ✅ Create Asset models with categorization
- ✅ Implement asset allocation system
- ✅ Create inventory management for consumables
- ✅ Implement maintenance tracking
- ✅ Add asset depreciation calculations
- ✅ Create asset API endpoints

### Tasks

#### 9.1: Create Asset Models

```bash
# Asset model
rails generate model Asset \
  academy:references \
  slug:uuid:uniq \
  name:string \
  description:text \
  asset_category_id:bigint \
  brand:string \
  model:string \
  serial_number:string \
  barcode:string \
  purchase_price:decimal \
  purchase_date:date \
  vendor:string \
  warranty_expires_at:date \
  current_value:decimal \
  condition:string \
  location:string \
  notes:text \
  is_active:boolean

# Asset Category model
rails generate model AssetCategory \
  academy_id:bigint \
  slug:uuid:uniq \
  name:string \
  description:text \
  parent_category_id:bigint \
  depreciation_rate:decimal \
  expected_lifespan_years:integer \
  requires_maintenance:boolean \
  is_consumable:boolean

# Asset Allocation model
rails generate model AssetAllocation \
  academy:references \
  asset:references \
  allocatable_type:string \
  allocatable_id:integer \
  allocated_at:datetime \
  expected_return_at:datetime \
  returned_at:datetime \
  condition_at_checkout:string \
  condition_at_return:string \
  checkout_notes:text \
  return_notes:text \
  allocated_by_user_id:bigint \
  returned_to_user_id:bigint

# Asset Maintenance Record model
rails generate model AssetMaintenanceRecord \
  academy:references \
  asset:references \
  maintenance_type:string \
  description:text \
  performed_at:datetime \
  performed_by:string \
  cost:decimal \
  parts_replaced:text \
  next_maintenance_due:date \
  warranty_claim:boolean \
  service_provider:string \
  invoice_number:string \
  notes:text \
  created_by_user_id:bigint

# Asset Inventory model
rails generate model AssetInventory \
  academy:references \
  asset_category:references \
  item_name:string \
  description:text \
  current_stock:integer \
  minimum_stock:integer \
  unit_cost:decimal \
  supplier:string \
  last_restocked_at:datetime \
  location:string
```text

#### 9.2: Implement Asset Services

- `AssetManagementService` - Asset CRUD with business logic
- `AssetAllocationService` - Check-out/check-in workflow
- `AssetMaintenanceService` - Maintenance scheduling and tracking
- `InventoryManagementService` - Stock level management

#### 9.3: Create Asset Controllers

- AssetsController with full CRUD operations
- AssetCategoriesController
- Asset allocation and maintenance endpoints

#### 9.4: Create Asset Serializers

- Include category, allocation status, maintenance history
- Asset valuation with depreciation calculations

#### 9.5: Create Background Jobs

- `AssetMaintenanceReminderJob`
- `AssetReturnReminderJob`
- `AssetDepreciationJob`

#### 9.6: Add Routes

Update routes with asset management endpoints

#### 9.7: Create Tests

- Model specs with business logic
- Service specs for all operations
- Controller specs
- Job specs

### Deliverables

- [ ] Asset management system operational
- [ ] Asset allocation tracking
- [ ] Inventory management for consumables
- [ ] Maintenance scheduling system
- [ ] Asset API endpoints
- [ ] Background jobs for automation
- [ ] Comprehensive test suite

---

## Phase 10: Reporting & Analytics

### Objectives

- ✅ Create financial transaction tracking
- ✅ Implement report generation system
- ✅ Create business intelligence analytics
- ✅ Add player development analytics
- ✅ Implement scheduled reporting
- ✅ Create dashboard API endpoints

### Tasks

#### 10.1: Create Reporting Models

```bash
# Report model
rails generate model Report \
  academy_id:bigint \
  slug:uuid:uniq \
  name:string \
  description:text \
  report_type:string \
  report_category:string \
  parameters:jsonb \
  is_scheduled:boolean \
  schedule_frequency:string \
  last_generated_at:datetime \
  next_generation_at:datetime \
  is_active:boolean \
  created_by_user_id:bigint

# Report Generation model
rails generate model ReportGeneration \
  academy_id:bigint \
  report:references \
  slug:uuid:uniq \
  generated_at:datetime \
  date_range_start:date \
  date_range_end:date \
  parameters_used:jsonb \
  status:string \
  file_url:string \
  file_format:string \
  file_size_bytes:bigint \
  generation_time_seconds:decimal \
  error_message:text \
  generated_by_user_id:bigint \
  expires_at:datetime

# Financial Transaction model
rails generate model FinancialTransaction \
  academy:references \
  slug:uuid:uniq \
  transaction_type:string \
  category:string \
  subcategory:string \
  amount:decimal \
  currency:string \
  description:text \
  transaction_date:date \
  payment_method:string \
  reference_number:string \
  player_id:bigint \
  team_id:bigint \
  asset_id:bigint \
  vendor_supplier:string \
  notes:text \
  is_recurring:boolean \
  parent_transaction_id:bigint \
  created_by_user_id:bigint

# Player Metric model
rails generate model PlayerMetric \
  academy:references \
  player:references \
  metric_type:string \
  metric_value:decimal \
  metric_date:date \
  season:string \
  notes:text \
  calculated_at:datetime
```text

#### 10.2: Implement Reporting Services

- `ReportGenerationService` - Generate reports in multiple formats
- `FinancialAnalyticsService` - P&L, cash flow, budget analysis
- `PlayerAnalyticsService` - Development tracking, performance metrics
- `BusinessIntelligenceService` - KPIs, benchmarking, projections

#### 10.3: Create Reporting Controllers

- ReportsController with generation endpoints
- AnalyticsController for dashboard data
- Financial analytics endpoints

#### 10.4: Create Report Templates

- PDF report templates for financial reports
- Excel templates with charts and calculations
- Dashboard JSON data formatters

#### 10.5: Create Background Jobs

- `ScheduledReportGenerationJob`
- `ReportCleanupJob`
- `MetricsCalculationJob`

#### 10.6: Add Routes

Update routes with reporting and analytics endpoints

#### 10.7: Create Tests

- Report generation testing
- Financial calculation accuracy tests
- Analytics data validation tests

### Deliverables

- [ ] Financial reporting system
- [ ] Player analytics capabilities
- [ ] Business intelligence dashboards
- [ ] Scheduled report generation
- [ ] Report API endpoints
- [ ] Analytics background jobs
- [ ] Comprehensive test suite

---

## Phase 11: Communication System

### Objectives

- ✅ Create multi-channel messaging system
- ✅ Implement parent portal
- ✅ Add push notification support
- ✅ Create message templates and scheduling
- ✅ Implement emergency alert system
- ✅ Add message delivery tracking

### Tasks

#### 11.1: Create Communication Models

```bash
# Message model
rails generate model Message \
  academy:references \
  slug:uuid:uniq \
  sender_id:bigint \
  subject:string \
  content:text \
  message_type:string \
  recipient_type:string \
  recipient_ids:jsonb \
  delivery_method:string \
  scheduled_at:datetime \
  sent_at:datetime \
  is_emergency:boolean \
  requires_acknowledgment:boolean \
  template_id:bigint

# Message Delivery model
rails generate model MessageDelivery \
  academy:references \
  message:references \
  recipient_type:string \
  recipient_id:bigint \
  delivery_method:string \
  status:string \
  delivered_at:datetime \
  read_at:datetime \
  acknowledged_at:datetime \
  error_message:text

# Parent Portal Access model
rails generate model ParentPortalAccess \
  academy:references \
  parent_email:string \
  player_id:bigint \
  access_token:string \
  expires_at:datetime \
  last_accessed_at:datetime \
  is_active:boolean
```text

#### 11.2: Implement Communication Services

- `MessageDeliveryService` - Multi-channel message routing
- `NotificationSchedulingService` - Automated reminders
- `ParentPortalService` - Secure parent access management
- `EmergencyAlertService` - Priority message broadcasting

#### 11.3: Create Communication Controllers

- MessagesController for sending and tracking
- ParentPortalController for parent access
- NotificationsController for system notifications

#### 11.4: Implement Message Channels

- Email delivery via SendGrid/Mailgun
- SMS delivery via Twilio
- Push notifications via Firebase
- In-app messaging system

#### 11.5: Create Parent Portal Features

- Secure login for parents
- Player information access
- Training schedule viewing
- Payment history access
- Direct messaging with coaches

#### 11.6: Create Background Jobs

- `MessageDeliveryJob`
- `ParentPortalReminderJob`
- `MessageCleanupJob`

#### 11.7: Add Routes

Update routes with communication endpoints

#### 11.8: Create Tests

- Message delivery testing
- Parent portal security tests
- Multi-channel communication tests

### Deliverables

- [ ] Multi-channel messaging system
- [ ] Parent portal operational
- [ ] Push notification support
- [ ] Message delivery tracking
- [ ] Emergency alert capabilities
- [ ] Communication API endpoints
- [ ] Comprehensive test suite

---

## Phase 12: Advanced Features

### Objectives

- ✅ Implement health and medical records management
- ✅ Create event and calendar management
- ✅ Add competition and tournament features
- ✅ Implement facility management
- ✅ Create staff management system
- ✅ Add integration capabilities

### Tasks

#### 12.1: Create Health Management Models

```bash
# Medical Record model
rails generate model MedicalRecord \
  academy:references \
  player:references \
  slug:uuid:uniq \
  allergies:text \
  medications:text \
  medical_conditions:text \
  emergency_contacts:jsonb \
  doctor_name:string \
  doctor_phone:string \
  insurance_provider:string \
  insurance_policy_number:string \
  last_physical_date:date \
  physical_expires_at:date \
  vaccinations:jsonb \
  created_by_user_id:bigint

# Injury Record model
rails generate model InjuryRecord \
  academy:references \
  player:references \
  slug:uuid:uniq \
  injury_type:string \
  injury_date:date \
  description:text \
  severity:string \
  treatment_plan:text \
  expected_recovery_date:date \
  actual_recovery_date:date \
  cleared_by:string \
  clearance_date:date \
  notes:text \
  created_by_user_id:bigint
```text

#### 12.2: Create Event Management Models

```bash
# Event model
rails generate model Event \
  academy:references \
  slug:uuid:uniq \
  title:string \
  description:text \
  event_type:string \
  start_datetime:datetime \
  end_datetime:datetime \
  location:string \
  is_recurring:boolean \
  recurrence_pattern:jsonb \
  max_participants:integer \
  registration_required:boolean \
  registration_deadline:datetime \
  created_by_user_id:bigint

# Event Registration model
rails generate model EventRegistration \
  event:references \
  participant_type:string \
  participant_id:bigint \
  registered_at:datetime \
  status:string \
  notes:text
```text

#### 12.3: Implement Advanced Services

- `HealthManagementService` - Medical records and injury tracking
- `EventManagementService` - Event scheduling and registration
- `CompetitionManagementService` - Tournament and match management
- `FacilityManagementService` - Resource booking and maintenance

#### 12.4: Create Advanced Controllers

- HealthController for medical records
- EventsController for calendar management
- CompetitionsController for tournaments
- FacilitiesController for resource management

#### 12.5: Create Integration APIs

- Payment processor integration (Stripe, PayPal)
- Email service integration (SendGrid, Mailchimp)
- Calendar integration (Google Calendar, Outlook)
- Accounting system integration (QuickBooks, Xero)

#### 12.6: Add Advanced Features

- Automated injury tracking and reporting
- Event registration and payment processing
- Tournament bracket generation
- Facility booking and conflict resolution

#### 12.7: Create Tests

- Health management testing
- Event management testing
- Integration testing
- Security testing for sensitive data

### Deliverables

- [ ] Health and medical management system
- [ ] Event and calendar management
- [ ] Competition management features
- [ ] Facility management system
- [ ] Integration capabilities
- [ ] Advanced API endpoints
- [ ] Comprehensive test suite

---

## 📊 Progress Tracking

Use this checklist to track overall progress:

### Phase Completion

- [ ] Phase 0: Project Foundation
- [ ] Phase 1: Core Infrastructure
- [ ] Phase 2: Academy Management
- [ ] Phase 3: Player Management
- [ ] Phase 4: Team Management
- [ ] Phase 5: Training Management
- [ ] Phase 6: Shared Resources
- [ ] Phase 7: Testing & Documentation
- [ ] Phase 8: Production Readiness
- [ ] Phase 9: Asset Management
- [ ] Phase 10: Reporting & Analytics
- [ ] Phase 11: Communication System
- [ ] Phase 12: Advanced Features

### Key Metrics

- **Test Coverage:** Target 90%+
- **API Endpoints:** Target 50+ endpoints
- **Documentation:** 100% coverage
- **Performance:** Response time < 200ms (p95)
- **Uptime:** Target 99.9%

---

## 🔄 Iteration Process

For each phase:

1. **Plan** - Review phase objectives and tasks
2. **Implement** - Complete all tasks in order
3. **Test** - Write and run comprehensive tests
4. **Review** - Code review and documentation update
5. **Verify** - Run verification steps
6. **Deploy** - Merge to main branch
7. **Retrospect** - Document learnings and improvements

---

## 📝 Notes

- Each phase should be completed in a separate Git branch
- All tests must pass before moving to next phase
- Documentation must be updated with each phase
- Regular code reviews should be conducted
- Performance should be monitored throughout

---

## 🎯 Success Criteria

The project is ready for production when:

- ✅ All 12 phases are complete (Phases 9-12 are optional for MVP)
- ✅ Test coverage > 90%
- ✅ All security audits pass
- ✅ Performance benchmarks met
- ✅ Documentation is complete
- ✅ CI/CD pipeline is functional
- ✅ Production deployment successful
- ✅ Monitoring and alerting active

---

For detailed phase-specific instructions, see:

- [PHASE_0_SETUP.md](./PHASE_0_SETUP.md)
- [PHASE_1_INFRASTRUCTURE.md](./PHASE_1_INFRASTRUCTURE.md)
- [PHASE_2_ACADEMY.md](./PHASE_2_ACADEMY.md)
- [PHASE_3_PLAYER.md](./PHASE_3_PLAYER.md)
- [PHASE_4_TEAM.md](./PHASE_4_TEAM.md)
- [PHASE_5_TRAINING.md](./PHASE_5_TRAINING.md)
- [PHASE_6_SHARED.md](./PHASE_6_SHARED.md)
- [PHASE_7_TESTING.md](./PHASE_7_TESTING.md)
- [PHASE_8_PRODUCTION.md](./PHASE_8_PRODUCTION.md)
