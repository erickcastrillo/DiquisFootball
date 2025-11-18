# Shared Components Usage Guide

This document provides detailed usage patterns and examples for the shared components in the Diquis application.

## BaseService Pattern

### Overview

The `BaseService` class provides a consistent pattern for implementing business logic with built-in validation, transaction management, and error handling.

### Key Principles

1. **Single Responsibility**: Each service should handle one specific business operation
2. **Validation First**: Parameters are validated before execution
3. **Transaction Safety**: All operations are wrapped in a database transaction
4. **Consistent Results**: Always returns a `ServiceResult` object

### Basic Service Example

```ruby
class CreatePlayerService < BaseService
  attr_accessor :name, :email, :age, :academy

  validates :name, presence: true
  validates :email, presence: true, format: { with: URI::MailTo::EMAIL_REGEXP }
  validates :age, numericality: { greater_than: 0, less_than: 100 }, allow_nil: true
  validates :academy, presence: true

  private

  def execute
    player = Player.create!(
      name: name,
      email: email,
      age: age,
      academy: academy
    )

    { player: player, message: 'Player created successfully' }
  end
end
```

### Using the Service in a Controller

```ruby
class Api::V1::PlayersController < Api::V1::BaseController
  def create
    service = CreatePlayerService.new(player_params.merge(academy: current_academy))
    result = service.call

    if result.success?
      render json: { 
        data: PlayerSerializer.new(result.data[:player]).serializable_hash,
        message: result.data[:message]
      }, status: :created
    else
      render json: { 
        errors: result.errors 
      }, status: :unprocessable_entity
    end
  end

  private

  def player_params
    params.require(:player).permit(:name, :email, :age)
  end
end
```

### Complex Service with Multiple Operations

```ruby
class RegisterPlayerService < BaseService
  attr_accessor :player_data, :team_id, :position_id, :academy, :registered_by

  validates :player_data, presence: true
  validates :team_id, presence: true
  validates :academy, presence: true
  validates :registered_by, presence: true

  private

  def execute
    player = create_player
    assign_to_team(player)
    create_audit_log(player)
    send_welcome_email(player)

    {
      player: player,
      team: player.teams.first,
      message: 'Player registered successfully'
    }
  end

  def create_player
    Player.create!(
      name: player_data[:name],
      email: player_data[:email],
      date_of_birth: player_data[:date_of_birth],
      academy: academy,
      position_id: position_id
    )
  end

  def assign_to_team(player)
    team = Team.find(team_id)
    PlayerTeam.create!(player: player, team: team)
  end

  def create_audit_log(player)
    player.create_audit_log(
      action: 'player_registered',
      user: registered_by,
      changes: {
        player_id: player.id,
        team_id: team_id
      }
    )
  end

  def send_welcome_email(player)
    PlayerMailer.welcome_email(player).deliver_later
  end
end
```

### Service with Custom Validation

```ruby
class UpdatePlayerStatusService < BaseService
  attr_accessor :player, :new_status, :updated_by

  validates :player, presence: true
  validates :new_status, presence: true, inclusion: { in: %w[active inactive suspended] }
  validates :updated_by, presence: true

  private

  def validate_params
    super
    return unless player && new_status

    if player.status == new_status
      errors.add(:base, "Player is already #{new_status}")
    end

    if new_status == 'suspended' && !updated_by.can_suspend_players?
      errors.add(:base, 'You do not have permission to suspend players')
    end
  end

  def execute
    old_status = player.status
    player.update!(status: new_status)

    player.create_audit_log(
      action: 'status_changed',
      user: updated_by,
      changes: { status: [old_status, new_status] }
    )

    { player: player, old_status: old_status, new_status: new_status }
  end
end
```

### Testing Services

