# Functional Requirement Specification: Team Organization

### 1. Executive Summary
This document details the functional requirements for organizing football teams within an academy. It covers the creation of teams, the management of player rosters, and the assignment of teams to the correct divisions.

### 2. Key Actors
- **`director_of_football`**: Responsible for the overall structure of teams and divisions.
- **`coach`**: Manages the roster for their specific, assigned team(s).
- **`academy_owner`**: Has full oversight and management rights over all teams in their academy.
- **`player`**: Can view the team(s) they are a member of.

---
### 3. Functional Capabilities

#### 3.1. Team Creation
- **User Story:** "As a `director_of_football`, I need to create new teams, assign a name, and link them to a coach and a division, so that we can structure our academy for the season."
- **Detailed Workflow:**
  - **Trigger:** The `director_of_football` starts the "Create Team" process from the "Teams" or "Organization" module.
  - **Process:**
    1. A form is presented to enter the `Team Name`.
    2. The user selects a `Coach` from a list of available coaches in the academy.
    3. The user selects a `Division` (e.g., "Under-13," "Under-15") from a list of predefined divisions.
    4. Upon submission, the system creates the new team, establishing links to the selected coach and division.
  - **Data Rules:**
    - The `Team Name` must be unique within the academy for a given season or division to avoid confusion.
    - A team must be assigned to exactly one `Coach`.
    - A team must be assigned to exactly one `Division`.
  - **Edge Cases (Failure states):**
    - If the proposed `Team Name` is already in use within the same division, the system prompts the user to choose a different name.
    - If the selected `Coach` or `Division` does not exist, the system returns an error.
- **Permission Matrix:**
  | Role                   | Create | Read   | Update | Delete |
  |------------------------|--------|--------|--------|--------|
  | `academy_owner`        | Yes    | Yes    | Yes    | Yes    |
  | `director_of_football` | Yes    | Yes    | Yes    | Yes    |
  | `coach`                | No     | Yes    | No     | No     |
- **Acceptance Criteria:**
  - [ ] A `director_of_football` can successfully create a new team with a unique name.
  - [ ] The new team must be associated with a registered coach and an existing division.
  - [ ] A `coach` can view the teams in the academy but cannot create or delete them.
  - [ ] An `academy_owner` has the ability to create, modify, or delete any team.

#### 3.2. Team Roster Management
- **User Story:** "As a `coach`, I need to add and remove players from my team's roster to finalize my squad for the season."
- **Detailed Workflow:**
  - **Trigger:** A `coach` selects their team and navigates to the "Manage Roster" section.
  - **Process:**
    1. The system displays the current roster and provides an "Add Player" function.
    2. When adding a player, the system presents a list of registered players in the academy who are not already on another team's roster.
    3. This list is pre-filtered to only show players whose age category matches the team's division requirements.
    4. The `coach` selects one or more players to add to the roster.
    5. To remove a player, the `coach` selects a player from the current roster and confirms the removal.
  - **Data Rules:**
    - A player can only be on one team's roster at a time.
    - Only players who meet the age requirements for the team's division can be added to the roster.
    - A roster cannot exceed the maximum number of players defined for that division.
  - **Edge Cases (Failure states):**
    - If a `coach` tries to add a player who is already on another team, the system shows a message indicating the player is unavailable.
    - If a `coach` attempts to add a player who is too old for the division, that player will not appear in the list of eligible players.
- **Permission Matrix:**
  | Role                   | Add/Remove Players (Own Team) | Add/Remove Players (All Teams) | Read Roster |
  |------------------------|-------------------------------|--------------------------------|-------------|
  | `academy_owner`        | N/A                           | Yes                            | Yes         |
  | `director_of_football` | N/A                           | Yes                            | Yes         |
  | `coach`                | Yes                           | No                             | Yes         |
  | `player`               | No                            | No                             | Yes (Own Team) |
- **Acceptance Criteria:**
  - [ ] A `coach` can view a list of all players on their assigned team.
  - [ ] A `coach` can add an eligible, unassigned player to their roster.
  - [ ] A `coach` cannot add a player who is too old for the team's division.
  - [ ] A `coach` can remove a player from their roster.
  - [ ] A `director_of_football` can add or remove any player from any team's roster.

#### 3.3. Division Management
- **User Story:** "As a `director_of_football`, I need to create and manage the divisions (e.g., 'Under-13 Boys', 'Under-15 Girls') for our academy, defining their age constraints and gender."
- **Detailed Workflow:**
  - **Trigger:** The `director_of_football` accesses the "Divisions" management area.
  - **Process:**
    1. The system displays a list of current divisions.
    2. To create a new division, the user provides a Name (e.g., "U-13 Boys"), an age range (e.g., min age 11, max age 13), and a gender (e.g., Male, Female, Co-ed).
    3. The user can also edit the parameters of an existing division.
  - **Data Rules:**
    - Each division name must be unique within the academy.
    - Age ranges must be logical (min age < max age).
  - **Edge Cases (Failure states):**
    - If a user tries to create a division with a name that already exists, the system prevents it.
    - If an invalid age range is entered, the system shows a validation error.
- **Permission Matrix:**
  | Role                   | Create | Read   | Update | Delete |
  |------------------------|--------|--------|--------|--------|
  | `academy_owner`        | Yes    | Yes    | Yes    | Yes    |
  | `director_of_football` | Yes    | Yes    | Yes    | Yes    |
  | `coach`                | No     | Yes    | No     | No     |
- **Acceptance Criteria:**
  - [ ] A `director_of_football` can create a new division with a name, age range, and gender.
  - [ ] A `director_of_football` can update the details of an existing division.
  - [ ] The defined age constraints for a division are used to filter players during roster management.
  - [ ] A `coach` can see what division their team belongs to but cannot create or edit divisions.
