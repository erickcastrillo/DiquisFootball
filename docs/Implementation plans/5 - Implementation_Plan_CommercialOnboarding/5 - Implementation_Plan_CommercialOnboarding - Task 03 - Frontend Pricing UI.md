# Task Context
Create the frontend interface for users to view subscription plans and initiate the upgrade process. This includes a reusable pricing card component and a main subscription management page that interacts with the backend to create Stripe Checkout sessions.

# Core References
- **Plan:** [5 - Implementation_Plan_CommercialOnboarding.md](./5%20-%20Implementation_Plan_CommercialOnboarding.md)

# Step-by-Step Instructions
1.  **Create `PricingCard.tsx`:**
    *   Path: `src/components/subscriptions/PricingCard.tsx`
    *   Props: `tierName`, `price`, `features`, `isCurrentPlan`, `onSelect`.
    *   UI: Bootstrap Card with list of features and a CTA button.
2.  **Create `SubscriptionPage.tsx`:**
    *   Path: `src/pages/settings/SubscriptionPage.tsx`
    *   Logic:
        *   Fetch current subscription details on mount.
        *   Render `PricingCard` for "Grassroots" and "Professional".
        *   Handle selection: Call API to get Stripe Checkout URL, then redirect `window.location.href`.
3.  **Create API Hook (`useSubscriptionApi`):**
    *   Implement `getCurrentSubscription` and `createCheckoutSession`.

# Acceptance Criteria
- [ ] `PricingCard` component exists.
- [ ] `SubscriptionPage` renders cards and handles clicks.
- [ ] API integration for checkout session creation is implemented.
