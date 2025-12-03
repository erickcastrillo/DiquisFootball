# Task Context
Create the floating Chat Widget for the public marketing site. It should handle visitor ID persistence, message history, and real-time interaction with the AI service.

# Core References
- **Plan:** [19 - Implementation_Plan_AISalesAgent.md](./19%20-%20Implementation_Plan_AISalesAgent.md)

# Step-by-Step Instructions
1.  **Create `ChatWidget.tsx`:**
    *   Path: `src/features/ai-sales/components/ChatWidget.tsx`.
    *   State: `isOpen`, `messages`, `visitorId`.
    *   On Mount: Check `localStorage` for `visitorId`.
    *   UI: Floating Button (toggles Card), Message List (scrollable), Input Form.
    *   Logic:
        *   Send user message to API.
        *   Receive response + new `visitorId` (if first time).
        *   Update `localStorage` and message list.
2.  **Integrate:**
    *   Add to the main public layout (e.g., `PublicLayout.tsx`).

# Acceptance Criteria
- [ ] `ChatWidget` renders and toggles.
- [ ] Messages are sent/received and displayed.
- [ ] `visitorId` persists across reloads.
