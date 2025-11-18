# Slice-Based Architecture Documentation

This document describes the slice-based architecture generators and tools available in the Diquis application.

## Overview

The slice-based architecture organizes code by business domain rather than technical layers. Each slice contains all the components needed for a specific feature or domain area, including models, controllers, services, views, tests, and factories.

## Two Approaches

### Approach 1: Complete Slice Generation (Recommended)

Use `rails generate slice ModuleName::ModelName` to create a complete, production-ready slice with model, controller, service, serializer, policy, React components, tests, migration, and routes all in one command.

### Approach 2: Manual Slice Building  

Use the individual `rails slice:*` commands to build slices piece by piece - first create the structure, then manually add models, controllers, and routes as needed.

## Available Generators

### 1. Complete Slice Generator (Recommended)

**Command:**

```bash
rails generate slice ModuleName::ModelName attribute:type attribute2:type
```

This is the **preferred method** for creating slices as it generates a complete, production-ready slice with all necessary components.

**Example:**

```bash
rails generate slice Football::Categories name:string description:text
```

**What it generates:**

1. **Database Migration** - With proper module-based table naming
2. **Model** - With Academy multi-tenancy, validations, and associations
3. **Service Layer** - Complete CRUD operations with error handling
4. **Controller** - Inertia.js-based with proper authorization
5. **Serializer** - For consistent API responses
6. **Policy** - Pundit authorization rules
7. **React Components** - Index, Show, Form with TypeScript
8. **Complete Test Suite** - RSpec + Vitest tests
9. **Routes** - Automatic namespace and resource routes

**Directory Structure Created:**

```txt
app/slices/football/categories/
├── controllers/
│   └── categories_controller.rb
├── services/
│   └── categories_service.rb  
├── models/
│   └── categories.rb
├── serializers/
│   └── categories_serializer.rb
├── policies/
│   └── categories_policy.rb
└── specs/
    ├── categories_spec.rb
    ├── categories_service_spec.rb
    ├── categories_controller_spec.rb
    └── categories_policy_spec.rb

app/controllers/football/
└── categories_controller.rb  # Copy for Rails routing

app/frontend/pages/Football/Categories/
├── Index.tsx
├── Show.tsx
├── Form.tsx
├── types.ts
└── __tests__/
    ├── Index.test.tsx
    ├── Show.test.tsx
    └── Form.test.tsx

db/migrate/
└── xxx_create_football_categories.rb
```

### 2. Basic Slice Structure Generator

**Command:**

```bash
rails slice:generate[slice_name]
```

Creates only the basic directory structure without any models or controllers.

**Example:**

```bash
rails slice:generate[user_management]
```

**What it does:**

- Creates the complete directory structure in `app/slices/user_management/`
- Adds a route namespace in `config/routes.rb` under the `app` namespace
- Creates `.keep` files to ensure empty directories are tracked by Git

**Basic Structure Created:**

```txt
Generated slice: user_management
Location: /home/project/app/slices/user_management

To generate a model in this slice:
  rails generate model ModelName --slice=user_management

To generate a factory in this slice:
  rails slice:factory[user_management,model_name]

Route namespace added: /app/user_management
```

**Routes Created:**
The command automatically adds this structure to `config/routes.rb`:

```ruby
namespace :app do
  namespace :user_management do
    # Add resources here
    # Example: resources :models
  end
end
```

### 3. Generate a Factory in a Slice

Create a FactoryBot factory within a specific slice.

```bash
rails slice:factory[slice_name,model_name]
```

**Example:**

```bash
rails slice:factory[user_management,profile]
```

**What it does:**

- Creates a factory file at `app/slices/user_management/spec/factories/profiles.rb`
- Generates a basic factory template with example attributes

**Generated Factory:**

```ruby
# frozen_string_literal: true

FactoryBot.define do
  factory :profile do
    # Add attributes here
    # Example:
    # name { Faker::Name.name }
    # email { Faker::Internet.email }
  end
end
```

### 4. Add a Resource Route to a Slice

Add a resource route to an existing slice namespace.

```bash
rails slice:add_resource[slice_name,resource_name]
```

**Example:**

```bash
rails slice:add_resource[user_management,profile]
```

**What it does:**

- Adds `resources :profiles` to the slice's route namespace
- Maintains proper indentation and formatting in `routes.rb`

**Before:**

```ruby
namespace :app do
  namespace :user_management do
    # Add resources here
    # Example: resources :models
  end
end
```

**After:**

