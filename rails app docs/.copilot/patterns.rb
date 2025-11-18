# frozen_string_literal: true

# This file contains coding patterns and examples for GitHub Copilot to learn from.
# It demonstrates the project's coding standards and conventions.

# ============================================================================
# RUBY CODING PATTERNS FOR COPILOT
# ============================================================================

# Pattern 1: Controller Structure (Inertia.js + Service Layer)
class ExampleController < ApplicationController
  # Standard callbacks for multi-tenant academy scoping
  before_action :set_example, only: [ :show, :edit, :update, :destroy ]
  before_action :set_academy

  # RESTful index with service layer pattern and Inertia.js
  def index
    service = ExampleService.new(academy: @academy)
    result = service.list(page: params[:page] || 1)

    if result.success?
      render inertia: "Examples/Index", props: {
        examples: ExampleSerializer.new(result.data).as_json,
        academy: { slug: @academy.slug, name: @academy.name }
      }
    else
      redirect_to root_path, alert: result.errors.join(", ")
    end
  end

  # RESTful show action
  def show
    render inertia: "Examples/Show", props: {
      example: ExampleSerializer.new(@example).as_json,
      academy: { slug: @academy.slug, name: @academy.name }
    }
  end

  # RESTful create with service layer
  def create
    service = ExampleService.new(academy: @academy, params: params)
    result = service.create

    if result.success?
      redirect_to example_path(result.data), notice: "Example was successfully created."
    else
      redirect_to new_example_path, inertia: { errors: result.errors }
    end
  end

  private

  # Standard resource loading pattern
  def set_example
    service = ExampleService.new(academy: @academy)
    result = service.find(params[:id])

    if result.success?
      @example = result.data
    else
      redirect_to examples_path, alert: result.errors.join(", ")
    end
  end

  # Standard academy resolution for multi-tenancy
  def set_academy
    @academy = current_user.academies.find_by!(slug: params[:academy_slug]) if params[:academy_slug]
    @academy = current_user.default_academy if @academy.nil?
  end
end

# Pattern 2: Service Layer Structure
class ExampleService
  include ActiveModel::Model
  include ActiveModel::Attributes

  attr_accessor :academy, :params

  def initialize(academy:, params: {})
    @academy = academy
    @params = params
  end

  # List with pagination
  def list(page: 1, per_page: 20)
    examples = academy.examples.page(page).per(per_page)
    ServiceResult.success(examples)
  rescue StandardError => e
    ServiceResult.failure([ "Failed to list examples: #{e.message}" ])
  end

  # Create with validation
  def create
    example = academy.examples.build(example_params)

    if example.save
      ServiceResult.success(example)
    else
      ServiceResult.failure(example.errors.full_messages)
    end
  rescue StandardError => e
    ServiceResult.failure([ "Failed to create example: #{e.message}" ])
  end

  # Update with validation
  def update(id)
    example = academy.examples.find(id)

    if example.update(example_params)
      ServiceResult.success(example)
    else
      ServiceResult.failure(example.errors.full_messages)
    end
  rescue StandardError => e
    ServiceResult.failure([ "Failed to update example: #{e.message}" ])
  end

  private

  def example_params
    params.require(:example).permit(:name, :description)
  end
end

# Pattern 3: Model Structure with Multi-tenancy
class Example < ApplicationRecord
  # Multi-tenancy with ActsAsTenant
  acts_as_tenant :academy

  # UUID primary key (already configured globally)
  # Associations
  belongs_to :academy

  # Validations
  validates :name, presence: true, length: { maximum: 255 }
  validates :description, length: { maximum: 1000 }
  validates :slug, presence: true, uniqueness: { scope: :academy_id }

  # Callbacks
  before_validation :generate_slug, if: :name_changed?

  # Scopes
  scope :active, -> { where(active: true) }
  scope :by_name, ->(name) { where("name ILIKE ?", "%#{name}%") }

  private

  def generate_slug
    self.slug = name.parameterize if name.present?
  end
end

# Pattern 4: Serializer Structure
class ExampleSerializer
  include ActiveModel::Serialization

  attr_reader :examples

  def initialize(examples)
    @examples = Array(examples)
  end

  def as_json(options = {})
    if examples.size == 1
      serialize_single(examples.first)
    else
      examples.map { |example| serialize_single(example) }
    end
  end

  private

  def serialize_single(example)
    {
      id: example.id,
      name: example.name,
      description: example.description,
      slug: example.slug,
      academy_id: example.academy_id,
      created_at: example.created_at,
      updated_at: example.updated_at
    }
  end
end

# Pattern 5: Policy Structure (Pundit)
class ExamplePolicy < ApplicationPolicy
  def index?
    user.present? && academy_member?
  end

  def show?
    user.present? && academy_member? && record.academy == user_academy
  end

  def create?
    user.present? && academy_admin?
  end

  def update?
    user.present? && academy_admin? && record.academy == user_academy
  end

  def destroy?
    user.present? && academy_admin? && record.academy == user_academy
  end

  private

  def academy_member?
    user.academies.include?(user_academy)
  end

  def academy_admin?
    academy_member? && user.admin_for?(user_academy)
  end

  def user_academy
    @user_academy ||= record.respond_to?(:academy) ? record.academy : record
  end
end

# Pattern 6: Migration Structure
class CreateExamples < ActiveRecord::Migration[8.0]
  def change
    create_table :examples, id: :uuid do |t|
      t.references :academy, null: false, foreign_key: true, type: :uuid
      t.string :name, null: false, limit: 255
      t.text :description
      t.string :slug, null: false, limit: 255
      t.boolean :active, default: true, null: false
      t.timestamps null: false
    end

    add_index :examples, :slug, unique: true
    add_index :examples, [ :academy_id, :name ], unique: true
    add_index :examples, :active
  end
end

# Pattern 7: RSpec Test Structure
RSpec.describe ExampleService, type: :service do
  let(:academy) { create(:academy) }
  let(:service) { described_class.new(academy: academy) }

  describe '#create' do
    let(:params) { { example: { name: 'Test Example', description: 'Test Description' } } }
    let(:service_with_params) { described_class.new(academy: academy, params: params) }

    context 'with valid parameters' do
      it 'creates a new example' do
        result = service_with_params.create

        expect(result).to be_success
        expect(result.data).to be_a(Example)
        expect(result.data.name).to eq('Test Example')
      end
    end

    context 'with invalid parameters' do
      let(:params) { { example: { name: '', description: 'Test Description' } } }

      it 'returns failure with errors' do
        result = service_with_params.create

        expect(result).to be_failure
        expect(result.errors).to include("Name can't be blank")
      end
    end
  end
end

# Pattern 8: Factory Structure
FactoryBot.define do
  factory :example do
    academy
    name { Faker::Lorem.words(number: 2).join(' ').titleize }
    description { Faker::Lorem.paragraph }
    slug { name.parameterize }
    active { true }

    trait :inactive do
      active { false }
    end

    trait :with_long_description do
      description { Faker::Lorem.paragraphs(number: 3).join("\n\n") }
    end
  end
end
