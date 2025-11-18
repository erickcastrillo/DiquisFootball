# Diquis - Gemfile Documentation

## Overview

This document provides comprehensive information about all gems included in the Diquis Football Academy management system, their versions, rationale for selection, and usage within the application.

**Total Gems:** 59 direct dependencies, 206 total installed gems  
**Ruby Version:** 3.4.5  
**Rails Version:** 8.0.3

---

## Core Framework & Infrastructure

### Rails Framework

```ruby
gem "rails", "~> 8.0.3"
```text

**Purpose:** Main web application framework  
**Rationale:** Latest stable Rails 8.0 with modern features including improved security, performance optimizations, and better development experience  
**Usage:** Foundation for the entire API application

### Database

```ruby
gem "pg", "~> 1.1"
```text

**Purpose:** PostgreSQL database adapter  
**Rationale:** Production-grade relational database with excellent performance, JSON support, and advanced features needed for multi-tenant architecture  
**Usage:** Primary data storage for all application models

### Web Server

```ruby
gem "puma", ">= 5.0"
```text

**Purpose:** Multi-threaded HTTP server  
**Rationale:** High-performance server with excellent concurrency handling, perfect for API-focused applications  
**Usage:** Development and production web server

---

## Authentication & Authorization

### Devise

```ruby
gem "devise", "~> 4.9"
```text

**Purpose:** Flexible authentication solution  
**Rationale:** Mature, well-tested authentication system with extensive customization options  
**Usage:** User registration, login, password management, email confirmation

### Devise JWT

```ruby
gem "devise-jwt", "~> 0.12"
```text

**Purpose:** JWT token-based authentication for Devise  
**Rationale:** Stateless authentication perfect for API-only applications and single-page applications  
**Usage:** Token generation and validation for API authentication

### Pundit

```ruby
gem "pundit", "~> 2.4"
```text

**Purpose:** Minimal authorization through OO design  
**Rationale:** Clean, policy-based authorization system that aligns with Rails conventions  
**Usage:** Role-based access control and resource authorization

### BCrypt

```ruby
gem "bcrypt", "~> 3.1.7"
```text

**Purpose:** Secure password hashing  
**Rationale:** Industry-standard password hashing with configurable work factor  
**Usage:** Password encryption for user authentication

---

## Multi-Tenancy

### ActsAsTenant

```ruby
gem "acts_as_tenant", "~> 1.0"
```text

**Purpose:** Multi-tenancy for Rails applications  
**Rationale:** Provides automatic scoping of data by tenant (academy) with minimal configuration  
**Usage:** Isolate data between different football academies

---

## Background Jobs & Queue Management

### Sidekiq

```ruby
gem "sidekiq", "~> 7.3"
```text

**Purpose:** Simple, efficient background processing  
**Rationale:** Redis-based job queue with excellent performance and monitoring capabilities  
**Usage:** Email sending, report generation, data synchronization, maintenance tasks

### Sidekiq Cron

```ruby
gem "sidekiq-cron", "~> 1.12"
```text

**Purpose:** Recurring jobs for Sidekiq  
**Rationale:** Cron-like scheduling for background jobs without external dependencies  
**Usage:** Daily reports, weekly maintenance, periodic data cleanup

---

## API & Serialization

### Active Model Serializers

```ruby
gem "active_model_serializers", "~> 0.10"
```text

**Purpose:** Fast JSON serialization for Ruby objects  
**Rationale:** Provides consistent, customizable JSON responses with relationship handling  
**Usage:** API response formatting, relationship inclusion/exclusion

### OJ (Optimized JSON)

```ruby
gem "oj", "~> 3.16"
```text

**Purpose:** Fast JSON parsing library  
**Rationale:** High-performance JSON parsing, significantly faster than default JSON library  
**Usage:** JSON response generation and parsing

---

## Cross-Origin Resource Sharing

### Rack CORS

```ruby
gem "rack-cors"
```text

**Purpose:** Handle Cross-Origin Resource Sharing (CORS)  
**Rationale:** Enable frontend applications from different domains to access the API  
**Usage:** Frontend integration, mobile app support

---

## File Upload & Processing

### Image Processing

```ruby
gem "image_processing", "~> 1.2"
```text

