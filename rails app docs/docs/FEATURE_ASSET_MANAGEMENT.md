# Diquis - Asset Management Feature

## Overview

The Asset Management module provides comprehensive tracking and management of physical assets owned by the academy,
including equipment, uniforms, training materials, and facility resources. This system enables academies to
maintain inventory control, track asset allocation, monitor maintenance schedules, and generate asset-related reports.

## Core Functionality

### Asset Categories

- **Equipment**: Balls, cones, goals, nets, training aids
- **Uniforms & Apparel**: Jerseys, shorts, socks, training gear, goalkeeper kits
- **Training Materials**: Bibs, markers, agility ladders, hurdles
- **Facility Assets**: Field maintenance equipment, benches, scoreboards
- **Technology**: Tablets, cameras, timing equipment, heart rate monitors

### Key Features

#### 1. Asset Registration & Cataloging

- **Unique Asset Identification**: Each asset gets a unique ID/barcode
- **Asset Details**: Name, description, category, brand, model, serial number
- **Financial Information**: Purchase price, purchase date, vendor, warranty info
- **Condition Tracking**: New, good, fair, poor, needs repair, retired
- **Photo Documentation**: Multiple images per asset
- **Location Assignment**: Current location/storage area

#### 2. Inventory Management

- **Stock Levels**: Track quantities for consumable items
- **Reorder Points**: Automatic alerts when stock runs low
- **Bulk Operations**: Add/update multiple similar items
- **Asset Valuation**: Current value calculations with depreciation
- **Audit Trail**: Complete history of asset changes

#### 3. Asset Allocation & Check-out

- **Player Assignment**: Assign uniforms and personal equipment to players
- **Team Allocation**: Assign training equipment to specific teams
- **Coach/Staff Assignment**: Assign tablets, keys, specialized equipment
- **Temporary Loans**: Short-term equipment checkout system
- **Return Tracking**: Monitor overdue returns

#### 4. Maintenance & Lifecycle Management

- **Maintenance Schedules**: Preventive maintenance calendars
- **Repair Tracking**: Log repairs, costs, and service providers
- **Replacement Planning**: Track asset lifecycle and replacement needs
- **Warranty Management**: Monitor warranty status and claims
- **Disposal Records**: Track asset retirement and disposal

#### 5. Reporting & Analytics

- **Asset Reports**: Comprehensive asset listings and valuations
- **Utilization Reports**: Track usage patterns and efficiency
- **Financial Reports**: Asset depreciation, maintenance costs, ROI
- **Compliance Reports**: Regulatory compliance and safety inspections
- **Custom Reports**: Flexible reporting with multiple filters

## Data Model

### Core Models

#### Asset

```ruby
class Asset < ApplicationRecord
  acts_as_tenant(:academy)
  
  # Attributes
  - academy_id (foreign key, required)
  - slug (UUID, indexed)
  - name (string, required)
  - description (text)
  - asset_category_id (foreign key, required)
  - brand (string)
  - model (string)
  - serial_number (string)
  - barcode (string, unique within academy)
  - purchase_price (decimal, precision: 10, scale: 2)
  - purchase_date (date)
  - vendor (string)
  - warranty_expires_at (date)
  - current_value (decimal, calculated)
  - condition (enum: new, good, fair, poor, needs_repair, retired)
  - location (string)
  - notes (text)
  - is_active (boolean, default: true)
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  belongs_to :academy
  belongs_to :asset_category
  has_many :asset_allocations, dependent: :destroy
  has_many :asset_maintenance_records, dependent: :destroy
  has_many :asset_photos, dependent: :destroy
  has_many_attached :images
  
  # Validations
  validates :name, presence: true
  validates :barcode, uniqueness: { scope: :academy_id }, allow_blank: true
  validates :condition, inclusion: { in: %w[new good fair poor needs_repair retired] }
  validates :purchase_price, numericality: { greater_than: 0 }, allow_nil: true
end
```text

#### AssetCategory

