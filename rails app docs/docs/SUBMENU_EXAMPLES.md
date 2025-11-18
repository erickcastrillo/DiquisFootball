# Submenu Implementation Guide

This guide shows how to implement collapsible submenus in the FlyonUI sidebar navigation.

## Basic Submenu Structure

The sidebar now uses HTML5 `<details>` and `<summary>` elements for native collapsible functionality:

```tsx
<details className="group" open>
  <summary className="text-base-content hover:bg-base-200 flex cursor-pointer items-center justify-between rounded-lg px-3 py-2 text-sm font-medium">
    <div className="flex items-center gap-3">
      <span className="icon-[tabler--layout-grid] size-5"></span>
      Section Title
    </div>
    <span className="icon-[tabler--chevron-down] size-4 transition-transform group-open:rotate-180"></span>
  </summary>
  <div className="ml-2 mt-1 space-y-1 border-l-2 border-base-content/10 pl-4">
    {/* Submenu items go here */}
  </div>
</details>
```

## Key Features

### 1. **Chevron Animation**

The chevron icon rotates 180° when the submenu is opened:

```tsx
<span className="icon-[tabler--chevron-down] size-4 transition-transform group-open:rotate-180"></span>
```

### 2. **Visual Hierarchy**

- Left border to show parent-child relationship
- Indentation for visual grouping
- Consistent spacing

```tsx
<div className="ml-2 mt-1 space-y-1 border-l-2 border-base-content/10 pl-4">
```

### 3. **Default State**

Add `open` attribute to expand by default:

```tsx
<details className="group" open>  {/* Opens by default */}
<details className="group">       {/* Closed by default */}
```

## Visual Preview

```
┌─────────────────────────────────────────┐
│  📊 Dashboard                           │  ← Active (no submenu)
│                                         │
│  ▼ 📋 Core Management                  │  ← Open submenu
│  │  👥 Players                          │
│  │  🏃 Teams                            │
│  │  📅 Training Sessions                │
│  │  🏆 Competitions                     │
│                                         │
│  ▶ 📅 Events & Calendar                │  ← Closed submenu
│                                         │
│  ▶ 📦 Resources & Assets               │  ← Closed submenu
│                                         │
│  ▶ 💊 Health & Medical                 │  ← Closed submenu
│                                         │
│  ▶ 💬 Communications                   │  ← Closed submenu
│                                         │
│  ▶ 📊 Analytics & Reports              │  ← Closed submenu
│                                         │
│  ▶ ⚙️ Administration                   │  ← Closed submenu
└─────────────────────────────────────────┘
```

## Customization Options

### 1. **Badge/Counter in Summary**

Add notification counts to submenu headers:

```tsx
<summary className="...">
  <div className="flex items-center gap-3">
    <span className="icon-[tabler--message] size-5"></span>
    Communications
  </div>
  <div className="flex items-center gap-2">
    <span className="badge badge-primary badge-sm">5</span>
    <span className="icon-[tabler--chevron-down] size-4 transition-transform group-open:rotate-180"></span>
  </div>
</summary>
```

### 2. **Nested Submenus**

Create multi-level navigation:

```tsx
<details className="group" open>
  <summary className="...">
    Analytics & Reports
  </summary>
  <div className="ml-2 mt-1 space-y-1 border-l-2 border-base-content/10 pl-4">
    <a href="#">Analytics Dashboard</a>
    
    {/* Nested submenu */}
    <details className="group/nested">
      <summary className="...">
        <div className="flex items-center gap-3">
          <span className="icon-[tabler--chart-dots] size-5"></span>
          Player Analytics
        </div>
        <span className="icon-[tabler--chevron-down] size-3 transition-transform group-open/nested:rotate-180"></span>
      </summary>
      <div className="ml-2 mt-1 space-y-1 border-l-2 border-base-content/10 pl-4">
        <a href="#">Performance Metrics</a>
        <a href="#">Skill Progression</a>
        <a href="#">Attendance Patterns</a>
      </div>
    </details>
    
    <a href="#">Financial Reports</a>
  </div>
</details>
```

### 3. **Active State in Submenu**

Highlight the active page within a submenu:

```tsx
<div className="ml-2 mt-1 space-y-1 border-l-2 border-base-content/10 pl-4">
  <a
    href="#"
    className="bg-primary/10 text-primary flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium"
  >
    <span className="icon-[tabler--users] size-5"></span>
    Players  {/* Active */}
  </a>
  
  <a
    href="#"
    className="text-base-content hover:bg-base-200 flex items-center gap-3 rounded-lg px-3 py-2 text-sm"
  >
    <span className="icon-[tabler--users-group] size-5"></span>
    Teams  {/* Inactive */}
  </a>
</div>
```