**Purpose:** Active Storage image transformations  
**Rationale:** Modern image processing with ImageMagick/Vips support  
**Usage:** Player photos, academy logos, document thumbnails

### AWS SDK S3

```ruby
gem "aws-sdk-s3", "~> 1.167"
```text

**Purpose:** AWS S3 integration for file storage  
**Rationale:** Scalable, reliable file storage for production environments  
**Usage:** Player photos, documents, report files in production

---

## Caching & Performance

### Redis

```ruby
gem "redis", "~> 5.3"
```text

**Purpose:** High-performance caching library  
**Rationale:** In-memory data structure store for caching and session storage  
**Usage:** Session storage, query caching, background job queue

### Rack Cache

```ruby
gem "rack-cache", "~> 1.17"
```text

**Purpose:** Middleware for caching responses  
**Rationale:** HTTP-aware caching to improve API response times  
**Usage:** API response caching, reduced database load

---

## Utilities & Helpers

### Friendly ID

```ruby
gem "friendly_id", "~> 5.5"
```text

**Purpose:** Slug generation and friendly URLs  
**Rationale:** Human-readable URLs and identifiers instead of numeric IDs  
**Usage:** Player slugs, team identifiers, URL generation

### HTTParty

```ruby
gem "httparty", "~> 0.22"
```text

**Purpose:** HTTP requests  
**Rationale:** Simple, elegant HTTP client for external API integration  
**Usage:** External service integration, webhook notifications

---

## Monitoring & Logging

### Lograge

```ruby
gem "lograge", "~> 0.14"
```text

**Purpose:** Structured logging  
**Rationale:** Clean, single-line log format ideal for log analysis and monitoring  
**Usage:** Request logging, performance monitoring, debugging

---

## Internationalization

### Rails I18n

```ruby
gem "rails-i18n", "~> 8.0"
```text

**Purpose:** Ruby internationalization support  
**Rationale:** Multi-language support for global academy management  
**Usage:** Error messages, UI text translation, locale-specific formatting

---

## Development, Test & Code Quality

### Testing Framework

#### RSpec Rails

```ruby
gem "rspec-rails", "~> 7.0"
```text

**Purpose:** Main testing framework  
**Rationale:** Behavior-driven development with expressive syntax and excellent Rails integration  
**Usage:** Unit tests, integration tests, API endpoint testing

#### Factory Bot Rails

```ruby
gem "factory_bot_rails", "~> 6.4"
```text

**Purpose:** Test data factories  
**Rationale:** Clean, maintainable test data creation with association handling  
**Usage:** Model factories, test data setup

#### Faker

```ruby
gem "faker", "~> 3.4"
```text

**Purpose:** Realistic fake data generation  
**Rationale:** Generate realistic test data for better test quality  
**Usage:** Player names, addresses, contact information in tests

### Code Quality

#### RuboCop Rails Omakase

```ruby
gem "rubocop-rails-omakase", require: false
```text

**Purpose:** Ruby styling and linting  
**Rationale:** Rails-optimized code style rules for consistent codebase  
**Usage:** Code style enforcement, automated corrections

#### RuboCop RSpec

```ruby
gem "rubocop-rspec", "~> 3.0", require: false
```text

**Purpose:** RSpec-specific linting rules  
**Rationale:** Enforce RSpec best practices and consistent test structure  
**Usage:** Test code quality, RSpec convention enforcement

#### RuboCop Factory Bot

```ruby
gem "rubocop-factory_bot", "~> 2.26", require: false
```text

**Purpose:** Factory Bot linting rules  
**Rationale:** Enforce Factory Bot best practices and conventions  
**Usage:** Factory definition quality, naming conventions

#### RuboCop Performance

```ruby
gem "rubocop-performance", "~> 1.22", require: false
```text

**Purpose:** Performance-focused linting rules  
**Rationale:** Detect performance anti-patterns and suggest optimizations  
**Usage:** Code performance optimization, bottleneck detection

### Testing Support

#### SimpleCov

```ruby
gem "simplecov", "~> 0.22", require: false
```text

**Purpose:** Test coverage reporting  
**Rationale:** Ensure comprehensive test coverage across the application  
**Usage:** Coverage reports, CI/CD integration

#### SimpleCov Console

```ruby
gem "simplecov-console", "~> 0.9", require: false
```text

