# Task Context
Implement the "Human-in-the-Loop" interface for reviewing AI-generated content. The critical component is the `FeedbackReviewList` which enforces a manual review before enabling the "Send" button.

# Core References
- **Plan:** [20 - Implementation_Plan_AICoaching.md](./20%20-%20Implementation_Plan_AICoaching.md)

# Step-by-Step Instructions
1.  **Create `FeedbackReviewList.tsx`:**
    *   Path: `src/features/ai-coaching/components/FeedbackReviewList.tsx`.
    *   Props: `feedbackItems` (List of drafts), `onApprove`.
    *   UI: Scrollable list of editable text areas.
    *   **Governance Logic:** "Approve & Send All" button is DISABLED by default.
    *   Enable button ONLY when user scrolls to the bottom of the list.
2.  **Create `GenerateFeedbackPage.tsx`:**
    *   Path: `src/pages/matches/[id]/GenerateFeedback.tsx`.
    *   Step 1: Voice Note Recorder/Uploader.
    *   Step 2: Loading (AI Generation).
    *   Step 3: `FeedbackReviewList`.
3.  **Unit Test:**
    *   Create `FeedbackReviewList.test.tsx`.
    *   Verify button is disabled initially and enabled after scroll.

# Acceptance Criteria
- [ ] `FeedbackReviewList` allows editing messages.
- [ ] "Approve" button is disabled until scroll-to-bottom.
- [ ] Unit tests verify governance logic.
