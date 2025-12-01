# Functional Requirement Specification: Sports Medicine & Bio-Passport

### 1. Executive Summary
This document outlines the requirements for a confidential medical module to track player health, manage injuries, and oversee rehabilitation. It ensures player welfare is prioritized and that sensitive medical data is handled with the utmost privacy and care, in line with GDPR and medical data best practices.

### 2. Key Actors
- **`director_of_football`**: Acts as the primary custodian for medical records, fulfilling the responsibilities of a head of sports medicine. *Note: For full compliance and best practice, a dedicated `Medical Staff` or `Doctor` role is highly recommended.*
- **`coach`**: Has restricted, need-to-know access to a player's fitness status but not their medical details.
- **`player`**: Can view their own complete medical history.
- **`parent`**: Can view their child's complete medical history.
- **`academy_owner`**: Can view anonymized, aggregated injury statistics but not individual records.

---
### 3. Functional Capabilities

#### 3.1. Injury Lifecycle Management
- **User Story:** "As a `director_of_football`, I need to log and manage the complete lifecycle of a player's injury—from initial report to final closure—to ensure a comprehensive and confidential medical history."
- **Detailed Workflow:**
  - **Trigger:** A player reports an injury, and the `director_of_football` selects the player and initiates the "Log New Injury" action.
  - **Process:**
    1. A secure form is presented to record the `Date of Injury`, `Body Part`, `Injury Type` (e.g., Sprain, Fracture), `Mechanism of Injury`, and a detailed `Diagnosis`.
    2. The record is created with a status of "Active". All data is encrypted.
    3. The user can attach medical documents (e.g., MRI reports, doctor's notes) to the injury record.
    4. Once the player has fully recovered and is cleared, the injury record can be marked as "Closed".
  - **Data Rules:**
    - All medical data, including notes and attachments, must be encrypted at rest and in transit.
    - An audit trail must be maintained, logging every user who views or modifies a medical record.
    - Access is strictly limited based on the permission matrix.
  - **Edge Cases (Failure states):**
    - If an uploaded document is not in a valid format (e.g., PDF, JPG), the system rejects it.
- **Permission Matrix:**
  | Role                   | Create/Update/Delete Injury Record | Read Full Injury Details | Read Anonymized Statistics |
  |------------------------|------------------------------------|--------------------------|----------------------------|
  | `director_of_football` | Yes                                | Yes                      | Yes                        |
  | `academy_owner`        | No                                 | No                       | Yes                        |
  | `coach`                | No                                 | No                       | No                         |
  | `parent` / `player`    | No                                 | Yes (Self/Child)         | No                         |
- **Acceptance Criteria:**
  - [ ] A `director_of_football` can log a new "Hamstring Strain" injury for a player.
  - [ ] All data entered in the injury form is stored in an encrypted format.
  - [ ] A `coach` viewing the player's profile cannot see the "Hamstring Strain" diagnosis.
  - [ ] A `player` can log in and view the details of their past and current injuries.
  - [ ] An `academy_owner` can see a chart showing "10 hamstring injuries academy-wide this season" without seeing player names.

#### 3.2. Rehabilitation & Return-to-Play (RTP) Protocol
- **User Story:** "As a `director_of_football`, I need to set a player's official 'Return-to-Play' status and document their rehabilitation progress to ensure a safe and managed recovery."
- **Detailed Workflow:**
  - **Trigger:** The `director_of_football` opens an "Active" injury record.
  - **Process:**
    1. The user updates the `Return-to-Play (RTP) Status` from a predefined list:
       - `Not Cleared` (No training)
       - `Cleared for Non-Contact` (Light, individual training only)
       - `Cleared for Full Training`
    2. The user adds time-stamped notes to a `Rehabilitation Log` to track progress (e.g., "Completed strength exercises").
    3. The simple RTP status (not the diagnosis) is surfaced on coach-facing views.
  - **Data Rules:**
    - The `RTP Status` is the only piece of medical information visible to the `coach`.
    - Only a user with medical write-access can change the `RTP Status`.
  - **Edge Cases (Failure states):**
    - A player cannot be added to a match roster if their `RTP Status` is not "Cleared for Full Training".
- **Permission Matrix:**
  | Role                   | Update Rehab Log & RTP Status | Read Rehab Log | Read ONLY RTP Status |
  |------------------------|-------------------------------|----------------|----------------------|
  | `director_of_football` | Yes                           | Yes            | Yes                  |
  | `coach`                | No                            | No             | Yes                  |
  | `parent` / `player`    | No                            | Yes            | Yes                  |
- **Acceptance Criteria:**
  - [ ] A `director_of_football` can change a player's status to "Cleared for Non-Contact".
  - [ ] A `coach`, viewing their team roster, sees a simple icon or label next to the player's name indicating "Non-Contact".
  - [ ] A `coach` trying to add this player to a starting lineup for a match receives an alert that the player is not cleared.
  - [ ] The player and parent can read the detailed rehab notes entered by the director.

#### 3.3. Medical Bio-Passport
- **User Story:** "As a `player`, I need to access my personal 'Bio-Passport,' which provides a complete, read-only history of my injuries, treatments, and medical clearances in one place."
- **Detailed Workflow:**
  - **Trigger:** A `player` or `parent` logs in and navigates to their "Medical Profile" section.
  - **Process:**
    1. The system presents a chronological, read-only view of all logged injuries (both active and closed).
    2. Each entry displays the full details, including diagnosis, associated rehab logs, and any attached documents.
    3. The current `RTP Status` is prominently displayed at the top.
  - **Data Rules:**
    - The data in the Bio-Passport is strictly read-only for the player/parent.
    - The user can only view their own or their child's data.
  - **Edge Cases (Failure states):**
    - If a medical document fails to load, a "Could not load document" message is shown instead of a broken link.
- **Permission Matrix:**
  | Role                | Read Bio-Passport |
  |---------------------|-------------------|
  | `parent` / `player` | Yes (Self/Child)  |
- **Acceptance Criteria:**
  - [ ] A `player` can successfully view a "Closed" ankle injury from a previous season.
  - [ ] A `parent` can open and view a PDF of a doctor's note that was attached to their child's injury record.
  - [ ] The interface is clear, easy to navigate, and presents the medical history in a logical order.