```ruby
class AssetCategory < ApplicationRecord
  acts_as_tenant(:academy)
  
  # Attributes
  - academy_id (foreign key, optional) # null = global category
  - slug (UUID, indexed)
  - name (string, required)
  - description (text)
  - parent_category_id (foreign key, optional) # for hierarchical categories
  - depreciation_rate (decimal, precision: 5, scale: 2) # annual depreciation %
  - expected_lifespan_years (integer)
  - requires_maintenance (boolean, default: false)
  - is_consumable (boolean, default: false)
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  belongs_to :academy, optional: true
  belongs_to :parent_category, class_name: 'AssetCategory', optional: true
  has_many :subcategories, class_name: 'AssetCategory', foreign_key: 'parent_category_id'
  has_many :assets, dependent: :restrict_with_error
  
  # Validations
  validates :name, presence: true, uniqueness: { scope: :academy_id }
  validates :depreciation_rate, numericality: { in: 0..100 }, allow_nil: true
end
```text

#### AssetAllocation

```ruby
class AssetAllocation < ApplicationRecord
  acts_as_tenant(:academy)
  
  # Attributes
  - academy_id (foreign key, required)
  - asset_id (foreign key, required)
  - allocatable_type (string, required) # Player, Team, User, etc.
  - allocatable_id (integer, required)
  - allocated_at (datetime, required)
  - expected_return_at (datetime)
  - returned_at (datetime)
  - condition_at_checkout (enum: new, good, fair, poor)
  - condition_at_return (enum: new, good, fair, poor)
  - checkout_notes (text)
  - return_notes (text)
  - allocated_by_user_id (foreign key, required)
  - returned_to_user_id (foreign key)
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  belongs_to :academy
  belongs_to :asset
  belongs_to :allocatable, polymorphic: true
  belongs_to :allocated_by, class_name: 'User'
  belongs_to :returned_to, class_name: 'User', optional: true
  
  # Scopes
  scope :active, -> { where(returned_at: nil) }
  scope :overdue, -> { where('expected_return_at < ?', Time.current) }
  
  # Validations
  validates :allocated_at, presence: true
  validates :condition_at_checkout, inclusion: { in: %w[new good fair poor] }
  validate :return_date_after_checkout
end
```text

#### AssetMaintenanceRecord

```ruby
class AssetMaintenanceRecord < ApplicationRecord
  acts_as_tenant(:academy)
  
  # Attributes
  - academy_id (foreign key, required)
  - asset_id (foreign key, required)
  - maintenance_type (enum: preventive, corrective, emergency, inspection)
  - description (text, required)
  - performed_at (datetime, required)
  - performed_by (string, required) # technician/service provider
  - cost (decimal, precision: 10, scale: 2)
  - parts_replaced (text)
  - next_maintenance_due (date)
  - warranty_claim (boolean, default: false)
  - service_provider (string)
  - invoice_number (string)
  - notes (text)
  - created_by_user_id (foreign key, required)
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  belongs_to :academy
  belongs_to :asset
  belongs_to :created_by, class_name: 'User'
  
  # Validations
  validates :maintenance_type, inclusion: { in: %w[preventive corrective emergency inspection] }
  validates :description, presence: true
  validates :performed_at, presence: true
  validates :cost, numericality: { greater_than_or_equal_to: 0 }, allow_nil: true
end
```text

#### AssetInventory (for consumable items)

```ruby
class AssetInventory < ApplicationRecord
  acts_as_tenant(:academy)
  
  # Attributes
  - academy_id (foreign key, required)
  - asset_category_id (foreign key, required)
  - item_name (string, required)
  - description (text)
  - current_stock (integer, default: 0)
  - minimum_stock (integer, default: 0)
  - unit_cost (decimal, precision: 10, scale: 2)
  - supplier (string)
  - last_restocked_at (datetime)
  - location (string)
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  belongs_to :academy
  belongs_to :asset_category
  has_many :inventory_transactions, dependent: :destroy
  
  # Scopes
  scope :low_stock, -> { where('current_stock <= minimum_stock') }
  
  # Validations
  validates :item_name, presence: true
  validates :current_stock, numericality: { greater_than_or_equal_to: 0 }
  validates :minimum_stock, numericality: { greater_than_or_equal_to: 0 }
end
```text

## API Endpoints

### Asset Management API

