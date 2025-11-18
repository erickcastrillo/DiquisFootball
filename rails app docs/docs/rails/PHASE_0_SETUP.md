# Phase 0: Project Foundation & Setup

**Duration:** 1 Week  
**Status:** 🟡 Not Started  
**Dependencies:** None

---

## 📋 Overview

This phase establishes the complete development environment and project structure. By the end of this phase, you'll have a fully configured Rails application ready for feature development.

## 🎯 Objectives

- Install all system dependencies
- Create Rails application with proper configuration
- Set up development tools (Overmind, RSpec, Rubocop)
- Configure database and Redis
- Create directory structure for Vertical Slice Architecture
- Initialize Git repository with proper .gitignore

## 📝 Prerequisites Checklist

Before starting, ensure you have:

- [ ] Ruby 3.3.0+ installed
- [ ] PostgreSQL 15+ installed and running
- [ ] Redis 7.0+ installed and running
- [ ] Git installed
- [ ] Text editor/IDE configured
- [ ] Terminal access

---

## 🔧 Step-by-Step Tasks

### Task 0.1: Install System Dependencies

#### macOS Installation

```bash
# Install Homebrew if not already installed
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Install Ruby using rbenv (recommended)
brew install rbenv ruby-build
rbenv install 3.3.0
rbenv global 3.3.0

# Add to ~/.zshrc or ~/.bash_profile
echo 'eval "$(rbenv init - zsh)"' >> ~/.zshrc
source ~/.zshrc

# Verify Ruby installation
ruby -v  # Should show 3.3.0

# Install Rails
gem install rails -v 8.0.3

# Install PostgreSQL
brew install postgresql@15
brew services start postgresql@15

# Install Redis
brew install redis
brew services start redis

# Install image processing libraries
brew install imagemagick vips

# Install tmux (required for Overmind)
brew install tmux

# Install Overmind
brew install overmind

# Verify installations
psql --version    # Should show PostgreSQL 15+
redis-cli ping    # Should return PONG
overmind version  # Should show version
```text

#### Ubuntu/Debian Installation

```bash
# Update package list
sudo apt update && sudo apt upgrade -y

# Install dependencies
sudo apt install -y curl git build-essential libssl-dev zlib1g-dev \
  libreadline-dev libyaml-dev libxml2-dev libxslt1-dev libcurl4-openssl-dev \
  libffi-dev libpq-dev

# Install rbenv
curl -fsSL https://github.com/rbenv/rbenv-installer/raw/HEAD/bin/rbenv-installer | bash

# Add to ~/.bashrc
echo 'export PATH="$HOME/.rbenv/bin:$PATH"' >> ~/.bashrc
echo 'eval "$(rbenv init -)"' >> ~/.bashrc
source ~/.bashrc

# Install Ruby
rbenv install 3.3.0
rbenv global 3.3.0

# Verify Ruby
ruby -v

# Install Rails
gem install rails -v 8.0.3

# Install PostgreSQL
sudo apt install -y postgresql postgresql-contrib libpq-dev
sudo systemctl start postgresql
sudo systemctl enable postgresql

# Create PostgreSQL user
sudo -u postgres createuser -s $USER

# Install Redis
sudo apt install -y redis-server
sudo systemctl start redis-server
sudo systemctl enable redis-server

# Install image processing
sudo apt install -y imagemagick libvips-dev

# Install tmux
sudo apt install -y tmux

# Install Overmind
wget https://github.com/DarthSim/overmind/releases/download/v2.4.0/overmind-v2.4.0-linux-amd64.gz
gunzip overmind-v2.4.0-linux-amd64.gz
chmod +x overmind-v2.4.0-linux-amd64
sudo mv overmind-v2.4.0-linux-amd64 /usr/local/bin/overmind

# Verify installations
psql --version
redis-cli ping
overmind version
```text

**Verification:**

```bash
ruby -v          # 3.3.0
rails -v         # 8.0.3
psql --version   # PostgreSQL 15+
redis-cli ping   # PONG
overmind version # 2.4.0+
```text

---

### Task 0.2: Create Rails Application

