# Functional Requirement Specification: Academy Operations

### 1. Executive Summary
This document outlines the functional requirements for managing the core operational aspects of a football academy. It covers the administration of the academy tenant itself, its inventory, financial records, and core configurations.

### 2. Key Actors
- **`super_user`**: Platform-wide administrator with access to all tenants.
- **`academy_owner`**: The primary stakeholder for a specific academy tenant.
- **`academy_admin`**: Administrative staff responsible for daily operational tasks within an academy.
- **`director_of_football`**: Head of football operations, has read-only access to some operational data.

---
### 3. Functional Capabilities

#### 3.1. Tenant Management
- **User Story:** "As a `super_user`, I need to create, read, update, and deactivate academy accounts (tenants) to manage the platform's clientele."
- **Detailed Workflow:**
  - **Trigger:** The `super_user` initiates a tenant creation process from their administrative dashboard.
  - **Process:**
    1. The `super_user` provides the necessary details for the new academy (e.g., Academy Name, primary contact/owner details).
    2. The system validates that the proposed academy name or identifier is unique across the platform.
    3. A new, isolated data schema/partition for the tenant is provisioned.
    4. An initial `academy_owner` account is created and associated with the new tenant.
    5. A confirmation is sent to the `super_user` and the new `academy_owner`.
  - **Data Rules:**
    - Each tenant must have a globally unique identifier.
    - Every tenant must be associated with at least one `academy_owner`.
  - **Edge Cases (Failure states):**
    - If the tenant identifier is not unique, the system rejects the creation and prompts for a different identifier.
    - If the `academy_owner` creation fails, the entire tenant creation process is rolled back.
- **Permission Matrix:**
  | Role           | Create | Read   | Update | Delete (Deactivate) |
  |----------------|--------|--------|--------|---------------------|
  | `super_user`   | Yes    | Yes    | Yes    | Yes                 |
  | `academy_owner`| No     | No     | No     | No                  |
  | `academy_admin`| No     | No     | No     | No                  |
- **Acceptance Criteria:**
  - [ ] A `super_user` can successfully create a new academy tenant.
  - [ ] A `super_user` can view a list of all active and inactive academy tenants.
  - [ ] A `super_user` can update the core details of an existing academy tenant.
  - [ ] A `super_user` can deactivate (soft delete) an academy tenant, preventing access for its users.
  - [ ] An `academy_owner` from one tenant cannot view or access information about another tenant.

#### 3.2. Academy Configuration
- **User Story:** "As an `academy_owner`, I want to configure my academy's profile, including its name and logo, to personalize our identity on the platform."
- **Detailed Workflow:**
  - **Trigger:** An authenticated `academy_owner` navigates to the "Academy Settings" section of the application.
  - **Process:**
    1. The system displays the current academy settings in an editable form.
    2. The `academy_owner` modifies the desired information (e.g., academy name, contact info).
    3. To change the logo, the owner uploads a new image file.
    4. The system validates the uploaded file (e.g., format, size).
    5. The changes are saved and applied only to the owner's tenant.
  - **Data Rules:**
    - Logo files must be in a supported format (e.g., PNG, JPG) and not exceed a specified size limit.
    - The academy name must not be empty.
  - **Edge Cases (Failure states):**
    - If an invalid file type or size is uploaded for the logo, the system rejects it with an error message.
    - If the academy name is cleared, the system prevents saving and shows a validation error.
- **Permission Matrix:**
  | Role           | Create | Read   | Update | Delete |
  |----------------|--------|--------|--------|--------|
  | `super_user`   | Yes    | Yes    | Yes    | Yes    |
  | `academy_owner`| No     | Yes    | Yes    | No     |
  | `academy_admin`| No     | Yes    | No     | No     |
- **Acceptance Criteria:**
  - [ ] An `academy_owner` can view their academy's current configuration.
  - [ ] An `academy_owner` can successfully update their academy's name and contact information.
  - [ ] An `academy_owner` can upload and replace the academy logo.
  - [ ] An `academy_admin` can view the academy's configuration but cannot make changes.

#### 3.3. Inventory Management
- **User Story:** "As an `academy_admin`, I need to add, track, and update the status of academy assets like equipment and uniforms, so we can manage our inventory effectively."
- **Detailed Workflow:**
  - **Trigger:** The `academy_admin` accesses the "Inventory" module.
  - **Process:**
    1. The system displays a list of all inventory items for the academy.
    2. The admin can add a new item, providing a name, description, quantity, and category (e.g., "Balls," "Uniforms").
    3. The admin can update the quantity of an existing item (e.g., decrease count when an item is distributed).
    4. The admin can archive or remove items that are no longer in use.
  - **Data Rules:**
    - Item quantity cannot be negative.
    - Every inventory item must have a name.
  - **Edge Cases (Failure states):**
    - If an admin attempts to set an item's quantity to a negative number, the action is blocked with a warning.
    - Attempting to add an item with no name results in a validation error.
- **Permission Matrix:**
  | Role                   | Create | Read   | Update | Delete |
  |------------------------|--------|--------|--------|--------|
  | `academy_owner`        | Yes    | Yes    | Yes    | Yes    |
  | `academy_admin`        | Yes    | Yes    | Yes    | Yes    |
  | `director_of_football` | No     | Yes    | No     | No     |
- **Acceptance Criteria:**
  - [ ] An `academy_admin` can add a new inventory item with a name and quantity.
  - [ ] An `academy_admin` can view a categorized list of all inventory.
  - [ ] An `academy_admin` can increase or decrease the quantity of an existing item.
  - [ ] An `academy_admin` can remove an inventory item.
  - [ ] A `director_of_football` can view the inventory list but cannot modify it.

#### 3.4. Financial Records Management
- **User Story:** "As an `academy_admin`, I need to process and log payments from parents or players to maintain accurate financial records for the academy."
- **Detailed Workflow:**
  - **Trigger:** The `academy_admin` navigates to the "Financials" section and chooses to log a new payment.
  - **Process:**
    1. The admin selects the player or parent making the payment.
    2. The admin enters the payment amount, date, and a brief description or reference number (e.g., "Invoice #123," "Monthly Fee").
    3. The system records the transaction and associates it with the selected user's profile within the tenant.
    4. A history of all transactions is viewable.
  - **Data Rules:**
    - The payment amount must be a positive number.
    - The payment must be associated with an existing player or parent in the academy.
  - **Edge Cases (Failure states):**
    - If the payment amount is zero or negative, the system rejects the entry.
    - If the selected player/parent does not exist, the system returns an error.
- **Permission Matrix:**
  | Role           | Create | Read   | Update | Delete |
  |----------------|--------|--------|--------|--------|
  | `academy_owner`| Yes    | Yes    | Yes    | Yes    |
  | `academy_admin`| Yes    | Yes    | Yes    | Yes    |
- **Acceptance Criteria:**
  - [ ] An `academy_admin` can successfully log a payment for a player.
  - [ ] An `academy_admin` can view a complete history of all financial transactions within the academy.
  - [ ] An `academy_admin` can edit or void an incorrectly logged payment.
  - [ ] An `academy_owner` has full access to create, view, and manage all financial records.
  - [ ] A `coach` or `parent` cannot access the academy-wide financial records module.
