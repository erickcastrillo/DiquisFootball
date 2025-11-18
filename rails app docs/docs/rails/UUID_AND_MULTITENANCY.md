# UUID and Multi-Tenancy Implementation

## Overview

This document describes the UUID primary key and multi-tenancy (ActsAsTenant) implementation in Diquis Football.

## UUID Primary Keys

### Configuration

UUID support is enabled through:

1. **PostgreSQL Extension**: The `pgcrypto` extension is enabled via migration:

   ```ruby
   # db/migrate/20251013221720_enable_pgcrypto.rb
   class EnablePgcrypto < ActiveRecord::Migration[8.0]
     def change
       enable_extension "pgcrypto" unless extension_enabled?("pgcrypto")
     end
   end
   ```

2. **Rails Generator Configuration**: All new models automatically use UUID primary keys:

   ```ruby
   # config/application.rb
   config.generators do |g|
     g.orm :active_record, primary_key_type: :uuid
   end
   ```

3. **ApplicationRecord**: Base configuration for all models:

   ```ruby
   # app/models/application_record.rb
   class ApplicationRecord < ActiveRecord::Base
     primary_abstract_class
     self.implicit_order_column = :created_at
   end
   ```

### Benefits

- **Security**: No sequential IDs that can be easily guessed
- **URL-Friendly**: UUIDs work well in URLs and APIs
- **Distributed Systems**: Safe for distributed database systems
- **External References**: Stable identifiers for external API integrations

### Creating UUID Tables

When creating migrations, specify `id: :uuid`:

```ruby
create_table :players, id: :uuid do |t|
  t.references :academy, type: :uuid, null: false, foreign_key: true
  t.string :first_name, null: false
  # ...
  t.timestamps
end
```

## Multi-Tenancy with ActsAsTenant

### Configuration

ActsAsTenant is configured in an initializer:

```ruby
# config/initializers/acts_as_tenant.rb
ActsAsTenant.configure do |config|
  config.require_tenant = true
end
```

### Tenant Model: Academy

The `Academy` model serves as the tenant for the application:

```ruby
class Academy < ApplicationRecord
  # Academy is the tenant model
  validates :name, presence: true, uniqueness: true
  validates :owner_name, :owner_email, :owner_phone, presence: true
end
```

### Tenant-Scoped Models

Models that should be scoped to a tenant use `acts_as_tenant`:

```ruby
class Player < ApplicationRecord
  acts_as_tenant(:academy)

  belongs_to :academy
  # ...
end
```

### Automatic Scoping

With ActsAsTenant configured, all queries are automatically scoped:

```ruby
# All queries automatically filtered by current tenant
ActsAsTenant.with_tenant(academy) do
  Player.all  # Returns only players for this academy
  Player.create!(first_name: "John", last_name: "Doe")  # Automatically associated with academy
end
```

### Controller Integration

The `ApplicationController` handles tenant resolution:

```ruby
class ApplicationController < ActionController::API
  before_action :set_current_tenant

  private

  def set_current_tenant
    academy = find_current_academy
    ActsAsTenant.current_tenant = academy if academy
  end

  def find_current_academy
    # 1. From URL parameter (preferred)
    academy_id = params[:academy_id] || params[:academy_slug]
    return Academy.find_by(id: academy_id) if academy_id

    # 2. From custom header
    academy_id = request.headers["X-Academy-Context"]
    return Academy.find_by(id: academy_id) if academy_id

    # 3. Return nil for routes that don't require tenant
    nil
  end
end
```

### Tenant Resolution Priority

1. **URL Parameter** (Recommended): `/api/v1/{academy_id}/players`
2. **Custom Header**: `X-Academy-Context: <academy_id>`
3. **No Tenant**: For public/auth endpoints

### Cross-Tenant Operations

For system administrators or background jobs that need to work across tenants:

```ruby
ActsAsTenant.without_tenant do
  # Query across all academies
  Player.count
  Academy.all
end
```

### Background Jobs

Set tenant context in background jobs:

```ruby
class PlayerRegistrationJob < ApplicationJob
  def perform(player_id, academy_id)
    ActsAsTenant.with_tenant(Academy.find(academy_id)) do
      player = Player.find(player_id)
      # All operations scoped to academy
    end
  end
end
```

## Testing

### Factory Setup

Factories include tenant associations:

```ruby
FactoryBot.define do
  factory :academy do
    name { Faker::Company.name }
    owner_name { Faker::Name.name }
    owner_email { Faker::Internet.email }
    owner_phone { Faker::PhoneNumber.phone_number }
  end

  factory :player do
    academy  # Associates with tenant
    first_name { Faker::Name.first_name }
    last_name { Faker::Name.last_name }
  end
end
```

