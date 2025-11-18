# Diquis - Setup Guide

## Prerequisites

### Required Software

- **Ruby:** 3.3.0 or higher
- **Rails:** 8.0.3 or higher
- **PostgreSQL:** 15 or higher
- **Redis:** 7.0 or higher
- **Node.js:** 18+ (for frontend)
- **Git:** Latest version

### Development Tools

- **Overmind** (recommended) or **Foreman** for process management
- **ImageMagick** or **libvips** for image processing
- **tmux** (required for Overmind on Linux)

### macOS Installation

```bash
# Install Homebrew (if not already installed)
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Install dependencies
brew install ruby@3.3
brew install postgresql@15
brew install redis
brew install imagemagick
brew install tmux
brew install overmind

# Start services
brew services start postgresql@15
brew services start redis
```text

### Ubuntu/Debian Installation

```bash
# Update package list
sudo apt update

# Install Ruby (via rbenv recommended)
curl -fsSL https://github.com/rbenv/rbenv-installer/raw/HEAD/bin/rbenv-installer | bash
rbenv install 3.3.0
rbenv global 3.3.0

# Install PostgreSQL
sudo apt install postgresql postgresql-contrib libpq-dev

# Install Redis
sudo apt install redis-server

# Install ImageMagick
sudo apt install imagemagick libvips-dev

# Install tmux (for Overmind)
sudo apt install tmux

# Install Overmind
wget https://github.com/DarthSim/overmind/releases/latest/download/overmind-v2.4.0-linux-amd64.gz
gunzip overmind-v2.4.0-linux-amd64.gz
chmod +x overmind-v2.4.0-linux-amd64
sudo mv overmind-v2.4.0-linux-amd64 /usr/local/bin/overmind

# Start services
sudo systemctl start postgresql
sudo systemctl enable postgresql
sudo systemctl start redis-server
sudo systemctl enable redis-server
```text

---

## Project Initialization

### 1. Create New Rails Application

```bash
# Create new Rails API application
rails new diquis --api --database=postgresql --skip-test

# Navigate to project directory
cd diquis
```text

### 2. Configure Gemfile

Replace the contents of `Gemfile` with:

```ruby
source 'https://rubygems.org'
git_source(:github) { |repo| "https://github.com/#{repo}.git" }

ruby '3.3.0'

# Rails and core gems
gem 'rails', '~> 8.0.3'
gem 'pg', '~> 1.1'
gem 'puma', '~> 6.0'

# Multi-tenancy
gem 'acts_as_tenant'

# Authentication & Authorization
gem 'devise'
gem 'devise-jwt'
gem 'pundit'

# API and serialization
gem 'active_model_serializers', '~> 0.10'
gem 'jsonapi-serializer'
gem 'kaminari'
gem 'ransack'
gem 'pagy'

# API Documentation
gem 'rswag'
gem 'rswag-api'
gem 'rswag-ui'

# Background jobs
gem 'sidekiq'

# Caching and storage
gem 'redis'
gem 'image_processing', '~> 1.2'

# Rails 8.0+ specific gems
gem 'solid_cable'
gem 'solid_cache'
gem 'solid_queue'

# Utilities
gem 'friendly_id'
gem 'bootsnap', require: false
gem 'discard', '~> 1.2'

# CORS for frontend
gem 'rack-cors'

group :development, :test do
  gem 'debug'
  gem 'rspec-rails'
  gem 'factory_bot_rails'
  gem 'faker'
  gem 'rswag-specs'
  gem 'shoulda-matchers'
  gem 'database_cleaner-active_record'
end

group :development do
  gem 'annotate'
  gem 'brakeman'
  gem 'rubocop-rails', require: false
  gem 'rubocop-rspec', require: false
  gem 'bullet'
  gem 'foreman'
  gem 'listen'
end
```text

### 3. Install Dependencies

```bash
# Install gems
bundle install

# Install RSpec
rails generate rspec:install

# Install Devise
rails generate devise:install
rails generate devise User

# Install Pundit
rails generate pundit:install

# Install Rswag
rails generate rswag:install
```text

---

## Database Configuration

### 1. Configure database.yml

Edit `config/database.yml`:

```yaml
default: &default
  adapter: postgresql
  encoding: unicode
  pool: <%= ENV.fetch("RAILS_MAX_THREADS") { 5 } %>
  username: <%= ENV.fetch("POSTGRES_USER") { "postgres" } %>
  password: <%= ENV.fetch("POSTGRES_PASSWORD") { "postgres" } %>
  host: <%= ENV.fetch("POSTGRES_HOST") { "localhost" } %>

development:
  <<: *default
  database: diquis_development

test:
  <<: *default
  database: diquis_test

production:
  <<: *default
  database: diquis_production
  username: diquis
  password: <%= ENV['DIQUIS_DATABASE_PASSWORD'] %>
```text

### 2. Create Databases

```bash
# Create databases
rails db:create

# Verify connection
rails db:migrate
```text

---

## Application Configuration

### 1. Configure Application (config/application.rb)

```ruby
require_relative "boot"
require "rails/all"

Bundler.require(*Rails.groups)

module Diquis
  class Application < Rails::Application
    config.load_defaults 8.0
    config.api_only = true
    
    # Time zone
    config.time_zone = 'UTC'
    config.active_record.default_timezone = :utc
    
    # Multi-tenancy configuration
    config.acts_as_tenant_default_class_name = 'Academy'
    
    # Background jobs
    config.active_job.queue_adapter = :sidekiq
    
    # Active Model Serializers
    config.active_model_serializers.config.adapter = :json_api
    config.active_model_serializers.config.key_transform = :underscore
    config.active_model_serializers.config.jsonapi_include_toplevel_object = true
    config.active_model_serializers.config.jsonapi_version = "1.0"
    
    # Autoload paths for vertical slices
    config.autoload_paths += %W[
      #{config.root}/app/slices
      #{config.root}/app/shared/services
      #{config.root}/app/shared/policies
    ]
    config.autoload_paths += Dir[Rails.root.join('app', 'slices', '*')]
    
    # CORS configuration
    config.middleware.insert_before 0, Rack::Cors do
      allow do
        origins ENV.fetch('CORS_ORIGINS', 'http://localhost:3000').split(',')
        resource '*',
          headers: :any,
          methods: [:get, :post, :put, :patch, :delete, :options, :head],
          credentials: true
      end
    end
  end
end
```text

### 2. Configure Routes (config/routes.rb)

```ruby
Rails.application.routes.draw do
  # API Documentation
  mount Rswag::Ui::Engine => '/api-docs'
  mount Rswag::Api::Engine => '/api-docs'
  
  # Sidekiq Web UI (protect in production)
  require 'sidekiq/web'
  mount Sidekiq::Web => '/sidekiq' if Rails.env.development?
  
  # Health check
  get '/health', to: proc { [200, {}, ['OK']] }
  
  # Authentication routes (Devise)
  devise_for :users, path: 'auth', defaults: { format: :json }, controllers: {
    sessions: 'auth/sessions',
    registrations: 'auth/registrations',
    passwords: 'auth/passwords'
  }
  
  # API routes
  namespace :api do
    namespace :v1 do
      # Academy Management
      resources :academies, param: :slug, controller: 'academy_management/academies' do
        member do
          post :join
          patch :activate
        end
      end
      
      # Academy-scoped routes
      scope '(:academy_slug)', academy_slug: /[a-f0-9\-]{36}/, defaults: { academy_slug: nil } do
        # Player Management
        resources :players, param: :slug, controller: 'player_management/players' do
          collection do
            get :search
            get :export
          end
          member do
            patch :activate
            get :statistics
          end
        end
        
        # Team Management
        resources :teams, param: :slug, controller: 'team_management/teams' do
          member do
            patch :activate
            post :add_player
            delete 'players/:player_slug', action: :remove_player
          end
          
          # Training Management
          resources :trainings, param: :slug, controller: 'training_management/trainings' do
            member do
              post :bulk_attendance
              get :attendance_report
            end
            collection do
              get :calendar
            end
          end
        end
        
        # Shared Resources (academy-specific)
        resources :skills, param: :slug, controller: 'shared_resources/skills'
        resources :positions, param: :slug, controller: 'shared_resources/positions'
      end
      
      # Global shared resources
      resources :categories, param: :slug, controller: 'shared_resources/categories'
      resources :divisions, param: :slug, controller: 'shared_resources/divisions'
    end
  end
end
```text

---

## Directory Structure Setup

### 1. Create Slice Directories

