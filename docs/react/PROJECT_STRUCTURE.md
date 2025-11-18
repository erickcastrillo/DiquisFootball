# React UI - Project Structure

This document provides an overview of the React UI application's structure, including key files and directories. The project is built using [Vite](https://vitejs.dev/) for a fast and modern development experience.

## Top-Level Files

-   **`package.json`**: Defines the project's dependencies, scripts (e.g., `npm run serve`, `npm run build`), and other metadata.
-   **`vite.config.ts`**: The configuration file for Vite. It includes settings for the development server, build process, and the API proxy that forwards requests to the ASP.NET Core backend.
-   **`tsconfig.json`**: The TypeScript configuration file, defining the compiler options and rules for the project.
-   **`index.html`**: The main entry point for the application. Vite injects the bundled JavaScript into this file.
-   **`.gitignore`**: Specifies files and folders that should be ignored by Git (e.g., `node_modules`, `dist`).
-   **`src/`**: The main directory containing all the application's source code.

## The `src` Directory

The `src` directory is where the core application logic resides. It is organized as follows:

-   **`main.tsx`**: The main entry point of the React application, where the root component is rendered into the DOM.
-   **`App.tsx`**: The root component of the application. It typically sets up the router and global providers.

### Core Application Structure

-   **`components/`**: Contains reusable UI components that are shared across the application (e.g., buttons, modals, form inputs).
-   **`layout/`**: Holds the main layout components of the application, such as the main navigation, sidebar, header, and footer. These components define the overall structure of the user interface.
-   **`pages/`**: Contains the top-level components for each route in the application. Each file in this directory typically corresponds to a specific URL path.
-   **`routes/`**: Defines the application's routing using `react-router-dom`. It maps URL paths to their corresponding page components.
-   **`store/`**: Contains the state management logic for the application, using [MobX](https://mobx.js.org/). Stores are used to manage global application state, such as user authentication, tenant information, and shared data.
-   **`services/`**: Holds modules responsible for making API calls to the backend. These services abstract the data fetching logic from the UI components.
-   **`hooks/`**: Contains custom React hooks that encapsulate reusable logic.
-   **`utils/`**: A directory for miscellaneous utility functions and helpers.
-   **`assets/`**: Contains static assets like images, fonts, and global CSS files.