```ruby
namespace :app do
  namespace :user_management do
    resources :profiles
  end
end
```

### 5. List All Slices

Display all available slices in the application.

```bash
rails slice:list
```

**Example Output:**

```txt
Available slices:
  - user_management
  - football
```

## Route Structure

All slice routes are organized under the `/app` namespace:

- Base URL: `/app/[slice_name]/[resource]`
- Example: `/app/user_management/profiles`

**Full Route Examples:**
For a `profiles` resource in the `user_management` slice:

```txt
GET    /app/user_management/profiles           # Index
POST   /app/user_management/profiles           # Create
GET    /app/user_management/profiles/new       # New
GET    /app/user_management/profiles/:id       # Show
PATCH  /app/user_management/profiles/:id       # Update
PUT    /app/user_management/profiles/:id       # Update
DELETE /app/user_management/profiles/:id       # Destroy
GET    /app/user_management/profiles/:id/edit  # Edit
```

## Factory Bot Configuration

The application is configured to load factories from slice directories automatically:

### Configuration Location

`spec/rails_helper.rb` contains the configuration:

```ruby
# Configure Factory Bot to load factories from slice directories
config.before(:suite) do
  # Load factories from traditional spec/factories
  FactoryBot.definition_file_paths = [Rails.root.join('spec', 'factories')]
  
  # Also load factories from slice directories
  slice_factory_paths = Dir.glob(Rails.root.join('app', 'slices', '*', 'spec', 'factories'))
  FactoryBot.definition_file_paths += slice_factory_paths
  
  # Reload factory definitions
  FactoryBot.reload
end
```

### Using Factories in Tests

Factories work exactly the same regardless of location:

```ruby
# In any spec file
describe Profile do
  let(:profile) { create(:profile) }
  
  it "has a valid factory" do
    expect(build(:profile)).to be_valid
  end
end
```

## RSpec Configuration

The application is configured to run specs from slice directories:

### Spec Pattern

```ruby
# In spec/rails_helper.rb
config.pattern = '{spec,app/slices}/**/*_spec.rb'
```

This allows RSpec to find and run tests in:

- `spec/` (traditional location)
- `app/slices/*/spec/` (slice locations)

### Running Tests

All standard RSpec commands work:

```bash
# Run all tests (including slice tests)
bundle exec rspec

# Run tests for a specific slice
bundle exec rspec app/slices/user_management/spec/

# Run a specific test file
bundle exec rspec app/slices/user_management/spec/models/profile_spec.rb
```

## Code Quality Standards

### Linting Rules Compliance

All generated code follows strict linting standards to ensure consistency and maintainability:

#### Ruby Code (RuboCop)

```ruby
# ✅ Correct: Frozen string literal at top
# frozen_string_literal: true

class ExampleController < ApplicationController
  # ✅ Correct: 2-space indentation, method length under limit
  def index
    service = ExampleService.new(academy: @academy)
    result = service.list(page: params[:page] || 1)

    if result.success?
      render_success(result.data)
    else
      render_error(result.errors)
    end
  end

  private

  # ✅ Correct: Private method separation, clear naming
  def render_success(data)
    render inertia: "Example/Index", props: {
      examples: ExampleSerializer.new(data).as_json
    }
  end
end
```

#### TypeScript/React (ESLint + Prettier)

```typescript
// ✅ Correct: Proper imports, interface definitions
import React from 'react';
import { InertiaLink } from '@inertiajs/react';

interface ExampleProps {
  examples: Example[];
  academy: Academy;
}

// ✅ Correct: Function component with proper typing
export default function Index({ examples, academy }: ExampleProps) {
  return (
    <div className="container">
      <h1>Examples for {academy.name}</h1>
      {examples.map((example) => (
        <div key={example.id} className="example-card">
          <h2>{example.name}</h2>
          <p>{example.description}</p>
        </div>
      ))}
    </div>
  );
}
```

#### ERB Templates

```erb
<%# ✅ Correct: Proper indentation and HTML structure %>
<div class="example-form">
  <%= form_with model: @example, local: false do |form| %>
    <div class="form-group">
      <%= form.label :name, class: "form-label" %>
      <%= form.text_field :name, class: "form-control" %>
    </div>
    
    <div class="form-actions">
      <%= form.submit "Save", class: "btn btn-primary" %>
    </div>
  <% end %>
</div>
```

### Generated Code Standards

#### 1. Ruby Files

