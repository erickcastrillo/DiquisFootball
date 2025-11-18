# Environment Variables Setup

This document explains the environment variable configuration for the Diquis application.

## Philosophy: Simple and Consolidated

We use **one `.env` file** for all local development configuration. This keeps things simple and prevents confusion.

## File Structure

### `.env` (Your Local Settings)

- **Location**: Root of project
- **Purpose**: All environment configuration for Docker and application
- **Status**: Gitignored (never committed)
- **How to create**: Copy from `.env.example`

```bash
cp .env.example .env
```

**Important:** Uses Docker service names (`postgres`, `redis`) for container networking.

### `.env.example` (Template)

- **Location**: Root of project
- **Purpose**: Template showing all available configuration options
- **Status**: Committed to git
- **Contains**: All environment variables with defaults and documentation

### `.env.local` (Optional - Host Machine Override)

- **Location**: Root of project
- **Purpose**: Override DATABASE_URL/REDIS_URL for host machine Rails commands
- **Status**: Gitignored (never committed)
- **When needed**: Running `rails console`, `rails db:seed`, etc. on host (not in Docker)
- **Contains**: `localhost` URLs instead of Docker service names

```bash
# .env.local - For running Rails on host machine
DATABASE_URL=postgresql://diquis_user:diquis_password@localhost:5432/diquis_development
REDIS_URL=redis://:diquis_redis_password@localhost:6379/0
```

## Common Configuration

### Default Password for Seed Data

Set a custom password for all development test accounts:

```bash
SEED_DEFAULT_PASSWORD=YourSecurePassword123!
```

If not set, defaults to `Dev3l0pment!2025`.

See [Seed Data Guide](./SEED_DATA.md) for more details.

### Database Configuration

Usually not needed - Rails defaults work for both local and Docker setups.

If you need custom settings:

```bash
DATABASE_HOST=localhost
DATABASE_PORT=5432
DATABASE_USERNAME=postgres
DATABASE_PASSWORD=postgres
```

### Redis Configuration

Usually not needed - defaults work for both local and Docker setups.

If you need custom settings:

```bash
REDIS_URL=redis://localhost:6379/0
```

### OpenTelemetry (Optional)

For production-like observability with Honeycomb.io:

```bash
OTEL_ENABLED=true
OTEL_SERVICE_NAME=diquis
OTEL_EXPORTER_OTLP_HEADERS=x-honeycomb-team=your-api-key,x-honeycomb-dataset=diquis-development
OTEL_EXPORTER_OTLP_ENDPOINT=https://api.honeycomb.io
OTEL_SAMPLE_RATE=1.0
```

See [OpenTelemetry Setup](./OPENTELEMETRY_QUICKSTART.md) for details.

## Development Workflow

### Docker Development (Recommended)

1. Copy example file:

   ```bash
   cp .env.example .env
   ```

2. Edit with your settings (optional):

   ```bash
   # Set custom seed password
   SEED_DEFAULT_PASSWORD=MyPassword123!
   
   # Enable OpenTelemetry (optional)
   OTEL_ENABLED=true
   OTEL_EXPORTER_OTLP_HEADERS=x-honeycomb-team=your-api-key,x-honeycomb-dataset=diquis-development
   ```

3. Start Docker (automatically loads `.env`):

   ```bash
   bin/docker/start
   ```

### Local Development (Without Docker)

For local development without Docker, you'll need to set environment variables manually:

```bash
# Option 1: Export in your shell
export SEED_DEFAULT_PASSWORD=MyPassword123!

# Option 2: Use a shell script (not recommended)
# Create bin/dev.local and source .env before running

# Start development
./bin/dev
```

## Best Practices

✅ **DO:**

- Copy `.env.example` to `.env` for local development
- Add new variables to `.env.example` when adding features
- Document all variables in `.env.example`
- Keep sensitive values in `.env` (gitignored)

❌ **DON'T:**

- Commit `.env` or `.env.docker.local` files
- Put real API keys or passwords in `.env.example`
- Create multiple `.env.*` files without good reason
- Hardcode configuration in source code

## Security Notes

🔒 **Important:**

- `.env` is gitignored and should never be committed
- `.env.example` should only contain example/dummy values
- Use Rails credentials (`rails credentials:edit`) for production secrets
- The `SEED_DEFAULT_PASSWORD` is only used for development test data

## Troubleshooting

### Environment not loading

Rails automatically loads `.env` files using the `dotenv-rails` gem in development/test.

Check:

```bash
# Verify .env exists
ls -la .env

# Check for syntax errors
cat .env
```

### Wrong password for seeds

```bash
# Check current setting
echo $SEED_DEFAULT_PASSWORD

# Set in .env
echo "SEED_DEFAULT_PASSWORD=YourPassword123!" >> .env

# Re-run seeds
rails db:seed
```

### Docker not using .env

Docker Compose uses `.env.docker`, not `.env`.

Create overrides in `.env.docker.local`:

```bash
cp .env.docker .env.docker.local
# Edit .env.docker.local
```

## Questions?

- General setup: See [README.md](../README.md)
- Seed data: See [SEED_DATA.md](./SEED_DATA.md)
- OpenTelemetry: See [OPENTELEMETRY_QUICKSTART.md](./OPENTELEMETRY_QUICKSTART.md)
- Docker setup: See [DOCKER_SETUP.md](./DOCKER_SETUP.md)