```bash
# Create main slices directory
mkdir -p app/slices

# Create individual slices
mkdir -p app/slices/academy_management/{controllers,services,models,serializers,policies,validators}
mkdir -p app/slices/player_management/{controllers,services,models,serializers,policies,validators}
mkdir -p app/slices/team_management/{controllers,services,models,serializers,policies,validators}
mkdir -p app/slices/training_management/{controllers,services,models,serializers,policies,validators}
mkdir -p app/slices/shared_resources/{controllers,services,models,serializers}

# Create shared directory
mkdir -p app/shared/{services,policies,models/concerns}

# Create jobs directory by slice
mkdir -p app/jobs/academy_management
mkdir -p app/jobs/player_management
mkdir -p app/jobs/training_management

# Create serializers directory
mkdir -p app/serializers

# Create documentation directory
mkdir -p docs

# Create swagger directory
mkdir -p swagger/v1
```text

### 2. Process Management with Overmind

Diquis uses **Overmind** for development process management, providing better process isolation and tmux-based interface compared to Foreman.

**Why Overmind?**

- Better process isolation and restart capabilities
- tmux-based interface for easy process monitoring
- Colored output with process names
- Automatic process restart on crash
- Environment variable management

**Process Configuration Files:**

**Procfile.dev** - Defines development processes:

```plaintext
# Procfile for Diquis Football Academy
# Use with Overmind: overmind start

web: bundle exec rails server -p 3000
worker: bundle exec sidekiq -C config/sidekiq.yml
css: bundle exec tailwindcss -i ./app/assets/stylesheets/application.tailwind.css -o ./app/assets/builds/application.css --watch
```text

**config/sidekiq.yml** - Sidekiq queue configuration:

```yaml
# Sidekiq configuration for Diquis Football Academy

# Basic configuration
:concurrency: 5
:timeout: 25

# Queues with priorities (higher numbers = higher priority)
:queues:
  - [critical, 10]
  - [default, 5]
  - [mailers, 3]
  - [low, 1]

# Scheduler for recurring jobs (if using sidekiq-cron)
:scheduler:
  :enabled: true

# Environment-specific settings
production:
  :concurrency: 10
  :timeout: 25

development:
  :concurrency: 2
  :timeout: 25

test:
  :concurrency: 1
  :timeout: 25
```text

**.overmind.env** - Overmind-specific configuration:

```bash
# Overmind configuration for Diquis Football Academy
# Start all processes: overmind start
# Start specific process: overmind start web

procfile: Procfile.dev

# Specify the port for the main web process
port: 3000

# Specify which networks to use for each process
networks:
  web: true
  worker: false
  css: false

# Process-specific environment variables
formation:
  web: 1
  worker: 1
  css: 1

# Stop timeout (how long to wait before force killing processes)
stop_timeout: 15

# Color output
colors: true

# Show process names
show_names: true

# Auto-restart processes if they crash
auto_restart: true
```text

**Development Commands:**

```bash
# Start all processes
overmind start

# Start specific process
overmind start web

# Connect to running process (access tmux session)
overmind connect web

# Stop all processes
overmind stop

# Restart specific process
overmind restart worker

# Kill all processes (force stop)
overmind kill
```text

**Overmind Process Management:**

The Overmind interface provides:

- **Colored output** with process names for easy identification
- **Process status** indicators (running, stopped, crashed)
- **tmux integration** for process session management
- **Auto-restart** for crashed processes
- **Environment variable** management per process

---

## Environment Variables

### 1. Create .env file

```bash
# Database
DATABASE_URL=postgresql://localhost:5432/diquis_development
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_HOST=localhost

# Redis
REDIS_URL=redis://localhost:6379/0

# Rails
RAILS_ENV=development
RAILS_LOG_LEVEL=debug
RAILS_MAX_THREADS=5
SECRET_KEY_BASE=your_secret_key_base_here

# CORS
CORS_ORIGINS=http://localhost:3000,http://localhost:3001

# Sidekiq
SIDEKIQ_WEB=true

# Active Storage (development uses local disk)
ACTIVE_STORAGE_SERVICE=local

# Production settings (commented out for development)
# ACTIVE_STORAGE_SERVICE=amazon
# AWS_ACCESS_KEY_ID=your_key
# AWS_SECRET_ACCESS_KEY=your_secret
# AWS_REGION=us-east-1
# AWS_BUCKET=diquis-production
```text

### 2. Add .env to .gitignore

```bash
echo ".env" >> .gitignore
echo ".env.local" >> .gitignore
echo ".overmind.env" >> .gitignore
```text

