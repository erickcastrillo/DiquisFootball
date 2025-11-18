# Copilot Configuration Guide

This directory contains files specifically designed to improve GitHub Copilot's code suggestions for the Diquis project.

## 📁 Files Overview

### `.copilotignore`

Excludes files that shouldn't influence Copilot suggestions:

- Dependencies (`node_modules/`, `vendor/`)
- Generated files that might contain noise
- Documentation (we want specific examples, not general docs)
- Configuration files with sensitive data

### `patterns.rb`

Ruby coding patterns that demonstrate:

- ✅ **Controller structure** with Inertia.js integration
- ✅ **Service layer pattern** with explicit error handling
- ✅ **Model structure** with ActsAsTenant multi-tenancy
- ✅ **Serializer patterns** for consistent API responses
- ✅ **Policy structure** with Pundit authorization
- ✅ **Migration patterns** with UUID primary keys
- ✅ **RSpec test structure** with factories
- ✅ **Factory Bot patterns** with traits

### `patterns.tsx`

TypeScript/React patterns that demonstrate:

- ✅ **Component structure** with proper TypeScript interfaces
- ✅ **Inertia.js integration** with forms and navigation
- ✅ **Index/Show/Form component patterns**
- ✅ **Custom hooks** for data management
- ✅ **Error handling** and loading states
- ✅ **Utility functions** for common operations

## 🎯 How It Works

### For Copilot Users

When you're writing code in VS Code with Copilot enabled:

1. **Open relevant pattern files** alongside your work
2. **Copilot learns from the patterns** in the currently open files
3. **Suggestions follow project conventions** automatically
4. **Code quality improves** through consistent patterns

### Example Workflow

```bash
# 1. Open a pattern file to establish context
code .copilot/patterns.rb

# 2. Create your new file
code app/controllers/my_new_controller.rb

# 3. Start typing - Copilot will suggest project-compliant code
class MyNewController < ApplicationController
  # Copilot suggests proper structure based on patterns.rb
```

## 🛠️ Best Practices

### When Creating New Code

1. **Open the relevant pattern file** first (`.copilot/patterns.rb` for Ruby, `.copilot/patterns.tsx` for React)
2. **Let Copilot suggest** the initial structure
3. **Review suggestions** for compliance with project standards
4. **Modify as needed** to fit your specific use case

### When Working on Existing Code

1. **Keep pattern files open** in separate tabs
2. **Reference the coding standards** in comments when needed
3. **Use Copilot's suggestions** as starting points, not final solutions

## 📋 Generated Code Quality

All patterns in these files follow the project's linting rules:

### Ruby (RuboCop)

- Frozen string literals at top of files
- 2-space indentation throughout
- Method length under 15 lines
- Proper naming conventions (snake_case)
- Explicit error handling patterns

### TypeScript/React (ESLint + Prettier)

- Proper TypeScript interfaces for all props
- Consistent 2-space indentation
- Single quotes for strings, double for JSX attributes
- No unused imports or variables
- React hooks best practices

### ERB Templates

- Valid HTML5 semantic structure
- Proper form labels and accessibility
- Consistent indentation with Ruby code

## 🔄 Updating Patterns

When you discover new patterns or improve existing ones:

1. **Update the pattern files** with better examples
2. **Add comments** explaining the reasoning
3. **Test with Copilot** to ensure suggestions improve
4. **Document changes** in project documentation

## 🚀 VS Code Integration

The `.vscode/settings.json` is configured to:

- **Enable Copilot** for all relevant file types
- **Auto-format** code on save
- **Run linters** automatically
- **Provide multiple suggestions** (up to 3 inline, 10 in list)

## 🎓 Training Copilot

Copilot learns from:

1. **Open files** in your editor (most important)
2. **Recently viewed files** in the session
3. **Project structure** and naming patterns
4. **Comments and documentation** in the code

By keeping these pattern files open, you're essentially "training" Copilot to understand your project's specific conventions.

## 📊 Expected Results

With these configurations, you should see:

- ✅ **Better code suggestions** that match project style
- ✅ **Fewer linting errors** in generated code
- ✅ **Consistent patterns** across all generated files
- ✅ **Reduced manual corrections** needed
- ✅ **Faster development** with project-aware suggestions

## 🔧 Troubleshooting

### If Copilot suggestions don't match patterns

1. **Ensure pattern files are open** in VS Code
2. **Restart VS Code** to refresh Copilot context
3. **Check file associations** in VS Code settings
4. **Update patterns** if they're outdated

### If suggestions are still generic

1. **Add more specific comments** to your code
2. **Use more descriptive variable names**
3. **Include example usage** in comments
4. **Reference the pattern files** in code comments

Remember: Copilot is a tool to accelerate development, but human review and project knowledge are still essential for quality code.
