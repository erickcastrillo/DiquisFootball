# UI Application Shell Integration

This document explains how to use the application shell that has been integrated into this project. The UI is based on modern design principles and provides a responsive layout for the application.

## Overview

The application shell provides a modern, responsive layout with:

- **Header**: A top bar with search, theme switcher, notifications, and a user profile dropdown.
- **Sidebar**: A navigation menu on the left for main application sections.
- **Search Modal**: A global search functionality, often triggered by a keyboard shortcut (e.g., `CTRL+K`).
- **Footer**: A standard footer with links and copyright information.

## File Structure (Conceptual for a React Project)

```txt
/src
├── /components
│   ├── /layout
│   │   ├── AppShell.tsx      # Main layout wrapper
│   │   ├── Header.tsx        # Top navigation bar
│   │   ├── Sidebar.tsx       # Left sidebar with menu
│   │   └── Footer.tsx        # Footer component
```

## Usage

To use the layout, you would typically wrap your page-level components with the main layout component.

**Example in a React application:**
```jsx
import React from "react";
import AppShell from "../components/layout/AppShell";

const MyPage = () => {
  return (
    <AppShell>
      {/* Your page-specific content goes here */}
      <div className="card">
        <div className="card-body">
          <h2 className="card-title">My Page Content</h2>
          <p>This content will be displayed within the main application layout.</p>
        </div>
      </div>
    </AppShell>
  );
};

export default MyPage;
```

## Features

### Header Components
- **Mobile Menu Toggle**: A hamburger menu for mobile devices to show/hide the sidebar.
- **Search Button**: Opens a global search modal.
- **Theme Switcher**: Allows users to switch between light and dark themes.
- **Notifications**: A dropdown or panel to display user notifications.
- **Profile Dropdown**: A menu for user-specific actions like viewing a profile or logging out.

### Sidebar Navigation
- A hierarchical navigation menu.
- Highlighting for the currently active page.
- Support for badges to show notification counts.
- A scrollable content area for long menus.

## Customization

### Modifying the Sidebar Menu
You would typically edit the component responsible for rendering the sidebar (e.g., `Sidebar.tsx`) and modify the navigation items.

**Example:**
```jsx
<li>
  <a href="/your-route">
    <span className="icon-class"></span>
    Your Menu Item
  </a>
</li>
```

### Active Menu Item
The active menu item is usually highlighted by applying a specific CSS class (e.g., `menu-active`) based on the current URL route.

### Icons
The layout uses an icon library (like Tabler Icons, Font Awesome, etc.) for a consistent look and feel. To use an icon, you would typically use a specific class name or component.

**Example:**
```html
<span className="icon-home"></span>
```

## Responsive Behavior

- **Mobile**: The sidebar is hidden by default and can be toggled open as an overlay.
- **Desktop**: The sidebar is always visible, and the main content area adjusts its width accordingly.

## Next Steps

1.  Integrate your application's routes into the sidebar navigation component.
2.  Connect the user profile dropdown with your authentication logic (e.g., to show the current user's name and handle logout).
3.  Implement the backend for the search functionality.
4.  Connect the notifications component to a real-time notification system.
