# Navigation Quick Reference

This document provides a quick overview of the Diquis dashboard navigation structure.

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

| Feature Group        | Status          | Phase     |
| -------------------- | --------------- | --------- |
| Dashboard            | ✅ Implemented | Phase 0   |
| Core Management      | ✅ Implemented | Phases 1-5|
| Events & Calendar    | 🚧 Planned      | Phase 12  |
| Resources & Assets   | 🚧 Planned      | Phase 9   |
| Health & Medical     | 🚧 Planned      | Phase 12  |
| Communications       | 🚧 Planned      | Phase 11  |
| Analytics & Reports  | 🚧 Planned      | Phase 10  |
| Administration       | ✅ Implemented | Phases 0-8|

## Icon Reference

This project uses Tabler Icons.

| Feature              | Icon |
| -------------------- | ---- |
| Dashboard            | 📊   |
| Players              | 👥   |
| Teams                | 🏃   |
| Training             | 📅   |
| Competitions         | 🏆   |
| Calendar             | 📆   |
| Events               | 🎉   |
| Tournaments          | 🏅   |
| Equipment            | 📦   |
| Facilities           | 🏟️   |
| Inventory            | 📋   |
| Medical              | 💊   |
| Injuries             | 🩹   |
| Messages             | 💬   |
| Notifications        | 🔔   |
| Parent Portal        | 👨‍👩‍👧 |
| Analytics            | 📊   |
| Player Analytics     | 📈   |
| Financial            | 💰   |
| Reports              | 📄   |
| Users                | 🛡️   |
| Academy              | 🏢   |
| Resources            | 🗄️   |
| Integrations         | 🔌   |
| Settings             | ⚙️   |

## Role-Based Access (Planned)

-   **Admin**: Full access to all features.
-   **Coach**: Access to Core Management, Events, Health, Communications, and Analytics.
-   **Parent**: Access to Parent Portal, Messages, Calendar, and analytics for their own child.
-   **Player**: Limited access to their own profile, schedule, and messages.

## API Routes (Conceptual)

-   `/api/v1/dashboard`
-   `/api/v1/players`
-   `/api/v1/teams`
-   `/api/v1/trainings`
-   `/api/v1/events`
-   `/api/v1/assets`
-   `/api/v1/analytics`
-   `/api/v1/admin/users`
-   `/api/v1/admin/settings`

---

*For detailed feature descriptions, see [NAVIGATION_STRUCTURE.md](./NAVIGATION_STRUCTURE.md)*
