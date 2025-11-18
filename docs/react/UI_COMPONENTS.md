# React UI - Pages, Layouts, and Components

The React application follows a component-based architecture that promotes reusability and separation of concerns. The UI is organized into three main categories: Pages, Layouts, and Components.

## Pages

**Location**: `src/pages/`

Pages are top-level components that correspond to a specific route or URL. Each page is responsible for fetching its required data (usually by calling functions from the `services` directory) and arranging the layout of the content.

For example, a `PlayerListPage.tsx` file would contain the logic and UI for displaying a list of players, including the data table, filters, and action buttons.

## Layouts

**Location**: `src/layout/`

Layout components define the overall structure and chrome of the application. They are responsible for elements that are present across multiple pages, such as:

-   **Main Navigation / Sidebar**: The primary navigation menu.
-   **Header**: The top bar, which might contain user information, notifications, and a search bar.
-   **Footer**: The bottom section of the application.
-   **`PageLayout`**: A wrapper component that pages use to ensure a consistent content structure, including page titles and breadcrumbs.

The main application layout is typically defined in `App.tsx` or a dedicated layout file, wrapping the router's output.

## Components

**Location**: `src/components/`

Components are the smallest, reusable building blocks of the UI. They are designed to be self-contained and independent. The project includes a set of pre-built components, often organized into subdirectories:

-   **Base UI Components**: These are fundamental building blocks styled with Bootstrap 5, such as:
    -   `Button`
    -   `Modal`
    -   `Card`
    -   `Input`
    -   `Select`
-   **Extended UI Components**: These are more complex components that may integrate third-party libraries, such as:
    -   Data Tables (`Tanstack Table`)
    -   Date Pickers
    -   Rich Text Editors
-   **Domain-Specific Components**: As the application grows, you might create components that are specific to a certain feature, like a `PlayerCard` or `TeamSelector`. These can be organized in sub-folders within `components`.
