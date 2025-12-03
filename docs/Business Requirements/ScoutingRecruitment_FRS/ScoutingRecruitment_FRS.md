# Functional Requirement Specification: Scouting & Recruitment

### 1. Executive Summary
This document defines the requirements for a scouting and recruitment module designed to identify and evaluate external talent. It provides tools for managing trialists, creating standardized evaluation reports, and seamlessly converting promising prospects into full academy members.

### 2. Key Actors
- **`director_of_football`**: Oversees the entire scouting process, manages scout assignments, and makes the final decision on recruitment.
- **`coach`**: Acts as a scout, responsible for evaluating assigned trialists and providing detailed reports.

---
### 3. Functional Capabilities

#### 3.1. Trialist Management & "Shadow Squads"
- **User Story:** "As a `director_of_football`, I need to create profiles for trialist players and organize them into 'Shadow Squads' so we can efficiently manage and track external talent without cluttering our main academy roster."
- **Detailed Workflow:**
  - **Trigger:** A user with scouting permissions initiates the "Add Trialist" process.
  - **Process:**
    1. A simplified form is used to create a trialist profile, capturing basic information: `Name`, `Date of Birth`, `Contact Info`, and `Previous Club`.
    2. The `director_of_football` can create and name "Shadow Squads" (e.g., "U15 Trialists - Fall 2025").
    3. The newly created trialist is assigned to a relevant Shadow Squad.
    4. These squads can be used to invite trialists to specific training sessions or trial matches.
  - **Data Rules:**
    - Trialist profiles are functionally separate from full academy player profiles.
    - Trialists cannot be added to official team rosters.
    - A trialist's data is subject to the same privacy rules as regular players.
  - **Edge Cases (Failure states):**
    - If a user attempts to add a trialist who already has an active trialist or full player profile, the system flags the potential duplicate.
- **Permission Matrix:**
  | Role                   | Create/Update/Delete Trialist | Create/Manage Shadow Squads |
  |------------------------|-------------------------------|-----------------------------|
  | `director_of_football` | Yes                           | Yes                         |
  | `coach`                | Yes (if assigned)             | No                          |
- **Acceptance Criteria:**
  - [ ] A `director_of_football` can create a new Shadow Squad.
  - [ ] A `coach` can add a new trialist they scouted at a local game.
  - [ ] A trialist profile is clearly distinguished from a full academy player profile in the user interface.
  - [ ] A user cannot assign a trialist to an official academy team roster.

#### 3.2. Standardized Talent Evaluation Cards
- **User Story:** "As a `coach` acting as a scout, I must use a standardized evaluation card to rate a trialist's abilities, ensuring that all prospects are judged by the same objective criteria."
- **Detailed Workflow:**
  - **Trigger:** A `coach` selects a trialist from a Shadow Squad and chooses to "Add Evaluation."
  - **Process:**
    1. The system presents a digital scorecard based on FIFA high-performance metrics, categorized into sections:
       - **Technical**: Dribbling, Passing, Finishing (rated 1-10).
       - **Tactical**: Positional Awareness, Decision Making (rated 1-10).
       - **Physical**: Speed, Stamina, Strength (rated 1-10).
       - **Psychological**: Work Ethic, Coachability (rated 1-10).
    2. A mandatory `Summary Notes` field requires the scout to provide qualitative feedback.
    3. The completed evaluation is saved to the trialist's profile, timestamped, and attributed to the scout.
  - **Data Rules:**
    - All ratings must be within the defined scale (1-10).
    - A trialist can have multiple evaluation cards from different scouts or different events.
  - **Edge Cases (Failure states):**
    - If the `Summary Notes` field is left blank, the form cannot be submitted.
- **Permission Matrix:**
  | Role                   | Create/Update Own Evaluations | Read All Evaluations | Delete Evaluations |
  |------------------------|-------------------------------|----------------------|--------------------|
  | `director_of_football` | Yes                           | Yes                  | Yes                |
  | `coach`                | Yes                           | Yes (Assigned Squad) | No                 |
- **Acceptance Criteria:**
  - [ ] A `coach` can successfully fill out and submit an evaluation card for a trialist.
  - [ ] The evaluation card includes distinct sections for Technical, Tactical, and Physical attributes.
  - [ ] A `director_of_football` can view and compare evaluation cards from two different coaches for the same trialist.
  - [ ] A `coach` cannot edit or delete an evaluation submitted by another coach.

#### 3.3. Convert Trialist to Full Academy Player
- **User Story:** "As a `director_of_football`, after deciding to recruit a promising trialist, I need a simple process to convert their trialist profile into a full academy player profile."
- **Detailed Workflow:**
  - **Trigger:** The `director_of_football` reviews a trialist's profile and selects the "Promote to Academy" action.
  - **Process:**
    1. The system opens the standard player registration form.
    2. It automatically populates the form with all available data from the trialist profile (`Name`, `Date of Birth`, etc.).
    3. The user fills in any remaining mandatory fields required for a full player (e.g., parent guardian info, medical consent).
    4. Upon submission, a new, full player profile is created.
    5. The original trialist profile is archived, but all associated evaluation cards are permanently linked to the new player profile for historical reference.
  - **Data Rules:**
    - The conversion process must be atomic; if it fails, the trialist profile remains active and no partial player profile is created.
    - All scouting history (evaluations) must be preserved.
  - **Edge Cases (Failure states):**
    - If the promotion process is initiated but cancelled before completion, no changes are made.
- **Permission Matrix:**
  | Role                   | Execute Promotion |
  |------------------------|-------------------|
  | `director_of_football` | Yes               |
  | All other roles        | No                |
- **Acceptance Criteria:**
  - [ ] A `director_of_football` can successfully promote a trialist.
  - [ ] The trialist's name and DoB are auto-filled in the new player registration form.
  - [ ] After promotion, the player appears in the main player list, not the trialist list.
  - [ ] The new player's profile contains a "Scouting History" tab showing all evaluations from their time as a trialist.
