# Contributing to Diquis Football Academy Management System

Thank you for considering contributing to Diquis! This document provides guidelines and instructions
for contributing to the project.

## Table of Contents

- [Getting Started](#getting-started)
- [Development Process](#development-process)
- [Code Quality Standards](#code-quality-standards)
- [Documentation Guidelines](#documentation-guidelines)
- [Markdown Style Guide](#markdown-style-guide)
- [Submitting Changes](#submitting-changes)

## Getting Started

1. Fork the repository
2. Clone your fork locally
3. Follow the [Setup Guide](./docs/SETUP_GUIDE.md) to configure your development environment
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

### Ruby Code

- Follow Rails conventions and the [Ruby Style Guide](https://rubystyle.guide/)
- Use RuboCop for code formatting: `bundle exec rubocop -A`
- Maintain test coverage above 90%
- Run security checks: `bundle exec brakeman`

### Testing

- Write comprehensive RSpec tests for all new functionality
- Follow the existing test patterns in the codebase
- Use FactoryBot for test data creation
- Run tests before submitting: `bundle exec rspec`

## Documentation Guidelines

### General Documentation

- Keep documentation up-to-date with code changes
- Use clear, concise language
- Include code examples where appropriate
- Follow the project's documentation structure

### API Documentation

- Document all API endpoints
- Include request/response examples
- Document error cases and status codes
- Use consistent formatting

## Markdown Style Guide

We use `markdownlint-cli2` to ensure consistent markdown formatting. Our configuration allows for:

### Line Length

- **Maximum**: 120 characters per line
- **Exception**: Code blocks, tables, and headings are exempt

### Code Blocks

- Always specify language for fenced code blocks:

```ruby
# Good
```ruby
def example
  "Always specify the language"
end
```

```text
# For plain text or file structures
app/
├── controllers/
└── models/
```

### Headings

- Use ATX-style headings (`#` format)
- Surround headings with blank lines
- Use sentence case for headings

### Lists

- Surround lists with blank lines
- Use consistent indentation (2 spaces)
- Use `-` for unordered lists
- Use `1.` format for ordered lists

### Links

- Use descriptive link text
- Prefer relative paths for internal links
- Test all links before submitting

### Linting Commands

```bash
# Check markdown files
npm run lint:md

# Auto-fix markdown issues
npm run lint:md:fix

# Get summary of issues
npm run lint:md:summary
```

### Configuration Files

- `.markdownlint.json` - Main configuration
- `.markdownlintignore` - Files to ignore
- `package.json` - npm scripts for linting

## Submitting Changes

### Before Submitting

1. **Run all checks**:

   ```bash
   # Ruby code quality
   bundle exec rubocop -A
   bundle exec brakeman
   
   # Tests
   bundle exec rspec
   
   # Markdown linting
   npm run lint:md:fix
   ```

2. **Update documentation** if needed
3. **Add tests** for new functionality
4. **Update CHANGELOG.md** with your changes

### Pull Request Guidelines

1. **Title**: Use descriptive title that explains the change
2. **Description**: Include:
   - What changes were made
   - Why the changes were necessary
   - Any breaking changes
   - Screenshots if UI changes
3. **Link issues**: Reference related GitHub issues
4. **Reviewers**: Request review from maintainers

### Pull Request Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Documentation update
- [ ] Refactoring

## Testing
- [ ] All tests pass
- [ ] New tests added
- [ ] Manual testing completed

## Checklist
- [ ] Code follows project style guidelines
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] No breaking changes (or documented)
```

## Code Review Process

1. All submissions require review
2. Reviews focus on:
   - Code quality and style
   - Test coverage
   - Documentation accuracy
   - Security considerations
3. Address reviewer feedback promptly
4. Maintain a collaborative tone

## Getting Help

- **Documentation**: Check [docs/](./docs/) directory
- **Issues**: Search existing GitHub issues
- **Discussions**: Use GitHub Discussions for questions
- **Contact**: Reach out to maintainers via GitHub

## Recognition

Contributors are recognized in:

- GitHub contributor list
- Release notes for significant contributions
- PROJECT_OVERVIEW.md acknowledgments

Thank you for contributing to Diquis! 🚀⚽
