# Submenu Implementation Guide

This guide shows how to implement collapsible submenus in a modern web application, using HTML and CSS, with optional JavaScript for advanced functionality.

## Basic Submenu Structure

The recommended approach uses the native HTML `<details>` and `<summary>` elements for collapsible sections, which is accessible and requires no JavaScript for basic functionality.

```html
<details class="group" open>
  <summary class="submenu-header">
    <div class="submenu-title">
      <span class="icon"></span>
      Section Title
    </div>
    <span class="chevron"></span>
  </summary>
  <div class="submenu-items">
    <!-- Submenu items go here -->
    <a href="/link1">Item 1</a>
    <a href="/link2">Item 2</a>
  </div>
</details>
```

### Key Features

1.  **Chevron Animation**: The chevron icon can be animated to rotate when the submenu is opened using CSS.
    ```css
    .group[open] .chevron {
      transform: rotate(180deg);
    }
    .chevron {
      transition: transform 0.2s;
    }
    ```

2.  **Visual Hierarchy**: Use indentation and borders to create a clear visual hierarchy.
    ```css
    .submenu-items {
      margin-left: 0.5rem;
      padding-left: 1rem;
      border-left: 2px solid #e5e7eb; /* light gray */
    }
    ```

3.  **Default State**: Add the `open` attribute to the `<details>` element to have it expanded by default.

## Visual Preview

```
┌─────────────────────────────────────────┐
│  📊 Dashboard                           │
│                                         │
│  ▼ 📋 Core Management                  │  <-- Open submenu
│  │  👥 Players                          │
│  │  🏃 Teams                            │
│                                         │
│  ▶ 📅 Events & Calendar                │  <-- Closed submenu
│                                         │
│  ... etc.                               │
└─────────────────────────────────────────┘
```

## Customization Options

-   **Badges/Counters**: Add notification counts to the submenu headers.
-   **Nested Submenus**: Place a `<details>` element inside another `submenu-items` div to create multi-level navigation.
-   **Active State**: Apply a specific CSS class (e.g., `active`) to the current page's link to highlight it.
-   **Disabled Items**: Use CSS to style links for features that are not available to the user (e.g., `cursor: not-allowed`, reduced opacity).

## JavaScript State Management (for advanced behavior)

For more complex interactions, such as accordion-style menus (where only one section can be open at a time), you can use JavaScript.

**Example with React:**
```jsx
import React, { useState } from "react";

const Sidebar = () => {
  const [activeSection, setActiveSection] = useState('core-management');

  const handleToggle = (section) => {
    setActiveSection(activeSection === section ? null : section);
  };

  return (
    <nav>
      <details open={activeSection === 'core-management'} onToggle={() => handleToggle('core-management')}>
        <summary>Core Management</summary>
        {/* ... items ... */}
      </details>
      <details open={activeSection === 'events'} onToggle={() => handleToggle('events')}>
        <summary>Events & Calendar</summary>
        {/* ... items ... */}
      </details>
    </nav>
  );
};
```

## Accessibility

Using the native `<details>` element provides excellent accessibility out of the box:
-   **Keyboard Navigation**: Users can navigate with `Tab` and toggle sections with `Enter` or `Space`.
-   **Screen Readers**: Correctly announce the expanded/collapsed state of the submenus.
-   **No ARIA needed**: The browser handles the necessary ARIA attributes automatically.

## Browser Support

The `<details>` element is supported by all modern browsers, making it a reliable choice.