### Testing with Tenants

Wrap tenant-scoped operations in `ActsAsTenant.with_tenant`:

```ruby
RSpec.describe Player, type: :model do
  let(:academy) { create(:academy) }

  it "scopes queries to current tenant" do
    ActsAsTenant.with_tenant(academy) do
      player = create(:player)
      expect(Player.all).to include(player)
    end
  end
end
```

### Controller Tests

Test tenant resolution in controllers:

```ruby
RSpec.describe PlayersController, type: :controller do
  let(:academy) { create(:academy) }

  it "sets tenant from params" do
    get :index, params: { academy_id: academy.id }
    expect(ActsAsTenant.current_tenant).to eq(academy)
  end

  it "sets tenant from header" do
    request.headers["X-Academy-Context"] = academy.id
    get :index
    expect(ActsAsTenant.current_tenant).to eq(academy)
  end
end
```

## Best Practices

### 1. Always Use Tenant Context

Never query tenant-scoped models without setting the tenant:

```ruby
# ❌ BAD - May raise error or leak data
Player.all

# ✅ GOOD - Explicit tenant context
ActsAsTenant.with_tenant(academy) do
  Player.all
end
```

### 2. Reference Foreign Keys with UUID Type

Always specify `:uuid` type for foreign key references:

```ruby
create_table :team_memberships, id: :uuid do |t|
  t.references :team, type: :uuid, null: false, foreign_key: true
  t.references :player, type: :uuid, null: false, foreign_key: true
end
```

### 3. Include Academy Association

All tenant-scoped models should explicitly declare the academy association:

```ruby
class Team < ApplicationRecord
  acts_as_tenant(:academy)
  belongs_to :academy  # Explicit association
end
```

### 4. Test Cross-Tenant Isolation

Always test that tenants cannot access each other's data:

```ruby
it "prevents cross-tenant data access" do
  academy1 = create(:academy)
  academy2 = create(:academy)

  player1 = create(:player, academy: academy1)
  player2 = create(:player, academy: academy2)

  ActsAsTenant.with_tenant(academy1) do
    expect(Player.all).to include(player1)
    expect(Player.all).not_to include(player2)
  end
end
```

### 5. Handle Missing Tenant Gracefully

For public endpoints, allow operations without tenant:

```ruby
def set_current_tenant
  academy = find_current_academy

  if academy
    ActsAsTenant.current_tenant = academy
  else
    ActsAsTenant.without_tenant do
      # Allow public access
    end
  end
end
```

## Migration Examples

### Academy (Tenant) Table

```ruby
create_table :academies, id: :uuid do |t|
  t.string :name, null: false
  t.string :owner_name, null: false
  t.string :owner_email, null: false
  t.boolean :is_active, default: true, null: false

  t.timestamps
end

add_index :academies, :name, unique: true
add_index :academies, :is_active
```

### Tenant-Scoped Table

```ruby
create_table :players, id: :uuid do |t|
  t.references :academy, type: :uuid, null: false, foreign_key: true, index: true
  t.string :first_name, null: false
  t.string :last_name, null: false
  t.boolean :is_active, default: true, null: false

  t.timestamps
end

add_index :players, [:academy_id, :first_name, :last_name]
add_index :players, [:academy_id, :is_active]
```

## Troubleshooting

### Error: "ActsAsTenant::Errors::NoTenantSet"

This error occurs when trying to access tenant-scoped data without setting the tenant:

```ruby
# Solution: Wrap in tenant context
ActsAsTenant.with_tenant(academy) do
  Player.all
end
```

### Foreign Key Type Mismatch

If you see errors about mismatched types, ensure foreign keys use `:uuid`:

```ruby
# ❌ Wrong
t.references :academy, null: false, foreign_key: true

# ✅ Correct
t.references :academy, type: :uuid, null: false, foreign_key: true
```

### Tenant Not Found from Header

Ensure the header value is a valid UUID:

```bash
# Correct format
curl -H "X-Academy-Context: 550e8400-e29b-41d4-a716-446655440000" \
     http://localhost:3000/api/v1/players
```

## Summary

- ✅ All models use UUID primary keys by default
- ✅ ActsAsTenant configured with `require_tenant = true`
- ✅ Academy model serves as the tenant
- ✅ ApplicationController handles tenant resolution
- ✅ Comprehensive test coverage for multi-tenancy
- ✅ Documentation for best practices and troubleshooting
