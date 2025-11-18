# OpenTelemetry & Honeycomb.io Setup Guide

This guide explains how to use OpenTelemetry instrumentation with Honeycomb.io as the observability backend for the Diquis Football Academy Management System.

## Overview

OpenTelemetry (OTel) is an open-source observability framework that provides:

- **Distributed Tracing**: Track requests across services
- **Metrics**: Collect performance data
- **Logs**: Structured logging with context

Honeycomb.io is a powerful observability platform that receives and analyzes this telemetry data.

## What's Instrumented

The following components are automatically instrumented:

### Rails Framework

- ✅ HTTP requests and responses
- ✅ Controller actions
- ✅ View rendering
- ✅ ActiveRecord database queries
- ✅ ActiveJob background jobs
- ✅ ActionMailer emails

### External Services

- ✅ HTTP client requests (Net::HTTP)
- ✅ PostgreSQL database queries (with SQL obfuscation)
- ✅ Redis operations
- ✅ Sidekiq background jobs

### Custom Application Code

- ✅ Request metadata (user, academy, IP, etc.)
- ✅ Custom service layer instrumentation

## Setup Instructions

### 1. Get Your Honeycomb.io API Key

1. Sign up at [Honeycomb.io](https://www.honeycomb.io/)
2. Create a new team or use an existing one
3. Go to **Account Settings** → **Team Settings** → **API Keys**
4. Create a new API key or copy an existing one

### 2. Configure Environment Variables

#### Option A: Docker Development (Recommended)

1. **Create your local environment file:**

   ```bash
   cp .env.local.example .env.local
   ```

2. **Edit `.env.local` and uncomment/set these variables:**

   ```bash
   OTEL_ENABLED=true
   OTEL_SERVICE_NAME=diquis
   OTEL_EXPORTER_OTLP_ENDPOINT="https://api.honeycomb.io"
   OTEL_EXPORTER_OTLP_HEADERS="x-honeycomb-team=YOUR_ACTUAL_API_KEY,x-honeycomb-dataset=diquis-development"
   OTEL_SAMPLE_RATE=1.0
   ```

3. **Docker Compose will automatically load `.env.local`** when you run:

   ```bash
   docker compose up
   ```

#### Option B: Non-Docker Development

Copy the example file and add your configuration:

```bash
cp .env.opentelemetry.example .env.local
```

Edit `.env.local` and set:

```env
# Enable OpenTelemetry
OTEL_ENABLED=true

# Your Honeycomb API Key
HONEYCOMB_API_KEY=your-actual-api-key-here

# Dataset name (environment-specific)
HONEYCOMB_DATASET=diquis-development

# Service name
OTEL_SERVICE_NAME=diquis

# Sampling rate (1.0 = 100%, 0.1 = 10%)
OTEL_SAMPLE_RATE=1.0
```

### 3. Start the Application

```bash
./bin/dev
```

You should see in the logs:

```txt
OpenTelemetry initialized with Honeycomb.io exporter
Service: diquis
Environment: development
Dataset: diquis-development
```

### 4. Generate Some Traffic

Navigate through your application to generate traces:

- Visit the dashboard
- Create/edit players, teams, or training sessions
- Trigger background jobs

### 5. View Traces in Honeycomb

1. Go to [Honeycomb.io](https://ui.honeycomb.io/)
2. Select your dataset (e.g., `diquis-development`)
3. You should see traces appearing in real-time!

## Custom Instrumentation

### Using the OpenTelemetryHelper

Include the helper in your service classes:

```ruby
class PlayerManagement::PlayersService
  include OpenTelemetryHelper

  def create_player(params)
    trace_span("PlayerService#create_player") do
      # Add custom attributes
      trace_attribute("player.name", params[:name])
      trace_attribute("academy.id", params[:academy_id])

      player = Player.create!(params)

      # Record an event
      trace_event("player.created", attributes: {
        "player.id" => player.id,
        "player.age_category" => player.age_category
      })

      player
    rescue => e
      # Record exceptions
      trace_exception(e)
      raise
    end
  end
end
```

### Automatic Method Tracing

Use the `trace_method` class method to automatically wrap methods:

```ruby
class TeamManagement::TeamsService
  include OpenTelemetryHelper

  def assign_players(team_id, player_ids)
    # Method implementation
  end

  # Automatically trace this method
  trace_method :assign_players, span_name: "TeamService#assign_players"
end
```

### Manual Spans

For more control, create spans manually:

```ruby
tracer = OpenTelemetry.tracer_provider.tracer("MyService", "1.0.0")

tracer.in_span("operation_name") do |span|
  span.set_attribute("custom.attribute", "value")
  span.add_event("checkpoint_reached")
  
  # Your code here
end
```

## Configuration Options

### Sampling Rates

Control what percentage of traces are sent to Honeycomb:

- **Development**: `OTEL_SAMPLE_RATE=1.0` (100% - trace everything)
- **Staging**: `OTEL_SAMPLE_RATE=0.5` (50% - half of traces)
- **Production**: `OTEL_SAMPLE_RATE=0.1` (10% - reduce costs)

### Different Datasets per Environment

Use different datasets for each environment:

```env
# Development
HONEYCOMB_DATASET=diquis-development

# Staging
HONEYCOMB_DATASET=diquis-staging

# Production
HONEYCOMB_DATASET=diquis-production
```

### Disabling Specific Instrumentation

Edit `config/initializers/opentelemetry.rb`:

```ruby
c.use_all({
  "OpenTelemetry::Instrumentation::ActiveRecord" => { enabled: false },
  # ... other instrumentations
})
```

## Honeycomb.io Best Practices

### 1. Use Queries to Find Issues

Example queries in Honeycomb:

**Slow database queries:**

```sql
WHERE db.system = "postgresql"
GROUP BY db.statement
ORDER BY P99(duration_ms) DESC
```

**Failed requests:**

```sql
WHERE http.status_code >= 500
GROUP BY http.route
ORDER BY COUNT DESC
```

**Slow background jobs:**

```sql
WHERE span.kind = "internal"
AND name STARTS_WITH "Sidekiq"
GROUP BY name
ORDER BY P95(duration_ms) DESC
```

### 2. Set Up Service Level Objectives (SLOs)

Define performance targets in Honeycomb:

- 95% of requests complete in < 200ms
- 99.9% of requests return 2xx/3xx status codes

### 3. Create Triggers for Alerts

Set up alerts for:

- Error rate spikes
- Slow requests (P99 > threshold)
- Database query performance degradation
- Background job failures

### 4. Use BubbleUp to Find Root Causes

Honeycomb's BubbleUp feature automatically identifies which attributes correlate with slow or failing traces.

## Performance Impact

OpenTelemetry has minimal performance overhead:

- ~1-5ms per traced operation
- Async export doesn't block requests
- Sampling reduces data volume in production

## Troubleshooting

### No Traces Appearing in Honeycomb

1. **Check if OTel is enabled:**

   ```bash
   echo $OTEL_ENABLED
   # Should output: true
   ```

2. **Verify API key is set:**

   ```bash
   echo $HONEYCOMB_API_KEY
   # Should output your API key
   ```

3. **Check Rails logs:**

   ```sql
   OpenTelemetry initialized with Honeycomb.io exporter
   ```

4. **Test the connection:**

   ```bash
   curl -X POST https://api.honeycomb.io/1/events/test \
     -H "X-Honeycomb-Team: $HONEYCOMB_API_KEY" \
     -d '{"test": "data"}'
   ```

### High Data Volume / Costs

1. **Reduce sampling rate:**

   ```env
   OTEL_SAMPLE_RATE=0.1  # Only 10% of traces
   ```

2. **Filter specific endpoints:**
   Create a custom sampler for health checks, etc.

3. **Disable verbose instrumentation:**
   Turn off ActiveRecord or other high-volume sources.

### Spans Not Showing Custom Attributes

Make sure you're calling methods on the current span:

```ruby
span = OpenTelemetry::Trace.current_span
span.set_attribute("key", "value")
```

Or use the helper:

```ruby
include OpenTelemetryHelper
trace_attribute("key", "value")
```

## Advanced Topics

### Distributed Tracing Across Services

If you have microservices, OTel automatically propagates trace context via HTTP headers:

- `traceparent` (W3C standard)
- `tracestate` (for vendor-specific data)

No additional configuration needed!

### Custom Exporters

To send data to multiple backends:

```ruby
c.add_span_processor(
  OpenTelemetry::SDK::Trace::Export::BatchSpanProcessor.new(
    OpenTelemetry::Exporter::OTLP::Exporter.new(
      endpoint: "https://another-backend.com"
    )
  )
)
```

### Metrics (Future)

OpenTelemetry also supports metrics. To enable:

```ruby
# In opentelemetry.rb
require "opentelemetry/sdk"
require "opentelemetry-metrics-sdk"  # Add this gem

OpenTelemetry::SDK.configure do |c|
  # ... existing config ...
  
  # Add metrics
  c.add_metric_reader(
    OpenTelemetry::SDK::Metrics::Export::PeriodicMetricReader.new(
      exporter: OpenTelemetry::Exporter::OTLP::MetricsExporter.new
    )
  )
end
```

## Resources

- [OpenTelemetry Ruby Docs](https://opentelemetry.io/docs/instrumentation/ruby/)
- [Honeycomb.io Documentation](https://docs.honeycomb.io/)
- [OpenTelemetry Specification](https://opentelemetry.io/docs/reference/specification/)
- [Honeycomb Query Language](https://docs.honeycomb.io/working-with-your-data/queries/)

## Support

For issues or questions:

1. Check the [Honeycomb Community Slack](https://pollinators.honeycomb.io/)
2. Review [OpenTelemetry GitHub](https://github.com/open-telemetry/opentelemetry-ruby)
3. Contact your team's DevOps or infrastructure team

---

**Last Updated:** November 5, 2025  
**Version:** 1.0.0
