# Phase 1: Core Infrastructure

**Duration:** 1-2 Weeks  
**Status:** 🟡 Not Started  
**Dependencies:** Phase 0 (Project Foundation)

---

## 📋 Overview

This phase builds the foundational infrastructure that all other features will depend on. It includes base classes, authentication, authorization, multi-tenancy, and essential patterns.

## 🎯 Objectives

- Implement base service class with error handling
- Create base controller with tenant context
- Set up Devise + JWT authentication
- Configure Pundit authorization
- Implement multi-tenancy with ActsAsTenant
- Create reusable model concerns
- Configure Sidekiq for background jobs
- Set up base serializers

## 📝 Prerequisites

- [ ] Phase 0 completed
- [ ] All dependencies installed
- [ ] Development servers running
- [ ] Database created

---

## 🔧 Step-by-Step Tasks

### Task 1.1: Enable UUID Support in PostgreSQL

Create migration:

```bash
rails generate migration EnablePgcrypto
```text

Edit `db/migrate/XXXXXX_enable_pgcrypto.rb`:

```ruby
class EnablePgcrypto < ActiveRecord::Migration[8.0]
  def change
    enable_extension 'pgcrypto' unless extension_enabled?('pgcrypto')
  end
end
```text

Run migration:

```bash
rails db:migrate
```text

**Verification:**

```bash
rails dbconsole
# In psql:
\dx  # Should show pgcrypto extension
\q
```text

---

### Task 1.2: Generate and Configure User Model

Generate Devise User model:

```bash
rails generate devise User
```text

Add additional fields:

```bash
rails generate migration AddFieldsToUsers \
  slug:uuid \
  first_name:string \
  last_name:string \
  is_system_admin:boolean
```text

Edit the migration to add defaults and indexes:

```ruby
class AddFieldsToUsers < ActiveRecord::Migration[8.0]
  def change
    add_column :users, :slug, :uuid, default: 'gen_random_uuid()', null: false
    add_column :users, :first_name, :string
    add_column :users, :last_name, :string
    add_column :users, :is_system_admin, :boolean, default: false, null: false
    
    add_index :users, :slug, unique: true
  end
end
```text

Run migrations:

```bash
rails db:migrate
```text

Edit `app/models/user.rb`:

```ruby
class User < ApplicationRecord
  # Devise modules
  devise :database_authenticatable, :registerable,
         :recoverable, :rememberable, :validatable,
         :jwt_authenticatable, jwt_revocation_strategy: JwtDenylist
  
  # Validations
  validates :slug, presence: true, uniqueness: true
  validates :email, presence: true, uniqueness: true
  
  # Associations
  has_many :academy_users, dependent: :destroy
  has_many :academies, through: :academy_users
  
  # Callbacks
  before_validation :generate_slug, on: :create
  
  # Instance methods
  def full_name
    "#{first_name} #{last_name}".strip.presence || email
  end
  
  def system_admin?
    is_system_admin
  end
  
  private
  
  def generate_slug
    self.slug ||= SecureRandom.uuid
  end
end
```text

---

### Task 1.3: Configure JWT Authentication

Create JwtDenylist model for token revocation:

```bash
rails generate model JwtDenylist \
  jti:string:index \
  exp:datetime
```text

Edit migration to add index:

```ruby
class CreateJwtDenylists < ActiveRecord::Migration[8.0]
  def change
    create_table :jwt_denylists do |t|
      t.string :jti, null: false
      t.datetime :exp, null: false
      
      t.timestamps
    end
    
    add_index :jwt_denylists, :jti, unique: true
  end
end
```text

Run migration:

```bash
rails db:migrate
```text

Edit `app/models/jwt_denylist.rb`:

```ruby
class JwtDenylist < ApplicationRecord
  include Devise::JWT::RevocationStrategies::Denylist
  
  self.table_name = 'jwt_denylists'
end
```text

Configure Devise JWT in `config/initializers/devise.rb`:

```ruby
Devise.setup do |config|
  # Existing configuration...
  
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
  
  config.navigational_formats = []
end
```text

Generate secret key:

```bash
# Generate and add to credentials
rails credentials:edit

# Add:
# devise_jwt_secret_key: <output from rake secret>
```text

---

### Task 1.4: Create Authentication Controllers

Create `app/controllers/auth/sessions_controller.rb`:

```ruby
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
        render json: {
          message: 'Logged out successfully'
        }, status: :ok
      else
        render json: {
          message: 'No active session'
        }, status: :unauthorized
      end
    end
  end
end
```text

Create `app/controllers/auth/registrations_controller.rb`:

```ruby
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
          message: 'Signed up successfully'
        }, status: :created
      else
        render json: {
          message: 'Sign up failed',
          errors: resource.errors.full_messages
        }, status: :unprocessable_entity
      end
    end
  end
end
```text

Update `config/routes.rb`:

```ruby
Rails.application.routes.draw do
  # Health check
  get '/health', to: proc { [200, {}, ['OK']] }
  get 'up', to: 'rails/health#show', as: :rails_health_check
  
  # Authentication
  devise_for :users, path: 'auth', defaults: { format: :json }, controllers: {
    sessions: 'auth/sessions',
    registrations: 'auth/registrations'
  }
  
  # API routes
  namespace :api do
    namespace :v1 do
      # Routes will be added here
    end
  end
end
```text

---

### Task 1.5: Implement Base Service Class

Create `app/shared/services/base_service.rb`:

```ruby
# app/shared/services/base_service.rb
class BaseService
  include ActiveModel::Validations
  include ActiveModel::AttributeAssignment
  
  attr_reader :result, :errors
  
  def initialize(params = {})
    @errors = ActiveModel::Errors.new(self)
    @result = nil
    assign_attributes(params) if params.any?
  end
  
  def call
    return failure("Service not implemented") unless respond_to?(:execute, true)
    
    validate_params
    return failure("Invalid parameters") if errors.any?
    
    begin
      ActiveRecord::Base.transaction do
        @result = execute
        success(@result)
      end
    rescue ActiveRecord::RecordInvalid => e
      failure(e.message, e.record&.errors)
    rescue ActiveRecord::RecordNotFound => e
      failure("Record not found: #{e.message}")
    rescue StandardError => e
      Rails.logger.error("Service Error in #{self.class.name}: #{e.message}")
      Rails.logger.error(e.backtrace.join("\n"))
      failure("An unexpected error occurred")
    end
  end
  
  def success?
    @success == true
  end
  
  def failure?
    !success?
  end
  
  private
  
  def success(data = nil)
    @success = true
    @result = data
    ServiceResult.new(success: true, data: data, errors: [])
  end
  
  def failure(message, validation_errors = nil)
    @success = false
    errors.add(:base, message)
    
    if validation_errors
      validation_errors.each do |field, field_errors|
        Array(field_errors).each { |error| errors.add(field, error) }
      end
    end
    
    ServiceResult.new(success: false, data: nil, errors: errors.full_messages)
  end
  
  def validate_params
    # Override in subclasses for parameter validation
    valid?
  end
  
  def execute
    # Override in subclasses for main business logic
    raise NotImplementedError, "Subclasses must implement #execute"
  end
end
```text

Create `app/shared/services/service_result.rb`:

```ruby
# app/shared/services/service_result.rb
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
  
  def error_message
    errors.first
  end
  
  def all_errors
    errors.join(', ')
  end
end
```text

---

### Task 1.6: Implement Model Concerns

Create `app/models/concerns/sluggable.rb`:

```ruby
# app/models/concerns/sluggable.rb
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

Create `app/models/concerns/auditable.rb`:

```ruby
# app/models/concerns/auditable.rb
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
      performed_at: Time.current
    )
  end
end
```text

Create AuditLog model:

```bash
rails generate model AuditLog \
  auditable:references{polymorphic} \
  user:references \
  action:string \
  changes:jsonb \
  performed_at:datetime
```text

Edit migration:

```ruby
class CreateAuditLogs < ActiveRecord::Migration[8.0]
  def change
    create_table :audit_logs do |t|
      t.references :auditable, polymorphic: true, null: false
      t.references :user, foreign_key: true
      t.string :action, null: false
      t.jsonb :changes, default: {}
      t.datetime :performed_at, null: false
      
      t.timestamps
    end
    
    add_index :audit_logs, [:auditable_type, :auditable_id]
    add_index :audit_logs, :performed_at
  end
