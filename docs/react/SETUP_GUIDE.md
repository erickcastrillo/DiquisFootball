# React UI - Setup Guide

This guide provides instructions for setting up and running the React UI application.

## Prerequisites

- **Node.js and NPM**: The React project requires a recent version of Node.js and the Node Package Manager (NPM). You can download them from [nodejs.org](https://nodejs.org/).
- **IDE**: A code editor like [Visual Studio Code](https://code.visualstudio.com/) is recommended for front-end development.

## Installation

1.  **Navigate to the UI Project**: The React project is located in a separate folder, typically named `Diquis.Web` or similar, at the same level as your .NET solution file.

2.  **Install Dependencies**: Open a terminal in the React project's root directory and run the following command to install the necessary NPM packages defined in `package.json`:

    ```bash
    npm install
    ```

## Running the Application

### Development Mode

To run the React application in development mode with hot-reloading, use the following command. This will start a local development server (usually on `http://localhost:3000`) that proxies API requests to your running .NET backend.

```bash
npm run serve
```

The API proxy is configured in `vite.config.ts`.

### Production Build

To build the application for production, run:

```bash
npm run build
```

This command creates an optimized, static build of the application in the `dist` directory.

## Production Integration

For production, the contents of the `dist` folder must be moved into the `wwwroot` directory of the main `Diquis.WebApi` project. The ASP.NET Core application is configured to serve these static files as the front-end. This process is typically automated in a CI/CD pipeline.
