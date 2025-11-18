# Diquis - Development Guide

## Table of Contents

1. [Development Workflow](#development-workflow)
2. [CI/CD Pipeline](#cicd-pipeline)
3. [Code Organization](#code-organization)
4. [Service Layer Development](#service-layer-development)
5. [Controller Development](#controller-development)
6. [Model Development](#model-development)
7. [Testing Guidelines](#testing-guidelines)
8. [Code Style Guidelines](#code-style-guidelines)

---

## Development Workflow

### Starting Development

```bash
# 1. Pull latest changes
git pull origin main

# 2. Install dependencies
bundle install

# 3. Run migrations
rails db:migrate

# 4. Start development servers
./bin/dev

# 5. Run tests
bundle exec rspec
```text

### Daily Development Cycle

1. **Create Feature Branch**

```bash
git checkout -b feature/player-skill-assessment
```text

2. **Write Failing Test**

```ruby
# spec/slices/player_management/services/skill_assessment_service_spec.rb
RSpec.describe PlayerManagement::SkillAssessmentService do
  it 'assigns skill level to player' do
    # Test implementation
  end
end
```text

3. **Implement Feature**

- Create service class
- Add controller action
- Implement serializer
- Add authorization policy

4. **Run Tests**

```bash
bundle exec rspec spec/slices/player_management/
```text

5. **Commit Changes**

```bash
git add .
git commit -m "Add player skill assessment feature"
```text

6. **Push and Create PR**

```bash
git push origin feature/player-skill-assessment
```text

---

## CI/CD Pipeline

### Overview

The project uses GitHub Actions for continuous integration and continuous deployment. The CI pipeline automatically runs on every pull request and push to the main branch.

### CI Workflow Jobs

The CI workflow (`.github/workflows/ci.yml`) consists of three parallel jobs:

#### 1. Security Scanning (`scan_ruby`)

Runs Brakeman to scan for common Rails security vulnerabilities:

```bash
# Run locally
bundle exec brakeman --no-pager
```

**What it checks:**

- SQL injection vulnerabilities
- Cross-site scripting (XSS) risks
- Mass assignment issues
- Insecure dependencies
- Authentication and authorization flaws

#### 2. Code Linting (`lint`)

Runs RuboCop to enforce code style and best practices:

```bash
# Run locally
bundle exec rubocop

# Auto-fix issues
bundle exec rubocop -A
```

**Configuration:**

- Extends Rails Omakase styling
- Includes RSpec and FactoryBot cops
- Custom rules in `.rubocop.yml`
- Excludes generated files and vendor code

#### 3. Test Suite (`test`)

Runs the full RSpec test suite with PostgreSQL:

```bash
# Run locally
RAILS_ENV=test bundle exec rspec

# With coverage report
COVERAGE=true bundle exec rspec
```

**Test environment:**

- PostgreSQL 15 database
- Isolated test data with DatabaseCleaner
- Randomized test order for independence
- FactoryBot for test data generation

### Required Checks for PR Merge

All three jobs must pass before a PR can be merged:

✅ Security scan passes (no high/critical vulnerabilities)
✅ Code linting passes (no RuboCop offenses)
✅ All tests pass

### Running CI Checks Locally

Before pushing your changes, run all CI checks locally:

```bash
# 1. Security scan
bundle exec brakeman --no-pager

# 2. Code linting
bundle exec rubocop

# 3. Run tests
RAILS_ENV=test bundle exec rspec

# Or use the bin scripts
./bin/brakeman
./bin/rubocop
```

### Troubleshooting CI Failures

**Security scan failures:**

- Review Brakeman report for specific vulnerabilities
- Add false positives to `config/brakeman.ignore` if needed
- Fix legitimate security issues before merging

**Linting failures:**

- Run `bundle exec rubocop -A` to auto-fix most issues
- Review remaining offenses manually
- Update `.rubocop.yml` if project-specific rules are needed

**Test failures:**

- Check test output for specific failures
- Ensure database migrations are up to date locally
- Run `RAILS_ENV=test bundle exec rails db:reset` if needed
- Check for flaky tests (tests that pass/fail intermittently)

### CI Environment Details

**Ruby version:** Set in `.ruby-version` (see file for current version)
**PostgreSQL version:** 15
**Test database:** Automatically created and migrated in CI
**Cache:** Bundler gems are cached between runs for speed

---

## Code Organization

### Vertical Slice Structure

Each slice is self-contained:

```text
app/slices/player_management/
├── controllers/
│   └── players_controller.rb          # HTTP interface
├── services/
│   ├── player_registration_service.rb # Business logic
│   ├── player_finder_service.rb
│   └── player_search_service.rb
├── models/
│   └── player.rb                       # Domain model
├── serializers/
│   └── player_serializer.rb            # JSON representation
├── policies/
│   └── player_policy.rb                # Authorization
└── validators/
    └── player_validator.rb             # Custom validations
```text

### Naming Conventions

**Services:**

- Action-based: `PlayerRegistrationService`, `TeamCreationService`
- Finder: `PlayerFinderService`, `AcademyFinderService`
- Operation: `AttendanceTrackingService`, `SkillAssessmentService`

**Controllers:**

- Plural resource name: `PlayersController`, `TeamsController`
- Namespaced: `PlayerManagement::PlayersController`

**Models:**

- Singular: `Player`, `Team`, `Training`
- Clear domain names: `TrainingAttendance`, `PlayerSkill`

**Serializers:**

- Model name + Serializer: `PlayerSerializer`, `TeamSerializer`

---

## Service Layer Development

### Creating a New Service

### Step **Step 1: Create Service Class**

```ruby
# app/slices/player_management/services/player_registration_service.rb
module PlayerManagement
  class PlayerRegistrationService < BaseService
    attr_accessor :first_name, :last_name, :age, :gender, :foot,
                  :parent_name, :parent_email, :phone_number,
                  :position_slug, :category_slug, :academy, :picture
    
    # Validations
    validates :first_name, :last_name, presence: true, length: { maximum: 100 }
    validates :age, presence: true, numericality: { greater_than: 4, less_than: 100 }
    validates :gender, inclusion: { in: %w[M F NB PNTS] }
    validates :parent_email, presence: true, format: { with: URI::MailTo::EMAIL_REGEXP }
    
    private
    
    def validate_params
      super
      validate_academy_context
      validate_position_exists
      validate_unique_name
      validate_age_category_match
    end
    
    def execute
      position = Position.find_by!(slug: position_slug)
      category = Category.find_by!(slug: category_slug)
      
      player = Player.new(
        first_name: first_name,
        last_name: last_name,
        age: age,
        gender: gender,
        foot: foot,
        parent_name: parent_name,
        parent_email: parent_email,
        phone_number: phone_number,
        position: position,
        category: category
      )
      
      player.picture.attach(picture) if picture.present?
      player.save!
      
      # Trigger background job
      PlayerManagement::PlayerRegistrationJob.perform_later(player.id, academy.id)
      
      player
    end
    
    def validate_academy_context
      errors.add(:base, "Academy context required") unless academy.present?
    end
    
    def validate_position_exists
      ActsAsTenant.with_tenant(academy) do
        unless Position.exists?(slug: position_slug)
          errors.add(:position_slug, "Position not found")
        end
      end
    end
    
    def validate_unique_name
      ActsAsTenant.with_tenant(academy) do
        if Player.exists?(first_name: first_name, last_name: last_name)
          errors.add(:base, "Player already exists")
        end
      end
    end
    
    def validate_age_category_match
      category = Category.find_by(slug: category_slug)
      return unless category&.name&.match?(/u-?\d+/i)
      
      category_age = category.name.match(/u-?(\d+)/i)[1].to_i
      if age > category_age
        errors.add(:age, "exceeds category maximum (#{category_age})")
      end
    end
  end
end
```text

### Step **Step 2: Write Service Tests**

```ruby
# spec/slices/player_management/services/player_registration_service_spec.rb
require 'rails_helper'

RSpec.describe PlayerManagement::PlayerRegistrationService, type: :service do
  let(:academy) { create(:academy) }
  let(:position) { create(:position, academy: academy) }
  let(:category) { create(:category, name: 'U-16') }
  
  let(:valid_params) do
    {
      first_name: 'John',
      last_name: 'Doe',
      age: 15,
      gender: 'M',
      parent_name: 'Jane Doe',
      parent_email: 'jane@example.com',
      phone_number: '+1234567890',
      position_slug: position.slug,
      category_slug: category.slug,
      academy: academy
    }
  end
  
  describe '#call' do
    context 'with valid parameters' do
      it 'creates a player successfully' do
        ActsAsTenant.with_tenant(academy) do
          service = described_class.new(valid_params)
          result = service.call
          
          expect(result.success?).to be true
          expect(result.data).to be_a(Player)
          expect(result.data.full_name).to eq('John Doe')
        end
      end
      
      it 'schedules player registration job' do
        ActsAsTenant.with_tenant(academy) do
          expect {
            described_class.new(valid_params).call
          }.to have_enqueued_job(PlayerManagement::PlayerRegistrationJob)
        end
      end
    end
    
    context 'with invalid age for category' do
      let(:invalid_params) { valid_params.merge(age: 18) }
      
      it 'fails with validation error' do
        ActsAsTenant.with_tenant(academy) do
          service = described_class.new(invalid_params)
          result = service.call
          
          expect(result.failure?).to be true
          expect(result.errors).to include(match(/age.*exceeds category maximum/i))
        end
      end
    end
    
    context 'with duplicate player name' do
      before do
        ActsAsTenant.with_tenant(academy) do
          create(:player, first_name: 'John', last_name: 'Doe')
        end
      end
      
      it 'fails with validation error' do
        ActsAsTenant.with_tenant(academy) do
          service = described_class.new(valid_params)
          result = service.call
          
          expect(result.failure?).to be true
          expect(result.errors).to include(match(/already exists/i))
        end
      end
    end
  end
end
```text

---

## Controller Development

### Creating a New Controller

```ruby
# app/slices/player_management/controllers/players_controller.rb
module PlayerManagement
  class PlayersController < Api::V1::BaseController
    before_action :set_academy_context
    
    # GET /api/v1/{academy_slug}/players
    def index
      service = PlayerSearchService.new(
        academy: @current_academy,
        search_term: params[:search],
        filters: filter_params,
        user: current_user
      )
      
      result = service.call
      
      if result.success?
        players = result.data
        pagy, records = pagy(players, items: params[:per_page] || 25)
        
        render json: {
          data: PlayerManagement::PlayerSerializer.new(
            records,
            serialization_options.merge(include: [:position, :category])
          ).serializable_hash,
          meta: pagination_meta(pagy)
        }
      else
        render_service_error(result)
      end
    end
    
    # GET /api/v1/{academy_slug}/players/:slug
    def show
      service = PlayerFinderService.new(
        academy: @current_academy,
        player_slug: params[:id],
        user: current_user
      )
      
      result = service.call
      
      if result.success?
        render json: {
          data: PlayerSerializer.new(
            result.data,
            serialization_options.merge(include: [:position, :category, :teams])
          ).serializable_hash
        }
      else
        render_service_error(result)
      end
    end
    
    # POST /api/v1/{academy_slug}/players
    def create
      authorize Player, :create?
      
      service = PlayerRegistrationService.new(
        player_params.merge(academy: @current_academy)
      )
      
      result = service.call
      
      if result.success?
        render json: {
          data: PlayerSerializer.new(result.data, serialization_options).serializable_hash
        }, status: :created
      else
        render_service_error(result)
      end
    end
    
    # PATCH /api/v1/{academy_slug}/players/:slug
    def update
      player = Player.find_by!(slug: params[:id])
      authorize player
      
      if player.update(player_params)
        render json: {
          data: PlayerSerializer.new(player, serialization_options).serializable_hash,
          message: "Player updated successfully"
        }
      else
        render_validation_errors(player)
      end
    end
    
    # DELETE /api/v1/{academy_slug}/players/:slug
    def destroy
      player = Player.find_by!(slug: params[:id])
      authorize player
      
      if player.destroy
        render json: { message: "Player deleted successfully" }
      else
        render_validation_errors(player)
      end
    end
    
    private
    
    def set_academy_context
      @current_academy = ActsAsTenant.current_tenant
      
      unless @current_academy
        render json: {
          error: 'ACADEMY_CONTEXT_REQUIRED',
          message: 'Academy context is required'
        }, status: :bad_request
      end
    end
    
    def player_params
      params.require(:player).permit(
        :first_name, :last_name, :age, :gender, :foot,
        :parent_name, :parent_email, :phone_number,
        :position_slug, :category_slug, :picture
      )
    end
    
    def filter_params
      params.permit(:age_min, :age_max, :gender, :position, :category, :is_active)
    end
  end
end
```text

### Controller Testing

```ruby
# spec/slices/player_management/controllers/players_controller_spec.rb
require 'rails_helper'

RSpec.describe PlayerManagement::PlayersController, type: :controller do
  let(:academy) { create(:academy) }
  let(:user) { create(:user) }
  let(:position) { create(:position, academy: academy) }
  let(:category) { create(:category) }
  
  before do
    sign_in user
    ActsAsTenant.current_tenant = academy
  end
  
  describe 'GET #index' do
    let!(:players) do
      ActsAsTenant.with_tenant(academy) do
        create_list(:player, 3, position: position, category: category)
      end
    end
    
    it 'returns list of players' do
      get :index, params: { academy_slug: academy.slug }
      
      expect(response).to have_http_status(:ok)
      json = JSON.parse(response.body)
      expect(json['data']).to be_an(Array)
      expect(json['data'].length).to eq(3)
    end
    
    it 'includes pagination metadata' do
      get :index, params: { academy_slug: academy.slug, per_page: 2 }
      
      json = JSON.parse(response.body)
      expect(json['meta']['pagination']).to include(
        'current_page' => 1,
        'per_page' => 2,
        'total_count' => 3
      )
    end
  end
  
  describe 'POST #create' do
    let(:valid_params) do
      {
        academy_slug: academy.slug,
        player: {
          first_name: 'John',
          last_name: 'Doe',
          age: 15,
          gender: 'M',
          parent_name: 'Jane Doe',
          parent_email: 'jane@example.com',
          phone_number: '+1234567890',
          position_slug: position.slug,
          category_slug: category.slug
        }
      }
    end
    
    it 'creates a new player' do
      expect {
        post :create, params: valid_params
      }.to change(Player, :count).by(1)
      
      expect(response).to have_http_status(:created)
    end
    
    it 'returns validation errors for invalid data' do
      post :create, params: { 
        academy_slug: academy.slug,
        player: { first_name: '' }
      }
      
      expect(response).to have_http_status(:unprocessable_entity)
      json = JSON.parse(response.body)
      expect(json['error']).to eq('VALIDATION_ERROR')
    end
  end
end
```text

---

## Model Development

### Creating Models with Concerns

```ruby
# app/slices/player_management/models/player.rb
class Player < ApplicationRecord
  acts_as_tenant(:academy)
  include Sluggable
  include Auditable
  
  # Constants
  GENDERS = %w[M F NB PNTS].freeze
  PREFERRED_FEET = %w[L R B].freeze
  
  # Validations
  validates :first_name, :last_name, presence: true, length: { maximum: 100 }
  validates :age, presence: true, numericality: {
    greater_than: 4,
    less_than: 100,
    only_integer: true
  }
  validates :gender, inclusion: { in: GENDERS }
  validates :foot, inclusion: { in: PREFERRED_FEET }, allow_blank: true
  validates :parent_name, presence: true
  validates :parent_email, presence: true, format: { with: URI::MailTo::EMAIL_REGEXP }
  validates :phone_number, presence: true
  validates :first_name, uniqueness: { scope: [:academy_id, :last_name] }
  
  # Associations
  belongs_to :academy
  belongs_to :position
  belongs_to :category
  has_many :player_skills, dependent: :destroy
  has_many :skills, through: :player_skills
  has_many :team_memberships, dependent: :destroy
  has_many :teams, through: :team_memberships
  has_many :training_attendances, dependent: :destroy
  
  # File attachments
  has_one_attached :picture
  
  # Scopes
  scope :active, -> { where(is_active: true) }
  scope :by_age_range, ->(min, max) { where(age: min..max) if min && max }
  scope :by_gender, ->(gender) { where(gender: gender) if gender.present? }
  scope :by_position, ->(slug) { joins(:position).where(positions: { slug: slug }) }
  scope :by_category, ->(slug) { joins(:category).where(categories: { slug: slug }) }
  
  # Instance methods
  def full_name
    "#{first_name} #{last_name}".strip
  end
  
  def age_category_match?
    return true unless category.name.match?(/u-?\d+/i)
    
    category_age = category.name.match(/u-?(\d+)/i)[1].to_i
    age <= category_age
  end
  
  def active_teams
    teams.where(team_memberships: { is_active: true })
  end
end
```text

### Model Testing

```ruby
# spec/slices/player_management/models/player_spec.rb
require 'rails_helper'

RSpec.describe Player, type: :model do
  let(:academy) { create(:academy) }
  let(:position) { create(:position, academy: academy) }
  let(:category) { create(:category, name: 'U-16') }
  
  describe 'validations' do
    subject { build(:player, academy: academy, position: position, category: category) }
    
    it { should validate_presence_of(:first_name) }
    it { should validate_presence_of(:last_name) }
    it { should validate_presence_of(:age) }
    it { should validate_numericality_of(:age).is_greater_than(4).is_less_than(100) }
    it { should validate_inclusion_of(:gender).in_array(%w[M F NB PNTS]) }
  end
  
  describe 'associations' do
    it { should belong_to(:academy) }
    it { should belong_to(:position) }
    it { should belong_to(:category) }
    it { should have_many(:teams).through(:team_memberships) }
  end
  
  describe 'scopes' do
    let!(:young_player) { create(:player, age: 10, academy: academy) }
    let!(:older_player) { create(:player, age: 15, academy: academy) }
    
    it 'filters by age range' do
      result = Player.by_age_range(12, 16)
      expect(result).to include(older_player)
      expect(result).not_to include(young_player)
    end
  end
  
  describe '#full_name' do
    it 'returns concatenated name' do
      player = build(:player, first_name: 'John', last_name: 'Doe')
      expect(player.full_name).to eq('John Doe')
    end
  end
  
  describe '#age_category_match?' do
    it 'returns true when age matches category' do
      player = build(:player, age: 15, category: category)
      expect(player.age_category_match?).to be true
    end
    
    it 'returns false when age exceeds category' do
      player = build(:player, age: 18, category: category)
      expect(player.age_category_match?).to be false
    end
  end
end
```text

---

## Testing Guidelines

### Test Structure

```ruby
# Use descriptive context blocks
RSpec.describe PlayerRegistrationService do
  describe '#call' do
    context 'with valid parameters' do
      it 'creates a player successfully' do
        # Test implementation
      end
    end
    
    context 'with invalid age' do
      it 'returns validation error' do
        # Test implementation
      end
    end
    
    context 'when player already exists' do
      it 'returns duplicate error' do
        # Test implementation
      end
    end
  end
end
```text

### Factory Usage

```ruby
# spec/factories/players.rb
FactoryBot.define do
  factory :player do
    first_name { Faker::Name.first_name }
    last_name { Faker::Name.last_name }
    age { rand(6..17) }
    gender { 'M' }
    foot { 'R' }
    parent_name { Faker::Name.name }
    parent_email { Faker::Internet.email }
    phone_number { Faker::PhoneNumber.phone_number }
    is_active { true }
    
    association :position
    association :category
    
    # Tenant context
    before(:create) do |player|
      player.academy = ActsAsTenant.current_tenant if ActsAsTenant.current_tenant
    end
    
    trait :with_picture do
      after(:build) do |player|
        player.picture.attach(
          io: StringIO.new('fake image'),
          filename: 'player.jpg',
          content_type: 'image/jpeg'
        )
      end
    end
    
    trait :inactive do
      is_active { false }
    end
  end
end
```text

### Running Tests

```bash
# Run all tests
bundle exec rspec

# Run specific file
bundle exec rspec spec/slices/player_management/services/player_registration_service_spec.rb

# Run specific test
bundle exec rspec spec/slices/player_management/services/player_registration_service_spec.rb:25

# Run tests with coverage
COVERAGE=true bundle exec rspec

# Run tests in parallel
bundle exec parallel_rspec spec/
```text

---

## Code Style Guidelines

### Ruby Style Guide

Follow standard Ruby conventions:

```ruby
# Good
def full_name
  "#{first_name} #{last_name}"
end

# Bad
def fullName
  first_name + " " + last_name
end
```text

### Rails Conventions

```ruby
# Use before_action for filters
before_action :authenticate_user!
before_action :set_academy_context

# Use strong parameters
def player_params
  params.require(:player).permit(:first_name, :last_name, :age)
end

# Use scopes for queries
scope :active, -> { where(is_active: true) }
```text

### Running Rubocop

```bash
# Check all files
bundle exec rubocop

# Auto-correct issues
bundle exec rubocop -a

# Check specific file
bundle exec rubocop app/slices/player_management/
```text

---

## Debugging Tips

### Using Rails Console

```ruby
# Start console
rails console

# Set tenant context
ActsAsTenant.current_tenant = Academy.first

# Test service
service = PlayerManagement::PlayerRegistrationService.new(
  first_name: 'Test',
  last_name: 'Player',
  # ... other params
)
result = service.call
puts result.errors if result.failure?
```text

### Debugging with Byebug

```ruby
def execute
  byebug  # Execution will stop here
  player = Player.new(attributes)
  player.save!
end
```text

### Logging

```ruby
# Add logging to services
Rails.logger.info "Creating player: #{first_name} #{last_name}"
Rails.logger.error "Failed to create player: #{errors.full_messages}"
```text

---

For more information, see:

- [PROJECT_OVERVIEW.md](./PROJECT_OVERVIEW.md)
- [ARCHITECTURE.md](./ARCHITECTURE.md)
- [API_DOCUMENTATION.md](./API_DOCUMENTATION.md)
