# OpenTelemetry Quick Start

## Enable Tracing

### For Local Development (Non-Docker)

1. **Set environment variables:**

   ```bash
   export OTEL_ENABLED=true
   export OTEL_EXPORTER_OTLP_HEADERS="x-honeycomb-team=your-key-here,x-honeycomb-dataset=diquis-development"
   ```

2. **Start the app:**

   ```bash
   ./bin/dev
   ```

### For Docker Development

1. **Create `.env.local` file:**

   ```bash
   cp .env.local.example .env.local
   ```

2. **Edit `.env.local` and uncomment/set:**

   ```bash
   OTEL_ENABLED=true
   OTEL_EXPORTER_OTLP_HEADERS="x-honeycomb-team=your-actual-key,x-honeycomb-dataset=diquis-development"
   ```

3. **Start Docker containers:**

   ```bash
   docker compose up
   ```

### View Traces

- Visit: https://ui.honeycomb.io/

## Add Custom Tracing to Your Code

### 1. Track Custom Properties/Attributes

Add context to any span with custom attributes:

```ruby
class UserService
  include OpenTelemetryHelper

  def create_user(params)
    trace_span("UserService.create_user") do
      # Add custom properties to the current span
      trace_attribute("user.email", params[:email])
      trace_attribute("user.role", params[:role])
      trace_attribute("academy.id", current_academy.id)
      trace_attribute("academy.name", current_academy.name)

      user = User.create!(params)

      # Track the result
      trace_attribute("user.id", user.id)
      trace_attribute("user.created", true)

      user
    end
  end
end
```

### 2. Track Business Metrics

```ruby
class PaymentService
  include OpenTelemetryHelper

  def process_payment(amount, currency)
    trace_span("PaymentService.process_payment") do
      # Track important business data
      trace_attribute("payment.amount", amount)
      trace_attribute("payment.currency", currency)
      trace_attribute("payment.method", "stripe")

      result = charge_card(amount, currency)

      # Track the outcome
      trace_attribute("payment.success", result.success?)
      trace_attribute("payment.transaction_id", result.id)

      # Add an event milestone
      trace_event("payment.completed", {
        "amount" => amount,
        "status" => result.status
      })

      result
    end
  end
end
```

### 3. Track Errors and Exceptions

```ruby
class DataImportService
  include OpenTelemetryHelper

  def import_players(file)
    trace_span("DataImportService.import_players") do
      trace_attribute("file.name", file.original_filename)
      trace_attribute("file.size", file.size)

      begin
        players = parse_file(file)
        trace_attribute("players.count", players.size)

        players.each do |player_data|
          import_player(player_data)
        end

        trace_event("import.success")
      rescue CSV::MalformedCSVError => e
        trace_exception(e)
        trace_attribute("error.type", "malformed_csv")
        raise
      rescue => e
        trace_exception(e)
        trace_attribute("error.type", "unknown")
        raise
      end
    end
  end
end
```

### 4. Auto-Trace Methods

Use `trace_method` to automatically wrap methods:

```ruby
class PlayerStatsCalculator
  extend OpenTelemetryHelper::ClassMethods

  # Automatically trace this method
  trace_method :calculate_average
  def calculate_average(player_id, season)
    player = Player.find(player_id)
    stats = player.stats.where(season: season)

    # These attributes are automatically added to the span
    stats.average(:goals)
  end

  # Manually add attributes for auto-traced methods
  trace_method :update_ranking, attributes: ->(player_id, new_rank) {
    {
      "player.id" => player_id,
      "ranking.new" => new_rank
    }
  }
  def update_ranking(player_id, new_rank)
    # Method automatically traced with custom attributes
    Player.find(player_id).update!(rank: new_rank)
  end
end
```

### 5. Track in Controllers

```ruby
class PlayersController < ApplicationController
  include OpenTelemetryHelper

  def create
    trace_span("PlayersController#create") do |span|
      # Track request parameters
      trace_attribute("player.first_name", player_params[:first_name])
      trace_attribute("player.position", player_params[:position])
      trace_attribute("current_user.id", current_user.id)
      trace_attribute("academy.id", current_academy.id)

      @player = Player.new(player_params)

      if @player.save
        trace_attribute("player.id", @player.id)
        trace_event("player.created")

        redirect_to @player, notice: "Player created"
      else
        trace_attribute("validation.failed", true)
        trace_attribute("errors", @player.errors.full_messages.join(", "))

        render :new, status: :unprocessable_entity
      end
    end
  end
end
```

### 6. Track Background Jobs

