# Task Context
Implement the core conflict detection logic in `BookingService.cs`. This algorithm must handle complex scenarios like "Pitch Splitting" (Full vs. Half A vs. Half B) and quantity-based equipment availability.

# Core References
- **Plan:** [11 - Implementation_Plan_FacilityOperations.md](./11%20-%20Implementation_Plan_FacilityOperations.md)

# Step-by-Step Instructions
1.  **Implement `CheckForConflictsAsync` (Private):**
    *   Input: `Booking newBooking`.
    *   Logic:
        *   Check if resource is `OutOfService`.
        *   Fetch overlapping confirmed bookings.
        *   **Pitch Logic:**
            *   If new is "Full", conflict with ANY overlap.
            *   If existing is "Full", conflict with new split.
            *   If zones match (e.g., Half A vs Half A), conflict.
        *   **Equipment Logic:**
            *   Sum quantities of overlapping bookings.
            *   Check if `Total - Booked < NewQuantity`.
        *   **Standard Logic:** Simple overlap check.
2.  **Integrate into `CreateBookingAsync`:**
    *   Call `CheckForConflictsAsync` before saving.
3.  **Unit Test:**
    *   Create `Diquis.Infrastructure.Tests/Bookings/ConflictDetectionTests.cs`.
    *   Test overlapping scenarios (Full vs Split, Split vs Split, Quantity limits).

# Acceptance Criteria
- [ ] `BookingService` implements robust conflict detection.
- [ ] Pitch splitting logic works correctly.
- [ ] Equipment quantity logic works correctly.
- [ ] Unit tests pass.