end
```text

Run migration:

```bash
rails db:migrate
```text

---

### Task 1.7: Implement Base Controllers

Create `app/controllers/application_controller.rb`:

```ruby
class ApplicationController < ActionController::API
  before_action :authenticate_user!
  
  rescue_from ActiveRecord::RecordNotFound, with: :record_not_found
  rescue_from ActiveRecord::RecordInvalid, with: :record_invalid
  rescue_from StandardError, with: :internal_server_error
  
  protected
  
  def current_user
    @current_user ||= super || User.find_by(id: decoded_token&.dig('sub'))
  end
  
  private
  
  def decoded_token
    return unless request.headers['Authorization'].present?
    
    token = request.headers['Authorization'].split(' ').last
    JWT.decode(token, Rails.application.credentials.devise_jwt_secret_key, true, algorithm: 'HS256').first
  rescue JWT::DecodeError
    nil
  end
  
  def record_not_found(exception)
    render json: {
      error: 'RESOURCE_NOT_FOUND',
      message: exception.message
    }, status: :not_found
  end
  
  def record_invalid(exception)
    render json: {
      error: 'VALIDATION_ERROR',
      message: 'Validation failed',
      details: exception.record.errors.full_messages,
      field_errors: exception.record.errors.messages
    }, status: :unprocessable_entity
  end
  
  def internal_server_error(exception)
    Rails.logger.error("Internal Server Error: #{exception.message}")
    Rails.logger.error(exception.backtrace.join("\n"))
    
    render json: {
      error: 'INTERNAL_SERVER_ERROR',
      message: 'An unexpected error occurred'
    }, status: :internal_server_error
  end
end
```text

Create `app/controllers/api/v1/base_controller.rb`:

```ruby
module Api
  module V1
    class BaseController < ApplicationController
      include Pagy::Backend
      
      protected
      
      # Resource helpers
      def resource_class
        controller_name.classify.constantize
      end
      
      def resource_name
        resource_class.name.humanize
      end
      
      def find_resource
        resource_class.find_by!(slug: params[:id] || params[:slug])
      end
      
      def build_resource
        resource_class.new(resource_params)
      end
      
      # Pagination helpers
      def pagination_meta(pagy)
        {
          pagination: {
            current_page: pagy.page,
            per_page: pagy.items,
            total_pages: pagy.pages,
            total_count: pagy.count,
            has_next_page: pagy.next.present?,
            has_prev_page: pagy.prev.present?
          }
        }
      end
      
      def resource_meta(resource)
        {
          resource_type: resource.class.name.underscore,
          created_at: resource.created_at,
          updated_at: resource.updated_at
        }
      end
      
      # Error handling
      def render_validation_errors(resource)
        render json: {
          error: 'VALIDATION_ERROR',
          message: 'Validation failed',
          details: resource.errors.full_messages,
          field_errors: resource.errors.messages
        }, status: :unprocessable_entity
      end
      
      def render_service_error(result)
        render json: {
          error: 'SERVICE_ERROR',
          message: result.errors.first,
          details: result.errors
        }, status: determine_error_status(result.errors.first)
      end
      
      private
      
      def determine_error_status(error_message)
        case error_message.downcase
        when /not found/, /doesn't exist/
          :not_found
        when /unauthorized/, /permission/
          :forbidden
        when /duplicate/, /already exists/
          :conflict
        else
          :unprocessable_entity
        end
      end
    end
  end
end
```text

---

### Task 1.8: Configure Pundit Authorization

Create `app/shared/policies/application_policy.rb`:

```ruby
class ApplicationPolicy
  attr_reader :user, :record
  
  def initialize(user, record)
    @user = user
    @record = record
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
  
  class Scope
    def initialize(user, scope)
      @user = user
      @scope = scope
    end
    
    def resolve
      raise NotImplementedError, "Subclasses must implement #resolve"
    end
    
    private
    
    attr_reader :user, :scope
  end
  
  protected
  
  def system_admin?
    user&.system_admin?
  end
end
```text

Include Pundit in ApplicationController:

```ruby
class ApplicationController < ActionController::API
  include Pundit::Authorization
  
  before_action :authenticate_user!
  
  rescue_from Pundit::NotAuthorizedError, with: :user_not_authorized
  # ... existing code ...
  
  private
  
  def user_not_authorized(exception)
    render json: {
      error: 'PERMISSION_DENIED',
      message: 'You are not authorized to perform this action',
      context: {
        action: exception.query,
        resource: exception.record&.class&.name
      }
    }, status: :forbidden
  end
  
  # ... existing code ...
