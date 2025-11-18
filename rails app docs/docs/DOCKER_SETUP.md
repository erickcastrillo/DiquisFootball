# Docker Development Setup

This document explains how to run the Diquis Football Academy application using Docker for development.

## Quick Start

1. **Copy environment configuration:**

   ```bash
   cp .env.example .env
   ```

   Then customize `.env` with your settings (database passwords, etc.)

2. **Start all services:**

   ```bash
   ./docker-dev
   ```

3. **Access the application:**
   - Rails App: http://localhost:3000
   - Sidekiq Web UI: http://localhost:4567
   - PostgreSQL: localhost:5432
   - Redis: localhost:6379

   **Note:** Vite dev server runs inside the web container via `bin/dev`

## Reset Docker Environment

When you install new gems or need to rebuild from scratch:

```bash
./docker-dev reset
```

This will:

- Stop and remove all containers
- Remove all volumes (⚠️ deletes database data!)
- Prune Docker system
- Rebuild images with `--no-cache`
- Recreate all containers

After reset, you'll need to:

```bash
docker compose exec web rails db:migrate
docker compose exec web rails db:seed
```

## Services

The Docker setup includes the following services:

### 🌐 Web (Rails Application)

- **Port:** 3000
- **Container:** `diquis_web`
- **Purpose:** Main Rails application server
- **Dependencies:** PostgreSQL, Redis

### 🔧 Sidekiq (Background Jobs)

- **Container:** `diquis_sidekiq`
- **Purpose:** Background job processing with cron jobs
- **Dependencies:** PostgreSQL, Redis, Web

### ⚡ Vite (Asset Development)

- **Port:** 5173
- **Container:** `diquis_vite`
- **Purpose:** Frontend asset compilation and hot reloading

### 🔧 Sidekiq Web UI

- **Port:** 4567
- **Container:** `diquis_sidekiq_web`
- **Purpose:** Web interface for monitoring Sidekiq jobs and queues

### 🗄️ PostgreSQL (Database)

- **Port:** 5432
- **Container:** `diquis_postgres`
- **Database:** `diquis_development`
- **Username:** `diquis_user`
- **Password:** `diquis_password`

### 🔴 Redis (Cache & Job Queue)

- **Port:** 6379
- **Container:** `diquis_redis`
- **Purpose:** Session storage, caching, and Sidekiq job queue
- **Password:** `diquis_redis_password`

## Management Scripts

### Main Operations

```bash
# Start all services
bin/docker/start

# Stop all services
bin/docker/stop

# View logs (all services or specific service)
bin/docker/logs
bin/docker/logs web
bin/docker/logs sidekiq

# Open shell in container
bin/docker/shell web
bin/docker/shell sidekiq
```

### Database Operations

```bash
# Run migrations
bin/docker/db migrate

# Seed database
bin/docker/db seed

# Reset database (drop, create, migrate, seed)
bin/docker/db reset

# Open Rails console
bin/docker/db console

# Open PostgreSQL console
bin/docker/db psql

# Open Redis console
bin/docker/db redis
```

### Sidekiq Operations

```bash
# Restart Sidekiq
bin/docker/sidekiq restart

# View Sidekiq logs
bin/docker/sidekiq logs

# Load cron jobs
bin/docker/sidekiq cron:load

# List cron jobs
bin/docker/sidekiq cron:list

# Clear job queues
bin/docker/sidekiq clear
```

## Docker Compose Commands

Direct docker compose commands for advanced usage:

```bash
# Start services in background
docker compose up -d

# Start specific service
docker compose up -d web

# Stop services
docker compose down

# Stop services and remove volumes
docker compose down -v

# View service status
docker compose ps

# Follow logs
docker compose logs -f

# Execute command in running container
docker compose exec web bundle exec rails console

# Build images
docker compose build

# Build images without cache
docker compose build --no-cache
```

## Environment Configuration

### Default Configuration (.env.docker)

```env
# Database
DATABASE_URL=postgresql://diquis_user:diquis_password@postgres:5432/diquis_development

# Redis
REDIS_URL=redis://:diquis_redis_password@redis:6379/0

# Rails
RAILS_ENV=development
RAILS_MAX_THREADS=5
```

