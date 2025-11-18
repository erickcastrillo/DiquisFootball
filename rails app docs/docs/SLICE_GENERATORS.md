# Slice Generators - Quick Reference

## Commands

### Generate Complete Slice with Model (Preferred)

```bash
rails generate slice ModuleName::ModelName attribute:type attribute2:type
```

**This is the preferred method** - Creates complete slice structure with model, controller, service, serializer, policy, React components, tests, migration, and routes.

### Generate Basic Slice Structure Only

```bash
rails slice:generate[slice_name]
```

Creates basic slice directory structure + route namespace under `/app/slice_name` (no model/controller)

### Generate Factory

```bash
rails slice:factory[slice_name,model_name]
```

Creates FactoryBot factory in `app/slices/slice_name/spec/factories/`

### Add Resource Route

```bash
rails slice:add_resource[slice_name,resource_name]
```

Adds `resources :resource_names` to slice's route namespace

### List Slices

```bash
rails slice:list
```

Shows all available slices

## Examples

### Complete Slice Generation (Recommended)

```bash
# Generate a complete football categories slice
rails generate slice Football::Categories name:string description:text

# Generate user management with attributes  
rails generate slice UserManagement::Profile first_name:string last_name:string bio:text

# Generate without Academy reference (for creating Academy model itself)
rails generate slice Academy::Academy name:string --skip-academy
```

### Manual Slice Building

```bash
# Create a basic slice structure first
rails slice:generate[user_management]

# Add routes for profiles and settings
rails slice:add_resource[user_management,profile]
rails slice:add_resource[user_management,setting]  

# Create factories
rails slice:factory[user_management,profile]
rails slice:factory[user_management,setting]

# Verify setup
rails slice:list
rails routes | grep user_management
```

## Directory Structure Created

### Complete Slice Generation (`rails generate slice`)

```txt
app/slices/slice_name/model_name/
├── controllers/
│   └── model_name_controller.rb
├── services/
│   └── model_name_service.rb  
├── models/
│   └── model_name.rb
├── serializers/
│   └── model_name_serializer.rb
├── policies/
│   └── model_name_policy.rb
└── specs/
    ├── model_name_spec.rb
    ├── model_name_service_spec.rb
    ├── model_name_controller_spec.rb
    └── model_name_policy_spec.rb

app/controllers/module_name/
└── model_name_controller.rb  # Copy for Rails routing

app/frontend/pages/ModuleName/ModelNames/
├── Index.tsx
├── Show.tsx
├── Form.tsx
├── types.ts
└── __tests__/
    ├── Index.test.tsx
    ├── Show.test.tsx
    └── Form.test.tsx

db/migrate/
└── xxx_create_module_model_names.rb
```

### Basic Slice Structure (`rails slice:generate`)

```txt
app/slices/slice_name/
├── controllers/
├── models/
├── services/
├── views/
└── spec/
    ├── controllers/
    ├── models/
    ├── services/
    ├── factories/
    └── support/
```

## Route Structure

- All routes: `/app/module_name/resources`
- Example: `/app/football/categories`
- Generated automatically with proper namespace using `scope '/app'`

## Key Features

- ✅ **Full-stack generation** - Backend + Frontend + Tests + Migration
- ✅ **Module-based organization** - `ModuleName::ModelName` structure
- ✅ **Automatic route namespace creation** - Uses `scope '/app'` for URL structure
- ✅ **React/TypeScript components** with Inertia.js integration
- ✅ **Complete test suite** - RSpec + Vitest
- ✅ **Service layer pattern** - Business logic separation
- ✅ **Policy-based authorization** - Pundit integration
- ✅ **Serializer for API responses**
- ✅ **Academy multi-tenancy** - Automatic academy scoping
- ✅ **Lint-compliant code** - Generated code follows all project linting rules

## Code Quality Standards

All generated code follows these linting standards:

### Ruby (RuboCop)

- 2-space indentation
- Frozen string literals
- Method length limits
- Proper naming conventions
- No trailing whitespace

### ERB Templates

- Proper HTML structure
- Consistent indentation
- Valid HTML5 markup

### TypeScript/React (ESLint)

- Consistent formatting with Prettier
- TypeScript strict mode compliance
- React hooks best practices
- No unused variables or imports

### CSS/SCSS

- Consistent property ordering
- No duplicate selectors
- Proper nesting limits

### Generated Code Validation

- All templates include linting compliance
- Automatic code formatting applied
- Style guide adherence verified