**Purpose:** Console output for SimpleCov  
**Rationale:** Immediate coverage feedback during test runs  
**Usage:** Development feedback, coverage visualization

#### Database Cleaner

```ruby
gem "database_cleaner-active_record", "~> 2.2"
```text

**Purpose:** Database cleaning between tests  
**Rationale:** Ensure clean test database state for reliable tests  
**Usage:** Test database management, transaction rollback

#### Timecop

```ruby
gem "timecop", "~> 0.9"
```text

**Purpose:** Time travel in tests  
**Rationale:** Test time-dependent functionality with predictable timestamps  
**Usage:** Testing scheduled tasks, date validations, time-based business logic

#### WebMock

```ruby
gem "webmock", "~> 3.23"
```text

**Purpose:** HTTP request stubbing  
**Rationale:** Mock external HTTP requests for isolated, fast tests  
**Usage:** External API testing, HTTP request isolation

#### VCR

```ruby
gem "vcr", "~> 6.3"
```text

**Purpose:** HTTP interaction recording  
**Rationale:** Record real HTTP interactions for playback in tests  
**Usage:** External API testing, integration test reliability

#### JSON Spec

```ruby
gem "json_spec", "~> 1.1"
```text

**Purpose:** JSON testing helpers  
**Rationale:** Specialized matchers for JSON API response testing  
**Usage:** API response validation, JSON structure testing

#### Shoulda Matchers

```ruby
gem "shoulda-matchers", "~> 6.0"
```text

**Purpose:** Additional RSpec matchers  
**Rationale:** Simplified testing for ActiveRecord validations and associations  
**Usage:** Model testing, validation testing, association testing

### Development Tools

#### Better Errors

```ruby
gem "better_errors", group: :development
```text

**Purpose:** Enhanced error pages  
**Rationale:** Improved debugging experience with interactive console  
**Usage:** Development debugging, error investigation

#### Binding of Caller

```ruby
gem "binding_of_caller", group: :development
```text

**Purpose:** Console in browser for better_errors  
**Rationale:** Interactive debugging directly in error pages  
**Usage:** Development debugging, variable inspection

#### Bullet

```ruby
gem "bullet", "~> 7.2"
```text

**Purpose:** N+1 query detection  
**Rationale:** Identify and prevent performance issues from inefficient queries  
**Usage:** Performance optimization, query optimization

#### Pry Rails

```ruby
gem "pry-rails", "~> 0.3"
```text

**Purpose:** Better Rails console  
**Rationale:** Enhanced console with syntax highlighting and debugging features  
**Usage:** Development debugging, data exploration

#### Pry Byebug

```ruby
gem "pry-byebug", "~> 3.10"
```text

**Purpose:** Debugger integration with Pry  
**Rationale:** Step-through debugging with breakpoints in development  
**Usage:** Code debugging, execution flow analysis

#### Listen

```ruby
gem "listen", "~> 3.9"
```text

**Purpose:** File change detection  
**Rationale:** Monitor file changes for auto-reloading and development tools  
**Usage:** Auto-reload, file watching

#### Spring

```ruby
gem "spring", "~> 4.2"
```text

**Purpose:** Application preloader  
**Rationale:** Faster Rails command execution in development  
**Usage:** Development performance, command speed

#### Spring Watcher Listen

```ruby
gem "spring-watcher-listen", "~> 2.1"
```text

**Purpose:** File watcher for Spring  
**Rationale:** Automatically restart Spring when files change  
**Usage:** Development auto-reload, Spring management

#### Letter Opener

```ruby
gem "letter_opener", "~> 1.10"
```text

**Purpose:** Email testing in development  
**Rationale:** Preview emails in browser instead of sending real emails  
**Usage:** Email development, template testing

#### Letter Opener Web

```ruby
gem "letter_opener_web", "~> 3.0"
```text

**Purpose:** Web interface for Letter Opener  
**Rationale:** Centralized email inbox for development email testing  
**Usage:** Email testing, email template development

### Performance Profiling

#### Ruby Prof

```ruby
gem "ruby-prof", "~> 1.7", require: false
```text

**Purpose:** Ruby profiler  
**Rationale:** Detailed performance profiling to identify bottlenecks  
**Usage:** Performance analysis, optimization identification

#### Memory Profiler

