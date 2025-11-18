# Diquis - Architecture Documentation

## Table of Contents

1. [Vertical Slice Architecture](#vertical-slice-architecture)
2. [Multi-Tenancy Architecture](#multi-tenancy-architecture)
3. [Service Layer Pattern](#service-layer-pattern)
4. [API Design](#api-design)
5. [Data Model](#data-model)
6. [Security Architecture](#security-architecture)
7. [Background Jobs Infrastructure](#background-jobs-infrastructure)
8. [CI/CD Pipeline](#cicd-pipeline)
9. [Deployment Infrastructure](#deployment-infrastructure)
10. [Performance Considerations](#performance-considerations)

---

## Vertical Slice Architecture

### Overview

Diquis implements Vertical Slice Architecture where features are organized by business capability rather than technical layer. Each slice contains all the components needed for a specific feature.

### Slice Structure

```text
app/slices/[slice_name]/
├── controllers/       # HTTP request handlers
├── services/          # Business logic
├── models/            # Domain models
├── serializers/       # JSON serialization
├── policies/          # Authorization rules
├── validators/        # Custom validations
└── jobs/              # Background jobs
```text

### Core Slices

#### 1. Academy Management Slice

**Purpose:** Manage football academies (tenants)

**Components:**

- `AcademyCreationService` - Create new academies with default structure
- `AcademyFinderService` - Query and search academies
- `AcademyUpdateService` - Modify academy details
- `AcademiesController` - REST API endpoints
- `Academy` model - Main tenant model
- `AcademyUser` model - User-academy association
- `AcademySerializer` - JSON representation
- `AcademyPolicy` - Authorization rules

**Key Operations:**

- Create academy with default positions and skills
- List user's accessible academies
- Update academy settings
- Activate/deactivate academy

#### 2. Player Management Slice

**Purpose:** Manage player registration, profiles, and data

**Components:**

- `PlayerRegistrationService` - Register new players with validations
- `PlayerFinderService` - Find player by slug
- `PlayerSearchService` - Advanced search with filters
- `PlayerUpdateService` - Update player information
- `PlayersController` - REST API endpoints
- `Player` model - Player domain model
- `PlayerSerializer` - JSON representation with relationships
- `PlayerPolicy` - Authorization rules

**Key Operations:**

- Register player with parent information
- Search players by name, age, position, category
- Assign players to teams
- Track player skills and assessments
- Upload player pictures

**Validations:**

- Age must match category (e.g., U-16 means max 16 years)
- Unique name within academy
- Valid parent contact information
- Position must exist in academy

#### 3. Team Management Slice

**Purpose:** Organize players into teams

**Components:**

- `TeamCreationService` - Create teams
- `TeamRosterService` - Manage team memberships
- `TeamsController` - REST API endpoints
- `Team` model - Team domain model
- `TeamMembership` model - Player-team association
- `TeamSerializer` - JSON representation
- `TeamPolicy` - Authorization rules

**Key Operations:**

- Create team with category/division
- Add/remove players from team
- List team roster with player details
- Schedule team trainings

#### 4. Training Management Slice

**Purpose:** Schedule and track training sessions

**Components:**

- `TrainingSchedulingService` - Schedule trainings with conflict detection
- `AttendanceTrackingService` - Record player attendance
- `TrainingsController` - REST API endpoints
- `Training` model - Training session model
- `TrainingAttendance` model - Player attendance records
- `TrainingSerializer` - JSON representation
- `TrainingPolicy` - Authorization rules
- `TrainingReminderJob` - Background job for notifications

**Key Operations:**

- Schedule training with date/time/location
- Bulk update attendance for entire team
- Generate attendance reports
- Send reminder notifications (24h and 2h before)
- Validate no scheduling conflicts

**Training Types:**

- Technical
- Tactical
- Physical
- Fitness
- Scrimmage

#### 5. Shared Resources Slice

**Purpose:** Manage resources shared across academies

**Components:**

- `Position` model - Player positions (Goalkeeper, Defender, etc.)
- `Skill` model - Player skills (Passing, Shooting, etc.)
- `Category` model - Age categories (U-8, U-10, etc.)
- `Division` model - Competition divisions (Primera, Amateur, etc.)

**Note:** Some resources can be academy-specific (Skills, Positions) while others are global (Categories, Divisions)

#### 6. Asset Management Slice

**Purpose:** Track and manage physical assets, equipment, and inventory

**Components:**

- `AssetManagementService` - Asset registration, valuation, and bulk operations
- `AssetAllocationService` - Check-out/check-in workflow and conflict detection
- `AssetMaintenanceService` - Maintenance scheduling and cost tracking
- `AssetsController` - REST API endpoints for asset management
- `Asset` model - Main asset domain model with condition tracking
- `AssetCategory` model - Hierarchical asset categorization
- `AssetAllocation` model - Asset assignment and return tracking
- `AssetSerializer` - JSON representation with relationships
- `AssetPolicy` - Authorization rules for asset access

**Key Operations:**

- Register assets with barcodes and financial information
- Allocate equipment to players, teams, or staff
- Schedule maintenance and track repair costs
- Generate depreciation and utilization reports
- Monitor inventory levels and reorder points

#### 7. Reporting & Analytics Slice

**Purpose:** Generate business intelligence and performance analytics

**Components:**

- `ReportGenerationService` - Create reports in multiple formats
- `FinancialAnalyticsService` - Revenue, expense, and P&L analysis
- `PlayerAnalyticsService` - Player development and performance metrics
- `BusinessIntelligenceService` - Cross-academy benchmarking and KPIs
- `ReportsController` - REST API endpoints for report management
- `Report` model - Report definitions and scheduling
- `ReportGeneration` model - Track report generation status and files
- `FinancialTransaction` model - Income and expense tracking
- `ReportSerializer` - JSON representation with metadata

**Key Operations:**

- Generate financial reports (P&L, cash flow, budget variance)
- Analyze player development and performance trends
- Create operational efficiency and utilization reports
- Schedule automated report generation and delivery
- Provide business intelligence dashboards and KPIs

#### 8. Communication & Notification Slice

**Purpose:** Multi-channel communication system for all stakeholders

**Components:**

- `MessageDeliveryService` - Multi-channel message routing and delivery
- `NotificationSchedulingService` - Automated reminders and alerts
- `ParentPortalService` - Secure parent access to player information
- `CommunicationController` - REST API endpoints for messaging
- `Message` model - Message content and delivery tracking
- `MessageDelivery` model - Track delivery status across channels
- `ParentPortalAccess` model - Parent authentication and permissions
- `MessageSerializer` - JSON representation with delivery status

**Key Operations:**

- Send messages via email, SMS, push notifications, and in-app
- Manage parent portal access and secure player data sharing
- Schedule automated reminders for trainings and events
- Track message delivery and read receipts
- Support emergency alerts and academy-wide announcements

---

## Multi-Tenancy Architecture

### ActsAsTenant Implementation

#### Tenant Model: Academy

Every academy operates as an isolated tenant with its own:

- Players
- Teams
- Trainings
- Academy-specific positions and skills

#### Automatic Scoping

```ruby
# All queries automatically scoped to current tenant
Player.all  # Returns only players for current academy
Team.where(active: true)  # Scoped to current academy
```text

#### Setting Tenant Context

**1. URL-Based (Preferred)**

```ruby
# Routes include optional academy_slug
GET /api/v1/{academy_slug}/players
GET /api/v1/550e8400-e29b-41d4-a716-446655440000/players
```text

**2. Header-Based**

```text
X-Academy-Context: 550e8400-e29b-41d4-a716-446655440000
```text

**3. User's Default Academy**

```ruby
current_user.academy_users.active.first.academy
```text

#### Tenant Resolution Priority

1. URL parameter (`academy_slug`)
2. Custom header (`X-Academy-Context`)
3. User's default academy
4. First accessible academy

#### Cross-Tenant Operations

For system administrators:

```ruby
ActsAsTenant.without_tenant do
  # Query across all academies
  Player.count
end
```text

#### Tenant Context in Background Jobs

```ruby
class PlayerRegistrationJob < ApplicationJob
  def perform(player_id, academy_id)
    ActsAsTenant.with_tenant(Academy.find(academy_id)) do
      # All operations scoped to academy
      player = Player.find(player_id)
      # ...
    end
  end
end
```text

### Hybrid Academy Context (Frontend)

#### Redux Store

```javascript
// Central state management
const academySlice = {
  current: null,        // Active academy
  available: [],        // User's academies
  switching: false,     // Switch in progress
  urlAcademy: null,     // From URL params
  userDefault: null     // User's default
}
```text

#### React Context Provider

```javascript
// Deep component access
const AcademyContext = createContext();

export const useAcademy = () => {
  const context = useContext(AcademyContext);
  return context.academy; // Effective academy slug
};
```text

#### TanStack Query Integration

```javascript
// Automatic cache invalidation on academy switch
const useAcademyAwarePlayers = (filters) => {
  const { academy } = useAcademy();
  
  return useQuery({
    queryKey: ['players', academy, filters],
    queryFn: () => api.players.list({ academy, ...filters }),
    enabled: !!academy,
    staleTime: 5 * 60 * 1000
  });
};
```text

---

## Service Layer Pattern

### Base Service Class

All services inherit from `BaseService` which provides:

- Parameter validation
- Transaction management
- Error handling
- Consistent result objects

```ruby
class BaseService
  include ActiveModel::Validations
  
  def initialize(params = {})
    @errors = ActiveModel::Errors.new(self)
    @result = nil
    assign_attributes(params) if params.any?
  end
  
  def call
    validate_params
    return failure("Invalid parameters") if errors.any?
    
    ActiveRecord::Base.transaction do
      @result = execute
      success(@result)
    end
  rescue StandardError => e
    failure(e.message)
  end
  
  private
  
  def execute
    raise NotImplementedError
  end
end
```text

### Service Result Pattern

```ruby
class ServiceResult
  attr_reader :data, :errors
  
  def initialize(success:, data:, errors:)
    @success = success
    @data = data
    @errors = errors
  end
  
  def success?
    @success
  end
  
  def failure?
    !@success
  end
end
```text

### Service Usage in Controllers

```ruby
def create
  service = PlayerRegistrationService.new(
    player_params.merge(academy: @current_academy)
  )
  
  result = service.call
  
  if result.success?
    render json: { data: serialize(result.data) }, status: :created
  else
    render json: { errors: result.errors }, status: :unprocessable_entity
  end
end
```text

### Service Responsibilities

- **Validation:** Business rule validation beyond model validations
- **Orchestration:** Coordinate multiple model operations
- **Side Effects:** Trigger jobs, send emails, create audit logs
- **Error Handling:** Graceful error handling with rollback
- **Return Values:** Consistent ServiceResult objects

---

## API Design

### REST Principles

#### Resource Naming

```text
/api/v1/academies                    # Collection
/api/v1/academies/{slug}             # Single resource
/api/v1/{academy_slug}/players       # Nested collection
/api/v1/{academy_slug}/players/{slug} # Nested resource
```text

#### HTTP Methods

- `GET` - Retrieve resources (safe, idempotent)
- `POST` - Create resources
- `PUT/PATCH` - Update resources (idempotent)
- `DELETE` - Remove resources (idempotent)

#### Status Codes

- `200 OK` - Successful GET, PUT, PATCH
- `201 Created` - Successful POST
- `204 No Content` - Successful DELETE
- `400 Bad Request` - Invalid request
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Authorization failed
- `404 Not Found` - Resource not found
- `422 Unprocessable Entity` - Validation errors
- `500 Internal Server Error` - Server error

### Response Format

#### Success Response

```json
{
  "data": {
    "id": 123,
    "slug": "550e8400-e29b-41d4-a716-446655440000",
    "first_name": "John",
    "last_name": "Doe",
    "full_name": "John Doe",
    "created_at": "2025-10-13T10:30:00Z",
    "updated_at": "2025-10-13T10:30:00Z"
  },
  "meta": {
    "resource_type": "player",
    "created_at": "2025-10-13T10:30:00Z",
    "updated_at": "2025-10-13T10:30:00Z"
  }
}
```text

#### Collection Response

```json
{
  "data": [
    { "id": 1, "name": "Player 1" },
    { "id": 2, "name": "Player 2" }
  ],
  "meta": {
    "pagination": {
      "current_page": 1,
      "per_page": 25,
      "total_pages": 5,
      "total_count": 120,
      "has_next_page": true,
      "has_prev_page": false
    }
  }
}
```text

#### Error Response

```json
{
  "error": "VALIDATION_ERROR",
  "message": "Validation failed",
  "details": [
    "Age must be greater than 4",
    "Parent email is invalid"
  ],
  "field_errors": {
    "age": ["must be greater than 4"],
    "parent_email": ["is invalid"]
  }
}
```text

### API Features

#### Pagination

```text
GET /api/v1/players?page=2&per_page=50
```text

#### Filtering

```text
GET /api/v1/players?age_min=10&age_max=15&gender=M&position=forward
```text

#### Search

```text
GET /api/v1/players?search=John+Doe
```text

#### Sorting

```text
GET /api/v1/players?sort=age:desc,last_name:asc
```text

#### Relationship Inclusion

```text
GET /api/v1/players?include=position,category,teams
```text

### Serialization (Active Model Serializers)

#### Base Serializer

```ruby
class ApplicationSerializer < ActiveModel::Serializer
  attributes :id, :slug, :created_at, :updated_at
  
  def created_at
    object.created_at&.iso8601
  end
  
  def updated_at
    object.updated_at&.iso8601
  end
end
```text

#### Custom Attributes

```ruby
class PlayerSerializer < ApplicationSerializer
  attributes :first_name, :last_name, :full_name, :picture_url
  
  def full_name
    "#{object.first_name} #{object.last_name}"
  end
  
  def picture_url
    return nil unless object.picture.attached?
    Rails.application.routes.url_helpers.rails_blob_url(object.picture)
  end
end
```text

#### Conditional Relationships

```ruby
class PlayerSerializer < ApplicationSerializer
  has_many :teams, if: :include_teams?
  has_many :player_skills, if: :include_skills?
  
  def include_teams?
    instance_options[:include]&.include?(:teams)
  end
end
```text

---

## Data Model

### UUID Primary Keys

All models use UUID slugs as primary identifiers for:

- Security (no sequential IDs)
- External API references
- URL-friendly identifiers
- Distributed system compatibility

### Core Models

#### Academy (Tenant)

```ruby
class Academy < ApplicationRecord
  # Attributes
  - slug (UUID, indexed)
  - name (string, required)
  - description (text)
  - owner_name (string, required)
  - owner_email (string, required)
  - owner_phone (string, required)
  - address_line_1 (string, required)
  - address_line_2 (string)
  - city (string, required)
  - state_province (string, required)
  - postal_code (string, required)
  - country (string, required)
  - founded_date (date)
  - website (string)
  - is_active (boolean, default: true)
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  has_many :academy_users
  has_many :users, through: :academy_users
  has_many :players
  has_many :teams
  has_many :skills
  has_many :positions
  has_one_attached :logo
end
```text

#### Player (Tenant-Scoped)

```ruby
class Player < ApplicationRecord
  acts_as_tenant(:academy)
  
  # Attributes
  - slug (UUID, indexed)
  - academy_id (foreign key, required)
  - first_name (string, required)
  - last_name (string, required)
  - age (integer, required)
  - gender (string: M/F/NB/PNTS, required)
  - foot (string: L/R/B)
  - parent_name (string, required)
  - parent_email (string, required, encrypted)
  - phone_number (string, required, encrypted)
  - position_id (foreign key, required)
  - category_id (foreign key, required)
  - is_active (boolean, default: true)
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  belongs_to :academy
  belongs_to :position
  belongs_to :category
  has_many :team_memberships
  has_many :teams, through: :team_memberships
  has_many :player_skills
  has_many :skills, through: :player_skills
  has_many :training_attendances
  has_one_attached :picture
  
  # Validations
  validates :first_name, uniqueness: { scope: [:academy_id, :last_name] }
  validates :age, numericality: { greater_than: 4, less_than: 100 }
end
```text

#### Team (Tenant-Scoped)

```ruby
class Team < ApplicationRecord
  acts_as_tenant(:academy)
  
  # Attributes
  - slug (UUID, indexed)
  - academy_id (foreign key, required)
  - name (string, required)
  - category_id (foreign key, required)
  - division_id (foreign key)
  - coach (string)
  - is_active (boolean, default: true)
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  belongs_to :academy
  belongs_to :category
  belongs_to :division, optional: true
  has_many :team_memberships
  has_many :players, through: :team_memberships
  has_many :trainings
end
```text

#### Training (Tenant-Scoped)

```ruby
class Training < ApplicationRecord
  acts_as_tenant(:academy)
  
  # Attributes
  - slug (UUID, indexed)
  - team_id (foreign key, required)
  - place (string, required)
  - date (date, required)
  - time (time, required)
  - duration (interval, required)
  - training_type (string, required)
  - coach (string, required)
  - description (text, required)
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  belongs_to :team
  has_many :training_attendances
  has_many :players, through: :training_attendances
  
  # Validations
  validates :training_type, inclusion: { 
    in: %w[Technical Tactical Physical Fitness Scrimmage] 
  }
end
```text

#### Shared Resources (Global or Academy-Specific)

**Position**

```ruby
class Position < ApplicationRecord
  # Can be academy-specific or global
  - slug (UUID, indexed)
  - academy_id (foreign key, optional)
  - name (string, required)
  - abbreviation (string)
  - category (string)
  - description (text)
  - language (string, default: 'en')
  
  belongs_to :academy, optional: true
  has_many :players
end
```text

**Category** (Global)

```ruby
class Category < ApplicationRecord
  - slug (UUID, indexed)
  - name (string, required) # U-8, U-10, U-12, etc.
  - language (string, default: 'en')
  
  has_many :players
  has_many :teams
end
```text

### Model Concerns

#### Sluggable

```ruby
module Sluggable
  extend ActiveSupport::Concern
  
  included do
    before_validation :generate_slug, on: :create
    validates :slug, presence: true, uniqueness: true
  end
  
  def to_param
    slug
  end
  
  private
  
  def generate_slug
    self.slug = SecureRandom.uuid if slug.blank?
  end
end
```text

#### Auditable

```ruby
module Auditable
  extend ActiveSupport::Concern
  
  included do
    has_many :audit_logs, as: :auditable, dependent: :destroy
  end
  
  def create_audit_log(action, user, changes = {})
    audit_logs.create!(
      action: action,
      user: user,
      changes: changes,
      timestamp: Time.current
    )
  end
end
```text

---

## Security Architecture

### Authentication (Devise + JWT)

Diquis uses Devise with JWT for stateless authentication.

#### JWT Token Flow

1. User logs in with email/password
2. Server validates credentials
3. Server generates JWT token with user ID and expiry
4. Client stores token (localStorage/sessionStorage)
5. Client includes token in Authorization header for all requests
6. Server validates token on each request

#### Token Format

```text
Authorization: Bearer eyJhbGciOiJIUzI1NiJ9.eyJ1c2VyX2lkIjoxMjM...
```text

#### Authentication Setup

**Devise Configuration**

```ruby
# config/initializers/devise.rb
Devise.setup do |config|
  config.mailer_sender = 'noreply@diquis.com'
  
  # JWT configuration
  config.jwt do |jwt|
    jwt.secret = Rails.application.credentials.devise_jwt_secret_key || ENV['DEVISE_JWT_SECRET_KEY']
    jwt.dispatch_requests = [
      ['POST', %r{^/auth/sign_in$}]
    ]
    jwt.revocation_requests = [
      ['DELETE', %r{^/auth/sign_out$}]
    ]
    jwt.expiration_time = 24.hours.to_i
  end
  
  # API-only mode
  config.navigational_formats = []
end
```text

**User Model**

```ruby
# app/models/user.rb
class User < ApplicationRecord
  devise :database_authenticatable, :registerable,
         :recoverable, :rememberable, :validatable,
         :jwt_authenticatable, jwt_revocation_strategy: JwtDenylist
  
  has_many :academy_users, dependent: :destroy
  has_many :academies, through: :academy_users
  
  def jwt_payload
    {
      'user_id' => id,
      'email' => email,
      'is_system_admin' => is_system_admin
    }
  end
end
```text

**Token Revocation (JwtDenylist)**

```ruby
# app/models/jwt_denylist.rb
class JwtDenylist < ApplicationRecord
  include Devise::JWT::RevocationStrategies::Denylist
  
  self.table_name = 'jwt_denylists'
end
```text

#### Authentication Controllers

**Sessions Controller**

```ruby
# app/controllers/auth/sessions_controller.rb
module Auth
  class SessionsController < Devise::SessionsController
    respond_to :json
    
    private
    
    def respond_with(resource, _opts = {})
      render json: {
        data: {
          id: resource.id,
          email: resource.email,
          first_name: resource.first_name,
          last_name: resource.last_name,
          is_system_admin: resource.is_system_admin
        },
        message: 'Logged in successfully'
      }, status: :ok
    end
    
    def respond_to_on_destroy
      if current_user
        render json: { message: 'Logged out successfully' }, status: :ok
      else
        render json: { message: 'No active session' }, status: :unauthorized
      end
    end
  end
end
```text

**Registrations Controller**

```ruby
# app/controllers/auth/registrations_controller.rb
module Auth
  class RegistrationsController < Devise::RegistrationsController
    respond_to :json
    
    private
    
    def respond_with(resource, _opts = {})
      if resource.persisted?
        render json: {
          data: {
            id: resource.id,
            email: resource.email,
            first_name: resource.first_name,
            last_name: resource.last_name
          },
          message: 'Registered successfully'
        }, status: :created
      else
        render json: {
          errors: resource.errors.full_messages
        }, status: :unprocessable_entity
      end
    end
  end
end
```text

#### API Usage Examples

**Login**

```bash
curl -X POST http://localhost:3000/auth/sign_in \
  -H "Content-Type: application/json" \
  -d '{
    "user": {
      "email": "admin@example.com",
      "password": "password123"
    }
  }'

# Response includes JWT token in Authorization header
# Authorization: Bearer eyJhbGciOiJIUzI1NiJ9...
```text

**Authenticated Request**

```bash
curl http://localhost:3000/api/v1/players \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiJ9..."
```text

**Logout**

```bash
curl -X DELETE http://localhost:3000/auth/sign_out \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiJ9..."
```text

### Authorization (Pundit)

Diquis uses Pundit for policy-based authorization with tenant-aware permissions.

#### Base Policy

```ruby
# app/policies/application_policy.rb
class ApplicationPolicy
  attr_reader :user, :record, :academy
  
  def initialize(user, record)
    @user = user
    @record = record
    @academy = ActsAsTenant.current_tenant
  end
  
  def index?
    false
  end
  
  def show?
    false
  end
  
  def create?
    false
  end
  
  def update?
    false
  end
  
  def destroy?
    false
  end
  
  protected
  
  def system_admin?
    user.system_admin?
  end
  
  def academy_user
    @academy_user ||= user.academy_users.active.find_by(academy: academy)
  end
  
  def has_permission?(permission)
    return true if system_admin?
    academy_user&.can?(permission)
  end
  
  def same_academy?
    record.respond_to?(:academy) && record.academy == academy
  end
end
```text

#### Example Policy Implementation

```ruby
# app/policies/player_policy.rb
class PlayerPolicy < ApplicationPolicy
  def index?
    has_permission?(:read)
  end
  
  def show?
    has_permission?(:read) && same_academy?
  end
  
  def create?
    has_permission?(:create)
  end
  
  def update?
    has_permission?(:update) && same_academy?
  end
  
  def destroy?
    has_permission?(:delete) && same_academy?
  end
  
  class Scope < Scope
    def resolve
      if user.system_admin?
        scope.all
      else
        scope.where(academy: ActsAsTenant.current_tenant)
      end
    end
  end
end
```text

#### Controller Integration

```ruby
# app/controllers/api/v1/players_controller.rb
class Api::V1::PlayersController < ApplicationController
  before_action :authenticate_user!
  
  def index
    authorize Player
    @players = policy_scope(Player).includes(:position, :category)
    render json: @players
  end
  
  def show
    @player = Player.find_by!(slug: params[:id])
    authorize @player
    render json: @player
  end
  
  def create
    @player = Player.new(player_params)
    authorize @player
    
    if @player.save
      render json: @player, status: :created
    else
      render json: { errors: @player.errors }, status: :unprocessable_entity
    end
  end
  
  def update
    @player = Player.find_by!(slug: params[:id])
    authorize @player
    
    if @player.update(player_params)
      render json: @player
    else
      render json: { errors: @player.errors }, status: :unprocessable_entity
    end
  end
  
  def destroy
    @player = Player.find_by!(slug: params[:id])
    authorize @player
    @player.destroy
    head :no_content
  end
end
```text

#### Role-Based Access Control (RBAC)

**Permission Levels**

- **System Admin:** Full access across all academies
- **Academy Admin:** Full CRUD access within their academy
- **Coach:** Read/write players, teams, trainings within academy
- **Assistant Coach:** Read players and teams, limited training management
- **Viewer:** Read-only access within academy

**AcademyUser Model**

```ruby
# app/models/academy_user.rb
class AcademyUser < ApplicationRecord
  belongs_to :user
  belongs_to :academy
  
  enum role: {
    viewer: 0,
    assistant_coach: 1,
    coach: 2,
    admin: 3
  }
  
  scope :active, -> { where(is_active: true) }
  
  PERMISSIONS = {
    viewer: [:read],
    assistant_coach: [:read, :create_attendance],
    coach: [:read, :create, :update],
    admin: [:read, :create, :update, :delete]
  }.freeze
  
  def can?(permission)
    PERMISSIONS[role.to_sym]&.include?(permission.to_sym) || false
  end
end
```text

#### Pundit Configuration

```ruby
# app/controllers/application_controller.rb
class ApplicationController < ActionController::API
  include Pundit::Authorization
  
  before_action :authenticate_user!
  
  rescue_from Pundit::NotAuthorizedError, with: :user_not_authorized
  
  private
  
  def user_not_authorized
    render json: {
      error: 'You are not authorized to perform this action'
    }, status: :forbidden
  end
end
```text

### Data Encryption (Rails 8.0)

#### Encrypted Attributes

```ruby
class Player < ApplicationRecord
  # Encrypt sensitive data
  encrypts :parent_email
  encrypts :phone_number
  encrypts :parent_name, deterministic: true  # Allows searching
  
  # Blind indexes for encrypted search
  blind_index :parent_email, :phone_number
end
```text

### CORS Configuration

```ruby
config.middleware.insert_before 0, Rack::Cors do
  allow do
    origins 'http://localhost:3000', 'https://diquis.com'
    resource '*',
      headers: :any,
      methods: [:get, :post, :put, :patch, :delete, :options, :head],
      credentials: true
  end
end
```text

### SQL Injection Prevention

- Use parameterized queries (ActiveRecord)
- Never interpolate user input directly into SQL
- Use strong parameters in controllers

### XSS Prevention

- API-only mode (no view rendering)
- JSON responses automatically escaped
- Frontend responsible for HTML sanitization

---

## Background Jobs Infrastructure

### Sidekiq Configuration

Diquis uses Sidekiq for background job processing with Redis as the backend.

#### Queue Priority System

```yaml
# config/sidekiq.yml
:queues:
  - [critical, 10]   # Urgent operations (e.g., password resets)
  - [default, 5]     # Standard background tasks
  - [mailers, 3]     # Email notifications
  - [low, 1]         # Non-urgent tasks (reports, cleanup)
```text

#### Job Configuration

```ruby
# app/jobs/application_job.rb
class ApplicationJob < ActiveJob::Base
  # Retry configuration
  retry_on StandardError, wait: :exponentially_longer, attempts: 5
  
  # Discard on specific errors
  discard_on ActiveJob::DeserializationError
  
  # Set queue priority
  queue_as :default
  
  # Tenant context support
  around_perform do |job, block|
    academy_id = job.arguments.first if job.arguments.any?
    if academy_id
      ActsAsTenant.with_tenant(Academy.find(academy_id)) do
        block.call
      end
    else
      block.call
    end
  end
end
```text

### Common Background Jobs

#### PlayerRegistrationJob

```ruby
class PlayerRegistrationJob < ApplicationJob
  queue_as :default
  
  def perform(player_id, academy_id)
    ActsAsTenant.with_tenant(Academy.find(academy_id)) do
      player = Player.find(player_id)
      
      # Send welcome email
      PlayerMailer.welcome_email(player).deliver_now
      
      # Create audit log
      AuditLog.create!(
        auditable: player,
        action: 'player_registered',
        performed_at: Time.current
      )
    end
  end
end
```text

#### TrainingReminderJob

```ruby
class TrainingReminderJob < ApplicationJob
  queue_as :mailers
  
  def perform(training_id, hours_before)
    training = Training.find(training_id)
    
    training.team.players.each do |player|
      TrainingMailer.reminder(player, training, hours_before).deliver_now
    end
  end
end
```text

### Scheduling Recurring Jobs (Sidekiq-Cron)

```ruby
# config/initializers/sidekiq.rb
Sidekiq::Cron::Job.load_from_hash({
  'cleanup_expired_tokens' => {
    'class' => 'CleanupExpiredTokensJob',
    'cron' => '0 2 * * *',  # Daily at 2 AM
    'queue' => 'low'
  },
  'send_training_reminders' => {
    'class' => 'SendTrainingRemindersJob',
    'cron' => '0 * * * *',  # Every hour
    'queue' => 'mailers'
  }
})
```text

### Monitoring & Dashboard

Access Sidekiq Web UI at `/sidekiq` (authentication required):

- View job queues and status
- Monitor failed jobs
- Retry failed jobs
- View job statistics and throughput

```ruby
# config/routes.rb
require 'sidekiq/web'
require 'sidekiq/cron/web'

Rails.application.routes.draw do
  # Protect Sidekiq dashboard
  authenticate :user, ->(u) { u.system_admin? } do
    mount Sidekiq::Web => '/sidekiq'
  end
end
```text

---

## CI/CD Pipeline

### GitHub Actions Workflow

Diquis uses GitHub Actions for continuous integration and deployment.

#### Workflow Configuration

```yaml
# .github/workflows/ci.yml
name: CI

on:
  pull_request:
  push:
    branches: [ main ]

jobs:
  scan_ruby:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
      
      - name: Set up Ruby
        uses: ruby/setup-ruby@v1
        with:
          ruby-version: .ruby-version
          bundler-cache: true
      
      - name: Security scan with Brakeman
        run: bin/brakeman --no-pager

  lint:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
      
      - name: Set up Ruby
        uses: ruby/setup-ruby@v1
        with:
          ruby-version: .ruby-version
          bundler-cache: true
      
      - name: Lint with RuboCop
        run: bin/rubocop -f github

  test:
    runs-on: ubuntu-latest
    services:
      postgres:
        image: postgres
        env:
          POSTGRES_USER: postgres
          POSTGRES_PASSWORD: postgres
        ports:
          - 5432:5432
        options: --health-cmd="pg_isready" --health-interval=10s
    
    steps:
      - name: Install packages
        run: sudo apt-get update && sudo apt-get install -y build-essential libpq-dev
      
      - name: Checkout code
        uses: actions/checkout@v4
      
      - name: Set up Ruby
        uses: ruby/setup-ruby@v1
        with:
          ruby-version: .ruby-version
          bundler-cache: true
      
      - name: Run tests
        env:
          RAILS_ENV: test
          DATABASE_URL: postgres://postgres:postgres@localhost:5432
        run: bin/rails db:test:prepare test
```text

### Pipeline Stages

#### 1. Security Scanning

- **Brakeman**: Static analysis for Rails security vulnerabilities
- Scans for common issues like SQL injection, XSS, mass assignment
- Fails build if critical vulnerabilities found

#### 2. Code Quality

- **RuboCop**: Enforces Ruby style guide and best practices
- Checks code formatting, complexity, and conventions
- Configuration in `.rubocop.yml`

#### 3. Automated Testing

- **RSpec**: Unit and integration tests
- **Database**: PostgreSQL test database
- **Coverage**: Tests all slices, services, and policies

### Local CI Simulation

```bash
# Run all CI checks locally before pushing
bin/brakeman --no-pager
bin/rubocop
bundle exec rspec

# Or use the verification script
./script/verify
```text

---

## Deployment Infrastructure

### Kamal Deployment

Diquis uses Kamal for zero-downtime Docker deployments.

#### Configuration

```yaml
# config/deploy.yml
service: diquis

image: erickcastrillo/diquis

servers:
  web:
    hosts:
      - 192.168.1.100
    labels:
      traefik.http.routers.diquis.rule: Host(`diquis.example.com`)
      traefik.http.routers.diquis-secure.entrypoints: websecure
      traefik.http.routers.diquis-secure.tls.certresolver: letsencrypt

registry:
  username: erickcastrillo
  password:
    - KAMAL_REGISTRY_PASSWORD

env:
  secret:
    - RAILS_MASTER_KEY
    - DATABASE_URL
    - REDIS_URL
  clear:
    RAILS_ENV: production

accessories:
  postgres:
    image: postgres:16
    host: 192.168.1.100
    port: 5432
    env:
      secret:
        - POSTGRES_PASSWORD
      clear:
        POSTGRES_USER: diquis
        POSTGRES_DB: diquis_production
    
  redis:
    image: redis:7
    host: 192.168.1.100
    port: 6379
```text

#### Deployment Commands

```bash
# Initial setup
kamal setup

# Deploy updates
kamal deploy

# Roll back to previous version
kamal rollback

# View application logs
kamal app logs --follow

# SSH into running container
kamal app exec --interactive bash

# Restart services
kamal app restart
```text

### Docker Configuration

```dockerfile
# Dockerfile
FROM ruby:3.3.0-slim

# Install dependencies
RUN apt-get update -qq && \
    apt-get install -y build-essential libpq-dev nodejs npm && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Install gems
COPY Gemfile Gemfile.lock ./
RUN bundle install

# Copy application
COPY . .

# Precompile assets (if needed)
RUN bundle exec rails assets:precompile

EXPOSE 3000

CMD ["bundle", "exec", "rails", "server", "-b", "0.0.0.0"]
```text

### Environment Variables

Required environment variables for production:

```bash
# Security
RAILS_MASTER_KEY=<your_master_key>
DEVISE_JWT_SECRET_KEY=<jwt_secret>

# Database
DATABASE_URL=postgresql://user:password@host:5432/diquis_production

# Redis (for Sidekiq and caching)
REDIS_URL=redis://host:6379/0

# Email (optional)
SMTP_HOST=smtp.example.com
SMTP_PORT=587
SMTP_USERNAME=user
SMTP_PASSWORD=password

# Application
RAILS_ENV=production
RAILS_LOG_LEVEL=info
```text

### Health Checks

```ruby
# app/controllers/health_controller.rb
class HealthController < ApplicationController
  skip_before_action :authenticate_user!
  
  def show
    render json: {
      status: 'ok',
      timestamp: Time.current,
      services: {
        database: database_healthy?,
        redis: redis_healthy?,
        sidekiq: sidekiq_healthy?
      }
    }
  end
  
  private
  
  def database_healthy?
    ActiveRecord::Base.connection.execute('SELECT 1')
    true
  rescue
    false
  end
  
  def redis_healthy?
    Redis.new.ping == 'PONG'
  rescue
    false
  end
  
  def sidekiq_healthy?
    Sidekiq::ProcessSet.new.size > 0
  rescue
    false
  end
end
```text

### Production Checklist

- [ ] Set all required environment variables
- [ ] Configure SSL certificates (Let's Encrypt)
- [ ] Set up database backups
- [ ] Configure log aggregation
- [ ] Set up monitoring (e.g., New Relic, Sentry)
- [ ] Configure CDN for assets (if needed)
- [ ] Set up Redis persistence
- [ ] Configure email delivery (SMTP/SendGrid)
- [ ] Test rollback procedure
- [ ] Document incident response

---

## Performance Considerations

### N+1 Query Prevention (Bullet)

```ruby
# Bad - N+1 queries
players.each { |player| puts player.position.name }

# Good - Eager loading
players.includes(:position).each { |player| puts player.position.name }
```text

### Caching Strategy

- Redis for session storage
- Query result caching for expensive operations
- Academy context caching (per-request)

### Background Job Best Practices

- Keep jobs idempotent
- Use exponential backoff for retries
- Pass IDs, not objects, to jobs
- Set tenant context explicitly

### Database Indexing

- UUID slugs (primary lookup)
- Foreign keys
- Academy ID (tenant scoping)
- Commonly queried fields (name, age, date)
- Composite indexes for multi-column queries

---

This architecture supports scalability, maintainability, and clear separation of concerns while maintaining consistency across the application.
