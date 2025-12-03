# Task Context
Establish the core domain enumerations in the .NET backend. These enums define the fundamental types used across the application for subscription tiers, player positions, gender, resource status, and injury status. They must be placed in the `Diquis.Domain` project to adhere to Clean Architecture principles.

# Core References
- **Plan:** [2 - Implementation_Plan_DomainDefinition.md](./2%20-%20Implementation_Plan_DomainDefinition.md)
- **Tech Guide:** [CommercialOnboarding_TechnicalGuide.md](../../Technical%20documentation/CommercialOnboarding_TechnicalGuide/CommercialOnboarding_TechnicalGuide.md) (for context on SubscriptionTier)

# Step-by-Step Instructions
1.  **Create Directory:** Ensure the directory `Diquis.Domain/Enums` exists.
2.  **Create `SubscriptionTier.cs`:**
    *   Namespace: `Diquis.Domain.Enums`
    *   Values: `Grassroots = 0`, `Professional = 1`, `Enterprise = 2`
3.  **Create `PlayerPosition.cs`:**
    *   Namespace: `Diquis.Domain.Enums`
    *   Values: `Goalkeeper = 0`, `Defender = 1`, `Midfielder = 2`, `Forward = 3`
4.  **Create `Gender.cs`:**
    *   Namespace: `Diquis.Domain.Enums`
    *   Values: `Male = 0`, `Female = 1`, `NonBinary = 2`
5.  **Create `ResourceStatus.cs`:**
    *   Namespace: `Diquis.Domain.Enums`
    *   Values: `Available = 0`, `OutOfService = 1`
6.  **Create `InjuryStatus.cs`:**
    *   Namespace: `Diquis.Domain.Enums`
    *   Values: `Active = 0`, `Closed = 1`
7.  **Configure Serialization:**
    *   Open `Diquis.WebApi/Program.cs`.
    *   Locate the `AddControllers()` call.
    *   Add `.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));` to ensure enums are serialized as strings.

# Acceptance Criteria
- [ ] All 5 enum files exist in `Diquis.Domain/Enums` with correct namespaces and values.
- [ ] `Program.cs` is configured to serialize enums as strings.
- [ ] Solution builds successfully.