### Custom Configuration (.env.docker.local)

Copy `.env.docker` to `.env.docker.local` and customize:

```bash
cp .env.docker .env.docker.local
```

Edit `.env.docker.local` with your preferred settings.

## Data Persistence

Docker volumes are used for data persistence:

- `postgres_data`: PostgreSQL database files
- `redis_data`: Redis data files
- `bundle_cache`: Ruby gems cache
- `rails_cache`: Rails application cache
- `node_modules`: Node.js dependencies

## Development Workflow

### 1. Initial Setup

```bash
# Start services
bin/docker/start

# Setup database
bin/docker/db migrate
bin/docker/db seed

# Load Sidekiq cron jobs
bin/docker/sidekiq cron:load
```

### 2. Daily Development

```bash
# Start services
bin/docker/start

# Make code changes (files are mounted)
# Changes are reflected immediately

# Run tests
bin/docker/shell web
bundle exec rspec

# Check logs
bin/docker/logs web
```

### 3. Database Changes

```bash
# Create migration
bin/docker/shell web
bundle exec rails generate migration AddFieldToModel

# Run migration
bin/docker/db migrate
```

### 4. Background Jobs

```bash
# Generate new job
bin/docker/shell web
rails generate background_job my_task --slice=MySlice

# Test job
bin/docker/shell web
MySlice::MyTaskJob.perform_later(args)

# Monitor in Sidekiq Web UI
# Visit: http://localhost:4567
```

## VS Code Integration

Use VS Code tasks for common Docker operations:

1. **Cmd/Ctrl + Shift + P**
2. **Tasks: Run Task**
3. Select from Docker tasks:
   - 🐳 Docker: Start All Services
   - 🐳 Docker: Stop All Services
   - 🐳 Docker: View Logs
   - 🐳 Docker: Open Shell
   - 🐳 Docker: Database Console
   - 🐳 Docker: Database Migrate

## Troubleshooting

### Services Won't Start

```bash
# Check Docker is running
docker info

# Check service status
docker compose ps

# View logs for specific service
bin/docker/logs postgres
bin/docker/logs redis
```

### Database Connection Issues

```bash
# Check PostgreSQL is ready
docker compose exec postgres pg_isready -U diquis_user

# Reset database
bin/docker/db reset
```

### Sidekiq Jobs Not Processing

```bash
# Check Sidekiq service
bin/docker/logs sidekiq

# Restart Sidekiq
bin/docker/sidekiq restart

# Check Redis connection
bin/docker/db redis
```

### Port Conflicts

If ports 3000, 5432, or 6379 are already in use:

1. Stop conflicting services
2. Or modify ports in `docker-compose.yml`

### Clean Slate Reset

```bash
# Stop everything and remove all data
docker compose down -v

# Remove all images
docker compose down -v --rmi all

# Start fresh
bin/docker/start
```

## Performance Tips

1. **Use bind mounts for development** (already configured)
2. **Keep volumes for dependencies** (bundle_cache, node_modules)
3. **Monitor resource usage**: `docker stats`
4. **Clean up unused resources**: `docker system prune`

## Production Deployment

This Docker setup is optimized for development. For production deployment:

1. Use the existing `Dockerfile` (production-ready)
2. Use Kamal for deployment (already configured)
3. Set proper environment variables
4. Use external databases and Redis clusters

## Security Notes

For development:

- Default passwords are used (change for production)
- All services are accessible on localhost
- No SSL/TLS encryption (fine for development)

For production:

- Use strong, unique passwords
- Enable SSL/TLS
- Use proper secrets management
- Restrict network access

## Next Steps

1. **Start developing**: Make code changes and see them reflected immediately
2. **Create background jobs**: Use the job generator for slice-based jobs
3. **Monitor jobs**: Use the Sidekiq Web UI to monitor background processing
4. **Run tests**: Execute RSpec tests in the Docker environment
5. **Deploy**: Use Kamal for production deployment when ready
