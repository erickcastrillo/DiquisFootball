# Task Context
Create the frontend representation of the domain enums in the React client. Instead of simple TypeScript enums, use constant arrays of objects to support metadata like translation keys (`labelKey`). This ensures type safety and prepares the application for internationalization (i18n).

# Core References
- **Plan:** [2 - Implementation_Plan_DomainDefinition.md](./2%20-%20Implementation_Plan_DomainDefinition.md)

# Step-by-Step Instructions
1.  **Create File:** Create `src/constants/enums.ts` in the React project.
2.  **Define `SUBSCRIPTION_TIERS`:**
    *   Array of objects with `value` (e.g., 'Grassroots') and `labelKey` (e.g., 'subscriptionTiers.grassroots').
    *   Use `as const` for type narrowing.
3.  **Define `PLAYER_POSITIONS`:**
    *   Array of objects for Goalkeeper, Defender, Midfielder, Forward.
    *   Include `labelKey` for each.
4.  **Define `GENDERS`:**
    *   Array of objects for Male, Female, NonBinary.
    *   Include `labelKey` for each.
5.  **Define Simple Enums:**
    *   `export enum ResourceStatus { Available = 'Available', OutOfService = 'OutOfService' }`
    *   `export enum InjuryStatus { Active = 'Active', Closed = 'Closed' }`
6.  **Define Helper Type:**
    *   Export `EnumOption<T>` type helper to extract union types from the constant arrays.

# Acceptance Criteria
- [ ] `src/constants/enums.ts` exists.
- [ ] Constants `SUBSCRIPTION_TIERS`, `PLAYER_POSITIONS`, and `GENDERS` are defined with `value` and `labelKey`.
- [ ] Simple enums `ResourceStatus` and `InjuryStatus` are defined.
- [ ] Type helper `EnumOption` is exported.
