# Task Context
Implement the frontend Notification Bell and a Preferences settings page. The bell should poll for unread notifications and show a dropdown list.

# Core References
- **Plan:** [16 - Implementation_Plan_CommunicationEngine.md](./16%20-%20Implementation_Plan_CommunicationEngine.md)

# Step-by-Step Instructions
1.  **Create `NotificationBell.tsx`:**
    *   Path: `src/layouts/components/NotificationBell.tsx`.
    *   Poll API for unread count.
    *   Render Badge.
    *   On click, fetch recent notifications and show in Dropdown.
2.  **Create `NotificationPreferences.tsx` (Optional/Future):**
    *   A settings page to update `UserNotificationPreference`.
3.  **Integrate:**
    *   Add `NotificationBell` to the main Topbar layout.

# Acceptance Criteria
- [ ] `NotificationBell` component created and integrated.
- [ ] Polling logic works.
- [ ] Dropdown displays notifications with deep links.
