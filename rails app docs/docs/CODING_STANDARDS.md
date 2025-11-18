# Coding Standards and Linting Guidelines

This document defines the coding standards and linting rules for the Diquis project to ensure consistent, high-quality code across all generated files and manual contributions.

## Overview

All code in this project must pass linting and style checks. The slice generators are configured to produce lint-compliant code automatically, and all manual contributions must follow these same standards.

## Ruby Standards (RuboCop)

### Basic Rules

```ruby
# ✅ Always start with frozen string literal
# frozen_string_literal: true

# ✅ Use 2-space indentation
class ExampleService
  def initialize(academy:)
    @academy = academy
  end

  def process_data
    # ✅ Keep methods under 15 lines
    validate_inputs
    perform_processing
    format_results
  end

  private

  # ✅ Use descriptive method names
  def validate_inputs
    return if @academy.present?

    raise ArgumentError, "Academy is required"
  end
end
```

### Key Rules

- **Indentation**: 2 spaces, no tabs
- **String literals**: Always frozen (`# frozen_string_literal: true`)
- **Method length**: Maximum 15 lines
- **Class length**: Maximum 100 lines  
- **Line length**: Maximum 120 characters
- **Naming**: snake_case for methods/variables, PascalCase for classes
- **Comments**: Document public methods and complex logic
- **Trailing whitespace**: None allowed

### RuboCop Configuration

The project uses these key RuboCop rules:

```yaml
# .rubocop.yml
AllCops:
  TargetRubyVersion: 3.3
  
Layout/IndentationWidth:
  Width: 2

Metrics/MethodLength:
  Max: 15

Metrics/ClassLength:
  Max: 100

Layout/LineLength:
  Max: 120
```

## TypeScript/React Standards (ESLint + Prettier)

### Component Structure

```typescript
// ✅ Proper imports and interface definitions
import React from 'react';
import { InertiaLink } from '@inertiajs/react';

interface ExampleProps {
  examples: Example[];
  academy: Academy;
  onUpdate?: (id: string) => void;
}

// ✅ Function component with proper typing
export default function ExampleIndex({ examples, academy, onUpdate }: ExampleProps) {
  const handleClick = (id: string) => {
    onUpdate?.(id);
  };

  return (
    <div className="example-container">
      <h1>Examples for {academy.name}</h1>
      {examples.map((example) => (
        <div key={example.id} className="example-card">
          <h2>{example.name}</h2>
          <p>{example.description}</p>
          <button type="button" onClick={() => handleClick(example.id)}>
            Update
          </button>
        </div>
      ))}
    </div>
  );
}
```

### Key Rules

- **Indentation**: 2 spaces consistently
- **Quotes**: Single quotes for strings, double quotes for JSX attributes
- **Semicolons**: Required at end of statements
- **Types**: Full TypeScript coverage with interfaces
- **Props**: All component props must be typed
- **Hooks**: Follow React hooks rules
- **Imports**: Sorted alphabetically, no unused imports
- **Functions**: Arrow functions for callbacks, regular functions for components

## ERB Template Standards

### HTML Structure

```erb
<%# ✅ Proper indentation and semantic HTML %>
<div class="form-container">
  <%= form_with model: @example, local: false, class: "example-form" do |form| %>
    <div class="form-group">
      <%= form.label :name, "Name", class: "form-label" %>
      <%= form.text_field :name, 
                          class: "form-control", 
                          placeholder: "Enter name",
                          required: true %>
    </div>
    
    <div class="form-group">
      <%= form.label :description, "Description", class: "form-label" %>
      <%= form.text_area :description, 
                         class: "form-control", 
                         rows: 4,
                         placeholder: "Enter description" %>
    </div>
    
    <div class="form-actions">
      <%= form.submit "Save Example", class: "btn btn-primary" %>
      <%= link_to "Cancel", examples_path, class: "btn btn-secondary" %>
    </div>
  <% end %>
</div>
```

### Key Rules

- **HTML5**: Valid markup with semantic elements
- **Indentation**: 2 spaces, consistent with Ruby
- **Accessibility**: Proper labels, ARIA attributes when needed
- **Forms**: Structured with labels and validation
- **Classes**: Consistent naming (BEM or Bootstrap classes)
- **Attributes**: Multi-line for readability when needed

## CSS/SCSS Standards

### Property Organization

```scss
// ✅ Logical property grouping
.example-card {
  // Positioning
  position: relative;
  top: 0;
  left: 0;

  // Box model
  display: flex;
  width: 100%;
  padding: 1rem;
  margin: 0.5rem 0;
  border: 1px solid #ddd;
  border-radius: 0.25rem;

  // Typography
  font-size: 1rem;
  line-height: 1.5;
  color: #333;

  // Visual
  background-color: #fff;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);

  // Interactions
  &:hover {
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
  }

  // Nested elements (max 3 levels)
  .example-title {
    font-weight: bold;
    margin-bottom: 0.5rem;
  }

  .example-content {
    flex: 1;

    p {
      margin: 0;
    }
  }
}
```

