## Module 16-B: ⚽ Operational AI (The Assistant Coach)

### 1. Executive Summary
This document specifies the requirements for a suite of internal AI-powered tools designed to reduce the administrative burden on coaching staff. By automating time-consuming tasks like session planning and post-match reporting, the "Assistant Coach" module allows coaches to focus more of their time on player development and on-field activities.

### 2. Key Actors
- **`Coach`**: The primary user of the AI tools, leveraging them to plan sessions and communicate with parents.
- **`Academy Director`**: Oversees the use of the tools and can review generated content for quality and consistency.
- **`Parent`**: The end consumer of AI-assisted communications, such as personalized match feedback reports.

### 3. Functional Capabilities

#### Feature: Smart Training Session Generator
- **User Story:** "As a Coach pressed for time, I want to generate a structured, age-appropriate, 90-minute session plan based on my team's specific weaknesses and our available equipment."
- **Detailed Business Logic:**
    - **Inputs:** The `Coach` navigates to the "Session Planner" and initiates the "Generate with AI" feature. A form is presented with the following inputs:
        - `Team`: Dropdown pre-populated with the coach's assigned teams (e.g., "U-14 Boys").
        - `Duration (minutes)`: A numerical input (e.g., `90`).
        - `Focus Area`: A multi-select or dropdown list of tactical and technical themes (e.g., "Defensive Transitions," "Attacking from Wide Areas," "Finishing Drills").
        - `Available Space`: A choice of pre-configured pitch sizes (e.g., "Full Pitch," "Half Pitch," "Penalty Box Area").
    - **Data Cross-Reference:** Before generating the plan, the system enriches the AI prompt with critical context from other modules:
        1.  **Module 3 (Team Organization):** The system fetches the selected team's age group (e.g., U-14). The AI is instructed to generate drills and language appropriate for that specific age and developmental stage.
        2.  **Module 6 (Asset Management):** The system queries the inventory for currently available equipment (e.g., `Cones: 40, Bibs: 20, Poles: 0, Mini-Goals: 4`). The AI is strictly constrained to only suggest drills that use the available items.
    - **Output:** The AI generates a structured timeline for the training session, presented in an editable text area. The output follows a logical coaching progression:
        - `0-15 mins: Warm-up` (e.g., Dynamic stretches, activation games).
        - `15-40 mins: Technical Drill` (e.g., A passing drill related to the "Focus Area").
        - `40-65 mins: Tactical Game` (e.g., A small-sided game with rules that encourage the "Focus Area").
        - `65-85 mins: Scrimmage` (e.g., A larger, less-conditioned game).
        - `85-90 mins: Cool-down` (e.g., Static stretching).
    - **Constraint:** A non-removable disclaimer **must** be appended to every AI-generated session plan: `**Disclaimer:** This is an AI-generated plan. The coach is solely responsible for the physical safety, well-being, and appropriate supervision of all players during the session.`

#### Feature: Automated Match Report & Feedback Writer
- **User Story:** "As a Coach managing a roster of 20 players, I want to quickly send personalized and encouraging post-match feedback to every parent without having to manually type 20 separate emails."
- **Detailed Business Logic:**
    - **Trigger:** After a match, the `Coach` enters key individual player stats into **Module 5 (Analytics & Reporting)** (e.g., goals, assists, tackles, interceptions). The coach then navigates to the "Post-Match Report" section and records a short voice note summarizing their overall thoughts (e.g., "Great team effort today, we showed a lot of resilience. John was a rock in defense, and Sarah's finishing was clinical.").
    - **Process:**
        1.  The system transcribes the coach's voice note into text.
        2.  The system initiates a loop for each player who participated in the match.
        3.  For each player, it feeds the AI a prompt containing the voice note transcript, the overall match result, the player's name, their parent's name, and one or two of that specific player's standout statistics from the match.
    - **Personalization:** The AI is instructed to expand these points into a polite, positive, and grammatically correct paragraph, dynamically inserting the player's name and their specific contribution.
        - *Example for John's Parent:* "Hi [Parent Name], I wanted to share a quick note about today's match. It was a great team effort, and as I noted after the game, the team's resilience was fantastic. John had a particularly strong performance and was a rock in defense, leading the team with **5 successful tackles**."
        - *Example for Sarah's Parent:* "Hi [Parent Name], I wanted to share a quick note about today's match. It was a great team effort, and as I noted after the game, the team's resilience was fantastic. Sarah was clinical in front of goal and made a huge impact, scoring **2 goals**."
    - **Governance: "Human-in-the-Loop" Approval**
        - The AI-generated messages are **not** sent automatically. They populate a "Review & Send" interface where the coach can see the personalized message for each parent.
        - The coach can make minor edits to any message if desired.
        - A single "Approve & Send All" button dispatches the messages via **Module 13 (Communication Engine)**. This button is disabled until the coach has scrolled through the list, ensuring they have had an opportunity to review the content.

### 4. Acceptance Criteria
- [ ] The Session Generator successfully creates a training plan and does not suggest drills requiring equipment marked as "0" in the inventory (e.g., no pole-weaving drills if `Poles: 0`).
- [ ] The Feedback Writer generates distinctly different paragraphs for two players, one highlighting a defensive stat (tackles) and the other highlighting an offensive stat (goals).
- [ ] The "Approve & Send All" button on the feedback generation screen is disabled by default and only becomes enabled after the user has interacted with the review list.
- [ ] The AI-generated session plan includes the mandatory safety disclaimer in its output.
