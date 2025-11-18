# Dashboard Layout - Visual Preview

This document shows how the updated navigation appears in the FlyonUI sidebar.

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

On mobile devices (< 768px), the sidebar collapses:

```
┌─────────────────────────────────────────┐
│  ☰  🔍 Search...  🌙 🌐 📊 🔔 👤      │  ← Header
├─────────────────────────────────────────┤
│                                         │
│         Main Content Area               │
│                                         │
└─────────────────────────────────────────┘

When menu is opened (☰ clicked):

┌───────────────┐
│  ✕            │  ← Close button
│               │
│  📊 Dashboard │
│               │
│  CORE MGMT    │
│  👥 Players   │
│  🏃 Teams     │
│  📅 Training  │
│  🏆 Comps     │
│               │
│  [etc...]     │
└───────────────┘
```

## Color Scheme

### Active/Selected Item

- Background: `bg-primary/10` (primary color at 10% opacity)
- Text: `text-primary` (primary color)
- Font: Semi-bold

### Hover State

- Background: `bg-base-200` (base color variant)
- Transition: Smooth 150ms

### Section Headers

- Text: `text-base-content/50` (50% opacity)
- Font: Extra small, semi-bold, uppercase
- Spacing: 16px top padding, 8px bottom padding

### Default Items

- Text: `text-base-content`
- Icon size: 20px (size-5)
- Font: Regular, 14px (text-sm)

## Responsive Behavior

| Screen Size | Behavior |
|-------------|----------|
| `< 768px` (mobile) | Sidebar hidden, accessible via hamburger menu |
| `≥ 768px` (tablet) | Sidebar visible, can be toggled |
| `≥ 1024px` (desktop) | Sidebar always visible |

## Accessibility Features

### Keyboard Navigation

- `Tab` - Navigate through items
- `Enter/Space` - Activate item
- `Escape` - Close sidebar (on mobile)

### Screen Reader Support

- ARIA labels on all navigation items
- Semantic HTML structure
- Role attributes for navigation

### Focus Indicators

- Visible focus ring on keyboard navigation
- High contrast mode support
- Reduced motion support

## State Management

### Active State

The current page is indicated by:

1. Background color (primary/10)
2. Text color (primary)
3. Bold font weight

### Badges/Counters (Future)

Some items may show counters:

```
💬 Messages                          (3)
🔔 Notifications                     (12)
```

### Loading States

When fetching data:

```
📊 Loading...
```

### Disabled States (Future)

For features not available in current plan:

```
💰 Financial Reports          🔒 Pro
```

## Internationalization

All labels support i18n with these locale files:

- `/config/locales/en.yml` - English
- `/config/locales/es.yml` - Spanish

### Adding New Languages

1. Create new locale file: `/config/locales/{locale}.yml`
2. Add translations under `app.layout.sidebar.*`
3. Add locale to available locales list
4. Update language dropdown in header

Example for French:

```yaml
# config/locales/fr.yml
fr:
  locale_name: "Français"
  app:
    layout:
      sidebar:
        core_management: "Gestion Principale"
        players: "Joueurs"
        teams: "Équipes"
        # ... etc
```

## Navigation Customization (Future)

### Role-Based Navigation

Show/hide menu items based on user role:

**Admin** - See all items
**Coach** - Hide some admin features
**Parent** - Only portal, messages, calendar
**Player** - Limited view access

### Collapsible Sections

Allow users to collapse/expand sections:

```
▼ CORE MANAGEMENT
  👥 Players
  🏃 Teams
  📅 Training Sessions
  🏆 Competitions

▶ EVENTS & CALENDAR

▶ RESOURCES & ASSETS
```

### Favorites/Shortcuts

Pin frequently used features at the top:

```
⭐ FAVORITES
  📅 Training Sessions
  👥 Players
  📊 Analytics Dashboard

──────────────────

📊 Dashboard
...
```

## Integration Points

### Search Integration

The search bar (`CTRL+K`) can quickly navigate to any section:

```
🔍 Type to search...

Results:
  👥 Players
  📅 Training Sessions
  💊 Medical Records
```

### Notifications Integration

Badge counts from real-time notification system:

```
💬 Messages                          (3)  ← Unread count
🔔 Notifications                     (7)  ← Unread count
```

### Theme Integration

Navigation adapts to light/dark theme automatically via TailwindCSS:

- Light theme: Lighter backgrounds, darker text
- Dark theme: Darker backgrounds, lighter text

---

## Implementation Files

### Main Files Modified

1. `/app/frontend/components/layout/FlyonUI/FlyonUISidebar.tsx`
   - Updated navigation structure
   - Added new menu items
   - Organized into logical groups

2. `/config/locales/en.yml`
   - Added English translations
   - Organized by feature groups

3. `/config/locales/es.yml`
   - Added Spanish translations
   - Matched English structure

### Supporting Documentation

1. `/docs/NAVIGATION_STRUCTURE.md` - Detailed feature descriptions
2. `/docs/NAVIGATION_QUICK_REFERENCE.md` - Quick lookup reference
3. `/docs/NAVIGATION_LAYOUT_PREVIEW.md` - This file

---

*Last Updated: November 5, 2025*
*Version: 1.0.0*