end
```text

---

### Task 1.9: Configure Sidekiq

Create `config/initializers/sidekiq.rb`:

```ruby
Sidekiq.configure_server do |config|
  config.redis = { url: ENV.fetch('REDIS_URL', 'redis://localhost:6379/0') }
end

Sidekiq.configure_client do |config|
  config.redis = { url: ENV.fetch('REDIS_URL', 'redis://localhost:6379/0') }
end
```text

Create `app/jobs/application_job.rb`:

```ruby
class ApplicationJob < ActiveJob::Base
  # Automatically retry jobs that encountered a deadlock
  retry_on ActiveRecord::Deadlocked
  
  # Most jobs are safe to ignore if the underlying records are no longer available
  discard_on ActiveJob::DeserializationError
  
  # Retry with exponential backoff
  retry_on StandardError, wait: :exponentially_longer, attempts: 3
  
  around_perform :set_locale
  
  private
  
  def set_locale(&block)
    I18n.with_locale(I18n.default_locale, &block)
  end
end
```text

Add Sidekiq routes (for development):

```ruby
# config/routes.rb
require 'sidekiq/web'

Rails.application.routes.draw do
  # Mount Sidekiq web UI (protect in production)
  mount Sidekiq::Web => '/sidekiq' if Rails.env.development?
  
  # ... existing routes ...
end
```text

---

### Task 1.10: Create Base Serializer

Create `app/serializers/application_serializer.rb`:

```ruby
class ApplicationSerializer < ActiveModel::Serializer
  attributes :id, :slug, :created_at, :updated_at
  
  def slug
    object.slug
  end
  
  def created_at
    object.created_at&.iso8601
  end
  
  def updated_at
    object.updated_at&.iso8601
  end
end
```text

---

### Task 1.11: Create Tests

Create `spec/shared/services/base_service_spec.rb`:

```ruby
require 'rails_helper'

# Test implementation of BaseService
class TestService < BaseService
  attr_accessor :name, :should_fail
  
  validates :name, presence: true
  
  private
  
  def execute
    raise StandardError, "Forced failure" if should_fail
    { name: name, processed: true }
  end
end

RSpec.describe BaseService, type: :service do
  describe '#call' do
    context 'with valid parameters' do
      it 'returns success result' do
        service = TestService.new(name: 'Test')
        result = service.call
        
        expect(result.success?).to be true
        expect(result.data[:name]).to eq('Test')
      end
    end
    
    context 'with invalid parameters' do
      it 'returns failure result' do
        service = TestService.new(name: nil)
        result = service.call
        
        expect(result.failure?).to be true
        expect(result.errors).to include('Invalid parameters')
      end
    end
    
    context 'when execute raises error' do
      it 'returns failure result' do
        service = TestService.new(name: 'Test', should_fail: true)
        result = service.call
        
        expect(result.failure?).to be true
        expect(result.errors).to include('An unexpected error occurred')
      end
    end
  end
end
```text

Create `spec/models/concerns/sluggable_spec.rb`:

```ruby
require 'rails_helper'

# Test model for Sluggable concern
class TestModel < ApplicationRecord
  self.table_name = 'users'  # Reuse existing table for test
  include Sluggable
end

RSpec.describe Sluggable, type: :model do
  let(:model) { TestModel.new }
  
  describe '#generate_slug' do
    it 'generates slug before validation' do
      model.email = 'test@test.com'
      model.valid?
      
      expect(model.slug).to be_present
      expect(model.slug).to match(/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}/)
    end
  end
  
  describe '#to_param' do
    it 'returns slug' do
      model.slug = 'test-slug-123'
      expect(model.to_param).to eq('test-slug-123')
    end
  end
end
```text

---

## ✅ Phase 1 Completion Checklist

- [ ] PostgreSQL UUID extension enabled
- [ ] User model with Devise configured
- [ ] JWT authentication working
- [ ] Authentication controllers implemented
- [ ] Base service class created and tested
- [ ] Model concerns (Sluggable, Auditable) created
- [ ] Base controllers (Application, Api::V1::Base) created
- [ ] Pundit authorization configured
- [ ] Sidekiq configured and running
- [ ] Base serializer created
- [ ] All tests passing

---

## 🧪 Verification Steps

```bash
# 1. Run migrations
rails db:migrate
rails db:migrate RAILS_ENV=test

