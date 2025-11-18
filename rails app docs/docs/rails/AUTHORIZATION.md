# Authorization with Pundit

This document describes how authorization is implemented in Diquis using the Pundit gem.

## Overview

Diquis uses [Pundit](https://github.com/varvet/pundit) for authorization. Pundit provides a set of helpers that guide you in leveraging regular Ruby classes and object-oriented design patterns to build a simple, robust authorization system.

## Architecture

### ApplicationPolicy

The base policy class is located at `shared/policies/application_policy.rb`. All other policies inherit from this class. By default, all actions are denied unless explicitly permitted.

```ruby
class ApplicationPolicy
  def index?
    false  # Deny by default
  end

  def show?
    false  # Deny by default
  end

  # ... more actions
end
```

### Slice-Specific Policies

Each slice has its own policy classes that inherit from `ApplicationPolicy` and define authorization rules specific to that domain:

- `app/slices/academy_management/policies/academy_policy.rb`
- `app/slices/player_management/policies/player_policy.rb`
- `app/slices/team_management/policies/team_policy.rb`
- `app/slices/training_management/policies/training_policy.rb`
- `app/slices/asset_management/policies/asset_policy.rb`
- `app/slices/reporting_analytics/policies/report_policy.rb`
- `app/slices/communication_notification/policies/notification_policy.rb`
- `app/slices/shared_resources/policies/shared_resource_policy.rb`

## Usage

### In Controllers

To authorize an action in a controller, use the `authorize` method:

```ruby
class PlayersController < Api::V1::BaseController
  def create
    @player = Player.new(player_params)
    authorize @player  # Calls PlayerPolicy#create?
    
    if @player.save
      render json: @player
    else
      render json: @player.errors, status: :unprocessable_entity
    end
  end

  def update
    @player = Player.find(params[:id])
    authorize @player  # Calls PlayerPolicy#update?
    
    if @player.update(player_params)
      render json: @player
    else
      render json: @player.errors, status: :unprocessable_entity
    end
  end
end
```

### Authorization Scopes

To filter collections based on user permissions, use policy scopes:

```ruby
class PlayersController < Api::V1::BaseController
  def index
    @players = policy_scope(Player)  # Calls PlayerPolicy::Scope#resolve
    render json: @players
  end
end
```

### Explicit Policy Selection

If the policy class cannot be inferred from the model, you can specify it explicitly:

```ruby
authorize @player, policy_class: PlayerManagement::PlayerPolicy
```

### Headless Policies

For actions that don't have a specific record, you can use symbol-based authorization:

```ruby
class ReportsController < Api::V1::BaseController
  def generate
    authorize :report, :generate?  # Calls ReportPolicy#generate?
    # ... generate report
  end
end
```

## Policy Methods

Standard policy methods map to controller actions:

- `index?` - Can list records
- `show?` - Can view a specific record
- `create?` / `new?` - Can create a new record
- `update?` / `edit?` - Can update an existing record
- `destroy?` - Can delete a record

You can define custom methods for specific actions:

```ruby
class PlayerPolicy < ApplicationPolicy
  def transfer?
    user.present? && (system_admin? || user_has_permission?(:transfer))
  end
end
```

## Authorization Rules

### System Admins

System admins have full access to all resources:

```ruby
def create?
  system_admin? || custom_logic
end
```

### Academy-Based Access

Most resources are scoped to academies. Users can only access resources in their academies:

```ruby
def show?
  user.present? && same_academy?
end

private

def same_academy?
  return true if system_admin?
  user.academy_ids.include?(record.academy_id)
end
```

### Permission-Based Access

Users have different permission levels within academies:

```ruby
def update?
  user.present? && same_academy? && (system_admin? || user_has_permission?(:update))
end

private

def user_has_permission?(permission)
  academy_user = user.academy_users.active.find_by(academy: record.academy)
  academy_user&.can?(permission)
end
```

## Error Handling

When authorization fails, Pundit raises a `Pundit::NotAuthorizedError`. This is rescued in `ApplicationController` and returns a JSON response:

```json
{
  "error": "PERMISSION_DENIED",
  "message": "You are not authorized to perform this action",
  "context": {
    "action": "create?",
    "resource": "Player"
  }
}
```

HTTP Status: `403 Forbidden`

## Testing Policies

### Policy Specs

Create policy specs in `spec/policies/` or `spec/slices/[slice_name]/policies/`:

```ruby
require "rails_helper"

RSpec.describe PlayerPolicy, type: :policy do
  subject(:policy) { described_class.new(user, player) }

  let(:player) { create(:player) }

  describe "with system admin" do
    let(:user) { create(:user, system_admin: true) }

    it { is_expected.to permit_action(:create) }
    it { is_expected.to permit_action(:update) }
    it { is_expected.to permit_action(:destroy) }
  end

  describe "with regular user" do
    let(:user) { create(:user) }

    it { is_expected.not_to permit_action(:create) }
  end
end
```

### Custom Matchers

The project includes custom RSpec matchers for testing policies:

- `permit_action(:action_name)` - Test a single action
- `permit_actions(:action1, :action2)` - Test multiple actions

### Controller Tests

Test authorization in controller specs:

```ruby
require "rails_helper"

RSpec.describe PlayersController, type: :controller do
  describe "POST #create" do
    context "when unauthorized" do
      it "returns forbidden status" do
        post :create, params: { player: attributes }
        expect(response).to have_http_status(:forbidden)
      end
    end

    context "when authorized" do
      before { sign_in admin_user }

      it "creates the player" do
        expect {
          post :create, params: { player: attributes }
        }.to change(Player, :count).by(1)
      end
    end
  end
end
```

## Best Practices

### 1. Keep Policies Focused

Each policy should correspond to a single model or concept:

```ruby
# Good
class PlayerPolicy < ApplicationPolicy
  # Rules specific to players
end

# Bad
class PlayerPolicy < ApplicationPolicy
  # Rules for players, teams, and trainings mixed together
end
```

### 2. Use Descriptive Method Names

For custom actions, use clear, descriptive names:

```ruby
def transfer_to_team?
  # Clear what this authorization checks
end
```

### 3. DRY with Helper Methods

Extract common logic into private helper methods:

```ruby
def update?
  authorized_for_academy?
end

def destroy?
  authorized_for_academy?
end

private

def authorized_for_academy?
  system_admin? || (same_academy? && user_has_permission?(:write))
end
```

### 4. Document Complex Logic

Add comments for non-obvious authorization rules:

```ruby
def transfer?
  # Only academy admins can transfer players between teams
  # System admins can transfer across academies
  system_admin? || (same_academy? && user_is_academy_admin?)
end
```

### 5. Test Edge Cases

Ensure policies are tested with various user states:

- Nil user (not authenticated)
- Regular user
- Academy admin
- System admin
- User from different academy

## Security Considerations

### 1. Deny by Default

Always deny access by default. Explicitly permit only what's needed:

```ruby
def special_action?
  false  # Start with deny
end
```

### 2. Never Trust Client Input

Always verify authorization server-side:

```ruby
# Bad - trusting client to send correct academy_id
def create?
  user.present?
end

# Good - verifying user has access to the academy
def create?
  user.present? && user.academy_ids.include?(params[:academy_id])
end
```

### 3. Check Multi-Tenancy

Always verify resources belong to the user's academy:

```ruby
def show?
  user.present? && same_academy?
end
```

### 4. Validate Related Records

When operating on related records, check authorization for all:

```ruby
def assign_to_team?
  user.present? && 
    authorized_for?(record) && 
    authorized_for?(team)
end
```

## Troubleshooting

### NotImplementedError in Scope

If you see `NotImplementedError: Subclasses must implement #resolve`:

```ruby
class MyPolicy < ApplicationPolicy
  class Scope < ApplicationPolicy::Scope
    def resolve
      # Add your scope logic here
      scope.all
    end
  end
end
```

### Wrong Policy Class

If Pundit can't find your policy, ensure:

1. The policy class name matches the model name + "Policy"
2. The policy is in the correct namespace
3. You've required/loaded the policy file

### Authorization Always Fails

Check:

1. `pundit_user` method returns the current user
2. User object has required methods (`system_admin?`, `academy_ids`, etc.)
3. Policy methods return boolean values

## Additional Resources

- [Pundit Documentation](https://github.com/varvet/pundit)
- [RSpec Pundit Matchers](https://github.com/pundit-community/pundit-matchers)
- [Multi-Tenancy with Pundit](https://github.com/ErwinM/acts_as_tenant/wiki/Using-Pundit)