---

## Initializers Setup

### 1. Sidekiq Configuration

**config/initializers/sidekiq.rb:**

```ruby
Sidekiq.configure_server do |config|
  config.redis = { url: ENV.fetch('REDIS_URL', 'redis://localhost:6379/0') }
end

Sidekiq.configure_client do |config|
  config.redis = { url: ENV.fetch('REDIS_URL', 'redis://localhost:6379/0') }
end
```text

### 2. CORS Configuration

**config/initializers/cors.rb:**

```ruby
# Already configured in config/application.rb
# This file can be used for additional CORS customization
```text

### 3. Rswag Configuration

**config/initializers/rswag_api.rb:**

```ruby
Rswag::Api.configure do |c|
  c.swagger_root = Rails.root.join('swagger').to_s
  c.swagger_filter = lambda { |swagger, env| swagger['host'] = env['HTTP_HOST'] }
end
```text

**config/initializers/rswag_ui.rb:**

```ruby
Rswag::Ui.configure do |c|
  c.swagger_endpoint '/api-docs/v1/swagger.yaml', 'API V1 Docs'
end
```text

---

## Testing Setup

### 1. Configure RSpec

Diquis uses RSpec for testing with additional gems for enhanced testing capabilities:

**Testing Stack:**

- **RSpec Rails** - Main testing framework
- **FactoryBot** - Test data generation
- **Faker** - Realistic fake data
- **Database Cleaner** - Database state management
- **SimpleCov** - Test coverage reporting
- **WebMock & VCR** - HTTP request stubbing
- **Timecop** - Time manipulation in tests

**Generate RSpec configuration:**

```bash
# Generate RSpec configuration files
bundle exec rails generate rspec:install

# Create test database
RAILS_ENV=test bundle exec rails db:create db:migrate
```text

**RSpec Configuration Files:**

`.rspec:`

```plaintext
--require spec_helper
--require rails_helper
--color
--format documentation
--order random
```text

`spec/rails_helper.rb` includes:

- SimpleCov coverage configuration
- Database cleaner setup
- VCR configuration for HTTP stubbing
- Custom matchers and helpers
- FactoryBot integration

### 2. Running Tests

```bash
# Run all tests
bundle exec rspec

# Run specific test file
bundle exec rspec spec/models/application_record_spec.rb

# Run tests with coverage
COVERAGE=true bundle exec rspec

# Run tests in parallel (future)
bundle exec rspec --parallel
```text

### 3. Code Quality with RuboCop

**RuboCop Configuration:**
Diquis extends Rails Omakase styling with project-specific rules in `.rubocop.yml`:

- **Base:** Inherits from `rubocop-rails-omakase`
- **Extensions:** Includes `rubocop-rspec` and `rubocop-factory_bot`
- **Custom Rules:** Project-specific style preferences
- **Excluded Paths:** Generated files and vendor code

```bash
# Check code style
bundle exec rubocop

# Auto-fix issues
bundle exec rubocop -A

# Check specific files
bundle exec rubocop app/models/
```text

### 4. Test Support Files

**spec/support/vcr.rb** - HTTP request recording:

```ruby
VCR.configure do |config|
  config.cassette_library_dir = 'spec/vcr_cassettes'
  config.hook_into :webmock
  config.default_cassette_options = {
    record: :new_episodes,
    match_requests_on: [:method, :uri, :headers, :body]
  }
  config.filter_sensitive_data('<API_KEY>') { ENV['API_KEY'] }
  config.ignore_localhost = true
  config.configure_rspec_metadata!
end
```text

**spec/support/database_cleaner.rb** - Database management:

```ruby
RSpec.configure do |config|
  config.before(:suite) do
    DatabaseCleaner.clean_with(:truncation)
  end

  config.before do |example|
    DatabaseCleaner.strategy = :transaction
    DatabaseCleaner.strategy = :truncation if example.metadata[:js] || example.metadata[:truncation]
    DatabaseCleaner.start
  end

  config.after do
    DatabaseCleaner.clean
  end
end
```text

**spec/support/custom_matchers.rb** - API testing helpers:

```ruby
# Custom matcher for JSON responses
RSpec::Matchers.define :have_json_type do |expected_type|
  match do |actual|
    begin
      JSON.parse(actual)
      case expected_type
      when :object
        JSON.parse(actual).is_a?(Hash)
      when :array
        JSON.parse(actual).is_a?(Array)
      else
        false
      end
    rescue JSON::ParserError
      false
    end
  end
end

# Helper method to parse JSON responses
def json_response
  JSON.parse(response.body)
end
```text

### 2. Configure Swagger Helper

**spec/swagger_helper.rb:**

```ruby
require 'rails_helper'

RSpec.configure do |config|
  config.swagger_root = Rails.root.join('swagger').to_s
  
  config.swagger_docs = {
    'v1/swagger.yaml' => {
      openapi: '3.0.1',
      info: {
        title: 'Diquis API V1',
        version: 'v1',
        description: 'Football Academy Management API with multi-tenant support'
      },
      paths: {},
      servers: [
        {
          url: 'http://localhost:3000',
          description: 'Development server'
        },
        {
          url: 'https://api.diquis.com',
          description: 'Production server'
        }
      ],
      components: {
        securitySchemes: {
          BearerAuth: {
            type: :http,
            scheme: :bearer,
            bearerFormat: :JWT
          }
        }
      }
    }
  }
  
  config.swagger_format = :yaml
end
```text

---

## Initial Migration

### 1. Generate Initial Models

```bash
# Generate Academy model (tenant)
rails generate model Academy slug:uuid:uniq name:string description:text \
  owner_name:string owner_email:string owner_phone:string \
  address_line_1:string address_line_2:string city:string \
  state_province:string postal_code:string country:string \
  founded_date:date website:string is_active:boolean

# Generate User model (if not created by Devise)
# Devise already creates this

# Generate AcademyUser join table
rails generate model AcademyUser academy:references user:references \
  role:string is_active:boolean

# Run migrations
rails db:migrate
```text

---

## Verification

### 1. Start Development Server

```bash
# Using Overmind
./bin/dev

# Or individual services
rails server           # Port 3000
bundle exec sidekiq    # Background jobs
redis-server           # Redis
```text

### 2. Verify Endpoints

```bash
# Health check
curl http://localhost:3000/health

# API documentation
open http://localhost:3000/api-docs

# Sidekiq dashboard
open http://localhost:3000/sidekiq
```text

### 3. Run Tests

```bash
# Run all tests
bundle exec rspec

# Run specific test
bundle exec rspec spec/models/academy_spec.rb

# Generate test coverage
COVERAGE=true bundle exec rspec
```text

---

## Next Steps

1. **Review Documentation:**
   - Read [ARCHITECTURE.md](./ARCHITECTURE.md) for detailed architecture
   - Review [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) for API design

2. **Implement Core Models:**
   - Create migrations for Player, Team, Training models
   - Add model validations and associations
   - Implement concerns (Sluggable, Auditable)

3. **Build Base Classes:**
   - Create ApplicationController with tenant setup
   - Implement BaseService class
   - Create ApplicationSerializer

4. **Develop First Slice:**
   - Start with Academy Management slice
   - Implement service layer
   - Add serializers and policies
   - Write comprehensive tests

5. **Frontend Integration:**
   - Set up React project in separate repository
   - Configure API client with academy context
   - Implement authentication flow

---

## Troubleshooting

### PostgreSQL Connection Issues

```bash
# Check if PostgreSQL is running
pg_isready

# Start PostgreSQL
brew services start postgresql@15  # macOS
sudo systemctl start postgresql    # Linux

# Create user if needed
createuser -s postgres
```text

### Redis Connection Issues

```bash
# Check if Redis is running
redis-cli ping

# Start Redis
brew services start redis          # macOS
sudo systemctl start redis-server  # Linux
```text

### Gem Installation Issues

```bash
# Update bundler
gem update bundler

# Install specific gem
bundle install --without production

# Clear bundler cache
rm -rf vendor/bundle
bundle install
```text

### Migration Issues

```bash
# Reset database
rails db:drop db:create db:migrate

# Rollback and re-migrate
rails db:rollback STEP=3
rails db:migrate
```text

---

## Additional Resources

- [Rails Guides](https://guides.rubyonrails.org/)
- [ActsAsTenant Documentation](https://github.com/ErwinM/acts_as_tenant)
- [Devise Documentation](https://github.com/heartcombo/devise)
- [Pundit Documentation](https://github.com/varvet/pundit)
- [Sidekiq Documentation](https://github.com/sidekiq/sidekiq)