```bash
# Create project directory (if not exists)
cd ~/RubymineProjects  # or your preferred location

# Create new Rails API application
rails new diquis \
  --api \
  --database=postgresql \
  --skip-test \
  -T \
  --skip-action-mailbox \
  --skip-action-text \
  --skip-active-storage

cd diquis
```text

**Verification:**

```bash
# Should see Rails directory structure
ls -la

# Expected output includes:
# app/ bin/ config/ db/ Gemfile lib/ public/ tmp/
```text

---

### Task 0.3: Configure Gemfile

Replace the entire `Gemfile` content:

```ruby
source 'https://rubygems.org'
git_source(:github) { |repo| "https://github.com/#{repo}.git" }

ruby '3.3.0'

# Rails core
gem 'rails', '~> 8.0.3'
gem 'pg', '~> 1.5'
gem 'puma', '~> 6.4'

# Multi-tenancy
gem 'acts_as_tenant', '~> 1.0'

# Authentication & Authorization
gem 'devise', '~> 4.9'
gem 'devise-jwt', '~> 0.11'
gem 'pundit', '~> 2.3'

# API and serialization
gem 'active_model_serializers', '~> 0.10'
gem 'jsonapi-serializer', '~> 2.2'
gem 'pagy', '~> 6.0'
gem 'kaminari', '~> 1.2'
gem 'ransack', '~> 4.0'

# API Documentation
gem 'rswag', '~> 2.13'
gem 'rswag-api', '~> 2.13'
gem 'rswag-ui', '~> 2.13'

# Background jobs
gem 'sidekiq', '~> 7.2'

# Caching and storage
gem 'redis', '~> 5.0'
gem 'hiredis-client', '~> 0.19'

# File processing
gem 'image_processing', '~> 1.12'
gem 'ruby-vips', '~> 2.2'

# Rails 8.0+ specific
gem 'solid_cable', '~> 0.5'
gem 'solid_cache', '~> 0.5'
gem 'solid_queue', '~> 0.3'

# Utilities
gem 'friendly_id', '~> 5.5'
gem 'bootsnap', require: false
gem 'discard', '~> 1.3'
gem 'rack-cors', '~> 2.0'

# Reducers redundancy
gem 'tzinfo-data', platforms: %i[mingw mswin x64_mingw jruby]

group :development, :test do
  gem 'debug', platforms: %i[mri mingw x64_mingw]
  gem 'rspec-rails', '~> 6.1'
  gem 'factory_bot_rails', '~> 6.4'
  gem 'faker', '~> 3.2'
  gem 'rswag-specs', '~> 2.13'
  gem 'shoulda-matchers', '~> 6.0'
  gem 'database_cleaner-active_record', '~> 2.1'
  gem 'dotenv-rails', '~> 2.8'
end

group :development do
  gem 'annotate', '~> 3.2'
  gem 'brakeman', '~> 6.1', require: false
  gem 'rubocop-rails', '~> 2.23', require: false
  gem 'rubocop-rspec', '~> 2.26', require: false
  gem 'rubocop-performance', '~> 1.20', require: false
  gem 'bullet', '~> 7.1'
  gem 'foreman', '~> 0.87'
  gem 'listen', '~> 3.8'
  gem 'spring'
  gem 'spring-watcher-listen'
end

group :test do
  gem 'simplecov', '~> 0.22', require: false
  gem 'webmock', '~> 3.20'
  gem 'vcr', '~> 6.2'
end
```text

**Install dependencies:**

```bash
bundle install
```text

**Verification:**

```bash
bundle list | grep rails  # Should show rails 8.0.3
bundle list | grep devise # Should show devise
bundle list | grep rspec  # Should show rspec-rails
```text

---

### Task 0.4: Initialize Testing Framework

```bash
# Install RSpec
rails generate rspec:install

# Install Devise
rails generate devise:install

# Install Pundit
rails generate pundit:install

# Install Rswag (API documentation)
rails generate rswag:install
```text

**Configure RSpec:**

Edit `spec/rails_helper.rb`:

