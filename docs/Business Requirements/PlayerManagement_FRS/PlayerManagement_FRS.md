# Functional Requirement Specification: Player Management

### 1. Executive Summary
This document defines the functional requirements for managing players within an academy. It covers the complete lifecycle of a player, from initial registration and profile creation to skill assignment and age validation.

### 2. Key Actors
- **`director_of_football`**: Manages all football-related activities, including player data.
- **`coach`**: Manages players within their assigned teams.
- **`parent`**: Guardian of a player, with limited access to their child's data.
- **`player`**: The athlete, with read-only access to their own profile.
- **`academy_owner`**: Has oversight and full management rights within their academy.

---
### 3. Functional Capabilities

#### 3.1. Player Registration
- **User Story:** "As a `director_of_football` or `coach`, I need to register new players with their essential personal and contact information so they can be part of the academy."
- **Detailed Workflow:**
  - **Trigger:** The user (e.g., `director_of_football`) initiates the player registration process from the "Players" module.
  - **Process:**
    1. A registration form is presented, requiring fields like Full Name, Date of Birth, Contact Information, and Parent/Guardian details.
    2. The system performs an initial check to see if a player with similar details already exists to prevent duplicates.
    3. The system automatically calculates the player's age based on their Date of Birth to determine eligible age categories.
    4. Upon submission, a new player profile is created in the system, linked to the current academy tenant.
  - **Data Rules:**
    - `Full Name` and `Date of Birth` are mandatory fields.
    - `Date of Birth` must be a valid date in the past.
    - At least one parent/guardian contact must be provided for players under a certain age (e.g., 18).
  - **Edge Cases (Failure states):**
    - If a potential duplicate player is found, the system alerts the user and asks for confirmation before proceeding.
    - If the Date of Birth is invalid or in the future, the system rejects the entry with a validation error.
- **Permission Matrix:**
  | Role                   | Create | Read   | Update | Delete |
  |------------------------|--------|--------|--------|--------|
  | `academy_owner`        | Yes    | Yes    | Yes    | Yes    |
  | `director_of_football` | Yes    | Yes    | Yes    | Yes    |
  - **coach`                 | Yes    | Yes    | Yes    | Yes    |
  | `parent`               | No     | No     | No     | No     |
- **Acceptance Criteria:**
  - [ ] A `director_of_football` can successfully register a new player.
  - [ ] A `coach` can register a new player.
  - [ ] The system prevents the creation of a player with a future birth date.
  - [ ] The system links the new player to the correct academy tenant.

#### 3.2. Player Profile Management
- **User Story:** "As a `coach`, I need to view and update my players' profiles with football-specific information, such as their preferred position, skills, and biometric data, to track their development."
- **Detailed Workflow:**
  - **Trigger:** A `coach` or `director_of_football` selects a player from a list to view their detailed profile.
  - **Process:**
    1. The system displays the player's profile, separated into sections (e.g., Personal Info, Football Skills, Biometrics).
    2. The user updates the relevant fields (e.g., assigns a "Primary Position" from a dropdown, rates a skill like "Dribbling" on a scale).
    3. A `parent` viewing the same profile can only edit fields in the "Personal Info" section.
    4. Changes are saved to the player's record.
  - **Data Rules:**
    - Skills and positions must be selected from a predefined list managed by the academy.
    - Biometric data (height, weight) must be positive numerical values.
    - `parent` roles cannot modify football-related data sections.
  - **Edge Cases (Failure states):**
    - If a user tries to enter non-numeric data into a biometric field, a validation error is shown.
    - If a `parent` attempts to modify a restricted field (e.g., skill rating), the action is blocked.
- **Permission Matrix:**
  | Role                   | Read (Own Child/Player) | Read (All Players) | Update (Personal Info) | Update (Football Info) |
  |------------------------|-------------------------|--------------------|------------------------|------------------------|
  | `academy_owner`        | N/A                     | Yes                | Yes                    | Yes                    |
  | `director_of_football` | N/A                     | Yes                | Yes                    | Yes                    |
  | `coach`                | N/A                     | Yes (Assigned Team)| Yes                    | Yes (Assigned Team)    |
  | `parent`               | Yes                     | No                 | Yes (Own Child)        | No                     |
  | `player`               | Yes                     | No                 | No                     | No                     |
- **Acceptance Criteria:**
  - [ ] A `coach` can update the skill ratings for a player on their team.
  - [ ] A `director_of_football` can update any player's football-related information.
  - [ ] A `parent` can update their child's contact phone number.
  - [ ] A `parent` cannot change their child's "Primary Position."
  - [ ] A `player` can view all information on their profile but cannot edit it.

#### 3.3. Age Category Validation
- **User Story:** "As a system, I must automatically validate and display a player's correct age category based on their date of birth to ensure they are assigned to appropriate teams and divisions."
- **Detailed Workflow:**
  - **Trigger:** A player's profile is created or their Date of Birth is updated.
  - **Process:**
    1. The system retrieves the player's Date of Birth.
    2. It compares the player's calculated age against the age brackets defined for the academy's divisions (e.g., "Under-13," "Under-15").
    3. The system assigns the player to the correct age category.
    4. This category information is displayed on the player's profile and used to filter eligible players during team rostering.
  - **Data Rules:**
    - Age category brackets are defined by the academy (e.g., via the `director_of_football`).
    - A player can only belong to one age category at a time.
  - **Edge Cases (Failure states):**
    - If a player's age falls outside of any defined category, they are marked as "Uncategorized" or ineligible.
- **Permission Matrix:**
  - This is an automated system process and has no direct user permissions.
- **Acceptance Criteria:**
  - [ ] A player born on a specific date is correctly assigned to the "Under-13" category as defined by the academy rules.
  - [ ] When a player's Date of Birth is edited, their age category is immediately recalculated.
  - [ ] When building a team roster for an "Under-15" division, players from the "Under-13" category are not shown as available for selection.