### Key Rules

- **Property order**: Positioning → Box model → Typography → Visual → Interactions
- **Nesting**: Maximum 3 levels deep
- **Naming**: BEM methodology or consistent class names
- **Variables**: Use CSS custom properties or SCSS variables
- **Units**: Prefer rem/em for scalability
- **Colors**: Use consistent color palette

## Migration Standards

### Database Migrations

```ruby
# frozen_string_literal: true

class CreateExampleTable < ActiveRecord::Migration[8.0]
  def change
    create_table :examples, id: :uuid do |t|
      t.references :academy, null: false, foreign_key: true, type: :uuid
      t.string :name, null: false, limit: 255
      t.text :description
      t.string :slug, null: false, limit: 255
      t.timestamps null: false
    end

    add_index :examples, :slug, unique: true
    add_index :examples, [:academy_id, :name], unique: true
  end
end
```

### Key Rules

- **UUID primary keys**: Always use `id: :uuid`
- **Null constraints**: Explicit `null: false` where appropriate
- **String limits**: Always specify `limit:` for string columns
- **Indexes**: Add performance indexes, especially for foreign keys
- **Unique constraints**: Use database-level uniqueness where needed

## Test Standards (RSpec)

### Test Structure

```ruby
# frozen_string_literal: true

require 'rails_helper'

RSpec.describe ExampleService, type: :service do
  let(:academy) { create(:academy) }
  let(:service) { described_class.new(academy: academy) }

  describe '#process_data' do
    context 'with valid inputs' do
      it 'processes data successfully' do
        result = service.process_data

        expect(result).to be_success
        expect(result.data).to be_present
      end
    end

    context 'with invalid inputs' do
      let(:academy) { nil }

      it 'returns error result' do
        result = service.process_data

        expect(result).to be_failure
        expect(result.errors).to include('Academy is required')
      end
    end
  end
end
```

### Key Rules

- **Describe blocks**: Use class/method structure
- **Context blocks**: Describe different scenarios
- **Let statements**: Use for test data setup
- **Expectations**: Clear and specific assertions
- **Factory usage**: Prefer factories over manual object creation

## Linting Commands

### Ruby

```bash
# Check Ruby style
bundle exec rubocop

# Auto-fix Ruby issues
bundle exec rubocop --auto-correct

# Check specific files
bundle exec rubocop app/slices/example/
```

### JavaScript/TypeScript

```bash
# Check JS/TS style
npm run lint

# Auto-fix JS/TS issues
npm run lint:fix

# Check specific files
npx eslint app/frontend/pages/Example/
```

### ERB Templates

```bash
# Check ERB templates
bundle exec erblint --lint-all

# Auto-fix ERB issues
bundle exec erblint --lint-all --autocorrect
```

## Pre-commit Hooks

The project uses pre-commit hooks to ensure code quality:

```bash
# Install pre-commit hooks
pre-commit install

# Run hooks manually
pre-commit run --all-files
```

## Continuous Integration

All code must pass these checks in CI:

- RuboCop for Ruby code style
- ESLint + Prettier for JavaScript/TypeScript
- ERB Lint for template validation
- RSpec test suite
- Security scanning with Brakeman

## Generator Compliance

The slice generators automatically produce code that follows all these standards:

- **Ruby files**: RuboCop compliant
- **TypeScript files**: ESLint + Prettier compliant
- **ERB templates**: Valid HTML5 with proper structure
- **Migrations**: Follow database best practices
- **Tests**: Structured RSpec with proper factories

## Manual Code Requirements

When writing code manually (not generated), ensure:

1. **Run linters** before committing
2. **Follow style guides** consistently
3. **Write tests** for all new functionality
4. **Document complex logic** with comments
5. **Use consistent naming** across the codebase

## Troubleshooting

### Common Linting Issues

**RuboCop:**

- Line too long → Break into multiple lines
- Missing frozen string literal → Add to top of file
- Trailing whitespace → Remove extra spaces

**ESLint:**

- Unused variables → Remove or prefix with underscore
- Missing types → Add TypeScript interfaces
- Inconsistent quotes → Use single quotes consistently

**ERB Lint:**

- Invalid HTML → Fix tag structure
- Missing labels → Add proper form labels
- Accessibility issues → Add ARIA attributes

For specific linting errors, consult the project's `.rubocop.yml`, `.eslintrc.js`, and `.erb-lint.yml` configuration files.
