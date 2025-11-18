# Navigation Quick Reference

Quick overview of the Diquis dashboard navigation structure.

## Navigation Menu Structure

```
📊 Dashboard
├─ 🏠 Overview

📋 Core Management
├─ 👥 Players
├─ 🏃 Teams
├─ 📅 Training Sessions
└─ 🏆 Competitions

📅 Events & Calendar
├─ 📆 Calendar
├─ 🎉 Events
└─ 🏅 Tournaments

📦 Resources & Assets
├─ 📦 Equipment & Assets
├─ 🏟️ Facilities
└─ 📋 Inventory

💊 Health & Medical
├─ 💊 Medical Records
└─ 🩹 Injuries & Treatment

💬 Communications
├─ 💬 Messages
├─ 🔔 Notifications
└─ 👨‍👩‍👧 Parent Portal

📊 Analytics & Reports
├─ 📊 Analytics Dashboard
├─ 📈 Player Analytics
├─ 💰 Financial Reports
└─ 📄 Custom Reports

⚙️ Administration
├─ 🛡️ Users & Roles
├─ 🏢 Academy Settings
├─ 🗄️ Shared Resources
├─ 🔌 Integrations
└─ ⚙️ Settings
```

## Feature to Implementation Phase Mapping

| Feature Group | Status | Phase |
|--------------|---------|-------|
| Dashboard | ✅ Implemented | Phase 0 |
| Core Management | ✅ Implemented | Phases 1-5 |
| Events & Calendar | 🚧 Planned | Phase 12 |
| Resources & Assets | 🚧 Planned | Phase 9 |
| Health & Medical | 🚧 Planned | Phase 12 |
| Communications | 🚧 Planned | Phase 11 |
| Analytics & Reports | 🚧 Planned | Phase 10 |
| Administration | ✅ Implemented | Phases 0-8 |

## Translation Keys

All navigation items use the following i18n pattern:

```yaml
app.layout.sidebar.{key}
```

### Available Keys

**Sections:**

- `core_management`
- `events_calendar`
- `resources_assets`
- `health_medical`
- `communications`
- `analytics_reports`
- `administration`

**Menu Items:**

- `dashboard`
- `players`, `teams`, `training`, `competitions`
- `calendar`, `events`, `tournaments`
- `equipment`, `facilities`, `inventory`
- `medical_records`, `injuries`
- `messages`, `notifications_menu`, `parent_portal`
- `analytics`, `player_analytics`, `financial_reports`, `custom_reports`
- `users_roles`, `academy_settings`, `shared_resources`, `integrations`, `settings`

## Icon Reference

| Feature | Icon | Tabler Icon Class |
|---------|------|-------------------|
| Dashboard | 📊 | `icon-[tabler--dashboard]` |
| Players | 👥 | `icon-[tabler--users]` |
| Teams | 🏃 | `icon-[tabler--users-group]` |
| Training | 📅 | `icon-[tabler--calendar-event]` |
| Competitions | 🏆 | `icon-[tabler--trophy]` |
| Calendar | 📆 | `icon-[tabler--calendar]` |
| Events | 🎉 | `icon-[tabler--confetti]` |
| Tournaments | 🏅 | `icon-[tabler--tournament]` |
| Equipment | 📦 | `icon-[tabler--package]` |
| Facilities | 🏟️ | `icon-[tabler--building-stadium]` |
| Inventory | 📋 | `icon-[tabler--box]` |
| Medical | 💊 | `icon-[tabler--heart-rate-monitor]` |
| Injuries | 🩹 | `icon-[tabler--bandage]` |
| Messages | 💬 | `icon-[tabler--message]` |
| Notifications | 🔔 | `icon-[tabler--bell]` |
| Parent Portal | 👨‍👩‍👧 | `icon-[tabler--user-heart]` |
| Analytics | 📊 | `icon-[tabler--chart-line]` |
| Player Analytics | 📈 | `icon-[tabler--chart-dots]` |
| Financial | 💰 | `icon-[tabler--report-money]` |
| Reports | 📄 | `icon-[tabler--file-report]` |
| Users | 🛡️ | `icon-[tabler--user-shield]` |
| Academy | 🏢 | `icon-[tabler--building]` |
| Resources | 🗄️ | `icon-[tabler--database]` |
| Integrations | 🔌 | `icon-[tabler--plug]` |
| Settings | ⚙️ | `icon-[tabler--settings]` |

## Role-Based Access (Planned)

### Admin

- Full access to all features

### Coach

- Core Management (all)
- Events & Calendar (view/edit)
- Health & Medical (view/limited edit)
- Communications (send messages)
- Analytics & Reports (view)

### Parent

- Parent Portal (full access)
- Messages (view/reply)
- Calendar (view only)
- Player Analytics (view own child only)

### Player

- Calendar (view only)
- Messages (view/reply)
- Player Analytics (view own data only)

## Routes (Planned)

```
/dashboard
/players
/players/:id
/teams
/teams/:id
/training
/training/:id
/competitions
/competitions/:id
/calendar
/events
/events/:id
/tournaments
/tournaments/:id
/equipment
/facilities
/inventory
/medical
/medical/:player_id
/injuries
/messages
/notifications
/parent-portal
/analytics
/analytics/players
/analytics/financial
/reports
/users
/academy/settings
/resources
/integrations
/settings
```

---

*For detailed feature descriptions, see [NAVIGATION_STRUCTURE.md](./NAVIGATION_STRUCTURE.md)*