```ruby
class PlayerReportJob < ApplicationJob
  include OpenTelemetryHelper

  def perform(player_id, report_type)
    trace_span("PlayerReportJob.perform") do
      trace_attribute("player.id", player_id)
      trace_attribute("report.type", report_type)
      trace_attribute("job.queue", queue_name)

      player = Player.find(player_id)
      trace_attribute("player.name", player.full_name)

      report = generate_report(player, report_type)

      trace_attribute("report.pages", report.page_count)
      trace_event("report.generated")

      report
    end
  end
end
```

### 7. Nested Spans for Complex Operations

```ruby
class TournamentService
  include OpenTelemetryHelper

  def create_tournament(params)
    trace_span("TournamentService.create_tournament") do
      trace_attribute("tournament.name", params[:name])

      tournament = Tournament.create!(params)
      trace_attribute("tournament.id", tournament.id)

      # Create a nested span for team setup
      trace_span("setup_teams") do
        trace_attribute("teams.count", params[:team_ids].size)
        tournament.teams = Team.where(id: params[:team_ids])
        trace_event("teams.assigned")
      end

      # Another nested span for scheduling
      trace_span("create_schedule") do
        matches = MatchScheduler.new(tournament).generate
        trace_attribute("matches.count", matches.size)
        trace_event("schedule.created")
      end

      tournament
    end
  end
end
```

### Available Attribute Types

OpenTelemetry supports these attribute value types:

- **String**: `trace_attribute("user.email", "user@example.com")`
- **Integer**: `trace_attribute("player.age", 25)`
- **Float**: `trace_attribute("score.average", 8.5)`
- **Boolean**: `trace_attribute("is_active", true)`
- **Array**: `trace_attribute("tags", ["soccer", "midfielder"])`

### Best Practices

✅ **DO:**

- Use consistent naming (e.g., `player.id`, `academy.name`)
- Track business-relevant metrics
- Add attributes before errors occur
- Use events for milestones
- Keep attribute values simple (no objects/hashes as values)

❌ **DON'T:**

- Add PII (passwords, credit cards, etc.)
- Track high-cardinality data (unique IDs as names)
- Add too many attributes (< 20 per span)
- Use trace for every method call

### Querying Your Custom Attributes in Honeycomb

Once you've added custom attributes, query them in Honeycomb:

```sql
-- Find slow user creation by academy
WHERE span.name = "UserService.create_user"
GROUP BY academy.id
ORDER BY P99(duration_ms) DESC

-- Payment success rate
WHERE span.name = "PaymentService.process_payment"
CALCULATE COUNT, AVG(payment.success)
GROUP BY payment.currency

-- Error breakdown
WHERE error.type EXISTS
GROUP BY error.type
CALCULATE COUNT
```

## Files Added

- `config/initializers/opentelemetry.rb` - Main configuration
- `app/lib/open_telemetry_helper.rb` - Helper module for custom tracing
- `app/middleware/open_telemetry_middleware.rb` - Request context enrichment
- `.env.opentelemetry.example` - Environment configuration template
- `docs/OPENTELEMETRY_HONEYCOMB_SETUP.md` - Complete documentation

## Environment Variables

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `OTEL_ENABLED` | Yes | `false` | Enable/disable OpenTelemetry |
| `HONEYCOMB_API_KEY` | Yes | - | Your Honeycomb.io API key |
| `HONEYCOMB_DATASET` | No | `diquis-{env}` | Dataset name in Honeycomb |
| `OTEL_SERVICE_NAME` | No | `diquis` | Service identifier |
| `OTEL_SAMPLE_RATE` | No | Varies by env | Sampling percentage (0.0-1.0) |

## What's Instrumented Automatically

✅ HTTP requests  
✅ Database queries (PostgreSQL)  
✅ Redis operations  
✅ Sidekiq jobs  
✅ Rails views  
✅ ActiveJob  

## Common Queries in Honeycomb

**Find slow requests:**

```sql
WHERE http.route EXISTS
GROUP BY http.route
ORDER BY P99(duration_ms) DESC
```

**Database query performance:**

```sql
WHERE db.system = "postgresql"
GROUP BY db.statement
ORDER BY P99(duration_ms) DESC
```

**Error rate:**

```sql
WHERE http.status_code >= 500
CALCULATE COUNT
```

## Disable for Testing

```bash
export OTEL_ENABLED=false
```

Or in your test helper:

```ruby
ENV["OTEL_ENABLED"] = "false"
```

## Learn More

See `docs/OPENTELEMETRY_HONEYCOMB_SETUP.md` for complete documentation.