# 2. Run tests
bundle exec rspec spec/shared/
bundle exec rspec spec/models/concerns/

# 3. Test authentication
# Register a user
curl -X POST http://localhost:3000/auth/sign_up \
  -H "Content-Type: application/json" \
  -d '{
    "user": {
      "email": "test@example.com",
      "password": "password123",
      "password_confirmation": "password123"
    }
  }'

# Login
curl -X POST http://localhost:3000/auth/sign_in \
  -H "Content-Type: application/json" \
  -d '{
    "user": {
      "email": "test@example.com",
      "password": "password123"
    }
  }'

# Copy the token from response

# 4. Test authenticated request
TOKEN="your_jwt_token_here"
curl http://localhost:3000/health \
  -H "Authorization: Bearer $TOKEN"

# 5. Check Sidekiq dashboard
open http://localhost:3000/sidekiq

# 6. Verify all services running
./bin/dev
# Should start: web, worker, redis
```text

---

## 📊 Phase 1 Metrics

Upon completion:

- **Models Created:** 3 (User, JwtDenylist, AuditLog)
- **Base Classes:** 5 (BaseService, BaseController, ApplicationPolicy, etc.)
- **Concerns:** 2 (Sluggable, Auditable)
- **Tests:** 10+ specs
- **Authentication:** Full JWT flow
- **Authorization:** Pundit foundation

---

## ➡️ Next Steps

Proceed to:

- **[Phase 2: Academy Management](./PHASE_2_ACADEMY.md)**
  - Create Academy model (tenant)
  - Implement academy services
  - Build academy API endpoints

---

## 📚 Infrastructure Usage Guide

### Authentication Usage

#### User Registration

```ruby
# In your tests or seeds
user = User.create!(
  email: 'admin@example.com',
  password: 'password123',
  password_confirmation: 'password123',
  first_name: 'Admin',
  last_name: 'User',
  is_system_admin: true
)
```text

#### API Authentication Flow

```bash
# 1. Register new user
curl -X POST http://localhost:3000/auth/sign_up \
  -H "Content-Type: application/json" \
  -d '{
    "user": {
      "email": "newuser@example.com",
      "password": "SecurePass123!",
      "password_confirmation": "SecurePass123!",
      "first_name": "John",
      "last_name": "Doe"
    }
  }'

# 2. Login and get JWT token
curl -X POST http://localhost:3000/auth/sign_in \
  -H "Content-Type: application/json" \
  -d '{
    "user": {
      "email": "newuser@example.com",
      "password": "SecurePass123!"
    }
  }' -i

# Response includes token in Authorization header:
# Authorization: Bearer eyJhbGciOiJIUzI1NiJ9...

# 3. Use token for authenticated requests
TOKEN="eyJhbGciOiJIUzI1NiJ9..."
curl http://localhost:3000/api/v1/players \
  -H "Authorization: Bearer $TOKEN"

# 4. Logout (revokes token)
curl -X DELETE http://localhost:3000/auth/sign_out \
  -H "Authorization: Bearer $TOKEN"
```text

### Authorization Usage

#### Setting Up Academy User Permissions

```ruby
# Create academy
academy = Academy.create!(
  name: 'FC Barcelona Academy',
  owner_name: 'John Smith',
  owner_email: 'owner@fcbarcelona.com',
  # ... other fields
)

# Assign user to academy with role
academy_user = AcademyUser.create!(
  user: user,
  academy: academy,
  role: :admin,  # or :coach, :assistant_coach, :viewer
  is_active: true
)

# Check permissions
academy_user.can?(:read)    # => true
academy_user.can?(:create)  # => true
academy_user.can?(:update)  # => true
academy_user.can?(:delete)  # => true (for admin)
```text

#### Using Policies in Controllers

```ruby
class PlayersController < ApplicationController
  def index
    # Authorize the action
    authorize Player
    
    # Use policy scope to filter records
    @players = policy_scope(Player).includes(:position)
    render json: @players
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
end
```text

### Background Jobs Usage

#### Creating and Enqueuing Jobs

```ruby
# Immediate execution
PlayerRegistrationJob.perform_now(player.id, academy.id)