```ruby
require 'spec_helper'
ENV['RAILS_ENV'] ||= 'test'
require_relative '../config/environment'
abort("The Rails environment is running in production mode!") if Rails.env.production?
require 'rspec/rails'

# Load support files
Dir[Rails.root.join('spec', 'support', '**', '*.rb')].sort.each { |f| require f }

begin
  ActiveRecord::Migration.maintain_test_schema!
rescue ActiveRecord::PendingMigrationError => e
  abort e.to_s.strip
end

RSpec.configure do |config|
  config.fixture_paths = [Rails.root.join('spec/fixtures')]
  config.use_transactional_fixtures = true
  config.infer_spec_type_from_file_location!
  config.filter_rails_from_backtrace!
  
  # FactoryBot
  config.include FactoryBot::Syntax::Methods
  
  # Shoulda Matchers
  config.include(Shoulda::Matchers::ActiveModel, type: :model)
  config.include(Shoulda::Matchers::ActiveRecord, type: :model)
end
```text

Create `spec/support/factory_bot.rb`:

```ruby
RSpec.configure do |config|
  config.include FactoryBot::Syntax::Methods
end
```text

Create `spec/support/database_cleaner.rb`:

```ruby
require 'database_cleaner/active_record'

RSpec.configure do |config|
  config.before(:suite) do
    DatabaseCleaner.strategy = :transaction
    DatabaseCleaner.clean_with(:truncation)
  end

  config.around(:each) do |example|
    DatabaseCleaner.cleaning do
      example.run
    end
  end
end
```text

Create `spec/support/shoulda_matchers.rb`:

```ruby
Shoulda::Matchers.configure do |config|
  config.integrate do |with|
    with.test_framework :rspec
    with.library :rails
  end
end
```text

**Verification:**

```bash
bundle exec rspec  # Should run with 0 examples, 0 failures
```text

---

### Task 0.5: Create Directory Structure

```bash
# Create slices directories
mkdir -p app/slices/academy_management/{controllers,services,models,serializers,policies,validators}
mkdir -p app/slices/player_management/{controllers,services,models,serializers,policies,validators}
mkdir -p app/slices/team_management/{controllers,services,models,serializers,policies,validators}
mkdir -p app/slices/training_management/{controllers,services,models,serializers,policies,validators}
mkdir -p app/slices/shared_resources/{controllers,services,models,serializers}

# Create shared directory
mkdir -p app/shared/services
mkdir -p app/shared/policies
mkdir -p app/shared/models/concerns

# Create jobs by slice
mkdir -p app/jobs/academy_management
mkdir -p app/jobs/player_management
mkdir -p app/jobs/team_management

# Create serializers directory
mkdir -p app/serializers

# Create API controllers
mkdir -p app/controllers/api/v1

# Create channels directory
mkdir -p app/channels

# Create mailers directory structure
mkdir -p app/mailers

# Create spec directories mirroring app structure
mkdir -p spec/slices/{academy_management,player_management,team_management,training_management,shared_resources}/{controllers,services,models,serializers,policies}
mkdir -p spec/shared/{services,policies}
mkdir -p spec/factories
mkdir -p spec/requests/api/v1
mkdir -p spec/support

# Create documentation directory
mkdir -p docs

# Create swagger directory
mkdir -p swagger/v1

# Create lib/tasks for custom rake tasks
mkdir -p lib/tasks
```text

**Verification:**

```bash
tree app/slices -L 2  # Should show directory structure
ls -la docs           # Should exist
ls -la swagger/v1     # Should exist
```text

---

### Task 0.6: Configure Database

Edit `config/database.yml`:

```yaml
default: &default
  adapter: postgresql
  encoding: unicode
  pool: <%= ENV.fetch("RAILS_MAX_THREADS") { 5 } %>
  username: <%= ENV.fetch("POSTGRES_USER") { ENV['USER'] } %>
  password: <%= ENV.fetch("POSTGRES_PASSWORD") { "" } %>
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
  username: <%= ENV['DATABASE_USERNAME'] %>
  password: <%= ENV['DATABASE_PASSWORD'] %>
```text

**Create databases:**

```bash
rails db:create
```text

**Expected output:**

```text
Created database 'diquis_development'
Created database 'diquis_test'
```text

---

### Task 0.7: Configure Application

