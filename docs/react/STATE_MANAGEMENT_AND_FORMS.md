# React UI - State, Data, and Forms

This document outlines the primary libraries and patterns used for managing state, handling data tables, and building forms within the React application.

## State Management with MobX

**Location**: `src/store/`

The application uses [MobX](https://mobx.js.org/) for global and local state management. MobX allows you to manage application state in a simple, scalable, and efficient way by making state observable and automatically updating the UI when it changes.

-   **Stores**: State is organized into "stores," which are classes that hold related state and actions. For example, a `userStore` might manage the current user's information and authentication status.
-   **Observables**: State properties within stores are marked as `observable`.
-   **Actions**: Methods that modify the state are marked as `action`.
-   **Observers**: React components that use observable state are wrapped in the `observer` higher-order component, which ensures they re-render automatically when the data they depend on changes.

## Data Tables with Tanstack Table

For displaying complex data grids, the application uses [Tanstack Table v8](https://tanstack.com/table/v8). It is a headless (logic-only) utility that provides all the hooks and logic needed to build powerful and fully-featured data tables.

**Key Features**:
-   **Client-side and Server-side Operations**: Supports both client-side and server-side pagination, sorting, and filtering.
-   **Customizable Rendering**: Because it's headless, you have full control over the markup and styling of the table.
-   **Rich Functionality**: Provides everything you need to build interactive data tables for complex data.

## Forms and Validation

The application uses a combination of [Formik](https://formik.org/) and [Yup](https://github.com/jquense/yup) for building and validating forms.

### Formik

Formik is a library that helps manage form state, handle submissions, and streamline form creation. It provides hooks and components to reduce the boilerplate code typically associated with forms in React.

-   **`useFormik` hook**: A custom hook for managing form state, validation, and submission.
-   **`<Formik />` component**: A component-based approach to form management.
-   **Field Components**: `<Field>`, `<Form>`, and `<ErrorMessage>` components to quickly build form inputs.

### Yup

Yup is a JavaScript schema builder for value parsing and validation. It is used to define a validation schema for your forms, which Formik then uses to validate the form fields.

**Example Usage**:
```javascript
import { useFormik } from 'formik';
import * as Yup from 'yup';

const validationSchema = Yup.object({
  name: Yup.string().required('Name is required'),
  email: Yup.string().email('Invalid email address').required('Email is required'),
});

const MyForm = () => {
  const formik = useFormik({
    initialValues: { name: '', email: '' },
    validationSchema: validationSchema,
    onSubmit: (values) => {
      // Handle form submission
    },
  });

  // Form JSX using formik properties
  return (
    <form onSubmit={formik.handleSubmit}>
      {/* Form fields */}
    </form>
  );
};
```
This combination provides a robust and scalable solution for handling forms and validation in the application.