# Enqueue for background processing
PlayerRegistrationJob.perform_later(player.id, academy.id)

# Delay execution
PlayerRegistrationJob.set(wait: 1.hour).perform_later(player.id, academy.id)

# Schedule for specific time
TrainingReminderJob.set(wait_until: training.date - 2.hours).perform_later(training.id)
```text

#### Custom Job Example

```ruby
# app/jobs/generate_report_job.rb
class GenerateReportJob < ApplicationJob
  queue_as :low
  
  retry_on StandardError, wait: :exponentially_longer, attempts: 3
  
  def perform(academy_id, report_type, user_id)
    ActsAsTenant.with_tenant(Academy.find(academy_id)) do
      report = ReportService.new(
        type: report_type,
        academy_id: academy_id
      ).call
      
      # Email report to user
      ReportMailer.send_report(user_id, report).deliver_now
    end
  end
end
```text

#### Monitoring Jobs

```ruby
# Check job status in console
stats = Sidekiq::Stats.new
puts "Processed: #{stats.processed}"
puts "Failed: #{stats.failed}"
puts "Enqueued: #{stats.enqueued}"

# View failed jobs
Sidekiq::RetrySet.new.each do |job|
  puts "#{job.klass} - #{job.error_message}"
end

# Retry all failed jobs
Sidekiq::RetrySet.new.retry_all

# Clear failed jobs
Sidekiq::RetrySet.new.clear
```text

### Multi-Tenancy Usage

#### Setting Tenant Context

```ruby
# In controllers
class ApplicationController < ActionController::API
  before_action :set_tenant
  
  private
  
  def set_tenant
    # Priority: URL param > Header > User default
    academy_slug = params[:academy_slug] || 
                   request.headers['X-Academy-Context'] ||
                   current_user&.academy_users&.first&.academy&.slug
    
    if academy_slug
      academy = Academy.find_by!(slug: academy_slug)
      ActsAsTenant.current_tenant = academy
    end
  end
end
```text

#### Tenant-Aware Queries

```ruby
# Automatically scoped to current tenant
Player.all                    # Only current academy's players
Team.where(is_active: true)   # Only current academy's active teams

# Explicit tenant context
ActsAsTenant.with_tenant(academy) do
  players = Player.all
  # Operations here are scoped to 'academy'
end

# System admin: query across all tenants
ActsAsTenant.without_tenant do
  all_players = Player.all  # All players from all academies
  total_count = Player.count
end
```text

### Service Pattern Usage

#### Creating a Service

```ruby
# app/services/player_registration_service.rb
class PlayerRegistrationService < BaseService
  attr_accessor :first_name, :last_name, :age, :position_id
  
  validates :first_name, :last_name, presence: true
  validates :age, numericality: { greater_than: 4, less_than: 100 }
  
  private
  
  def execute
    player = Player.create!(
      first_name: first_name,
      last_name: last_name,
      age: age,
      position_id: position_id,
      academy: ActsAsTenant.current_tenant
    )
    
    # Enqueue welcome email
    PlayerRegistrationJob.perform_later(player.id, player.academy_id)
    
    player
  end
end
```text

#### Using the Service

```ruby
# In controller
def create
  service = PlayerRegistrationService.new(player_params)
  result = service.call
  
  if result.success?
    render json: result.data, status: :created
  else
    render json: { errors: result.errors }, status: :unprocessable_entity
  end
end

# In tests
it 'registers a player successfully' do
  service = PlayerRegistrationService.new(
    first_name: 'John',
    last_name: 'Doe',
    age: 12,
    position_id: position.id
  )
  
  result = service.call
  
  expect(result.success?).to be true
  expect(result.data).to be_a(Player)
  expect(result.data.first_name).to eq('John')
end
```text

### Testing Infrastructure

#### Testing Authentication

```ruby
# spec/support/auth_helpers.rb
module AuthHelpers
  def auth_headers(user)
    token = Warden::JWTAuth::UserEncoder.new.call(user, :user, nil).first
    { 'Authorization' => "Bearer #{token}" }
  end
end

RSpec.configure do |config|
  config.include AuthHelpers, type: :request
end

