# Diquis - Infrastructure Setup Guide

This document provides a comprehensive guide to the infrastructure components of Diquis Football Academy Management System.

## Table of Contents

1. [Authentication Setup](#authentication-setup)
2. [Authorization Setup](#authorization-setup)
3. [Background Jobs Setup](#background-jobs-setup)
4. [Multi-Tenancy Setup](#multi-tenancy-setup)
5. [CI/CD Pipeline](#cicd-pipeline)
6. [Deployment](#deployment)
7. [Monitoring & Maintenance](#monitoring--maintenance)

---

## Authentication Setup

### Overview

Diquis uses **Devise** with **JWT** tokens for stateless API authentication.

### Configuration

#### 1. Install Dependencies

```ruby
# Gemfile
gem "devise", "~> 4.9"
gem "devise-jwt", "~> 0.12"
```text

```bash
bundle install
rails generate devise:install
rails generate devise User
```text

#### 2. Configure JWT

```ruby
# config/initializers/devise.rb
Devise.setup do |config|
  config.mailer_sender = 'noreply@diquis.com'
  
  config.jwt do |jwt|
    jwt.secret = Rails.application.credentials.devise_jwt_secret_key || ENV['DEVISE_JWT_SECRET_KEY']
    jwt.dispatch_requests = [['POST', %r{^/auth/sign_in$}]]
    jwt.revocation_requests = [['DELETE', %r{^/auth/sign_out$}]]
    jwt.expiration_time = 24.hours.to_i
  end
  
  config.navigational_formats = []
end
```text

#### 3. Generate Secret Key

```bash
# Generate secret
rake secret

# Add to credentials
rails credentials:edit
# Add: devise_jwt_secret_key: <your_generated_secret>

# Or use environment variable
export DEVISE_JWT_SECRET_KEY=<your_generated_secret>
```text

#### 4. Create JwtDenylist Model

```bash
rails generate model JwtDenylist jti:string:index exp:datetime
rails db:migrate
```text

```ruby
# app/models/jwt_denylist.rb
class JwtDenylist < ApplicationRecord
  include Devise::JWT::RevocationStrategies::Denylist
  self.table_name = 'jwt_denylists'
end
```text

#### 5. Update User Model

```ruby
# app/models/user.rb
class User < ApplicationRecord
  devise :database_authenticatable, :registerable,
         :recoverable, :rememberable, :validatable,
         :jwt_authenticatable, jwt_revocation_strategy: JwtDenylist
  
  has_many :academy_users, dependent: :destroy
  has_many :academies, through: :academy_users
  
  def jwt_payload
    { 'user_id' => id, 'email' => email, 'is_system_admin' => is_system_admin }
  end
end
```text

### Usage

#### Registration

```bash
curl -X POST http://localhost:3000/auth/sign_up \
  -H "Content-Type: application/json" \
  -d '{
    "user": {
      "email": "user@example.com",
      "password": "password123",
      "password_confirmation": "password123",
      "first_name": "John",
      "last_name": "Doe"
    }
  }'
```text

#### Login

```bash
curl -X POST http://localhost:3000/auth/sign_in \
  -H "Content-Type: application/json" \
  -d '{
    "user": {
      "email": "user@example.com",
      "password": "password123"
    }
  }' -i
```text

Response includes JWT token in `Authorization` header:
```text
Authorization: Bearer eyJhbGciOiJIUzI1NiJ9...
```text

#### Authenticated Requests

```bash
TOKEN="eyJhbGciOiJIUzI1NiJ9..."
curl http://localhost:3000/api/v1/players \
  -H "Authorization: Bearer $TOKEN"
```text

#### Logout

```bash
curl -X DELETE http://localhost:3000/auth/sign_out \
  -H "Authorization: Bearer $TOKEN"
```text

---

## Authorization Setup

### Overview

Diquis uses **Pundit** for policy-based authorization with role-based access control (RBAC).

### Configuration

#### 1. Install Pundit

```ruby
# Gemfile
gem "pundit", "~> 2.4"
```text

```bash
bundle install
rails generate pundit:install
```text

#### 2. Configure ApplicationController

```ruby
# app/controllers/application_controller.rb
class ApplicationController < ActionController::API
  include Pundit::Authorization
  
  before_action :authenticate_user!
  
  rescue_from Pundit::NotAuthorizedError, with: :user_not_authorized
  
  private
  
  def user_not_authorized
    render json: { error: 'You are not authorized to perform this action' }, 
           status: :forbidden
  end
end
```text

#### 3. Create Base Policy

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

### Roles & Permissions

#### AcademyUser Model

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

### Usage in Controllers

```ruby
class PlayersController < ApplicationController
  def index
    authorize Player
    @players = policy_scope(Player).includes(:position)
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
end
```text

---

## Background Jobs Setup

### Overview

Diquis uses **Sidekiq** for background job processing with **Redis** as the backend.

### Configuration

#### 1. Install Dependencies

```ruby
# Gemfile
gem "sidekiq", "~> 7.3"
gem "sidekiq-cron", "~> 1.12"
gem "redis", "~> 5.0"
```text

```bash
bundle install
```text

#### 2. Configure Sidekiq

```yaml
# config/sidekiq.yml
:concurrency: 5
:timeout: 25

:queues:
  - [critical, 10]
  - [default, 5]
  - [mailers, 3]
  - [low, 1]

:scheduler:
  :enabled: true

production:
  :concurrency: 10
  :timeout: 25
```text

#### 3. Configure Redis

```ruby
# config/initializers/sidekiq.rb
Sidekiq.configure_server do |config|
  config.redis = { url: ENV.fetch('REDIS_URL', 'redis://localhost:6379/0') }
end

Sidekiq.configure_client do |config|
  config.redis = { url: ENV.fetch('REDIS_URL', 'redis://localhost:6379/0') }
end
```text

#### 4. Create Application Job

```ruby
# app/jobs/application_job.rb
class ApplicationJob < ActiveJob::Base
  retry_on StandardError, wait: :exponentially_longer, attempts: 5
  discard_on ActiveJob::DeserializationError
  
  queue_as :default
  
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

### Running Sidekiq

#### Development

```bash
# Start Redis
redis-server

# Start Sidekiq
bundle exec sidekiq

# Or use Procfile.dev
./bin/dev
```text

#### Production

```bash
# Using systemd
sudo systemctl start sidekiq
sudo systemctl enable sidekiq

# Or using Kamal
# Sidekiq runs automatically with the application
```text

### Creating Jobs

```ruby
# app/jobs/player_registration_job.rb
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

### Enqueuing Jobs

```ruby
# Immediate
PlayerRegistrationJob.perform_now(player.id, academy.id)

# Background
PlayerRegistrationJob.perform_later(player.id, academy.id)

# Delayed
PlayerRegistrationJob.set(wait: 1.hour).perform_later(player.id, academy.id)

# Scheduled
TrainingReminderJob.set(wait_until: training.date - 2.hours).perform_later(training.id)
```text

### Monitoring

Access Sidekiq dashboard at `/sidekiq`:

```ruby
# config/routes.rb
require 'sidekiq/web'
require 'sidekiq/cron/web'

Rails.application.routes.draw do
  authenticate :user, ->(u) { u.system_admin? } do
    mount Sidekiq::Web => '/sidekiq'
  end
end
```text

---

## Multi-Tenancy Setup

### Overview

Diquis uses **ActsAsTenant** for multi-tenancy with Academy as the tenant model.

### Configuration

#### 1. Install ActsAsTenant

```ruby
# Gemfile
gem "acts_as_tenant", "~> 1.0"
```text

```bash
bundle install
```text

#### 2. Configure Tenant Model

```ruby
# app/models/academy.rb
class Academy < ApplicationRecord
  acts_as_tenant(:academy)
  
  has_many :players
  has_many :teams
  has_many :trainings
  has_many :academy_users
  has_many :users, through: :academy_users
end
```text

#### 3. Configure Tenant-Scoped Models

```ruby
# app/models/player.rb
class Player < ApplicationRecord
  acts_as_tenant(:academy)
  
  belongs_to :academy
  # ... other associations
end
```text

#### 4. Set Tenant Context in Controllers

```ruby
# app/controllers/application_controller.rb
class ApplicationController < ActionController::API
  before_action :set_tenant
  
  private
  
  def set_tenant
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

### Usage

#### Tenant-Scoped Queries

```ruby
# Automatically scoped to current tenant
Player.all                    # Only current academy's players
Team.where(is_active: true)   # Only current academy's teams

# Explicit tenant context
ActsAsTenant.with_tenant(academy) do
  players = Player.all
end

# Cross-tenant (system admin only)
ActsAsTenant.without_tenant do
  all_players = Player.all
end
```text

---

## CI/CD Pipeline

### Overview

Diquis uses GitHub Actions for continuous integration.

### Workflow File

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
      - name: Security scan
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
      - name: Lint code
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

### Running CI Locally

```bash
# Security scan
bin/brakeman --no-pager

# Lint
bin/rubocop

# Tests
bundle exec rspec

# All checks
./script/verify
```text

---

## Deployment

### Overview

Diquis uses **Kamal** for Docker-based deployments.

### Prerequisites

```bash
# Install Kamal
gem install kamal

# Install Docker
# Follow instructions at https://docs.docker.com/get-docker/

# Set up servers with SSH access
# Ensure Docker is installed on servers
```text

### Configuration

```yaml
# config/deploy.yml
service: diquis

image: your-dockerhub-username/diquis

servers:
  web:
    hosts:
      - your-server-ip
    labels:
      traefik.http.routers.diquis.rule: Host(`yourdomain.com`)

registry:
  username: your-dockerhub-username
  password:
    - KAMAL_REGISTRY_PASSWORD

env:
  secret:
    - RAILS_MASTER_KEY
    - DATABASE_URL
    - REDIS_URL
  clear:
    RAILS_ENV: production
```text

### Deployment Commands

```bash
# Initial setup
kamal setup

# Deploy
kamal deploy

# Rollback
kamal rollback

# View logs
kamal app logs --follow

# SSH into container
kamal app exec --interactive bash
```text

### Environment Variables

Required for production:

```bash
RAILS_MASTER_KEY=<your_master_key>
DEVISE_JWT_SECRET_KEY=<jwt_secret>
DATABASE_URL=postgresql://user:password@host:5432/db_name
REDIS_URL=redis://host:6379/0
```text

---

## Monitoring & Maintenance

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

### Monitoring Sidekiq

```bash
# View queue stats
bundle exec rails console
stats = Sidekiq::Stats.new
puts "Processed: #{stats.processed}"
puts "Failed: #{stats.failed}"
puts "Enqueued: #{stats.enqueued}"

# View failed jobs
Sidekiq::RetrySet.new.each do |job|
  puts "#{job.klass} - #{job.error_message}"
end

# Retry all failed
Sidekiq::RetrySet.new.retry_all

# Clear queue
Sidekiq::Queue.new('default').clear
```text

### Database Maintenance

```bash
# Backup database
pg_dump -h localhost -U username database_name > backup.sql

# Restore database
psql -h localhost -U username database_name < backup.sql

# Vacuum analyze (optimize)
rails runner "ActiveRecord::Base.connection.execute('VACUUM ANALYZE')"
```text

### Log Management

```bash
# View application logs
tail -f log/production.log

# View Sidekiq logs
tail -f log/sidekiq.log

# Rotate logs
logrotate -f /etc/logrotate.d/rails

# Or use Kamal
kamal app logs --follow
kamal app logs --since 1h
```text

---

## Troubleshooting

### Authentication Issues

**Token expired**
```bash
# Request new token via sign_in endpoint
```text

**Invalid token**
```bash
# Check DEVISE_JWT_SECRET_KEY matches between environments
# Regenerate: rake secret
```text

### Authorization Issues

**Forbidden errors**
```ruby
# Check user has correct role
user.academy_users.find_by(academy: academy).role

# Check permissions
academy_user.can?(:create)
```text

### Background Job Issues

**Jobs not processing**
```bash
# Check Redis
redis-cli ping

# Check Sidekiq
ps aux | grep sidekiq

# Start Sidekiq
bundle exec sidekiq
```text

**Stuck jobs**
```ruby
# Clear queue
Sidekiq::Queue.new('default').clear

# Retry failed
Sidekiq::RetrySet.new.retry_all
```text

### Multi-Tenancy Issues

**No tenant set**
```ruby
# Ensure tenant context is set
ActsAsTenant.current_tenant
# => #<Academy id: 1, slug: "...">

# Set manually if needed
ActsAsTenant.current_tenant = Academy.find_by(slug: slug)
```text

**Cross-tenant access errors**
```ruby
# For system admin operations
ActsAsTenant.without_tenant do
  # Operations here
end
```text

---

## Additional Resources

- [Devise Documentation](https://github.com/heartcombo/devise)
- [Devise JWT Documentation](https://github.com/waiting-for-dev/devise-jwt)
- [Pundit Documentation](https://github.com/varvet/pundit)
- [Sidekiq Documentation](https://github.com/sidekiq/sidekiq)
- [ActsAsTenant Documentation](https://github.com/ErwinM/acts_as_tenant)
- [Kamal Documentation](https://kamal-deploy.org/)
- [Diquis Architecture Documentation](./ARCHITECTURE.md)
- [Phase 1 Infrastructure Guide](./PHASE_1_INFRASTRUCTURE.md)

---

**Last Updated:** October 2024  
**Maintainer:** Diquis Development Team
