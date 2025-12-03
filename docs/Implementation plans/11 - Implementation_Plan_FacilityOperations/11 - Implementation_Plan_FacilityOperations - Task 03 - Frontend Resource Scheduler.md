# Task Context
Create the frontend Resource Scheduler using `FullCalendar`'s Timeline view. This visualizes resources on the vertical axis and time on the horizontal axis, allowing for easy identification of gaps and conflicts.

# Core References
- **Plan:** [11 - Implementation_Plan_FacilityOperations.md](./11%20-%20Implementation_Plan_FacilityOperations.md)

# Step-by-Step Instructions
1.  **Install Dependencies:**
    *   `npm install @fullcalendar/react @fullcalendar/resource-timeline`.
2.  **Create `ResourceScheduler.tsx`:**
    *   Path: `src/features/facilities/components/ResourceScheduler.tsx`.
    *   Fetch Resources and Bookings.
    *   Format data for FullCalendar (`resourceId`, `title`, `start`, `end`).
    *   Render `FullCalendar` with `resourceTimelineWeek` view.
3.  **Integrate:**
    *   Use in `FacilitiesPage.tsx`.

# Acceptance Criteria
- [ ] Dependencies installed.
- [ ] `ResourceScheduler` renders timeline view.
- [ ] Resources and Bookings are correctly displayed.