# In request specs
RSpec.describe 'Players API', type: :request do
  let(:user) { create(:user) }
  let(:academy) { create(:academy) }
  let!(:academy_user) { create(:academy_user, user: user, academy: academy, role: :admin) }
  
  describe 'GET /api/v1/:academy_slug/players' do
    it 'returns players for authenticated user' do
      get "/api/v1/#{academy.slug}/players", headers: auth_headers(user)
      
      expect(response).to have_http_status(:ok)
      expect(JSON.parse(response.body)).to be_an(Array)
    end
    
    it 'returns 401 for unauthenticated request' do
      get "/api/v1/#{academy.slug}/players"
      
      expect(response).to have_http_status(:unauthorized)
    end
  end
end
```text

#### Testing Authorization

```ruby
# spec/policies/player_policy_spec.rb
RSpec.describe PlayerPolicy do
  subject { described_class.new(user, player) }
  
  let(:academy) { create(:academy) }
  let(:player) { create(:player, academy: academy) }
  
  context 'for system admin' do
    let(:user) { create(:user, is_system_admin: true) }
    
    it { is_expected.to permit_action(:index) }
    it { is_expected.to permit_action(:show) }
    it { is_expected.to permit_action(:create) }
    it { is_expected.to permit_action(:update) }
    it { is_expected.to permit_action(:destroy) }
  end
  
  context 'for academy admin' do
    let(:user) { create(:user) }
    let!(:academy_user) { create(:academy_user, user: user, academy: academy, role: :admin) }
    
    before { ActsAsTenant.current_tenant = academy }
    
    it { is_expected.to permit_action(:index) }
    it { is_expected.to permit_action(:show) }
    it { is_expected.to permit_action(:create) }
    it { is_expected.to permit_action(:update) }
    it { is_expected.to permit_action(:destroy) }
  end
  
  context 'for viewer' do
    let(:user) { create(:user) }
    let!(:academy_user) { create(:academy_user, user: user, academy: academy, role: :viewer) }
    
    before { ActsAsTenant.current_tenant = academy }
    
    it { is_expected.to permit_action(:index) }
    it { is_expected.to permit_action(:show) }
    it { is_expected.to forbid_action(:create) }
    it { is_expected.to forbid_action(:update) }
    it { is_expected.to forbid_action(:destroy) }
  end
end
```text

#### Testing Background Jobs

```ruby
# spec/jobs/player_registration_job_spec.rb
RSpec.describe PlayerRegistrationJob, type: :job do
  include ActiveJob::TestHelper
  
  let(:academy) { create(:academy) }
  let(:player) { create(:player, academy: academy) }
  
  it 'enqueues the job' do
    expect {
      PlayerRegistrationJob.perform_later(player.id, academy.id)
    }.to have_enqueued_job(PlayerRegistrationJob)
      .with(player.id, academy.id)
      .on_queue('default')
  end
  
  it 'sends welcome email' do
    expect(PlayerMailer).to receive(:welcome_email)
      .with(player)
      .and_return(double(deliver_now: true))
    
    PlayerRegistrationJob.perform_now(player.id, academy.id)
  end
  
  it 'creates audit log' do
    expect {
      PlayerRegistrationJob.perform_now(player.id, academy.id)
    }.to change(AuditLog, :count).by(1)
  end
end
```text

### Common Troubleshooting

#### JWT Token Issues

```bash
# Token expired
# Solution: Request new token via sign_in

# Token not included
# Solution: Ensure Authorization header is set:
# Authorization: Bearer <token>

# Invalid token
# Solution: Check DEVISE_JWT_SECRET_KEY is set correctly
# Generate new secret: rake secret
```text

#### Multi-Tenancy Issues

```ruby
# No tenant set
# Solution: Ensure ActsAsTenant.current_tenant is set before queries

# Cross-tenant access
# Solution: Check authorization and tenant context in controller

# System admin bypass
ActsAsTenant.without_tenant do
  # Query across all tenants (use sparingly)
end
```text

#### Sidekiq Issues

```bash
# Jobs not processing
# Check Redis is running: redis-cli ping
# Check Sidekiq is running: ps aux | grep sidekiq
# Start Sidekiq: bundle exec sidekiq

# View logs
tail -f log/sidekiq.log

# Clear stuck jobs
bundle exec rails console
Sidekiq::Queue.new('default').clear
```text

---

**Phase 1 Status:** Ready to begin  
**Estimated Completion:** 1-2 weeks  
**Difficulty:** ⭐⭐⭐ (Medium-Hard)
