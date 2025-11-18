# Code Quality and Linting

This document outlines the code quality standards and linting setup for the Diquis ASP.NET Core project.

## 🎯 Quality Standards

We enforce strict code quality standards through a combination of built-in .NET tools and established best practices.

- **.NET Analyzers**: The .NET SDK includes a powerful set of code analyzers that check for quality, performance, and security issues.
- **StyleCop**: Enforces a common style convention for C# code.
- **.editorconfig**: A file in the root of the repository defines and maintains consistent coding styles between different editors and IDEs.
- **Automated Checks**: Pre-commit hooks and CI/CD pipelines can be configured to run checks automatically.

## 🔧 Tools and Setup

### Automatic Enforcement

1.  **IDE Integration**: Visual Studio, Rider, and VS Code (with the C# Dev Kit) have built-in support for `.editorconfig` and Roslyn analyzers, providing real-time feedback and auto-formatting.
2.  **Build-time Analysis**: Code analysis runs during every build, and warnings or errors will be reported in the build output.
3.  **GitHub Actions**: The CI pipeline is configured to build the solution, which includes running all analyzers.

### Manual Tools

You can run code quality checks manually from the command line.

```bash
# Run code formatters and analyzers to fix issues
dotnet format

# Verify that no formatting changes are needed (useful for CI)
dotnet format --verify-no-changes
```

## 🚫 Blocked Actions

To maintain a high-quality codebase, the following can be configured to prevent commits or break the CI build:

-   **Analyzer Errors**: Any issue configured as an `error` in the `.editorconfig` will fail the build.
-   **Formatting Inconsistencies**: The CI pipeline can be set up to fail if `dotnet format --verify-no-changes` reports that changes are needed.

## 🛠️ IDE Integration

### Visual Studio / Rider / VS Code

The project includes an `.editorconfig` file that is automatically used by most modern .NET IDEs. This ensures that:

-   Code is automatically formatted on save.
-   Style violations are shown as warnings or errors in the editor.
-   Quick-fixes are often available to correct issues automatically.

### Recommended Extensions (for VS Code)

-   **C# Dev Kit**: The official extension pack from Microsoft for C# development.
-   **.NET MAUI**: For mobile development.
-   **IntelliCode for C# Dev Kit**: AI-assisted development features.

## 🔄 Workflow

### Daily Development

1.  **Write Code**: Your IDE will provide real-time feedback based on the configured analyzers.
2.  **Format Code**: Use your IDE's format-on-save feature or run `dotnet format` manually.
3.  **Commit**: Before committing, ensure your code builds without any new analysis errors.
4.  **Push**: The CI pipeline will run all checks again to verify the changes.

### Fixing Issues

If `dotnet format` or your IDE reports issues, you can often fix them automatically.

```bash
# Auto-fix most formatting and style issues
dotnet format
```

For more complex issues reported by analyzers, your IDE will typically offer a "Quick Fix" suggestion to resolve the problem.

## 📋 Quality Checklist

Before each commit, ensure:

-   [ ] The code builds successfully without errors.
-   [ ] `dotnet format --verify-no-changes` reports no required changes.
-   [ ] All new and existing tests pass (`dotnet test`).
-   [ ] There are no new analyzer warnings (or any new warnings have been reviewed and deemed acceptable).

## 🚨 CI/CD Integration

The GitHub Actions workflow (`.github/workflows/dotnet-build.yml`) will automatically:

1.  **Build the Solution**: This implicitly runs all configured code analyzers.
2.  **Run the Test Suite**: Executes all unit and integration tests.

The build will fail if there are any analyzer issues configured as errors.

## 📚 Resources

-   [.NET Code Style Rule Options](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/)
-   [StyleCop.Analyzers GitHub Repository](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
-   [dotnet format command](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-format)
