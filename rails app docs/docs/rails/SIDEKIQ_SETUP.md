# Sidekiq and Redis Setup

This document describes the Sidekiq and Redis configuration for background job processing in Diquis Football Academy.

## Overview

Sidekiq is configured as the background job processor with Redis as the message broker. This setup enables asynchronous task processing for operations like email sending, data processing, and scheduled tasks.

## Quick Start

### Generate Jobs with Our Custom Generator

Use our custom `background_job` generator to create jobs with slice support:

```bash
# Generate a background job for a specific slice
rails generate background_job player_stats --slice=Football --type=background --queue=reports

# Generate a cron job
rails generate background_job match_reminder --slice=Football --type=cron --cron="0 18 * * *" --description="Daily match reminders" --queue=mailers

# Generate both background and cron job
rails generate background_job team_analytics --slice=Academy --type=both --cron="0 3 * * *" --description="Nightly analytics" --queue=reports

# Generate a regular job without slice
rails generate background_job notification_cleanup --type=background --queue=maintenance
```

### Generator Options

- `--slice=SliceName` - Creates job in `app/slices/slice_name/jobs/` (optional)
- `--type=background|cron|both` - Job type (default: background)
- `--queue=queue_name` - Target queue (default: default)
- `--cron="expression"` - Cron expression (required for cron jobs)
- `--description="text"` - Job description for cron schedule

## Configuration

### Gems

The following gems are included in the Gemfile:

- `sidekiq (~> 7.3)` - Background job processor
- `sidekiq-cron (~> 1.12)` - Recurring job scheduler
- `redis (~> 5.3)` - Redis client library

### Initializer

The Sidekiq configuration is located in `config/initializers/sidekiq.rb`:

```ruby
Sidekiq.configure_server do |config|
  config.redis = { url: ENV.fetch('REDIS_URL', 'redis://localhost:6379/0') }
end

Sidekiq.configure_client do |config|
  config.redis = { url: ENV.fetch('REDIS_URL', 'redis://localhost:6379/0') }
end
```text

### Application Configuration

In `config/application.rb`, Sidekiq is set as the default queue adapter:

```ruby
config.active_job.queue_adapter = :sidekiq
```text

### Queue Configuration

Queue priorities are defined in `config/sidekiq.yml`:

- `critical` (priority 10) - High-priority jobs
- `default` (priority 5) - Standard jobs
- `mailers` (priority 3) - Email jobs
- `low` (priority 1) - Background maintenance tasks

### Routes

The Sidekiq Web UI is mounted at `/sidekiq` in development mode only:

```ruby
require 'sidekiq/web'
mount Sidekiq::Web => '/sidekiq' if Rails.env.development?
```text

## Environment Variables

Set the following environment variable:

```bash
REDIS_URL=redis://localhost:6379/0
```text

## Running Sidekiq

### Development

Use the Procfile.dev to run all services:

```bash
overmind start
```text

Or run Sidekiq separately:

```bash
bundle exec sidekiq -C config/sidekiq.yml
```text

### Production

Sidekiq should be run as a separate process:

```bash
bundle exec sidekiq -C config/sidekiq.yml -e production
```text

## Creating Background Jobs

### Example Job

All jobs should inherit from `ApplicationJob`:

```ruby
class ExampleJob < ApplicationJob
  queue_as :default

  def perform(name, message = "Hello from Sidekiq!")
    Rails.logger.info "ExampleJob executing for #{name}"
    Rails.logger.info "Message: #{message}"
    
    # Your job logic here
    
    Rails.logger.info "ExampleJob completed for #{name}"
  end
end
```text

### Enqueuing Jobs

```ruby
# Enqueue job to run immediately
ExampleJob.perform_later('John Doe', 'Welcome!')

# Enqueue job to run at a specific time
ExampleJob.set(wait: 1.hour).perform_later('John Doe', 'Reminder!')

# Enqueue job with specific queue
ExampleJob.set(queue: :critical).perform_later('John Doe', 'Urgent!')
```text

## Testing

### Test Configuration

The test environment uses the `:test` queue adapter (in-memory):

```ruby
# config/environments/test.rb
config.active_job.queue_adapter = :test
```text

### Example Test

```ruby
RSpec.describe ExampleJob, type: :job do
  it 'enqueues the job' do
    expect {
      ExampleJob.perform_later('Test User')
    }.to have_enqueued_job(ExampleJob)
      .with('Test User')
      .on_queue('default')
  end
end
```text

## Monitoring

### Web UI

Access the Sidekiq Web UI in development at:

```text
http://localhost:3000/sidekiq
```text

The UI provides:

- Real-time job statistics
- Queue monitoring
- Job retry management
- Dead job inspection

### Redis CLI

Monitor Redis directly:

```bash
redis-cli
> KEYS *
> LLEN queue:default
> LRANGE queue:default 0 -1
```text

### Custom Generator Benefits

Our `background_job` generator provides several advantages over Rails' default job generator:

- **Slice Support**: Automatically organizes jobs by business domain
- **Cron Integration**: Creates both job classes and schedule configurations
- **Best Practices**: Includes retry logic, error handling, and logging
- **Testing**: Generates RSpec tests with proper job testing patterns
- **VS Code Integration**: Works with our predefined VS Code tasks

### Generated Files Structure

For slice jobs:

```text
app/slices/football/jobs/player_stats_job.rb
spec/slices/football/jobs/player_stats_job_spec.rb
```

For regular jobs:

```text
app/jobs/notification_cleanup_job.rb  
spec/jobs/notification_cleanup_job_spec.rb
```

Cron jobs also update:

```text
config/schedule.yml
```

## Best Practices

1. **Job Design**
   - Keep jobs small and focused
   - Make jobs idempotent (safe to run multiple times)
   - Handle failures gracefully

2. **Error Handling**
   - Use `retry_on` for transient errors
   - Use `discard_on` for permanent failures
   - Log important errors

3. **Performance**
   - Use appropriate queue priorities
   - Avoid long-running jobs (break them into smaller tasks)
   - Monitor job execution times

4. **Security**
   - Protect Sidekiq Web UI in production
   - Use Redis authentication in production
   - Encrypt sensitive data in job arguments

## Troubleshooting

### Redis Connection Issues

Check if Redis is running:

```bash
redis-cli ping
# Should return: PONG
```text

### Job Not Processing

1. Check if Sidekiq is running
2. Verify Redis connection
3. Check job queue:

```bash
bundle exec rails runner "puts Sidekiq::Queue.new('default').size"
```text

### View Failed Jobs

```bash
bundle exec rails runner "puts Sidekiq::RetrySet.new.size"
bundle exec rails runner "puts Sidekiq::DeadSet.new.size"
```text

## Resources

- [Sidekiq Documentation](https://github.com/sidekiq/sidekiq/wiki)
- [Sidekiq Best Practices](https://github.com/sidekiq/sidekiq/wiki/Best-Practices)
- [Redis Documentation](https://redis.io/documentation)
