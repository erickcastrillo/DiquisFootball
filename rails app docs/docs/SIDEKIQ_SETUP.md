# Sidekiq Setup and Configuration

This document explains the Sidekiq setup for background job processing in the Diquis application.

## 📋 Overview

Sidekiq is configured to handle background job processing with the following features:

- **Multi-queue Support**: `critical`, `default`, `low`, `mailers`, `reports`, `maintenance`, `monitoring`
- **Scheduled Jobs**: Sidekiq-cron for recurring tasks (daily, hourly, weekly, monthly)
- **Development Debugging**: VS Code debugging support for jobs and cron tasks
- **Web UI**: Monitoring dashboard at `/sidekiq` with cron job management
- **Retry Logic**: Automatic retries with exponential backoff
- **Environment-specific Configuration**: Different settings per environment

## 🚀 Quick Start

### Prerequisites

1. **Redis Server**: Sidekiq requires Redis for job storage

   ```bash
   # Install Redis (Ubuntu/Debian)
   sudo apt-get install redis-server
   
   # Install Redis (macOS)
   brew install redis && brew services start redis
   
   # Docker alternative
   docker run -d -p 6379:6379 redis:7-alpine
   ```

2. **Start Development Environment**

   ```bash
   # Start all services (Rails + Vite + Sidekiq)
   ./bin/dev
   
   # Or start Sidekiq separately
   bundle exec sidekiq
   ```

### Testing the Setup

```bash
# Test Sidekiq configuration
bundle exec rake sidekiq:test

# Check queue statistics  
bundle exec rake sidekiq:stats

# Enqueue a test job
bundle exec rails runner "ExampleJob.perform_later(1, 'Hello Sidekiq!')"

# Load and test cron jobs
bundle exec rake sidekiq:cron:load
bundle exec rake sidekiq:cron:list
```

## 🔧 Configuration

### Queue Configuration (`config/sidekiq.yml`)

```yaml
# Global settings
:max_retries: 5
:timeout: 30

development:
  :concurrency: 5
  :queues:
    - critical    # High priority jobs
    - default     # Standard jobs  
    - low         # Low priority jobs

production:
  :concurrency: 25
  :queues:
    - critical
    - default
    - low
    - mailers
    - reports
```

### Queue Priorities

- **critical**: Urgent jobs (user-facing operations)
- **default**: Standard background tasks
- **low**: Non-urgent maintenance tasks
- **mailers**: Email sending jobs
- **reports**: Analytics and reporting jobs
- **maintenance**: System maintenance and cleanup tasks
- **monitoring**: Health checks and system monitoring

### Cron Jobs Configuration (`config/schedule.yml`)

Recurring jobs are configured using cron expressions:

```yaml
# Daily maintenance at 2 AM
daily_maintenance:
  cron: "0 2 * * *"
  class: "DailyMaintenanceJob"
  queue: "maintenance"

# Hourly statistics
hourly_stats_update:
  cron: "0 * * * *"
  class: "HourlyStatsJob"
  queue: "reports"
```

**Cron Expression Format**: `"minute hour day month weekday"`

- `"0 2 * * *"` - Daily at 2:00 AM
- `"0 */6 * * *"` - Every 6 hours
- `"0 9 * * 1"` - Every Monday at 9:00 AM
- `"*/15 * * * *"` - Every 15 minutes

## 📝 Creating Background Jobs

### Basic Job Structure

```ruby
class MyCustomJob < ApplicationJob
  queue_as :default
  
  # Retry configuration
  retry_on StandardError, wait: :exponentially_longer, attempts: 5
  retry_on CustomError, attempts: 3
  
  def perform(user_id, options = {})
    # Your job logic here
    user = User.find(user_id)
    
    # Example: Send notification
    NotificationService.new(user).send_welcome_email
  end
end
```

### Job Usage Examples

```ruby
# Enqueue job immediately
MyCustomJob.perform_later(user.id, { type: 'welcome' })

# Enqueue for later execution
MyCustomJob.set(wait: 1.hour).perform_later(user.id)

# Enqueue at specific time
MyCustomJob.set(wait_until: Date.tomorrow.noon).perform_later(user.id)

# High priority queue
MyCustomJob.set(queue: :critical).perform_later(user.id)
```

## ⏰ Scheduled Jobs (Sidekiq-Cron)

### Managing Cron Jobs

```bash
# Load cron jobs from config/schedule.yml
bundle exec rake sidekiq:cron:load

# List all cron jobs
bundle exec rake sidekiq:cron:list

# Enable/disable specific jobs
bundle exec rake sidekiq:cron:enable[job_name]
bundle exec rake sidekiq:cron:disable[job_name]

# Clear all cron jobs
bundle exec rake sidekiq:cron:clear

# Test cron configuration
bundle exec rake sidekiq:cron:test
```

### Example Scheduled Jobs

#### Daily Maintenance (2 AM daily)

```ruby
class DailyMaintenanceJob < ApplicationJob
  queue_as :maintenance
  
  def perform
    # Cleanup old logs, temp files, update metrics
  end
end
```

#### Health Monitoring (every 5 minutes)

```ruby
class HealthCheckJob < ApplicationJob
  queue_as :monitoring
  
  def perform
    # Check database, Redis, external services
  end
end
```

#### Weekly Reports (Mondays at 9 AM)

```ruby
class WeeklyReportJob < ApplicationJob
  queue_as :reports
  
  def perform
    # Generate and send weekly reports
  end
end
```

## 🔍 Monitoring and Debugging

### Sidekiq Web UI

Access the monitoring dashboard at: **http://localhost:3000/sidekiq**

Features:

- Live queue monitoring
- Failed job inspection and retry
- Real-time statistics
- Worker management
- Scheduled job overview
- **Cron jobs management** (enable/disable, view schedules, trigger manually)

### VS Code Debugging

1. **Set breakpoints** in your job files
2. **Start Sidekiq with debugging**:
   - Press `F5` → "🔄 Debug Sidekiq" (standard jobs)
   - Press `F5` → "📅 Debug Sidekiq with Cron" (includes cron jobs)
3. **Enqueue a job** to trigger the breakpoint
4. **Debug normally** with step-through, variable inspection, etc.

#### Debugging Cron Jobs

- **Manual trigger**: Use Sidekiq web UI to trigger cron jobs manually
- **Test schedule**: Use `bundle exec rake sidekiq:cron:test` to validate configuration
- **Monitor execution**: Watch cron job logs in the Sidekiq web UI

### Rake Tasks

```bash
# Show queue statistics
bundle exec rake sidekiq:stats

# Clear all queues
bundle exec rake sidekiq:clear

# Load test (enqueue multiple jobs)
COUNT=50 bundle exec rake sidekiq:load_test

# Test configuration
bundle exec rake sidekiq:test
```

## 🏗️ Slice-based Jobs

For slice-based architecture, place jobs in slice directories:

```text
app/slices/
├── football/
│   ├── jobs/
│   │   ├── player_stats_job.rb
│   │   └── team_report_job.rb
│   └── ...
└── academy/
    ├── jobs/
    │   ├── enrollment_job.rb
    │   └── payment_reminder_job.rb
    └── ...
```

Example slice job:

```ruby
# app/slices/football/jobs/player_stats_job.rb
module Football
  class PlayerStatsJob < ApplicationJob
    queue_as :reports
    
    def perform(player_id, season_id)
      player = Football::Player.find(player_id)
      Football::StatisticsService.new(player).calculate_season_stats(season_id)
    end
  end
end
```

## 🔄 Background Job Patterns

### Email Jobs

```ruby
class WelcomeEmailJob < ApplicationJob
  queue_as :mailers
  
  def perform(user_id)
    user = User.find(user_id)
    UserMailer.welcome_email(user).deliver_now
  end
end
```

### Report Generation

```ruby  
class MonthlyReportJob < ApplicationJob
  queue_as :reports
  
  def perform(academy_id, month, year)
    academy = Academy.find(academy_id)
    ReportService.generate_monthly_report(academy, month, year)
  end
end
```

### Data Processing

```ruby
class PlayerDataImportJob < ApplicationJob
  queue_as :default
  
  def perform(file_path, academy_id)
    ImportService.new(academy_id).process_player_csv(file_path)
  end
end
```

## 🛠️ Troubleshooting

### Common Issues

1. **Redis Connection Error**

   ```bash
   # Check Redis status
   redis-cli ping
   # Should return: PONG
   ```

2. **Jobs Not Processing**

   ```bash
   # Check Sidekiq is running
   ps aux | grep sidekiq
   
   # Check queue status
   bundle exec rake sidekiq:stats
   ```

3. **Memory Issues**
   - Adjust concurrency in `config/sidekiq.yml`
   - Monitor job memory usage
   - Consider job size limits

### Performance Tips

- **Queue Separation**: Use different queues for different job types
- **Batch Processing**: Group similar operations
- **Timeout Settings**: Set appropriate timeouts for long-running jobs
- **Resource Limits**: Monitor memory and CPU usage

## 🔗 Integration with Development Tools

### Procfile.dev Integration

Sidekiq automatically starts with `./bin/dev`:

```yaml
vite: bin/vite dev
web: bin/rails s  
sidekiq: bundle exec sidekiq
```

### VS Code Tasks

Available tasks in VS Code (`Ctrl+Shift+P` → "Tasks: Run Task"):

- **🔄 Start Sidekiq**: Start Sidekiq manually
- **🔄 Sidekiq Stats**: Show queue statistics  
- **🧹 Clear Sidekiq Jobs**: Clear all queues
- **📅 Load Cron Jobs**: Load cron jobs from schedule.yml
- **📋 List Cron Jobs**: Show all configured cron jobs
- **🧪 Test Cron Configuration**: Validate cron job configuration

### Testing Integration

Jobs use the `:test` adapter in test environment for synchronous execution:

```ruby
# In tests, jobs run immediately
expect { MyJob.perform_later(user.id) }.to change(User, :count)
```

## 🎉 Summary

This setup provides a comprehensive background job processing system with:

### ✅ Core Features Configured

- **Sidekiq 7.2**: High-performance background job processing
- **Sidekiq-Cron 1.12**: Scheduled/recurring job management
- **Multi-queue Support**: Priority-based job processing
- **Redis Integration**: Reliable job storage and coordination

### ✅ Development Tools

- **VS Code Debugging**: Full breakpoint debugging for jobs and cron tasks
- **Web Dashboard**: Complete monitoring at `/sidekiq` with cron management
- **Rake Tasks**: Comprehensive job and cron management commands
- **RSpec Testing**: Automated testing for all job classes

### ✅ Pre-configured Jobs

- **Daily Maintenance**: System cleanup (2 AM daily)
- **Hourly Statistics**: Metrics updates (every hour)
- **Weekly Reports**: Report generation (Mondays 9 AM)
- **Monthly Cleanup**: Data archival (1st of month, 3 AM)
- **Health Monitoring**: System checks (every 5 minutes)

### ✅ Production Ready

- **Environment-specific Configuration**: Different settings per environment
- **Retry Logic**: Automatic job retries with exponential backoff
- **Error Handling**: Comprehensive error logging and alerting
- **Security**: Protected web UI for production deployments

This system integrates seamlessly with your Rails application's slice architecture and provides a solid foundation for scalable background job processing.
