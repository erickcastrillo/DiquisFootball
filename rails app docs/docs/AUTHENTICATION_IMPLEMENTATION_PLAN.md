# Authentication & Authorization Implementation Plan

## 🎯 Project: Diquis Football Academy Management System

**Branch**: `feature/authentication-authorization`  
**Created**: November 5, 2025

---

## 📋 Overview

This document outlines the implementation plan for adding comprehensive authentication and authorization to the Diquis system.

### Roles Hierarchy

```txt
Level 6: Super Admin     - System-wide access
Level 5: Academy Owner   - Full academy control
Level 4: Academy Admin   - Operations management
Level 3: Coach           - Team & player management
Level 2: Staff           - Administrative support
Level 1: Parent          - Child data access
Level 0: Player          - Self-service only
```

### Tech Stack

- **Authentication**: Devise
- **Authorization**: Pundit
- **UI Framework**: FlyonUI (TailwindCSS)
- **Testing**: RSpec

---

## 🎨 UI Design Templates

Auth pages will follow FlyonUI Pro design patterns:

- **Login**: https://flyonui.com/pro/preview/blocks/marketing-ui/login/login5.html?theme=light
- **Register**: https://flyonui.com/pro/preview/blocks/marketing-ui/register/register5.html?theme=light
- **Forgot Password**: https://flyonui.com/pro/preview/blocks/marketing-ui/forgot-password/forgot-password5.html?theme=light
- **Reset Password**: https://flyonui.com/pro/preview/blocks/marketing-ui/reset-password/reset-password5.html?theme=light
- **Verify Email**: https://flyonui.com/pro/preview/blocks/marketing-ui/verify-email/verify-email5.html?theme=light

---

## 📅 Implementation Phases

### Phase 1: Setup Authentication Foundation ✅ (In Progress)

**Duration**: 2-3 days

#### Tasks

- [x] Create feature branch
- [x] Create implementation plan document
- [ ] Add Pundit gem
- [ ] Configure Devise (already installed)
- [ ] Create User migration with role and fields
- [ ] Update User model with role enum
- [ ] Create user factory for testing
- [ ] Setup basic specs

**Files to Create/Modify**:

```txt
Gemfile                              # Add pundit
db/migrate/xxxxx_add_role_to_users  # Add role and user fields
app/models/user.rb                   # Add role enum and helpers
spec/models/user_spec.rb             # User model specs
spec/factories/users.rb              # User factory
```

---

### Phase 2: Implement Authorization with Pundit

**Duration**: 3-4 days

#### Tasks

- [ ] Install Pundit
- [ ] Generate Pundit initializer
- [ ] Create ApplicationPolicy base class
- [ ] Create UserPolicy
- [ ] Add Pundit to ApplicationController
- [ ] Setup unauthorized error handling
- [ ] Create policy specs

**Files to Create**:

```txt
app/policies/application_policy.rb
app/policies/user_policy.rb
app/controllers/application_controller.rb  # Update
spec/policies/user_policy_spec.rb
```

---

### Phase 3: User Management Implementation

**Duration**: 4-5 days

#### Tasks

- [ ] Create UserManagement slice structure
- [ ] Create Users controller
- [ ] Implement CRUD operations
- [ ] Add authorization checks
- [ ] Create placeholder views (to be styled later)
- [ ] Add routes
- [ ] Create controller specs
- [ ] Create request specs

**Files to Create**:

```txt
app/slices/user_management/controllers/users_controller.rb
app/slices/user_management/views/users/index.html.erb
app/slices/user_management/views/users/show.html.erb
app/slices/user_management/views/users/new.html.erb
app/slices/user_management/views/users/edit.html.erb
app/slices/user_management/views/users/_form.html.erb
config/routes.rb  # Update
spec/requests/user_management/users_spec.rb
```

---

### Phase 4: Parent-Player Relationship

**Duration**: 3-4 days

#### Tasks

- [ ] Create PlayerGuardian model
- [ ] Create migration for player_guardians table
- [ ] Add associations to User and Player models
- [ ] Create PlayerGuardianPolicy
- [ ] Implement parent invitation service
- [ ] Create specs

**Files to Create**:

```txt
db/migrate/xxxxx_create_player_guardians.rb
app/models/player_guardian.rb
app/policies/player_guardian_policy.rb
app/services/parent_invitation_service.rb
spec/models/player_guardian_spec.rb
spec/services/parent_invitation_service_spec.rb
```

---

### Phase 5: Devise Views with FlyonUI Design

**Duration**: 4-5 days

#### Tasks

- [ ] Generate Devise views
- [ ] Create custom Devise controllers
- [ ] Style login page (FlyonUI template)
- [ ] Style registration page (FlyonUI template)
- [ ] Style forgot password page (FlyonUI template)
- [ ] Style reset password page (FlyonUI template)
- [ ] Style email verification page (FlyonUI template)
- [ ] Add custom validation messages
- [ ] Test responsive design

**Files to Create/Modify**:

```txt
app/controllers/auth/sessions_controller.rb
app/controllers/auth/registrations_controller.rb
app/controllers/auth/passwords_controller.rb
app/controllers/auth/confirmations_controller.rb
app/views/auth/sessions/new.html.erb
app/views/auth/registrations/new.html.erb
app/views/auth/registrations/edit.html.erb
app/views/auth/passwords/new.html.erb
app/views/auth/passwords/edit.html.erb
app/views/auth/confirmations/new.html.erb
```

---

### Phase 6: User Management UI

**Duration**: 3-4 days

#### Tasks

- [ ] Design user listing page
- [ ] Design user detail page
- [ ] Design user form (new/edit)
- [ ] Add role badges/indicators
- [ ] Create role assignment interface
- [ ] Add search and filtering
- [ ] Add proper error/success messages
- [ ] Make responsive

---

### Phase 7: Security Hardening

**Duration**: 2-3 days

#### Tasks

- [ ] Configure strong password requirements
- [ ] Add account lockout (Devise lockable)
- [ ] Configure email confirmation (Devise confirmable)
- [ ] Add session timeout
- [ ] Implement rate limiting
- [ ] Add audit logging for sensitive actions
- [ ] Security testing
- [ ] Penetration testing checklist

---

### Phase 8: Seeding & Documentation

**Duration**: 2 days

#### Tasks

- [ ] Create comprehensive seed data
- [ ] Update README with auth documentation
- [ ] Create user role documentation
- [ ] Create permission matrix documentation
- [ ] Add inline code documentation
- [ ] Create usage examples

**Files to Create/Modify**:

```txt
db/seeds.rb
docs/ROLES_AND_PERMISSIONS.md
docs/USER_MANAGEMENT_GUIDE.md
README.md  # Update
```

---

## 📊 Complete Permission Matrix

See `docs/ROLES_AND_PERMISSIONS.md` for the complete permission matrix (will be created in Phase 8).

---

## 🧪 Testing Strategy

### Unit Tests

- Model validations and associations
- Role helper methods
- Policy permissions

### Integration Tests

- User CRUD operations
- Role assignment flows
- Parent-player relationships

### System Tests

- Login/logout flows
- Registration flows
- Password reset flows
- User management UI

### Security Tests

- Unauthorized access attempts
- Role escalation attempts
- SQL injection protection
- XSS protection

---

## 📈 Success Metrics

- [ ] All 7 roles implemented
- [ ] 100% test coverage for policies
- [ ] 90%+ test coverage overall
- [ ] Zero security vulnerabilities
- [ ] All auth pages match FlyonUI design
- [ ] Mobile responsive
- [ ] Proper error handling
- [ ] Comprehensive documentation

---

## 🚀 Deployment Checklist

- [ ] All tests passing
- [ ] Security audit complete
- [ ] Documentation complete
- [ ] Seed data tested
- [ ] Email templates configured
- [ ] Environment variables documented
- [ ] Database migrations tested
- [ ] Rollback plan documented

---

## 📝 Notes

- Start with basic authentication, then layer authorization
- Keep parent portal separate (will be implemented later)
- Focus on security from day one
- Write tests before implementation
- Follow Rails and project conventions
- Use OpenTelemetry for tracking auth events

---

**Next Steps**: Begin Phase 1 - Setup Authentication Foundation