```ruby
gem "memory_profiler", "~> 1.1", require: false
```text

**Purpose:** Memory usage profiling  
**Rationale:** Identify memory leaks and optimization opportunities  
**Usage:** Memory analysis, leak detection

### API Documentation

#### Rswag

```ruby
gem "rswag", "~> 2.15"
```text

**Purpose:** API documentation generation  
**Rationale:** Generate interactive OpenAPI/Swagger documentation from tests  
**Usage:** API documentation, endpoint testing

### Process Management

#### Overmind

```ruby
gem "overmind", "~> 2.5"
```text

**Purpose:** Process manager for development  
**Rationale:** Better process isolation and management compared to Foreman  
**Usage:** Development process orchestration, service management

---

## Rails Built-in Gems

### Solid Cache

```ruby
gem "solid_cache"
```text

**Purpose:** Database-backed cache store  
**Rationale:** Rails 8.0 built-in caching solution using database storage  
**Usage:** Application caching, session storage

### Solid Queue

```ruby
gem "solid_queue"
```text

**Purpose:** Database-backed job queue  
**Rationale:** Rails 8.0 built-in job queue using database storage  
**Usage:** Background job processing (alternative to Sidekiq)

### Solid Cable

```ruby
gem "solid_cable"
```text

**Purpose:** Database-backed ActionCable adapter  
**Rationale:** Rails 8.0 built-in WebSocket support using database storage  
**Usage:** Real-time features, live updates

### Bootsnap

```ruby
gem "bootsnap", require: false
```text

**Purpose:** Boot time optimization  
**Rationale:** Reduces application boot time through caching  
**Usage:** Development and production boot performance

---

## Deployment & Production

### Kamal

```ruby
gem "kamal", require: false
```text

**Purpose:** Docker container deployment  
**Rationale:** Simple, Docker-based deployment solution  
**Usage:** Production deployment, container orchestration

### Thruster

```ruby
gem "thruster", require: false
```text

**Purpose:** HTTP asset caching and compression  
**Rationale:** X-Sendfile acceleration and caching for Puma  
**Usage:** Production performance, asset delivery

---

## Security & Vulnerability Detection

### Brakeman

```ruby
gem "brakeman", require: false
```text

**Purpose:** Static security analysis  
**Rationale:** Detect security vulnerabilities in Rails applications  
**Usage:** Security auditing, vulnerability detection

---

## Commented/Future Gems

### Production Monitoring (Commented)

```ruby
# gem "sentry-ruby", "~> 5.18"
# gem "sentry-rails", "~> 5.18"
# gem "newrelic_rpm", "~> 9.16"
```text

**Purpose:** Error tracking and application performance monitoring  
**Rationale:** Ready for production monitoring when needed  
**Usage:** Error tracking, performance monitoring, alerting

### Rails 8.0 Compatibility Issues (Commented)

```ruby
# gem "annotate", "~> 3.2"
# gem "rails-erd", "~> 1.7", require: false
```text

**Purpose:** Model annotation and schema diagrams  
**Rationale:** Currently incompatible with Rails 8.0, will be enabled when updated  
**Usage:** Model documentation, schema visualization

---

## Installation and Usage

### Bundle Install

```bash
bundle install
```text

### Key Commands

```bash
# Run tests
bundle exec rspec

# Check code quality
bundle exec rubocop

# Start development processes
overmind start

# Generate API documentation
bundle exec rails rswag:specs:swaggerize

# Security audit
bundle exec brakeman

# Performance profiling
bundle exec ruby-prof script/performance_test.rb
```text

---

## Version Management Strategy

- **Pessimistic versioning** (`~>`) for most gems to allow patch updates while preventing breaking changes
- **Explicit versioning** for critical gems (Rails, authentication, database)
- **Latest stable versions** preferred for new features and security updates
- **Regular updates** planned for security patches and compatibility improvements

---

## Production Considerations

1. **Monitoring gems** are commented and ready for production activation
2. **Performance gems** (Redis, caching) configured for scalability
3. **Security gems** (Brakeman, BCrypt) ensure application security
4. **File storage** configured for AWS S3 in production
5. **Background jobs** ready with Sidekiq for production workloads

---

This gem selection provides a comprehensive foundation for the Diquis Football Academy management system, covering all aspects from development through production deployment.
