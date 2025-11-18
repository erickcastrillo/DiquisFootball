# FlyonUI Application Shell Integration

This document explains how to use the FlyonUI application shell that has been integrated into this project.

## Overview

The FlyonUI application shell provides a modern, responsive layout with:

- **Header** with search, theme switcher, language selector, notifications, and profile dropdown
- **Sidebar** with navigation menu and user profile
- **Search Modal** for global search functionality
- **Activity Drawer** for recent activity tracking
- **Footer** with links and copyright

## File Structure

```txt
app/frontend/
├── components/layout/
│   ├── FlyonUILayout.tsx          # Main layout wrapper
│   └── FlyonUI/
│       ├── FlyonUIHeader.tsx      # Top navigation bar
│       ├── FlyonUISidebar.tsx     # Left sidebar with menu
│       ├── FlyonUIFooter.tsx      # Footer component
│       ├── SearchModal.tsx        # Global search modal
│       └── ActivityDrawer.tsx     # Activity notifications drawer
└── stylesheets/
    └── user.scss                  # Custom styles for menu-active state
```

## Usage

### 1. Using FlyonUI Layout in a Page

Import the `FlyonUILayout` component and wrap your page content with it:

```tsx
import React from "react";
import FlyonUILayout from "../components/layout/FlyonUILayout";

const MyPage: React.FC = () => {
  return (
    <FlyonUILayout>
      {/* Your page content goes here */}
      <div className="grid grid-cols-1 gap-6">
        <div className="card">
          <div className="card-body">
            <h2 className="card-title">My Content</h2>
            <p>This content will be displayed within the FlyonUI layout.</p>
          </div>
        </div>
      </div>
    </FlyonUILayout>
  );
};

export default MyPage;
```

### 2. Example Page

See `app/frontend/pages/FlyonUIExample.tsx` for a complete working example.

## Features

### Header Components

- **Mobile Menu Toggle**: Hamburger menu for mobile devices
- **Search Button**: Opens global search modal (CTRL + K)
- **Theme Dropdown**: Switch between Light, Dark, and System themes
- **Language Dropdown**: Select from multiple languages
- **Activity Button**: Opens activity drawer
- **Notifications**: Dropdown with notification tabs (Inbox, General)
- **Profile Dropdown**: User menu with account options and logout

### Sidebar Navigation

The sidebar includes:

- User profile section with avatar and social links
- Hierarchical navigation menu
- Active state highlighting for current page
- Badge support for notifications/counts
- Scrollable content area
- Brand logo at bottom

### Interactive Components

All interactive components use FlyonUI's JavaScript:

- Dropdowns auto-close appropriately
- Modals and drawers with overlay effects
- Tabs for content organization
- Theme switching with localStorage persistence

## Customization

### Modifying the Sidebar Menu

Edit `app/frontend/components/layout/FlyonUI/FlyonUISidebar.tsx`:

```tsx
<li>
  <a href="/your-route" className="px-2">
    <span className="icon-[tabler--home] size-4.5"></span>
    Your Menu Item
  </a>
</li>
```

### Active Menu Item

Add the `menu-active` class to highlight the current page:

```tsx
<li>
  <a href="#" className="menu-active px-2">
    <span className="icon-[tabler--chart-pie-2] size-4.5"></span>
    Current Page
  </a>
</li>
```

### Adding Badges

```tsx
<li>
  <a href="#" className="px-2">
    <span className="icon-[tabler--dashboard] size-4.5"></span>
    <span className="grow">Dashboard</span>
    <span className="badge badge-sm badge-primary rounded-full">2</span>
  </a>
</li>
```

### Customizing User Profile

Edit the profile section in `FlyonUISidebar.tsx`:

```tsx
<div className="text-center">
  <h3 className="text-base-content text-lg font-semibold">Your Name</h3>
  <p className="text-base-content/80">your@email.com</p>
</div>
```

## Icons

The layout uses Tabler icons via Iconify. Available icons can be found at:
https://icon-sets.iconify.design/tabler/

To use an icon:

```tsx
<span className="icon-[tabler--home] size-4.5"></span>
```

Common icon sizes:

- `size-4` (16px) - Small icons
- `size-4.5` (18px) - Default for menu items
- `size-5` (20px) - Medium icons
- `size-6` (24px) - Large icons

## Theme Support

The layout supports FlyonUI themes:

- Light (default)
- Dark
- Black
- Luxury

Custom CSS in `user.scss` ensures the active menu state works correctly across all themes.

## Responsive Behavior

- **Mobile (< 1024px)**: Sidebar hidden by default, shown via overlay when menu button clicked
- **Desktop (≥ 1024px)**: Sidebar always visible, content area adjusts with `lg:ps-75` padding

## Next Steps

1. Replace placeholder content in `FlyonUIExample.tsx` with your actual page
2. Update user profile information in the sidebar
3. Customize navigation menu items to match your routes
4. Add your logo/branding to replace the default FlyonUI logo
5. Implement search functionality in `SearchModal.tsx`
6. Connect activity data to `ActivityDrawer.tsx`

## Notes

- All FlyonUI components are automatically initialized via the setup in `inertia.ts`
- The layout uses Tailwind CSS utility classes extensively
- Interactive components require FlyonUI's JavaScript (already imported)
- Icons are loaded on-demand from Iconify CDN
