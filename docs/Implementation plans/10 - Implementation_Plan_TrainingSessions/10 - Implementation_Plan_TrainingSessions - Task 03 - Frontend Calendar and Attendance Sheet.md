# Task Context
Create the frontend UI for Training Sessions. This includes a Calendar view using `FullCalendar` for scheduling and an Attendance Sheet for tracking player participation.

# Core References
- **Plan:** [10 - Implementation_Plan_TrainingSessions.md](./10%20-%20Implementation_Plan_TrainingSessions.md)

# Step-by-Step Instructions
1.  **Install Dependencies:**
    *   `npm install @fullcalendar/react @fullcalendar/daygrid @fullcalendar/timegrid @fullcalendar/interaction`.
2.  **Create `TrainingCalendar.tsx`:**
    *   Path: `src/features/training/components/TrainingCalendar.tsx`.
    *   Use `FullCalendar` with dayGrid/timeGrid plugins.
    *   Handle `dateClick` to open `SessionFormModal`.
3.  **Create `AttendanceSheet.tsx`:**
    *   Path: `src/features/training/components/AttendanceSheet.tsx`.
    *   Fetch roster/attendance.
    *   Render table with status dropdowns.
    *   Save attendance to API.

# Acceptance Criteria
- [ ] Dependencies installed.
- [ ] `TrainingCalendar` renders and handles events.
- [ ] `AttendanceSheet` allows marking and saving attendance.
