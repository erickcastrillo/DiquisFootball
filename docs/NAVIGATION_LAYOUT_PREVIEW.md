# Dashboard Layout - Visual Preview

This document shows a visual representation of the dashboard navigation layout.

## Sidebar Navigation Preview

```
┌─────────────────────────────────────────┐
│  ┌─────────────────────────────────┐   │
│  │  🏅 Diquis                      │   │
│  │  Workspace                  ⌄   │   │
│  └─────────────────────────────────┘   │
├─────────────────────────────────────────┤
│                                         │
│  📊 Dashboard                           │  ← Active
│                                         │
│  CORE MANAGEMENT                        │
│  👥 Players                             │
│  🏃 Teams                               │
│  📅 Training Sessions                   │
│  🏆 Competitions                        │
│                                         │
│  EVENTS & CALENDAR                      │
│  📆 Calendar                            │
│  🎉 Events                              │
│  🏅 Tournaments                         │
│                                         │
│  RESOURCES & ASSETS                     │
│  📦 Equipment & Assets                  │
│  🏟️ Facilities                          │
│  📋 Inventory                           │
│                                         │
│  HEALTH & MEDICAL                       │
│  💊 Medical Records                     │
│  🩹 Injuries & Treatment                │
│                                         │
│  COMMUNICATIONS                         │
│  💬 Messages                            │
│  🔔 Notifications                       │
│  👨‍👩‍👧 Parent Portal                      │
│                                         │
│  ANALYTICS & REPORTS                    │
│  📊 Analytics Dashboard                 │
│  📈 Player Analytics                    │
│  💰 Financial Reports                   │
│  📄 Custom Reports                      │
│                                         │
│  ADMINISTRATION                         │
│  🛡️ Users & Roles                       │
│  🏢 Academy Settings                    │
│  🗄️ Shared Resources                    │
│  🔌 Integrations                        │
│  ⚙️ Settings                            │
│                                         │
├─────────────────────────────────────────┤
│  ┌───────────────────────────────────┐ │
│  │  ⬆️ Upgrade Your Plan             │ │
│  │  Trial ends in 12 days            │ │
│  │  [██████████░░░░] 60%             │ │
│  │  [ See All Plans ]                │ │
│  └───────────────────────────────────┘ │
└─────────────────────────────────────────┘
```

## Mobile Navigation Preview

On mobile devices, the sidebar collapses and is accessible via a hamburger menu icon in the header.

```
┌─────────────────────────────────────────┐
│  ☰  🔍 Search...  🌙 🌐 📊 🔔 👤      │  ← Header
├─────────────────────────────────────────┤
│                                         │
│         Main Content Area               │
│                                         │
└─────────────────────────────────────────┘
```

When the menu is opened, it appears as an overlay:

```
┌───────────────┐
│  ✕            │  ← Close button
│               │
│  📊 Dashboard │
│               │
│  CORE MGMT    │
│  👥 Players   │
│  🏃 Teams     │
│  ... etc.     │
└───────────────┘
```

## Color Scheme & Styling

-   **Active Item**: Highlighted with the application's primary color.
-   **Hover State**: A subtle background color change on hover.
-   **Section Headers**: Styled distinctly from navigation items (e.g., uppercase, different color).
-   **Icons**: Each navigation item has an associated icon for quick visual identification.

## Responsive Behavior

-   **Desktop**: Sidebar is permanently visible.
-   **Tablet**: Sidebar may be collapsed by default, with a toggle to show/hide.
-   **Mobile**: Sidebar is hidden and accessed via an overlay menu.

## Accessibility

-   **Keyboard Navigation**: Fully navigable using the `Tab` and `Enter` keys.
-   **Screen Reader Support**: Uses semantic HTML and ARIA labels for screen readers.
-   **Focus Indicators**: Clear focus styles for keyboard users.

This layout is designed to be clean, intuitive, and responsive across all devices.
