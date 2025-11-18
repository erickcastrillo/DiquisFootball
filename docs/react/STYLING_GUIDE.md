# React UI - Theme and Styling Guide

This document provides an overview of the styling and theming approach for the React application.

## Core Framework: Bootstrap 5

The UI is built on top of [Bootstrap 5](https://getbootstrap.com/). It uses the [react-bootstrap](https://react-bootstrap.github.io/) library, which provides a set of React components that encapsulate Bootstrap's markup and functionality.

This approach offers several advantages:
-   **Component-Based**: Use familiar React components like `<Button>` and `<Modal>` instead of CSS classes.
-   **Consistency**: Ensures a consistent look and feel across the application.
-   **Responsiveness**: Leverages Bootstrap's powerful responsive grid system out of the box.

## Customizing the Theme

While Bootstrap provides a solid foundation, the application's theme can be customized to match a specific brand identity.

The primary way to customize the theme is by overriding Bootstrap's Sass variables.

1.  **Sass/SCSS**: The project is configured to use Sass (`.scss` files) for styling. This allows for the use of variables, mixins, and other advanced CSS features.

2.  **Variable Overrides**: To change the default Bootstrap theme (e.g., colors, fonts, spacing), you can override the Bootstrap Sass variables. This is typically done in a dedicated file, such as `src/assets/scss/_variables.scss`.

    **Example (`_variables.scss`):**
    ```scss
    // Override the primary color
    $primary: #5E35B1;

    // Override the default font family
    $font-family-sans-serif: "Inter", -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;

    // Import Bootstrap to apply the overrides
    @import "bootstrap/scss/bootstrap";
    ```

3.  **Global Styles**: Global styles and additional custom component styles can be added in a main `index.scss` or `App.scss` file, which imports the variable overrides and the main Bootstrap library.

## Component-Specific Styles

For styles that are specific to a single component, you can create a separate `.scss` file and import it directly into your component file. To avoid style conflicts, it's a good practice to use CSS Modules or to scope your styles using a top-level class for the component.
