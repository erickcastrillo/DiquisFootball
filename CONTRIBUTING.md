# Contributing to Diquis Football Academy Management System

Thank you for considering contributing to Diquis! This document provides guidelines and instructions for contributing to the project.

## Table of Contents

- [Getting Started](#getting-started)
- [Development Process](#development-process)
- [Code Quality Standards](#code-quality-standards)
- [Documentation Guidelines](#documentation-guidelines)
- [Submitting Changes](#submitting-changes)

## Getting Started

1. Fork the repository.
2. Clone your fork locally.
3. Follow the [Setup Guide](./docs/aspnet/SETUP_GUIDE.md) to configure your development environment.
4. Create a feature branch: `git checkout -b feature/your-feature-name`

## Development Process

### Branch Naming Convention

- **Features**: `feature/description-of-feature`
- **Bug fixes**: `fix/description-of-fix`
- **Documentation**: `docs/description-of-change`
- **Refactoring**: `refactor/description-of-change`

### Commit Messages

Follow the conventional commit format:

```text
type(scope): brief description

Longer description if needed

- List of changes
- Another change
```

**Types**: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

## Code Quality Standards

### C# Code

- Follow the [.NET Runtime coding style](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md).
- Use the `.editorconfig` file provided in the repository for consistent formatting.
- Run `dotnet format` to ensure code style consistency.
- Maintain test coverage above 90%.

### Testing

- Write comprehensive xUnit tests for all new functionality.
- Follow the existing test patterns in the codebase (e.g., using Moq and FluentAssertions).
- Run tests before submitting: `dotnet test`

## Documentation Guidelines

### General Documentation

- Keep documentation up-to-date with code changes.
- Use clear, concise language.
- Include C# code examples where appropriate.
- Follow the project's documentation structure in the `/docs` directory.

### API Documentation

- Document all API endpoints using XML comments for Swashbuckle.
- Include request/response examples.
- Document error cases and status codes.

## Submitting Changes

### Before Submitting

1. **Run all checks**:

   ```bash
   # Format the code
   dotnet format

   # Run all tests
   dotnet test
   ```

2. **Update documentation** if needed.
3. **Add tests** for new functionality.
4. **Update CHANGELOG.md** with your changes.

### Pull Request Guidelines

1. **Title**: Use a descriptive title that explains the change.
2. **Description**: Include:
   - What changes were made.
   - Why the changes were necessary.
   - Any breaking changes.
   - Screenshots if UI changes are involved.
3. **Link issues**: Reference related GitHub issues.
4. **Reviewers**: Request a review from the maintainers.

## Code Review Process

1. All submissions require a review.
2. Reviews focus on:
   - Code quality and style
   - Test coverage
   - Documentation accuracy
   - Security considerations
3. Address reviewer feedback promptly.
4. Maintain a collaborative and respectful tone.

## Getting Help

- **Documentation**: Check the [docs/](./docs/) directory.
- **Issues**: Search existing GitHub issues.
- **Discussions**: Use GitHub Discussions for questions.
- **Contact**: Reach out to maintainers via GitHub.

Thank you for contributing to Diquis! 🚀⚽
