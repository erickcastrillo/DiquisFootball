# Task Context
Create a reusable `AIDisclaimer` component to display mandatory liability disclaimers for AI-generated content.

# Core References
- **Plan:** [22 - Implementation_Plan_AIGovernance.md](./22%20-%20Implementation_Plan_AIGovernance.md)

# Step-by-Step Instructions
1.  **Create `AIDisclaimer.tsx`:**
    *   Path: `src/components/ui/AIDisclaimer.tsx`.
    *   Props: `type` ('coaching' | 'financial' | 'medical').
    *   Render specific text based on type.
    *   Style: Muted, italic, small font.
2.  **Integrate:**
    *   Add to `SessionGenerator.tsx` (Coaching type).
    *   Add to `CashFlowForecast.tsx` (Financial type).
    *   Add to `PlayersAtRiskWidget.tsx` (Financial type).

# Acceptance Criteria
- [ ] `AIDisclaimer` component created.
- [ ] Component integrated into key AI pages.
- [ ] Correct text displayed for each type.
