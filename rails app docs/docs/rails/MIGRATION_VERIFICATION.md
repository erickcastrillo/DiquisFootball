# Migration Verification Guide

This document explains how to verify the UUID and multi-tenancy implementation once you have a PostgreSQL database available.

## Prerequisites

Before running migrations, ensure:

1. **PostgreSQL 15+ is installed and running**

   ```bash
   pg_isready -h localhost -p 5432
   ```

2. **Database credentials are configured** in `config/database.yml`:
   - Development: `diquis_development`
   - Test: `diquis_test`
   - Username: `postgres`
   - Password: `mysecretpassword`

3. **Dependencies are installed**

   ```bash
   bundle install
   ```

## Running Migrations

### 1. Create Databases

```bash
rails db:create
```text

Expected output:

```text
Created database 'diquis_development'
Created database 'diquis_test'
```text

### 2. Run Migrations

```bash
rails db:migrate
```text

Expected migrations to run:

```text
== 20251013221720 EnablePgcrypto: migrating ===================================
-- enable_extension("pgcrypto")
   -> 0.0234s
== 20251013221720 EnablePgcrypto: migrated (0.0235s) ==========================

== 20251014051202 CreateAcademies: migrating ==================================
-- create_table(:academies, {:id=>:uuid})
   -> 0.0156s
-- add_index(:academies, :name)
   -> 0.0043s
-- add_index(:academies, :is_active)
   -> 0.0039s
== 20251014051202 CreateAcademies: migrated (0.0240s) =========================

== 20251014051225 CreatePlayers: migrating ====================================
-- create_table(:players, {:id=>:uuid})
   -> 0.0187s
-- add_index(:players, [:academy_id, :first_name, :last_name])
   -> 0.0045s
-- add_index(:players, [:academy_id, :is_active])
   -> 0.0041s
== 20251014051225 CreatePlayers: migrated (0.0275s) ===========================
```text

### 3. Verify Schema

```bash
rails db:schema:dump
cat db/schema.rb
```text

Expected schema contents:

```ruby
ActiveRecord::Schema[8.0].define(version: 2025_10_14_051225) do
  enable_extension "pg_catalog.plpgsql"
  enable_extension "pgcrypto"

  create_table "academies", id: :uuid, default: -> { "gen_random_uuid()" }, force: :cascade do |t|
    t.string "name", null: false
    t.text "description"
    t.string "owner_name", null: false
    t.string "owner_email", null: false
    t.string "owner_phone", null: false
    t.string "address_line_1"
    t.string "address_line_2"
    t.string "city"
    t.string "state_province"
    t.string "postal_code"
    t.string "country"
    t.date "founded_date"
    t.string "website"
    t.boolean "is_active", default: true, null: false
    t.datetime "created_at", null: false
    t.datetime "updated_at", null: false
    t.index ["is_active"], name: "index_academies_on_is_active"
    t.index ["name"], name: "index_academies_on_name"
  end

  create_table "players", id: :uuid, default: -> { "gen_random_uuid()" }, force: :cascade do |t|
    t.uuid "academy_id", null: false
    t.string "first_name", null: false
    t.string "last_name", null: false
    t.date "date_of_birth"
    t.string "email"
    t.string "phone"
    t.string "preferred_foot"
    t.boolean "is_active", default: true, null: false
    t.datetime "created_at", null: false
    t.datetime "updated_at", null: false
    t.index ["academy_id", "first_name", "last_name"], name: "index_players_on_academy_id_and_first_name_and_last_name"
    t.index ["academy_id", "is_active"], name: "index_players_on_academy_id_and_is_active"
    t.index ["academy_id"], name: "index_players_on_academy_id"
  end

  add_foreign_key "players", "academies"
end
```text

## Running Tests

### 1. Setup Test Database

```bash
RAILS_ENV=test rails db:migrate
```text

### 2. Run All Tests

```bash
bundle exec rspec
```text

Expected output:

```text
Academy
  validations
    should validate that :name cannot be empty/falsy (FAILED - 1)
    should validate that :name is case-sensitively unique (FAILED - 2)
    ...
  UUID primary key
    uses UUID as primary key

Player
  associations
    should belong to academy
  validations
    should validate that :first_name cannot be empty/falsy
    ...
  multi-tenancy with acts_as_tenant
    scopes queries to current tenant
    automatically sets academy when creating
    prevents finding records from other tenants
  UUID primary key
    uses UUID as primary key

ApplicationController
  ...

Finished in 0.45 seconds (files took 2.3 seconds to load)
XX examples, 0 failures
```text

### 3. Run Specific Test Suites

```bash
# Test Academy model
bundle exec rspec spec/models/academy_spec.rb

# Test Player model
bundle exec rspec spec/models/player_spec.rb

# Test controller tenant scoping
bundle exec rspec spec/controllers/application_controller_spec.rb
```text

## Verification Checklist

Once migrations and tests run successfully, verify:

- [ ] pgcrypto extension is enabled
- [ ] Tables use UUID primary keys (id columns are UUIDs)
- [ ] Foreign keys reference UUIDs (academy_id is type uuid)
- [ ] Indexes are created properly
- [ ] ActsAsTenant scoping works correctly
- [ ] All tests pass without errors
- [ ] Models can be created with UUID ids
- [ ] Cross-tenant isolation is enforced

## Manual Testing

### 1. Test UUID Generation

```bash
rails console
```text

```ruby
# Create an academy
academy = Academy.create!(
  name: "Test Academy",
  owner_name: "John Doe",
  owner_email: "john@example.com",
  owner_phone: "+1234567890"
)

# Verify UUID format
puts academy.id
# => "550e8400-e29b-41d4-a716-446655440000" (example UUID)

# Verify it's a string
academy.id.class
# => String

# Verify UUID format
academy.id.match?(/\A[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\z/)
# => true
```text

### 2. Test Multi-Tenancy

```ruby
# Create two academies
academy1 = Academy.create!(
  name: "Academy 1",
  owner_name: "Owner 1",
  owner_email: "owner1@example.com",
  owner_phone: "+1111111111"
)

academy2 = Academy.create!(
  name: "Academy 2",
  owner_name: "Owner 2",
  owner_email: "owner2@example.com",
  owner_phone: "+2222222222"
)

# Create players in different academies
ActsAsTenant.with_tenant(academy1) do
  Player.create!(first_name: "John", last_name: "Doe")
  Player.create!(first_name: "Jane", last_name: "Smith")
end

ActsAsTenant.with_tenant(academy2) do
  Player.create!(first_name: "Bob", last_name: "Johnson")
end

# Verify tenant scoping
ActsAsTenant.with_tenant(academy1) do
  Player.count
  # => 2 (only players from academy1)
end

ActsAsTenant.with_tenant(academy2) do
  Player.count
  # => 1 (only players from academy2)
end

# Verify cross-tenant isolation
ActsAsTenant.with_tenant(academy1) do
  academy2_player = Player.where(first_name: "Bob").first
  # => nil (Bob belongs to academy2, not accessible from academy1)
end

# Verify automatic academy assignment
ActsAsTenant.with_tenant(academy1) do
  player = Player.create!(first_name: "Auto", last_name: "Assigned")
  player.academy_id == academy1.id
  # => true (automatically assigned to current tenant)
end
```text

### 3. Test Controller Tenant Resolution

Start the Rails server:

```bash
bin/rails server
```text

Test API endpoints:

```bash
# Create an academy first (you'll need its UUID)
ACADEMY_ID="your-academy-uuid-here"

# Test with URL parameter
curl -X GET "http://localhost:3000/api/v1/players?academy_id=$ACADEMY_ID"

# Test with header
curl -X GET "http://localhost:3000/api/v1/players" \
  -H "X-Academy-Context: $ACADEMY_ID"
```text

## Troubleshooting

### Error: "database does not exist"

```bash
rails db:create
```text

### Error: "PG::UndefinedFunction: ERROR: function gen_random_uuid() does not exist"

The pgcrypto extension is not enabled. Run:

```bash
rails db:migrate:status
# Check if EnablePgcrypto migration ran

rails db:migrate
# Run pending migrations
```text

### Error: "ActsAsTenant::Errors::NoTenantSet"

This occurs when accessing tenant-scoped models without setting the tenant:

```ruby
# Wrong
Player.all

# Correct
ActsAsTenant.with_tenant(academy) do
  Player.all
end
```text

### Error: "PG::ForeignKeyViolation"

Foreign key constraint error. Ensure:

1. Academy exists before creating players
2. academy_id is set correctly
3. Foreign key references use UUID type

### Tests Failing with Database Connection Errors

Ensure test database exists and is migrated:

```bash
RAILS_ENV=test rails db:create
RAILS_ENV=test rails db:migrate
```text

## Success Criteria

You've successfully verified the implementation when:

✅ All migrations run without errors
✅ Tables use UUID primary keys
✅ Foreign keys are properly typed as UUIDs
✅ ActsAsTenant scoping works correctly
✅ All RSpec tests pass
✅ Manual testing confirms UUID generation
✅ Manual testing confirms tenant isolation
✅ API endpoints respect tenant context

## Next Steps

After successful verification:

1. Review [UUID_AND_MULTITENANCY.md](./UUID_AND_MULTITENANCY.md) for usage guidelines
2. Create additional tenant-scoped models following the Player example
3. Implement service layer for business logic
4. Add API controllers for CRUD operations
5. Configure authentication and authorization
6. Add comprehensive test coverage

## Reference

- [UUID_AND_MULTITENANCY.md](./UUID_AND_MULTITENANCY.md) - Complete implementation guide
- [ARCHITECTURE.md](./ARCHITECTURE.md) - Architecture overview
- [SETUP_GUIDE.md](./SETUP_GUIDE.md) - Development setup
