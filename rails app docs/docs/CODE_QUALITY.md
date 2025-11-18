# Code Quality and Linting

This document outlines the code quality standards and linting setup for the Diquis Football Academy project.

## 🎯 Quality Standards

We enforce strict code quality standards through multiple layers:

- **RuboCop**: Ruby style guide enforcement
- **Security Audits**: Vulnerability scanning
- **Debug Statement Prevention**: No debug code in commits
- **Automated Checks**: Pre-commit and pre-push hooks

## 🔧 Tools and Setup

### Automatic Enforcement

Quality checks are automatically enforced at multiple stages:

1. **VS Code Integration**: Auto-format on save
2. **Pre-commit Hooks**: Block commits with issues
3. **Pre-push Hooks**: Comprehensive checks before push
4. **GitHub Actions**: CI pipeline validation

### Manual Tools

```bash
# Quality control script (recommended)
bin/quality check          # Run all checks
bin/quality fix            # Auto-fix issues
bin/quality pre-commit     # Pre-commit checks
bin/quality pre-push       # Pre-push checks

# Direct RuboCop usage
bin/rubocop               # Check all files
bin/rubocop -A            # Auto-fix all issues
bin/rubocop app/          # Check specific directory
bin/rubocop file.rb       # Check specific file

# Rake tasks
rake quality:all          # All quality checks
rake quality:rubocop      # Just RuboCop
rake quality:rubocop_fix  # Auto-fix RuboCop
rake quality:security     # Security audit
rake quality:pre_commit   # Pre-commit checks
rake quality:pre_push     # Pre-push checks
```

## 🚫 Blocked Actions

The following will prevent commits/pushes:

### Pre-commit Blocks

- RuboCop violations in staged files
- Debug statements (`binding.pry`, `debugger`, `console.log`)
- Files larger than 1MB without warning

### Pre-push Blocks

- Any RuboCop violations in entire codebase
- Security vulnerabilities (if bundle-audit installed)
- Direct pushes to main branch (with confirmation)

## 🛠️ IDE Integration

### VS Code Settings

The project includes VS Code settings (`.vscode/settings.json`) that:

- Auto-format Ruby files on save using RuboCop
- Run RuboCop linting in real-time
- Show inline error indicators
- Auto-fix common issues on save

### Recommended Extensions

- **Ruby LSP** (Shopify.ruby-lsp): Language server
- **RuboCop** (misogi.ruby-rubocop): Linting integration
- **GitLens** (eamodio.gitlens): Git integration

## 🔄 Workflow

### Daily Development

1. **Write code** - VS Code auto-formats on save
2. **Stage changes** - `git add .`
3. **Commit** - Pre-commit hook runs automatically
4. **Push** - Pre-push hook runs comprehensive checks

### Fixing Issues

```bash
# Auto-fix most issues
bin/quality fix

# Check what needs manual fixes
bin/quality check

# Fix specific issues manually
bin/rubocop --only Layout/LineLength app/models/user.rb
```

### Emergency Bypass

If you need to bypass hooks (not recommended):

```bash
# Skip pre-commit hook
git commit --no-verify -m "Emergency fix"

# Skip pre-push hook  
git push --no-verify
```

## 📋 Quality Checklist

Before each commit, ensure:

- [ ] RuboCop passes: `bin/quality rubocop`
- [ ] No debug statements: `bin/quality debug`
- [ ] Tests pass: `bin/rails test`
- [ ] No security issues: `bin/quality security`

Before each push, additionally ensure:

- [ ] All quality checks pass: `bin/quality check`
- [ ] Documentation is updated
- [ ] Commit messages are descriptive

## 🚨 CI/CD Integration

GitHub Actions will automatically:

1. **Lint Check**: Run RuboCop with GitHub formatting
2. **Security Scan**: Check for vulnerabilities with Brakeman
3. **Debug Check**: Scan for debug statements
4. **Test Suite**: Run full test suite
5. **Build Check**: Ensure application builds correctly

## 📚 Resources

- [RuboCop Documentation](https://rubocop.org/)
- [Ruby Style Guide](https://github.com/rubocop/ruby-style-guide)
- [Brakeman Security Scanner](https://brakemanscanner.org/)
- [Bundle Audit](https://github.com/rubysec/bundler-audit)

## 🤝 Contributing

When contributing:

1. Ensure all quality checks pass locally
2. Use descriptive commit messages
3. Keep commits focused and atomic
4. Update documentation as needed
5. Add tests for new features

The automated quality checks help maintain consistent code quality across all contributors.