Edit `config/application.rb`:

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
    
    # Multi-tenancy
    config.acts_as_tenant_default_class_name = 'Academy'
    
    # Background jobs
    config.active_job.queue_adapter = :sidekiq
    
    # Active Model Serializers
    config.active_model_serializers.config.adapter = :json_api
    config.active_model_serializers.config.key_transform = :underscore
    
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
        origins ENV.fetch('CORS_ORIGINS', 'http://localhost:3000,http://localhost:3001').split(',')
        resource '*',
          headers: :any,
          methods: [:get, :post, :put, :patch, :delete, :options, :head],
          credentials: true
      end
    end
  end
end
```text

---

### Task 0.8: Create Environment Variables

Create `.env` file in project root:

```bash
# Database
DATABASE_URL=postgresql://localhost:5432/diquis_development
POSTGRES_USER=
POSTGRES_PASSWORD=
POSTGRES_HOST=localhost

# Redis
REDIS_URL=redis://localhost:6379/0

# Rails
RAILS_ENV=development
RAILS_LOG_LEVEL=debug
RAILS_MAX_THREADS=5
SECRET_KEY_BASE=

# CORS
CORS_ORIGINS=http://localhost:3000,http://localhost:3001

# Sidekiq
SIDEKIQ_WEB=true

# Active Storage (development)
ACTIVE_STORAGE_SERVICE=local
```text

**Generate secret key:**

```bash
rails secret
# Copy output and paste into SECRET_KEY_BASE in .env
```text

Create `.env.test`:

```bash
DATABASE_URL=postgresql://localhost:5432/diquis_test
REDIS_URL=redis://localhost:6379/1
RAILS_ENV=test
```text

**Add to .gitignore:**

```bash
echo ".env" >> .gitignore
echo ".env.local" >> .gitignore
echo ".env.*.local" >> .gitignore
echo ".overmind.env" >> .gitignore
```text

---

### Task 0.9: Set up Process Management

Create `Procfile.dev`:

```yaml
web: bundle exec rails server -p 3000
worker: bundle exec sidekiq
redis: redis-server
```text

Create `bin/dev`:

```bash
#!/usr/bin/env bash

if ! command -v overmind &> /dev/null
then
  echo "Overmind not found. Install it with: brew install overmind (macOS) or download from GitHub"
  echo "Falling back to foreman..."
  
  if ! command -v foreman &> /dev/null
  then
    echo "Foreman not found either. Install it with: gem install foreman"
    exit 1
  fi
  
  exec foreman start -f Procfile.dev "$@"
else
  exec overmind start -f Procfile.dev "$@"
fi
```text

Make it executable:

```bash
chmod +x bin/dev
```text

Create `.overmind.env`:

```bash
REDIS_URL=redis://localhost:6379/0
DATABASE_URL=postgresql://localhost:5432/diquis_development
RAILS_LOG_LEVEL=debug
SIDEKIQ_WEB=true
CORS_ORIGINS=http://localhost:3000,http://localhost:3001
```text

---

### Task 0.10: Configure Routes

Edit `config/routes.rb`:

```ruby
Rails.application.routes.draw do
  # Health check endpoint
  get '/health', to: proc { [200, { 'Content-Type' => 'text/plain' }, ['OK']] }
  
  # Up endpoint for load balancers
  get 'up', to: 'rails/health#show', as: :rails_health_check
  
  # API routes (to be populated in Phase 1+)
  namespace :api do
    namespace :v1 do
      # Routes will be added here
    end
  end
end
```text

---

### Task 0.11: Initialize Git Repository

```bash
# Initialize Git
git init

# Create .gitignore (Rails should have created one, but let's enhance it)
cat >> .gitignore << 'EOF'

# Environment variables
.env
.env.local
.env.*.local
.overmind.env

# IDE files
.idea/
.vscode/
*.swp
*.swo
*~

# OS files
.DS_Store
Thumbs.db

# Coverage reports
coverage/

# Documentation builds
.yardoc/
doc/

# Temporary files
tmp/
*.log

