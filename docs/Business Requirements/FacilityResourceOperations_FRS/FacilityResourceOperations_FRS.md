# Functional Requirement Specification: Facility & Resource Operations

### 1. Executive Summary
This document specifies the requirements for a facility and resource management module. The system will enable the booking of pitches, rooms, and other shared resources, with a focus on preventing scheduling conflicts and optimizing usage.

### 2. Key Actors
- **`academy_admin`**: Manages the master schedule, resolves conflicts, and defines available resources.
- **`director_of_football`**: Can book and manage resources for all football-related activities.
- **`coach`**: Can view resource availability and place booking requests for their team's activities.

---
### 3. Functional Capabilities

#### 3.1. Pitch Booking and Splitting
- **User Story:** "As a `director_of_football`, I need a visual calendar to book a full pitch for a match or split a single pitch into multiple zones (e.g., Half A, Half B, Quarters) to schedule several training groups at once."
- **Detailed Workflow:**
  - **Trigger:** A user with booking rights accesses the "Facility Calendar".
  - **Process:**
    1. The calendar displays all pitches and their existing bookings in a timeline view (daily, weekly, monthly).
    2. The user clicks on an empty time slot for a specific pitch (e.g., "Pitch 1").
    3. A booking form appears, asking for the `Event Title`, `Start Time`, and `End Time`.
    4. The user is presented with a `Booking Type` option:
       - **Full Pitch**: Reserves the entire pitch.
       - **Split Pitch**: Reveals sub-options for a pre-configured layout (e.g., "Half A," "Half B"). The user can then book one or more of these zones.
    5. The booking is added to the calendar, visually indicating which part of the pitch is in use.
  - **Data Rules:**
    - Pitches and their possible split configurations (e.g., "Pitch 1" can be split into two halves) are defined by the `academy_admin`.
    - A "Full Pitch" booking conflicts with any "Split Pitch" bookings for the same pitch at the same time.
    - Two bookings cannot reserve the same split zone (e.g., "Half A") at the same time.
  - **Edge Cases (Failure states):**
    - If a user tries to book "Full Pitch" when "Half A" is already booked, the system will show a conflict and prevent the booking.
- **Permission Matrix:**
  | Role                   | Define Pitches & Layouts | Book/Modify Any Booking | Request a Booking |
  |------------------------|--------------------------|-------------------------|-------------------|
  | `academy_admin`        | Yes                      | Yes                     | Yes               |
  | `director_of_football` | No                       | Yes                     | Yes               |
  | `coach`                | No                       | No                      | Yes               |
- **Acceptance Criteria:**
  - [ ] An `academy_admin` can define a new facility named "Pitch 3" and configure it to be splittable into four quarters.
  - [ ] A `director_of_football` can book "Pitch 1 - Half A" from 9:00 to 10:00 for the U17s.
  - [ ] A `director_of_football` can then book "Pitch 1 - Half B" for the same time for the U19s.
  - [ ] The system prevents another user from booking "Pitch 1 - Full Pitch" from 9:30 to 10:30 due to the conflict.

#### 3.2. Real-Time Conflict Detection
- **User Story:** "As the system, I must instantly detect and clearly report any booking conflicts to prevent double-booking of facilities or resources."
- **Detailed Workflow:**
  - **Trigger:** A user attempts to save a new booking or modify an existing one.
  - **Process:**
    1. The system takes the requested `Resource ID` (e.g., "Pitch 1"), `Zone` (e.g., "Half A" or "Full"), `Start Time`, and `End Time`.
    2. It queries all existing bookings for that same `Resource ID`.
    3. It checks if any existing booking's time range overlaps with the requested time range AND if their zones are incompatible (e.g., Full vs. Half, or the same Half).
    4. If a conflict is found, the save action is rejected.
    5. The system presents a clear error message to the user, stating what the conflict is (e.g., "This time conflicts with 'U17 Training' booked by Coach Smith on Pitch 1 - Half A").
  - **Data Rules:**
    - A conflict exists if `(NewStartTime < ExistingEndTime)` and `(NewEndTime > ExistingStartTime)`.
    - The zone check must account for the hierarchy (Full conflicts with all sub-zones).
  - **Edge Cases (Failure states):**
    - The primary function is to handle the failure state (conflict).
- **Permission Matrix:**
  - This is an automated system process and has no direct user permissions.
- **Acceptance Criteria:**
  - [ ] If a booking exists from 9:00-10:00, the system prevents a new booking from 9:30-10:30 for the same resource.
  - [ ] If "Pitch 2 - Full" is booked, the system prevents a new booking for "Pitch 2 - Half A" at the same time.
  - [ ] The error message provided to the user clearly identifies the conflicting event.

#### 3.3. Shared Resource Management (Non-Pitch)
- **User Story:** "As an `academy_admin`, I need to manage a list of bookable, non-pitch resources like 'GPS Vests' or 'Conference Room' and be able to mark them as unavailable for maintenance."
- **Detailed Workflow:**
  - **Trigger:** The `academy_admin` navigates to the "Shared Resources" management page.
  - **Process:**
    1. The admin can add a new resource, giving it a name (e.g., "Team Projector") and specifying if it is quantity-based (e.g., "GPS Vests", quantity: 20).
    2. The admin can toggle a resource's status between `Available` and `Out of Service`.
    3. A `coach` wishing to use a resource follows a similar booking workflow as for pitches, selecting the resource and time. For quantity-based resources, they specify the amount needed.
  - **Data Rules:**
    - A resource marked as `Out of Service` cannot be booked.
    - A user cannot book a quantity of a resource greater than what is available at that time.
  - **Edge Cases (Failure states):**
    - If a coach tries to book 25 GPS vests when only 20 exist, the request is denied.
    - If a coach tries to book the "Team Projector" when it is marked `Out of Service`, the requested time slot will appear as unavailable.
- **Permission Matrix:**
  | Role            | Add/Remove Resources & Set Status | Book Resources |
  |-----------------|-----------------------------------|----------------|
  | `academy_admin` | Yes                               | Yes            |
  | `coach`         | No                                | Yes            |
- **Acceptance Criteria:**
  - [ ] An `academy_admin` can add "Conference Room" as a new, bookable resource.
  - [ ] The `academy_admin` can mark the "Conference Room" as `Out of Service` for a specific day.
  - [ ] A `coach` is able to successfully book 15 GPS vests for their training session.
  - [ ] The system prevents another coach from booking 10 more vests at the same time, as only 5 would be left.
