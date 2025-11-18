# Quick Start: UUID and Multi-Tenancy

This guide helps you quickly verify and use the UUID and multi-tenancy implementation.

## Prerequisites Check

```bash
# Check Ruby version (need 3.3.0+)
ruby --version

# Check PostgreSQL (need 15+)
psql --version

# Check if PostgreSQL is running
pg_isready -h localhost -p 5432
```

## Setup (First Time)

```bash
# 1. Install dependencies
bundle install

# 2. Create databases
rails db:create

# 3. Run migrations
rails db:migrate

# 4. Verify migrations
rails db:migrate:status
```

Expected output should show these migrations as "up":

- `20251013221720_enable_pgcrypto.rb`
- `20251014051202_create_academies.rb`
- `20251014051225_create_players.rb`

## Quick Test

```bash
# Run all tests
bundle exec rspec

# Run specific test suites
bundle exec rspec spec/models/academy_spec.rb
bundle exec rspec spec/models/player_spec.rb
bundle exec rspec spec/integration/uuid_multitenancy_integration_spec.rb
```

## Interactive Console Testing

```bash
rails console
```

### Test 1: UUID Generation

```ruby
# Create an academy
academy = Academy.create!(
  name: "Quick Test Academy",
  owner_name: "John Doe",
  owner_email: "john@test.com",
  owner_phone: "+1234567890"
)

# Verify UUID format
academy.id
# => "550e8400-e29b-41d4-a716-446655440000" (your UUID will be different)

# Check it's a valid UUID
academy.id.match?(/\A[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\z/)
# => true
```

### Test 2: Multi-Tenancy Isolation

```ruby
# Create two academies
academy1 = Academy.create!(
  name: "Academy One",
  owner_name: "Owner 1",
  owner_email: "owner1@test.com",
  owner_phone: "+1111111111"
)

academy2 = Academy.create!(
  name: "Academy Two",
  owner_name: "Owner 2",
  owner_email: "owner2@test.com",
  owner_phone: "+2222222222"
)

# Add players to academy1
ActsAsTenant.with_tenant(academy1) do
  Player.create!(first_name: "John", last_name: "Doe")
  Player.create!(first_name: "Jane", last_name: "Smith")
end

# Add players to academy2
ActsAsTenant.with_tenant(academy2) do
  Player.create!(first_name: "Bob", last_name: "Johnson")
end

# Verify isolation - academy1 sees only its players
ActsAsTenant.with_tenant(academy1) do
  Player.count
  # => 2
  Player.all.map(&:full_name)
  # => ["John Doe", "Jane Smith"]
end

# Verify isolation - academy2 sees only its players
ActsAsTenant.with_tenant(academy2) do
  Player.count
  # => 1
  Player.all.map(&:full_name)
  # => ["Bob Johnson"]
end

# System-wide query (for admins)
ActsAsTenant.without_tenant do
  Player.count
  # => 3
  Academy.count
  # => 2
end
```

### Test 3: Automatic Tenant Assignment

```ruby
academy = Academy.first

ActsAsTenant.with_tenant(academy) do
  # Create without specifying academy
  player = Player.create!(
    first_name: "Auto",
    last_name: "Assigned"
  )

  # Academy is automatically set
  player.academy_id == academy.id
  # => true
end
```

## API Testing (when server is running)

Start the server:

```bash
bin/rails server
```

In another terminal:

```bash
# Get your academy UUID first
ACADEMY_ID="your-academy-uuid"

# Test with URL parameter
curl "http://localhost:3000/health?academy_id=$ACADEMY_ID"

# Test with header
curl "http://localhost:3000/health" \
  -H "X-Academy-Context: $ACADEMY_ID"
```

## Common Commands

```bash
# Run migrations
rails db:migrate

# Rollback last migration
rails db:rollback

# Reset database (CAUTION: deletes all data)
rails db:reset

# Check migration status
rails db:migrate:status

# Run specific test file
bundle exec rspec spec/models/player_spec.rb

# Run tests with coverage
COVERAGE=true bundle exec rspec

# Start Rails console
rails console

# Start Rails server
rails server
```

## Quick Verification Checklist

Run through this checklist to verify everything works:

- [ ] Migrations run successfully
- [ ] Can create Academy with UUID
- [ ] Can create Player with UUID
- [ ] Player automatically gets academy_id when using ActsAsTenant.with_tenant
- [ ] Players from academy1 are not visible from academy2 context
- [ ] All RSpec tests pass
- [ ] UUID format is correct (8-4-4-4-12 hex digits)
- [ ] Foreign keys are properly typed as UUID

## Troubleshooting

### "database does not exist"

```bash
rails db:create
```

### "PG::UndefinedFunction: function gen_random_uuid() does not exist"

```bash
rails db:migrate
# Make sure EnablePgcrypto migration ran
```

### "ActsAsTenant::Errors::NoTenantSet"

```ruby
# Wrong
Player.all

# Correct
ActsAsTenant.with_tenant(academy) do
  Player.all
end
```

### Tests failing

```bash
# Setup test database
RAILS_ENV=test rails db:create db:migrate

# Run tests
bundle exec rspec
```

## Next Steps

1. **Read the full guide:** [docs/UUID_AND_MULTITENANCY.md](docs/UUID_AND_MULTITENANCY.md)
2. **Detailed verification:** [docs/MIGRATION_VERIFICATION.md](docs/MIGRATION_VERIFICATION.md)
3. **Create more models:** Follow the Player pattern for Team, Training, etc.
4. **Add API endpoints:** Create controllers with tenant scoping
5. **Add authentication:** Integrate Devise/JWT for user auth

## Example: Creating a New Tenant-Scoped Model

```ruby
# 1. Generate migration
rails generate migration CreateTeams \
  slug:uuid:uniq \
  academy:references \
  name:string \
  is_active:boolean

# 2. Edit migration - ensure UUID types
class CreateTeams < ActiveRecord::Migration[8.0]
  def change
    create_table :teams, id: :uuid do |t|
      t.references :academy, type: :uuid, null: false, foreign_key: true
      t.string :name, null: false
      t.boolean :is_active, default: true, null: false

      t.timestamps
    end

    add_index :teams, [:academy_id, :name]
  end
end

# 3. Create model with acts_as_tenant
class Team < ApplicationRecord
  acts_as_tenant(:academy)

  belongs_to :academy

  validates :name, presence: true
end

# 4. Run migration
rails db:migrate

# 5. Test it
ActsAsTenant.with_tenant(Academy.first) do
  Team.create!(name: "U-16 Team A")
end
```

## Quick Reference

### UUID Regex

```ruby
/\A[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\z/
```

### Tenant Context Patterns

```ruby
# Set tenant for block
ActsAsTenant.with_tenant(academy) do
  # All queries scoped to academy
end

# Query without tenant (admin)
ActsAsTenant.without_tenant do
  # Can see all data
end

# Check current tenant
ActsAsTenant.current_tenant
```

### Migration Template

```ruby
create_table :table_name, id: :uuid do |t|
  t.references :academy, type: :uuid, null: false, foreign_key: true
  # other columns
  t.timestamps
end

add_index :table_name, [:academy_id, :other_column]
```

---

**Need Help?** Check the full documentation:

- [UUID_AND_MULTITENANCY.md](docs/UUID_AND_MULTITENANCY.md)
- [MIGRATION_VERIFICATION.md](docs/MIGRATION_VERIFICATION.md)
- [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)