### 4. **Disabled Items**

Show locked features (Pro plan required):

```tsx
<a
  href="#"
  className="text-base-content/40 flex items-center justify-between gap-3 rounded-lg px-3 py-2 text-sm cursor-not-allowed"
  aria-disabled="true"
>
  <div className="flex items-center gap-3">
    <span className="icon-[tabler--report-money] size-5"></span>
    Financial Reports
  </div>
  <span className="icon-[tabler--lock] size-4"></span>
</a>
```

## JavaScript State Management

For more complex interactions, you can manage state with React:

```tsx
import React, { useState } from "react";

const FlyonUISidebar: React.FC = () => {
  const [openSections, setOpenSections] = useState<string[]>(['core-management']);
  
  const toggleSection = (section: string) => {
    setOpenSections(prev => 
      prev.includes(section) 
        ? prev.filter(s => s !== section)
        : [...prev, section]
    );
  };

  return (
    <details 
      className="group" 
      open={openSections.includes('core-management')}
      onToggle={(e) => {
        const isOpen = (e.target as HTMLDetailsElement).open;
        if (isOpen && !openSections.includes('core-management')) {
          toggleSection('core-management');
        } else if (!isOpen && openSections.includes('core-management')) {
          toggleSection('core-management');
        }
      }}
    >
      {/* ... */}
    </details>
  );
};
```

## Accordion Behavior (One-at-a-Time)

Make only one section open at a time:

```tsx
const [activeSection, setActiveSection] = useState<string>('core-management');

const handleToggle = (section: string, isOpen: boolean) => {
  if (isOpen) {
    setActiveSection(section);
  } else if (activeSection === section) {
    setActiveSection('');
  }
};

// Then for each details element:
<details 
  className="group" 
  open={activeSection === 'core-management'}
  onToggle={(e) => handleToggle('core-management', (e.target as HTMLDetailsElement).open)}
>
```

## Accessibility

The current implementation includes:

- ✅ **Keyboard Navigation**: Use `Tab` to navigate, `Enter/Space` to toggle
- ✅ **Screen Readers**: Native `<details>` announces expand/collapse state
- ✅ **ARIA**: No additional ARIA needed (native semantics)
- ✅ **Focus Management**: Visible focus indicators on all interactive elements

## Performance Considerations

- ✅ **CSS-only Animation**: Chevron rotation uses CSS transforms
- ✅ **No JavaScript Required**: Basic functionality works without JS
- ✅ **Progressive Enhancement**: JavaScript adds extra features
- ✅ **Minimal Re-renders**: Only affected components update

## Browser Support

The `<details>` element is supported in:

- ✅ Chrome 12+
- ✅ Firefox 49+
- ✅ Safari 6+
- ✅ Edge 79+
- ✅ All modern mobile browsers

For older browsers, consider a polyfill or JavaScript fallback.

## Styling Variations

### Rounded Submenus

```tsx
<div className="ml-2 mt-1 space-y-1 rounded-lg bg-base-200/50 p-2">
  {/* Items */}
</div>
```

### No Border Style

```tsx
<div className="ml-6 mt-1 space-y-1">
  {/* Items - just indentation, no border */}
</div>
```

### Icon-only Summary

```tsx
<summary className="...">
  <span className="icon-[tabler--layout-grid] size-5"></span>
  <span className="icon-[tabler--chevron-down] size-4 ml-auto transition-transform group-open:rotate-180"></span>
</summary>
```

## Current Implementation

The Diquis sidebar uses collapsible submenus for:

1. **Core Management** (open by default)
   - Players, Teams, Training Sessions, Competitions

2. **Events & Calendar**
   - Calendar, Events, Tournaments

3. **Resources & Assets**
   - Equipment & Assets, Facilities, Inventory

4. **Health & Medical**
   - Medical Records, Injuries & Treatment

5. **Communications**
   - Messages, Notifications, Parent Portal

6. **Analytics & Reports**
   - Analytics Dashboard, Player Analytics, Financial Reports, Custom Reports

7. **Administration**
   - Users & Roles, Academy Settings, Shared Resources, Integrations, Settings

---

*Last Updated: November 5, 2025*
*Version: 1.0.0*