- **Frozen string literals**: All Ruby files start with `# frozen_string_literal: true`
- **Indentation**: 2 spaces, no tabs
- **Method length**: Maximum 10-15 lines per method
- **Class length**: Maximum 100 lines per class
- **Naming**: snake_case for methods/variables, PascalCase for classes
- **Documentation**: Public methods have comments explaining purpose
- **Error handling**: Explicit error handling with service layer pattern

#### 2. JavaScript/TypeScript Files

- **Formatting**: Prettier-compliant with 2-space indentation
- **Types**: Full TypeScript coverage with interface definitions
- **Imports**: Sorted alphabetically, unused imports removed
- **Functions**: Arrow functions for callbacks, regular functions for components
- **Props**: Properly typed interfaces for all component props
- **Hooks**: Follow React hooks rules and best practices

#### 3. CSS/SCSS Files

- **Property ordering**: Logical grouping (positioning, box model, typography, visual)
- **Nesting**: Maximum 3 levels deep
- **Variables**: Use CSS custom properties or SCSS variables
- **Naming**: BEM methodology or consistent class naming convention

#### 4. Template Files (ERB)

- **HTML5**: Valid markup with proper semantic elements
- **Indentation**: 2 spaces consistent with Ruby code
- **Accessibility**: ARIA labels and semantic HTML
- **Forms**: Proper form structure with labels and validation

### Quality Assurance

#### Pre-commit Hooks

```bash
# Ruby linting
bundle exec rubocop --auto-correct

# JavaScript/TypeScript linting  
npm run lint:fix

# ERB linting
bundle exec erblint --lint-all --autocorrect
```

#### Continuous Integration

- All generated code passes linting in CI/CD pipeline
- Automated formatting checks prevent non-compliant code
- Style guide violations fail the build

## Best Practices

### 1. Slice Naming

- Use snake_case for slice names
- Choose descriptive, domain-focused names
- Examples: `user_management`, `inventory_control`, `financial_reporting`

### 2. Factory Organization

- Keep factories close to their models within the slice
- Use traits for different factory variations
- Include realistic test data using Faker

### 3. Route Organization

- Use RESTful routes within slice namespaces
- Keep related resources grouped in the same slice
- Use nested routes when appropriate

### 4. Testing Strategy

- Write comprehensive tests within each slice
- Use slice-specific factories
- Test slice boundaries and interactions

### 5. Code Quality

- Run linters before committing code
- Follow established style guides consistently
- Use automated formatting tools
- Review generated code for compliance

## Example: Complete Slice Setup

Here's a complete example of setting up a slice for managing sports teams:

```bash
# 1. Generate the slice
rails slice:generate[sports_management]

# 2. Add resource routes
rails slice:add_resource[sports_management,team]
rails slice:add_resource[sports_management,player]

# 3. Generate factories
rails slice:factory[sports_management,team]
rails slice:factory[sports_management,player]

# 4. Verify setup
rails slice:list
rails routes | grep app_sports_management
```

**Resulting Structure:**

```txt
app/slices/sports_management/
├── controllers/
├── models/
├── services/
├── views/
└── spec/
    ├── controllers/
    ├── models/
    ├── services/
    ├── factories/
    │   ├── teams.rb
    │   └── players.rb
    └── support/
```

**Resulting Routes:**

```txt
/app/sports_management/teams
/app/sports_management/players
```

## Troubleshooting

### Common Issues

1. **Factory not found**: Ensure the factory is in the correct slice directory and RSpec configuration is loaded
2. **Route conflicts**: Check that slice names don't conflict with existing routes
3. **Directory permissions**: Ensure the application has write permissions to create slice directories

### Debugging Commands

```bash
# Check factory load paths
rails runner "puts FactoryBot.definition_file_paths"

# View all routes
rails routes

# Check slice directory structure
tree app/slices/

# Verify factory availability
rails runner "puts FactoryBot.factories.map(&:name)"
```

## Migration Guide

### From Traditional Structure

To migrate existing code to slice-based architecture:

1. **Create the slice:** `rails slice:generate[domain_name]`
2. **Move models:** Copy models to `app/slices/domain_name/models/`
3. **Move controllers:** Copy controllers to `app/slices/domain_name/controllers/`
4. **Move tests:** Copy specs to `app/slices/domain_name/spec/`
5. **Move factories:** Copy factories to `app/slices/domain_name/spec/factories/`
6. **Update routes:** Use `rails slice:add_resource` for each resource
7. **Run tests:** Verify everything works with `bundle exec rspec`

This slice-based architecture provides better organization, clearer boundaries, and improved maintainability for large Rails applications.
