# Task Context
Define the `ChatSession` and `KnowledgeBaseArticle` entities. These are global entities stored in the `BaseDbContext` (not tenant-specific) to support anonymous visitor interactions.

# Core References
- **Plan:** [19 - Implementation_Plan_AISalesAgent.md](./19%20-%20Implementation_Plan_AISalesAgent.md)
- **Tech Guide:** [AI_Sales_Agent_TechnicalGuide.md](../../Technical%20documentation/AI_Sales_Agent_TechnicalGuide/AI_Sales_Agent_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Create `ChatSession.cs`:**
    *   Path: `Diquis.Domain/Entities/ChatSession.cs`.
    *   Inherit from `BaseEntity`. **Do NOT** implement `IMustHaveTenant`.
    *   Properties: `VisitorId`, `DetectedLanguage`, `Transcript` (jsonb), `Status` (Enum), `CreatedAt`.
2.  **Create `KnowledgeBaseArticle.cs`:**
    *   Path: `Diquis.Domain/Entities/KnowledgeBaseArticle.cs`.
    *   Inherit from `BaseEntity`. **Do NOT** implement `IMustHaveTenant`.
    *   Properties: `Title`, `Content`, `SourceUrl`, `IsEnabled`.
3.  **Configure Context:**
    *   Add `DbSet<ChatSession>` and `DbSet<KnowledgeBaseArticle>` to **`BaseDbContext`**.
    *   Create migration for `BaseDbContext`.

# Acceptance Criteria
- [ ] Entities created in `Diquis.Domain`.
- [ ] `BaseDbContext` updated with new DbSets.
- [ ] Migration created.