#### List Assets

```text
GET /api/v1/{academy_slug}/assets
```text

**Query Parameters:**

- `category` - Filter by asset category
- `condition` - Filter by condition
- `location` - Filter by location
- `allocated` - Filter by allocation status (true/false)
- `search` - Search by name, description, brand, model
- `include` - Include relationships (category,allocations,maintenance_records)

#### Get Asset Details

```text
GET /api/v1/{academy_slug}/assets/{slug}
```text

#### Create Asset

```text
POST /api/v1/{academy_slug}/assets
```text

#### Update Asset

```text
PATCH /api/v1/{academy_slug}/assets/{slug}
```text

#### Asset Allocation

```text
POST /api/v1/{academy_slug}/assets/{slug}/allocate
DELETE /api/v1/{academy_slug}/assets/{slug}/deallocate
```text

#### Maintenance Records

```text
GET /api/v1/{academy_slug}/assets/{slug}/maintenance
POST /api/v1/{academy_slug}/assets/{slug}/maintenance
```text

### Asset Categories API

#### List Categories

```text
GET /api/v1/{academy_slug}/asset_categories
```text

#### Inventory Management API

#### List Inventory Items

```text
GET /api/v1/{academy_slug}/inventory
```text

#### Update Stock

```text
PATCH /api/v1/{academy_slug}/inventory/{id}/stock
```text

## Service Classes

### AssetManagementService

- Asset registration and updates
- Automatic barcode generation
- Asset valuation calculations with depreciation
- Bulk asset operations

### AssetAllocationService

- Check-out/check-in workflow
- Availability validation
- Allocation conflict detection
- Overdue return notifications

### AssetMaintenanceService

- Maintenance scheduling
- Maintenance record tracking
- Cost tracking and reporting
- Warranty management

### InventoryManagementService

- Stock level monitoring
- Reorder point alerts
- Stock transaction tracking
- Supplier management

## Background Jobs

### AssetMaintenanceReminderJob

- Schedule maintenance reminders
- Send notifications for upcoming maintenance
- Generate maintenance reports

### AssetReturnReminderJob

- Send overdue return notifications
- Escalate overdue returns to managers

### AssetDepreciationJob

- Calculate monthly asset depreciation
- Update current asset values
- Generate depreciation reports

## Integration Points

### With Player Management

- Assign uniforms and personal equipment to players
- Track equipment sizes and preferences
- Link equipment condition to player feedback

### With Team Management

- Allocate training equipment to teams
- Track team-specific asset needs
- Manage shared resources across teams

### With Training Management

- Reserve equipment for specific training sessions
- Track equipment usage during trainings
- Monitor wear and tear from training activities

### With Reporting Module

- Asset valuation reports
- Equipment utilization analytics
- Maintenance cost analysis
- ROI calculations

## User Interface Considerations

### Dashboard Views

- **Asset Overview**: Total assets, current value, condition breakdown
- **Low Stock Alerts**: Items needing reorder
- **Maintenance Due**: Upcoming maintenance schedules
- **Overdue Returns**: Assets past return date

### Mobile Support

- **Barcode Scanning**: Quick asset identification
- **Photo Capture**: Document asset condition
- **Offline Capability**: Basic functions without internet
- **Location Services**: Auto-populate asset locations

## Compliance & Reporting

### Audit Requirements

- Complete asset lifecycle documentation
- Financial audit trails
- Compliance with sports organization requirements
- Insurance documentation support

### Export Capabilities

- Asset registers in multiple formats (CSV, PDF, Excel)
- Financial reports for accounting systems
- Maintenance schedules for planning
- Custom report generation

## Implementation Priority

### Phase 1: Core Asset Management

- Basic asset registration and tracking
- Simple allocation system
- Basic reporting

### Phase 2: Advanced Features

- Maintenance scheduling
- Inventory management for consumables
- Advanced reporting and analytics

### Phase 3: Integration & Automation

- Barcode/QR code scanning
- Mobile app integration
- Automated depreciation calculations
- Advanced workflow automation

This asset management system provides comprehensive control over academy resources while maintaining the multi-tenant architecture and integration with existing modules.
