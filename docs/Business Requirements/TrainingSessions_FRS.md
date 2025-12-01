# Functional Requirement Specification: Training Sessions

### 1. Executive Summary
This document specifies the requirements for managing team training sessions. It enables coaches to schedule practices, record player attendance, and attach relevant training materials for their teams.

### 2. Key Actors
- **`director_of_football`**: Oversees all football activities and can manage any session.
- **`coach`**: Manages all aspects of training sessions for their assigned teams.
- **`player`**: Can view details of upcoming and past sessions for their team.
- **`parent`**: Can view session details for their child's team.

---
### 3. Functional Capabilities

#### 3.1. Session Scheduling
- **User Story:** "As a `coach`, I need to schedule new training sessions for my team by specifying the date, time, location, and session objectives, so my players know when and where to be."
- **Detailed Workflow:**
  - **Trigger:** A `coach` or `director_of_football` accesses the team calendar or schedule and initiates the "Create Session" action.
  - **Process:**
    1. A form is displayed requiring the user to select the `Team`, `Date`, `Start Time`, `End Time`, and `Location`.
    2. An optional field is available for `Session Focus` or `Notes` (e.g., "Focus on defensive drills").
    3. The system checks for scheduling conflicts for the selected team, coach, and location.
    4. Upon confirmation, the session is created and appears on the schedules for the team, coach, and all associated players/parents.
  - **Data Rules:**
    - `Start Time` must be before `End Time`.
    - `Location` must be selected from a predefined list of available training facilities managed by the academy.
    - The system must prevent a coach from being scheduled for two different sessions at the same time.
  - **Edge Cases (Failure states):**
    - If a scheduling conflict is detected (e.g., the team already has a session), the system blocks the creation and notifies the user.
    - If the selected location is marked as "unavailable" at the chosen time, the creation is blocked.
- **Permission Matrix:**
  | Role                   | Create | Read   | Update | Delete |
  |------------------------|--------|--------|--------|--------|
  | `director_of_football` | Yes    | Yes    | Yes    | Yes    |
  | `coach`                | Yes    | Yes    | Yes    | Yes    |
  | `player`               | No     | Yes    | No     | No     |
  | `parent`               | No     | Yes    | No     | No     |
- **Acceptance Criteria:**
  - [ ] A `coach` can successfully schedule a new training session for their team.
  - [ ] A `director_of_football` can schedule a session for any team in the academy.
  - [ ] The system prevents the creation of a session that conflicts with another of the team's events.
  - [ ] Players and parents associated with the team can view the scheduled session details.

#### 3.2. Attendance Marking
- **User Story:** "As a `coach`, I need to take attendance for each training session to track my players' commitment and participation."
- **Detailed Workflow:**
  - **Trigger:** A `coach` selects a session from the schedule that has already occurred.
  - **Process:**
    1. The system displays the session details along with a list of all players rostered on the team at the time of the session.
    2. Next to each player's name, the coach is presented with options: `Present`, `Absent`, `Excused`.
    3. The default status for all players is "Pending" or "Not Marked".
    4. The coach marks each player's status and saves the attendance record. The system timestamps the submission.
  - **Data Rules:**
    - Attendance can only be marked for sessions with a `Start Time` in the past.
    - Once saved, the attendance record can be edited by a `coach` or `director_of_football`.
  - **Edge Cases (Failure states):**
    - If a coach attempts to mark attendance for a session that has not yet started, the feature is disabled.
- **Permission Matrix:**
  | Role                   | Read Attendance (Own Team/Child) | Read Attendance (All) | Mark/Update Attendance |
  |------------------------|----------------------------------|-----------------------|------------------------|
  | `director_of_football` | N/A                              | Yes                   | Yes                    |
  | `coach`                | Yes                              | No                    | Yes (Own Team)         |
  | `player`               | Yes                              | No                    | No                     |
  | `parent`               | Yes                              | No                    | No                     |
- **Acceptance Criteria:**
  - [ ] A `coach` can mark a player as 'Present' for a completed session.
  - [ ] A `coach` can change a player's attendance status after it has been saved.
  - [ ] A `player` can view their own attendance history.
  - [ ] A `parent` can view their child's attendance history.
  - [ ] The system prevents attendance marking for future sessions.

#### 3.3. Session Content & Video Links
- **User Story:** "As a `director_of_football`, I want to attach training plans, drill descriptions, and links to coaching videos to a session, so I can standardize our training methodology."
- **Detailed Workflow:**
  - **Trigger:** While creating or editing a session, a user clicks "Add Content" or a similar action.
  - **Process:**
    1. A text area is provided for detailed `Drill Descriptions` or `Coaching Points`.
    2. A separate field allows the user to paste one or more URLs for `Video Links`.
    3. The user saves the content, which becomes associated with that specific session.
  - **Data Rules:**
    - The `Video Links` field must validate that the input is a properly formatted URL (starts with http/https).
    - Content is visible to all roles with read access to the session.
  - **Edge Cases (Failure states):**
    - If an invalid URL is entered, the system shows a validation error and does not save the link.
- **Permission Matrix:**
  | Role                   | Create/Update Content | Read Content |
  |------------------------|-----------------------|--------------|
  | `director_of_football` | Yes                   | Yes          |
  | `coach`                | Yes (Own Team)        | Yes          |
  | `player`               | No                    | Yes          |
  | `parent`               | No                    | Yes          |
- **Acceptance Criteria:**
  - [ ] A `coach` can add a description of drills to a session they created.
  - [ ] A `director_of_football` can add a YouTube video link to any session in the academy.
  - [ ] A `player` can view the drill descriptions and click the video links for their upcoming sessions.
  - [ ] The system correctly identifies and rejects an invalid URL format.
