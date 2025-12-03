# Functional Requirement Specification: Asset Management

### 1. Executive Summary
This document specifies the requirements for managing academy assets. It covers the lifecycle of physical goods, from initial inventory stocking to distribution to players and tracking by coaches.

### 2. Key Actors
- **`academy_owner`**: Has full oversight of all academy assets and inventory levels.
- **`academy_admin`**: Responsible for the day-to-day management, tracking, and auditing of all inventory.
- **`director_of_football`**: Can view inventory levels and asset assignments.
- **`coach`**: Can check out communal equipment for sessions and view assets assigned to their players.

---
### 3. Functional Capabilities

#### 3.1. Equipment Tracking & Inventory
- **User Story:** "As an `academy_admin`, I need a centralized system to track all our physical assets, from balls and cones to medical kits, so I always know our current stock levels."
- **Detailed Workflow:**
  - **Trigger:** The `academy_admin` navigates to the "Asset Management" or "Inventory" module.
  - **Process:**
    1. The system displays a categorized list of all assets with their `Total Quantity` and `Available Quantity`.
    2. To add new stock, the admin selects an existing asset and increases its `Total Quantity` or creates a new asset type entirely.
    3. To remove old stock, the admin can "retire" an asset, providing a reason (e.g., "Damaged," "Lost"), which reduces the `Total Quantity`.
  - **Data Rules:**
    - `Available Quantity` = `Total Quantity` - `Checked Out Quantity` - `Assigned Quantity`. This is a calculated field and cannot be edited directly.
    - Asset quantities cannot be negative.
- **Permission Matrix:**
  | Role                   | Create/Update Asset Types & Quantities | Read Inventory Levels |
  |------------------------|----------------------------------------|-----------------------|
  | `academy_owner`        | Yes                                    | Yes                   |
  | `academy_admin`        | Yes                                    | Yes                   |
  | `director_of_football` | No                                     | Yes                   |
  | `coach`                | No                                     | Yes                   |
- **Acceptance Criteria:**
  - [ ] An `academy_admin` can add 50 new training balls to the inventory, updating the total and available count.
  - [ ] An `academy_admin` can create a new asset category called "First-Aid Supplies".
  - [ ] A `coach` can view that there are 45 available training balls.
  - [ ] A `director_of_football` can see the total inventory but cannot change the quantities.

#### 3.2. Uniform & Equipment Distribution
- **User Story:** "As an `academy_admin`, I need to assign a specific uniform kit (jersey number, size) to each player to track who has received their gear for the season."
- **Detailed Workflow:**
  - **Trigger:** The `academy_admin` opens a player's profile and navigates to the "Assigned Assets" tab.
  - **Process:**
    1. The admin clicks "Assign Asset".
    2. They select an asset type (e.g., "Home Kit Jersey") from a list of assignable items.
    3. The admin enters specific details for that assignment (e.g., `Size: Large`, `Jersey Number: 10`).
    4. Upon saving, the system links that specific asset assignment to the player's profile.
    5. The `Available Quantity` for the assigned asset type in the main inventory is automatically reduced by one.
  - **Data Rules:**
    - A player cannot be assigned the same unique item (e.g., Jersey #10 on the U15 team) as another player on the same team.
    - An asset cannot be assigned if its `Available Quantity` is zero.
  - **Edge Cases (Failure states):**
    - If an admin tries to assign a jersey number that is already taken on that team, the system shows an error.
    - If the admin attempts to assign a uniform that is out of stock, the action is blocked.
- **Permission Matrix:**
  | Role                   | Assign/Unassign Assets to Player | View Assigned Assets |
  |------------------------|----------------------------------|----------------------|
  | `academy_owner`        | Yes                              | Yes                  |
  | `academy_admin`        | Yes                              | Yes                  |
  | `director_of_football` | No                               | Yes                  |
  | `coach`                | No                               | Yes (Own Team)       |
  | `player` / `parent`    | No                               | Yes (Self/Child)     |
- **Acceptance Criteria:**
  - [ ] An `academy_admin` can assign Home Kit #7, Size M to a player.
  - [ ] After assignment, the inventory shows one less "Home Kit Jersey - Size M" available.
  - [ ] A `coach` can view a list of their players and the jersey numbers assigned to them.
  - [ ] A `player` can log in and see the details of the uniform kit assigned to them.

#### 3.3. Inventory Checks & Audits
- **User Story:** "As an `academy_admin`, I need to conduct periodic inventory audits to compare our physical stock against the system records and identify any discrepancies."
- **Detailed Workflow:**
  - **Trigger:** An `academy_admin` starts a "New Audit" from the Asset Management dashboard.
  - **Process:**
    1. The system generates a snapshot list of all assets and their expected `Total Quantity` at that moment.
    2. The admin goes through the physical inventory and enters the `Physical Count` for each item into a corresponding field.
    3. After completing the counts, the admin submits the audit.
    4. The system displays an "Audit Report" highlighting any items where the `Physical Count` does not match the `System Quantity`, showing the variance.
    5. The admin can then use this report to manually adjust the system quantities to reflect reality, providing a reason for the change (e.g., "Correction after audit").
  - **Data Rules:**
    - An audit is a point-in-time record and does not block regular asset management operations.
  - **Edge Cases (Failure states):**
    - N/A - this is a data-entry and comparison process.
- **Permission Matrix:**
  | Role            | Perform Audit & View Report | Adjust Quantities Post-Audit |
  |-----------------|-----------------------------|------------------------------|
  | `academy_owner` | Yes                         | Yes                          |
  | `academy_admin` | Yes                         | Yes                          |
- **Acceptance Criteria:**
  - [ ] An `academy_admin` can start a new audit and gets a list of all trackable assets.
  - [ ] The admin can enter physical counts for each asset.
  - [ ] The final audit report correctly shows a variance of -2 for "Training Balls" if the system expected 50 but only 48 were counted.
  - [ ] The `academy_admin` can update the system quantity to 48, leaving a note about the audit result.