# Uploads
storage/*
public/uploads/
EOF

# Initial commit
git add .
git commit -m "Initial commit: Rails 8.0.3 API with Vertical Slice Architecture setup"
```text

---

### Task 0.12: Create Initial Documentation

The documentation files have already been created in the `/docs` directory. Verify they exist:

```bash
ls -la docs/
# Should show:
# PROJECT_OVERVIEW.md
# ARCHITECTURE.md
# SETUP_GUIDE.md
# API_DOCUMENTATION.md
# DEVELOPMENT_GUIDE.md
# IMPLEMENTATION_PHASES.md
# PHASE_0_SETUP.md (this file)
```text

---

## ✅ Phase 0 Completion Checklist

Verify everything is set up correctly:

- [ ] Ruby 3.3.0 installed and active
- [ ] Rails 8.0.3 installed
- [ ] PostgreSQL 15+ running
- [ ] Redis 7.0+ running
- [ ] All gems installed (`bundle install` successful)
- [ ] RSpec configured and working
- [ ] Database created (development and test)
- [ ] Directory structure created (app/slices, etc.)
- [ ] Environment variables configured (.env file)
- [ ] Overmind/Foreman configured (bin/dev executable)
- [ ] Git repository initialized
- [ ] Documentation files present in /docs

---

## 🧪 Verification Steps

Run these commands to verify the setup:

```bash
# 1. Check Ruby and Rails versions
ruby -v    # Should show 3.3.0
rails -v   # Should show 8.0.3

# 2. Check dependencies
psql --version      # PostgreSQL 15+
redis-cli ping      # Should return PONG

# 3. Verify gems
bundle list | grep rails
bundle list | grep rspec
bundle list | grep devise

# 4. Check database
rails db:migrate:status
# Should show "database: diquis_development" with no migrations yet

# 5. Run tests
bundle exec rspec
# Should complete with 0 examples, 0 failures

# 6. Start development servers
./bin/dev
```text

In another terminal:

```bash
# 7. Test health endpoint
curl http://localhost:3000/health
# Should return: OK

# 8. Check Rails info
curl http://localhost:3000/up
# Should return Rails health check response
```text

---

## 🐛 Common Issues & Solutions

### Issue: PostgreSQL won't start

**Solution for macOS:**

```bash
brew services restart postgresql@15
# Or manually:
pg_ctl -D /usr/local/var/postgresql@15 start
```text

**Solution for Linux:**

```bash
sudo systemctl restart postgresql
```text

### Issue: Redis won't start

**Solution for macOS:**

```bash
brew services restart redis
```text

**Solution for Linux:**

```bash
sudo systemctl restart redis-server
```text

### Issue: Bundle install fails with native extensions error

**Solution:**

```bash
# Install missing system libraries
# macOS:
brew install openssl readline

# Linux:
sudo apt install libssl-dev libreadline-dev
```text

### Issue: Permission denied for PostgreSQL

**Solution:**

```bash
# Create PostgreSQL user
sudo -u postgres createuser -s $USER

# Or in psql:
sudo -u postgres psql
CREATE ROLE your_username WITH SUPERUSER LOGIN;
```text

### Issue: Overmind not found

**Solution:**

```bash
# Use foreman as fallback
gem install foreman
foreman start -f Procfile.dev
```text

---

## 📊 Phase 0 Metrics

Upon completion, you should have:

- **Files Created:** ~50+ configuration and setup files
- **Directories:** 30+ directories created
- **Gems Installed:** 50+ gems
- **Documentation:** 6+ comprehensive markdown files
- **Time Spent:** ~4-8 hours (depending on familiarity)

---

## ➡️ Next Steps

Once Phase 0 is complete, proceed to:

- **[Phase 1: Core Infrastructure](./PHASE_1_INFRASTRUCTURE.md)**
  - Implement base service class
  - Set up authentication and authorization
  - Create model concerns
  - Configure multi-tenancy

---

## 📚 Related Documentation

- [Project Overview](./PROJECT_OVERVIEW.md)
- [Setup Guide](./SETUP_GUIDE.md)
- [Implementation Phases](./IMPLEMENTATION_PHASES.md)

---

**Phase 0 Status:** Ready to begin  
**Estimated Completion:** 1 week  
**Difficulty:** ⭐⭐ (Medium)