```ruby
require 'rails_helper'

RSpec.describe CreatePlayerService, type: :service do
  let(:academy) { create(:academy) }
  let(:valid_params) do
    {
      name: 'Lionel Messi',
      email: 'messi@fcbarcelona.com',
      age: 16,
      academy: academy
    }
  end

  describe '#call' do
    context 'with valid parameters' do
      it 'creates a player successfully' do
        service = CreatePlayerService.new(valid_params)
        result = service.call

        expect(result).to be_success
        expect(result.data[:player]).to be_a(Player)
        expect(result.data[:player].name).to eq('Lionel Messi')
        expect(result.data[:message]).to eq('Player created successfully')
      end

      it 'creates a player record in the database' do
        service = CreatePlayerService.new(valid_params)
        
        expect {
          service.call
        }.to change(Player, :count).by(1)
      end
    end

    context 'with invalid parameters' do
      it 'returns failure when name is missing' do
        service = CreatePlayerService.new(valid_params.merge(name: nil))
        result = service.call

        expect(result).to be_failure
        expect(result.errors).to include("Name can't be blank")
      end

      it 'returns failure with invalid email format' do
        service = CreatePlayerService.new(valid_params.merge(email: 'invalid-email'))
        result = service.call

        expect(result).to be_failure
        expect(result.errors).to include('Email is invalid')
      end

      it 'does not create a player record when validation fails' do
        service = CreatePlayerService.new(valid_params.merge(name: nil))
        
        expect {
          service.call
        }.not_to change(Player, :count)
      end
    end

    context 'when database operation fails' do
      it 'rolls back the transaction' do
        allow_any_instance_of(Player).to receive(:save!).and_raise(ActiveRecord::RecordInvalid)
        service = CreatePlayerService.new(valid_params)
        
        result = service.call

        expect(result).to be_failure
        expect(Player.count).to eq(0)
      end
    end
  end
end
```

## Model Concerns

### Sluggable Concern

The `Sluggable` concern adds UUID-based slug generation for use in URLs.

#### Usage

```ruby
class Academy < ApplicationRecord
  include Sluggable

  validates :name, presence: true
end
```

#### Migration

```ruby
class CreateAcademies < ActiveRecord::Migration[8.0]
  def change
    create_table :academies do |t|
      t.string :slug, null: false, index: { unique: true }
      t.string :name, null: false
      
      t.timestamps
    end
  end
end
```

#### Examples

```ruby
# Creating a record with automatic slug generation
academy = Academy.create!(name: 'FC Barcelona Academy')
academy.slug # => "550e8400-e29b-41d4-a716-446655440000"

# Using slug in URLs
academy_path(academy) # => "/academies/550e8400-e29b-41d4-a716-446655440000"

# Finding by slug
Academy.find_by!(slug: params[:id])
```

#### Testing

```ruby
require 'rails_helper'

RSpec.describe Sluggable, type: :concern do
  let(:model_class) do
    Class.new(ApplicationRecord) do
      self.table_name = 'academies'
      include Sluggable
    end
  end

  it 'generates a UUID slug on creation' do
    instance = model_class.new(name: 'Test Academy')
    instance.valid?
    
    expect(instance.slug).to be_present
    expect(instance.slug).to match(/\A[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\z/)
  end

  it 'does not override existing slug' do
    existing_slug = SecureRandom.uuid
    instance = model_class.new(name: 'Test Academy', slug: existing_slug)
    instance.valid?
    
    expect(instance.slug).to eq(existing_slug)
  end

  it 'validates slug presence' do
    instance = model_class.new(name: 'Test Academy')
    instance.slug = nil
    
    expect(instance).not_to be_valid
    expect(instance.errors[:slug]).to include("can't be blank")
  end

  describe '#to_param' do
    it 'returns the slug' do
      instance = model_class.new(name: 'Test Academy', slug: 'custom-slug')
      expect(instance.to_param).to eq('custom-slug')
    end
  end
end
```

### Auditable Concern

The `Auditable` concern adds audit logging capabilities to track changes.

#### Usage

```ruby
class Player < ApplicationRecord
  include Auditable

  validates :name, presence: true
end
```

#### Creating Audit Logs

```ruby
# In a service or controller
player = Player.find_by!(slug: params[:id])

player.create_audit_log(
  action: 'profile_updated',
  user: current_user,
  changes: {
    email: [player.email_was, player.email],
    phone: [player.phone_was, player.phone]
  }
)
```

#### Querying Audit Logs

```ruby
# Get all audit logs for a player
player.audit_logs.recent

# Get audit logs for specific action
player.audit_logs.for_action('profile_updated')

# Get audit logs within date range
player.audit_logs.where(
  performed_at: 1.week.ago..Time.current
).order(performed_at: :desc)

# Get audit logs by user
player.audit_logs.where(user: current_user)
```

#### Testing

```ruby
require 'rails_helper'

RSpec.describe Auditable, type: :concern do
  let(:user) { create(:user) }
  let(:player) { create(:player) }

  describe '#create_audit_log' do
    it 'creates an audit log entry' do
      expect {
        player.create_audit_log(
          action: 'status_changed',
          user: user,
          changes: { status: ['active', 'inactive'] }
        )
      }.to change(AuditLog, :count).by(1)
    end

    it 'stores the correct data' do
      player.create_audit_log(
        action: 'profile_updated',
        user: user,
        changes: { email: ['old@example.com', 'new@example.com'] }
      )

      log = player.audit_logs.last
      expect(log.action).to eq('profile_updated')
      expect(log.user).to eq(user)
      expect(log.changes['email']).to eq(['old@example.com', 'new@example.com'])
      expect(log.performed_at).to be_within(1.second).of(Time.current)
    end
  end

  describe 'associations' do
    it 'destroys audit logs when record is destroyed' do
      player.create_audit_log(
        action: 'created',
        user: user,
        changes: {}
      )

      expect {
        player.destroy
      }.to change(AuditLog, :count).by(-1)
    end
  end
end
```

## Best Practices

### Service Layer

1. **Keep Services Small**: If a service is doing too much, consider breaking it into multiple services
2. **Use Descriptive Names**: Service names should clearly indicate what they do (verb + noun pattern)
3. **Return Meaningful Data**: The `execute` method should return a hash with all relevant data
4. **Log Important Actions**: Use audit logs for significant operations
5. **Handle Errors Gracefully**: Let the BaseService handle common errors, add custom handling only when needed
6. **Write Comprehensive Tests**: Test success cases, validation failures, and error scenarios

### Model Concerns

1. **Document Requirements**: Clearly document what columns/associations are needed
2. **Keep Concerns Focused**: Each concern should add one specific capability
3. **Test Independently**: Use temporary test models to avoid coupling
4. **Consider Performance**: Be mindful of N+1 queries when adding associations
5. **Use Scopes**: Provide useful scopes for common queries

## Common Patterns

### Service Composition

```ruby
class CompletePlayerRegistrationService < BaseService
  attr_accessor :player_data, :team_id, :academy, :registered_by

  private

  def execute
    # Use other services for complex operations
    player_result = CreatePlayerService.new(
      player_data.merge(academy: academy)
    ).call
    
    return player_result unless player_result.success?

    player = player_result.data[:player]
    
    team_result = AssignPlayerToTeamService.new(
      player: player,
      team_id: team_id,
      assigned_by: registered_by
    ).call

    return team_result unless team_result.success?

    {
      player: player,
      team: team_result.data[:team],
      message: 'Registration completed successfully'
    }
  end
end
```

### Chaining Concerns

```ruby
class Academy < ApplicationRecord
  include Sluggable
  include Auditable

  validates :name, presence: true
  # ... rest of model
end

# Both concerns work together
academy = Academy.create!(name: 'Test Academy')
academy.slug # Generated by Sluggable
academy.create_audit_log(action: 'created', user: user, changes: {})
```

## Error Handling

### Service Errors

The BaseService automatically handles common errors:

- `ActiveRecord::RecordInvalid` - Returns validation errors from the record
- `ActiveRecord::RecordNotFound` - Returns "Record not found" message
- `StandardError` - Logs the error and returns generic error message

### Custom Error Handling

```ruby
class CustomService < BaseService
  private

  def execute
    # Your logic that might raise custom exceptions
  rescue CustomBusinessError => e
    errors.add(:base, e.message)
    return ServiceResult.new(success: false, data: nil, errors: errors.full_messages)
  end
end
```

## Performance Considerations

### Avoiding N+1 Queries in Services

```ruby
class FetchPlayersWithDetailsService < BaseService
  attr_accessor :academy

  private

  def execute
    players = academy.players
      .includes(:teams, :position, :audit_logs)
      .order(created_at: :desc)

    { players: players }
  end
end
```

### Using Batch Operations

```ruby
class BulkUpdatePlayerStatusService < BaseService
  attr_accessor :player_ids, :new_status, :updated_by

  validates :player_ids, presence: true
  validates :new_status, presence: true

  private

  def execute
    players = Player.where(id: player_ids)
    
    players.update_all(
      status: new_status,
      updated_at: Time.current
    )

    # Create audit logs in batch
    audit_logs = players.map do |player|
      {
        auditable: player,
        user: updated_by,
        action: 'bulk_status_update',
        changes: { status: new_status },
        performed_at: Time.current
      }
    end
    
    AuditLog.create!(audit_logs)

    { updated_count: players.count }
  end
end
```

## Conclusion

The shared components provide a solid foundation for building consistent, maintainable, and testable code. Follow these patterns and best practices to ensure your code integrates well with the rest of the application.
