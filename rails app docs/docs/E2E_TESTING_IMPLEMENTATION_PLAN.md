# E2E Testing Implementation Plan for Diquis

**Document Version:** 1.0  
**Created:** November 6, 2025  
**Project:** Diquis - Football Academy Management System  
**Stack:** Rails 8.1.1 + Inertia.js + React 19 + TypeScript + FlyonUI  

---

## Executive Summary

This document provides a complete implementation plan for adding End-to-End (E2E) testing to the Diquis application using **Playwright**. The plan is designed to be executed by any developer or AI agent without requiring prior context about the project.

### Why Playwright Over Capybara

**Technology Stack Context:**

- Backend: Ruby on Rails 8.1.1
- Frontend: React 19.2.0 with TypeScript 5.9.3
- Bridge: Inertia.js 2.2.12 (SPA-like experience without traditional API)
- CSS: TailwindCSS 4.1.16 + FlyonUI 2.4.1
- Testing: Currently RSpec for backend, Vitest for frontend unit tests

**Rationale:**

1. **Inertia.js Compatibility**: Inertia uses client-side navigation and AJAX requests. Playwright handles modern SPAs better than Capybara/Selenium.
2. **TypeScript Alignment**: Tests written in TypeScript match the frontend codebase language.
3. **React Components**: Playwright can test React components and client-side state better.
4. **Developer Experience**: Better debugging tools, auto-waiting, and network interception.
5. **Speed & Reliability**: Faster execution and more stable than Capybara with JavaScript drivers.

**Existing Test Infrastructure:**

- RSpec installed with Capybara and Selenium (but not used for E2E yet)
- FactoryBot for test data generation
- Database Cleaner for test isolation
- Devise test helpers for authentication
- PaperTrail for audit logging (important for E2E validation)

---

## Project Context

### Application Architecture

**Authentication System:**

- Devise 4.9 with custom controllers in `app/controllers/users/`
- Modules enabled: `:database_authenticatable`, `:registerable`, `:recoverable`, `:rememberable`, `:validatable`, `:confirmable`, `:lockable`, `:trackable`, `:timeoutable`
- Session timeout: 2 hours
- Account lockout: After maximum failed attempts (configured in Devise)
- Password requirements: Minimum 12 characters with uppercase, lowercase, digit, and special character

**Authorization System:**

- Pundit 2.4 for role-based access control
- Policies defined in `app/policies/`
- User roles (hierarchical from lowest to highest):
  - `player` (0)
  - `parent` (1)
  - `staff` (2)
  - `coach` (3)
  - `academy_admin` (4)
  - `academy_owner` (5)
  - `super_admin` (6)

**User Management:**

- Located in slice architecture: `app/slices/user_management/`
- Controller: `app/slices/user_management/users_controller.rb`
- Routes: `/admin/users` (index, show, new, create, edit, update, destroy)
- Frontend pages: `app/frontend/pages/UserManagement/Users/` (Index.tsx, Show.tsx, New.tsx, Edit.tsx)

**Audit Logging:**

- PaperTrail 16.0 tracks all user create/update/destroy events
- Versions stored in `versions` table
- Important for E2E validation of security-critical operations

**Database:**

- PostgreSQL (production and test)
- Test database: `diquis_test`
- Docker setup available (optional)

**Frontend Routing:**

- Inertia.js handles navigation
- No traditional API endpoints - Inertia sends JSON props to React components
- Forms submit via Inertia methods (`post`, `patch`, `delete`)
- Redirects handled via `X-Inertia-Location` header

---

## Implementation Phases

### Phase 1: Initial Setup (Day 1)

#### 1.1 Install Playwright

```bash
# From project root: /home/erick/RubymineProjects/Diquis
npm install -D @playwright/test
npx playwright install chromium firefox webkit
```

**Expected files created:**

- `node_modules/@playwright/test/`
- `~/.cache/ms-playwright/` (browsers)

#### 1.2 Create Playwright Configuration

**File:** `playwright.config.ts` (project root)

```typescript
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  
  // Maximum time one test can run for
  timeout: 30 * 1000,
  
  // Test execution settings
  fullyParallel: false, // Run tests sequentially to avoid database conflicts
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: 1, // Single worker to avoid race conditions
  
  // Reporter configuration
  reporter: [
    ['html', { outputFolder: 'playwright-report' }],
    ['list'],
    ['json', { outputFile: 'test-results/results.json' }]
  ],
  
  // Shared settings for all tests
  use: {
    // Base URL for the application
    baseURL: 'http://localhost:3000',
    
    // Collect trace for debugging
    trace: 'retain-on-failure',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    
    // Viewport size
    viewport: { width: 1280, height: 720 },
    
    // Ignore HTTPS errors (for local development)
    ignoreHTTPSErrors: true,
  },

  // Browser configurations
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    // Uncomment for multi-browser testing
    // {
    //   name: 'firefox',
    //   use: { ...devices['Desktop Firefox'] },
    // },
    // {
    //   name: 'webkit',
    //   use: { ...devices['Desktop Safari'] },
    // },
  ],

  // Run development server before starting tests
  webServer: {
    command: 'bin/rails server -e test -p 3000',
    port: 3000,
    timeout: 120 * 1000,
    reuseExistingServer: !process.env.CI,
    env: {
      RAILS_ENV: 'test',
    },
  },
});
```

**Important notes:**

- `fullyParallel: false` prevents database conflicts
- `workers: 1` ensures sequential test execution
- `webServer` automatically starts Rails in test mode
- Base URL is `http://localhost:3000` (standard Rails development port)

#### 1.3 Create Directory Structure

```bash
mkdir -p e2e/{auth,admin,authorization,dashboard,fixtures,helpers}
mkdir -p test-results
mkdir -p playwright-report
```

**Expected structure:**

```txt
e2e/
├── auth/                 # Authentication tests
├── admin/                # User management tests
├── authorization/        # Pundit policy tests
├── dashboard/            # Dashboard access tests
├── fixtures/             # Test data
└── helpers/              # Reusable test utilities
```

#### 1.4 Add npm Scripts

**File:** `package.json` (modify existing file)

Add to `"scripts"` section:

```json
{
  "scripts": {
    "check": "tsc -p tsconfig.app.json && tsc -p tsconfig.node.json",
    "test": "vitest",
    "test:ui": "vitest --ui",
    "test:run": "vitest run",
    "test:coverage": "vitest run --coverage",
    "e2e": "playwright test",
    "e2e:ui": "playwright test --ui",
    "e2e:debug": "playwright test --debug",
    "e2e:headed": "playwright test --headed",
    "e2e:report": "playwright show-report"
  }
}
```

#### 1.5 Add Test-Only API Endpoints (Backend)

**File:** `config/routes.rb`

Add at the end (before the closing `end`):

```ruby
# Test-only endpoints for E2E validation
if Rails.env.test?
  namespace :test_helpers do
    resources :users, only: [:show] do
      collection do
        delete :cleanup
      end
    end
  end
end
```

**File:** `app/controllers/test_helpers/users_controller.rb` (create new)

```ruby
# frozen_string_literal: true

module TestHelpers
  class UsersController < ApplicationController
    skip_before_action :authenticate_user!
    skip_before_action :verify_authenticity_token
    
    # GET /test_helpers/users/:id
    # Returns user data including audit trail for E2E validation
    def show
      user = User.find(params[:id])
      render json: user.as_json(
        methods: [:full_name],
        include: {
          versions: {
            only: [:id, :event, :whodunnit, :created_at],
            methods: [:changeset]
          }
        }
      )
    rescue ActiveRecord::RecordNotFound
      render json: { error: 'User not found' }, status: :not_found
    end
    
    # DELETE /test_helpers/users/cleanup
    # Removes test users by email pattern
    def cleanup
      email_pattern = params[:email]
      return head :bad_request if email_pattern.blank?
      
      users = User.where('email LIKE ?', "%#{email_pattern}%")
      count = users.count
      users.destroy_all
      
      render json: { deleted: count }
    end
  end
end
```

**Important:** These endpoints are only available in `test` environment and allow E2E tests to:

1. Validate backend data after UI operations
2. Check audit trail (PaperTrail versions)
3. Clean up test data

---

### Phase 2: Helper Utilities (Day 1-2)

#### 2.1 Authentication Helper

**File:** `e2e/helpers/auth.ts`

```typescript
import { Page } from '@playwright/test';

export interface LoginCredentials {
  email: string;
  password: string;
}

/**
 * Default test user credentials from db/seeds.rb
 */
export const TEST_USERS = {
  super_admin: {
    email: 'admin@diquis.com',
    password: 'Dev3l0pment!2025',
  },
  academy_admin: {
    email: 'academy@diquis.com',
    password: 'Dev3l0pment!2025',
  },
  coach: {
    email: 'coach@diquis.com',
    password: 'Dev3l0pment!2025',
  },
  player: {
    email: 'player@diquis.com',
    password: 'Dev3l0pment!2025',
  },
} as const;

/**
 * Login to the application via UI
 * @param page - Playwright page object
 * @param credentials - Login credentials or role name
 */
export async function login(
  page: Page,
  credentials: LoginCredentials | keyof typeof TEST_USERS
) {
  const creds = typeof credentials === 'string'
    ? TEST_USERS[credentials]
    : credentials;

  await page.goto('/users/sign_in');
  await page.fill('input[type="email"]', creds.email);
  await page.fill('input[type="password"]', creds.password);
  await page.click('button[type="submit"]');
  
  // Wait for redirect to dashboard or intended page
  await page.waitForURL(/\/(dashboard|app)/);
}

/**
 * Logout from the application
 * @param page - Playwright page object
 */
export async function logout(page: Page) {
  // Get CSRF token from meta tag
  const csrfToken = await page.locator('meta[name="csrf-token"]').getAttribute('content');

  if (!csrfToken) {
    throw new Error("CSRF token not found on the page.");
  }

  // Send a POST request to the logout path with _method set to 'delete'
  // This bypasses the Inertia Link component and directly hits the Rails backend
  await page.request.post("/users/sign_out", {
    form: {
      _method: "delete",
      authenticity_token: csrfToken,
    },
  });

  // Wait for redirect to login page
  await page.waitForURL(/\/(users\/sign_in|\/)/, { timeout: 15000 });
}

/**
 * Check if user is logged in
 * @param page - Playwright page object
 * @returns true if logged in
 */
export async function isLoggedIn(page: Page): Promise<boolean> {
  try {
    // Check for presence of profile menu or logout link
    await page.waitForSelector('[data-testid="profile-menu"], a:has-text("Logout")', {
      timeout: 2000
    });
    return true;
  } catch {
    return false;
  }
}
```

**Usage in tests:**

```typescript
import { login, TEST_USERS } from './helpers/auth';

test('admin can access users', async ({ page }) => {
  await login(page, 'super_admin');
  await page.goto('/admin/users');
  // ... assertions
});
```

#### 2.2 Database Validation Helper

**File:** `e2e/helpers/database.ts`

```typescript
import { APIRequestContext } from '@playwright/test';

export interface User {
  id: string;
  email: string;
  first_name: string | null;
  last_name: string | null;
  full_name: string;
  phone: string | null;
  role: string;
  created_at: string;
  updated_at: string;
  confirmed_at: string | null;
  locked_at: string | null;
  sign_in_count: number;
  versions?: AuditVersion[];
}

export interface AuditVersion {
  id: number;
  event: 'create' | 'update' | 'destroy';
  whodunnit: string | null;
  created_at: string;
  changeset: Record<string, [any, any]>;
}

/**
 * Fetch user data from test helper endpoint
 * @param request - Playwright API request context
 * @param userId - User ID to fetch
 * @returns User object with audit trail
 */
export async function getUser(
  request: APIRequestContext,
  userId: string | number
): Promise<User> {
  const response = await request.get(
    `http://localhost:3000/test_helpers/users/${userId}`
  );
  
  if (!response.ok()) {
    throw new Error(`Failed to fetch user ${userId}: ${response.status()}`);
  }
  
  return await response.json();
}

/**
 * Cleanup test users by email pattern
 * @param request - Playwright API request context
 * @param emailPattern - Email pattern to match (e.g., "test-123@example.com")
 */
export async function cleanupUsers(
  request: APIRequestContext,
  emailPattern: string
): Promise<number> {
  const response = await request.delete(
    `http://localhost:3000/test_helpers/users/cleanup?email=${emailPattern}`
  );
  
  if (!response.ok()) {
    throw new Error(`Failed to cleanup users: ${response.status()}`);
  }
  
  const result = await response.json();
  return result.deleted;
}

/**
 * Validate audit trail for a user
 * @param user - User object with versions
 * @param expectedEvents - Array of expected events in order
 */
export function validateAuditTrail(
  user: User,
  expectedEvents: Array<'create' | 'update' | 'destroy'>
) {
  const events = user.versions?.map(v => v.event) || [];
  
  if (events.length !== expectedEvents.length) {
    throw new Error(
      `Expected ${expectedEvents.length} audit events, got ${events.length}`
    );
  }
  
  expectedEvents.forEach((expected, index) => {
    if (events[index] !== expected) {
      throw new Error(
        `Expected event ${index} to be "${expected}", got "${events[index]}"`
      );
    }
  });
}
```

**Usage in tests:**

```typescript
import { getUser, cleanupUsers, validateAuditTrail } from './helpers/database';

test('user creation is audited', async ({ page, request }) => {
  // ... create user via UI ...
  
  const user = await getUser(request, userId);
  validateAuditTrail(user, ['create']);
  
  await cleanupUsers(request, user.email);
});
```

#### 2.3 Test Data Fixtures

**File:** `e2e/fixtures/users.ts`

```typescript
/**
 * Generate unique email for test user
 * @param prefix - Email prefix (default: 'test')
 * @returns Unique email address
 */
export function generateTestEmail(prefix: string = 'test'): string {
  const timestamp = Date.now();
  const random = Math.floor(Math.random() * 1000);
  return `${prefix}-${timestamp}-${random}@example.com`;
}

/**
 * Valid user data for creating users
 */
export const VALID_USER_DATA = {
  player: {
    first_name: 'John',
    last_name: 'Doe',
    role: 'player',
    password: 'SecurePass123!',
  },
  parent: {
    first_name: 'Jane',
    last_name: 'Smith',
    role: 'parent',
    password: 'SecurePass123!',
  },
  coach: {
    first_name: 'Mike',
    last_name: 'Johnson',
    role: 'coach',
    password: 'SecurePass123!',
    phone: '+1234567890',
  },
  staff: {
    role: 'staff',
    password: 'SecurePass123!',
  },
} as const;

/**
 * Invalid password test cases
 */
export const INVALID_PASSWORDS = {
  too_short: 'Short1!',
  no_uppercase: 'lowercase123!',
  no_lowercase: 'UPPERCASE123!',
  no_digit: 'PasswordNoDigit!',
  no_special: 'Password1234',
  common: 'Password123!',
} as const;

/**
 * Generate complete user data for testing
 * @param role - User role
 * @param overrides - Optional field overrides
 * @returns Complete user object
 */
export function generateUserData(
  role: keyof typeof VALID_USER_DATA,
  overrides: Partial<typeof VALID_USER_DATA[typeof role]> = {}
) {
  const baseData = VALID_USER_DATA[role];
  return {
    email: generateTestEmail(role),
    ...baseData,
    ...overrides,
  };
}
```

**Usage in tests:**

```typescript
import { generateUserData, INVALID_PASSWORDS } from './fixtures/users';

test('create player', async ({ page }) => {
  const userData = generateUserData('player');
  await page.fill('input[type="email"]', userData.email);
  await page.fill('input[name="first_name"]', userData.first_name);
  // ...
});
```

---

### Phase 3: Core Test Implementation (Day 2-5)

#### 3.1 Authentication Tests

**File:** `e2e/auth/login.spec.ts`

```typescript
import { test, expect } from '@playwright/test';
import { TEST_USERS } from '../helpers/auth';

test.describe('User Login', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/users/sign_in');
  });

  test('successful login with valid credentials', async ({ page }) => {
    await page.fill('input[type="email"]', TEST_USERS.super_admin.email);
    await page.fill('input[type="password"]', TEST_USERS.super_admin.password);
    await page.click('button[type="submit"]');

    // Should redirect to dashboard
    await expect(page).toHaveURL(/\/(dashboard|app)/);
    
    // Should show user menu or profile indicator
    await expect(page.locator('text=Logout, [data-testid="profile-menu"]')).toBeVisible();
  });

  test('failed login with invalid password', async ({ page }) => {
    await page.fill('input[type="email"]', TEST_USERS.super_admin.email);
    await page.fill('input[type="password"]', 'WrongPassword123!');
    await page.click('button[type="submit"]');

    // Should stay on login page
    await expect(page).toHaveURL(/\/users\/sign_in/);
    
    // Should show error message
    await expect(page.locator('text=Invalid Email or password')).toBeVisible();
  });

  test('failed login with non-existent user', async ({ page }) => {
    await page.fill('input[type="email"]', 'nonexistent@example.com');
    await page.fill('input[type="password"]', 'SomePassword123!');
    await page.click('button[type="submit"]');

    await expect(page).toHaveURL(/\/users\/sign_in/);
    await expect(page.locator('text=Invalid Email or password')).toBeVisible();
  });

  test('remember me checkbox persists session', async ({ page, context }) => {
    await page.fill('input[type="email"]', TEST_USERS.super_admin.email);
    await page.fill('input[type="password"]', TEST_USERS.super_admin.password);
    await page.check('input[name="remember_me"]');
    await page.click('button[type="submit"]');

    await expect(page).toHaveURL(/\/(dashboard|app)/);

    // Close and reopen page to simulate browser restart
    await page.close();
    const newPage = await context.newPage();
    await newPage.goto('/dashboard');

    // Should still be logged in
    await expect(newPage).toHaveURL(/\/(dashboard|app)/);
  });
});
```

**File:** `e2e/auth/logout.spec.ts`

```typescript
import { test, expect } from '@playwright/test';
import { login } from '../helpers/auth';

test.describe('User Logout', () => {
  test('successful logout', async ({ page }) => {
    await login(page, 'super_admin');
    
    // Click profile menu
    await page.click('[data-testid="profile-menu"], .dropdown:has-text("Profile")');
    
    // Click logout
    await page.click('text=Logout, a[href*="sign_out"]');

    // Should redirect to home or login
    await expect(page).toHaveURL(/\/(users\/sign_in|^\/)/);
    
    // Attempting to access protected page should redirect
    await page.goto('/dashboard');
    await expect(page).toHaveURL(/\/users\/sign_in/);
  });
});
```

**File:** `e2e/auth/password-reset.spec.ts`

```typescript
import { test, expect } from '@playwright/test';
import { TEST_USERS } from '../helpers/auth';

test.describe('Password Reset', () => {
  test('request password reset', async ({ page }) => {
    await page.goto('/users/password/new');
    
    await page.fill('input[type="email"]', TEST_USERS.player.email);
    await page.click('button[type="submit"]');

    // Should show success message
    await expect(page.locator('text=You will receive an email')).toBeVisible();
  });

  test('password reset with invalid email', async ({ page }) => {
    await page.goto('/users/password/new');
    
    await page.fill('input[type="email"]', 'nonexistent@example.com');
    await page.click('button[type="submit"]');

    // Devise typically still shows success to prevent email enumeration
    await expect(page.locator('text=You will receive an email')).toBeVisible();
  });
});
```

**File:** `e2e/auth/account-lockout.spec.ts`

```typescript
import { test, expect } from '@playwright/test';
import { TEST_USERS, cleanupUsers } from '../helpers/auth';
import { generateTestEmail } from '../fixtures/users';

test.describe('Account Lockout', () => {
  test('account locks after maximum failed attempts', async ({ page, request }) => {
    const testEmail = generateTestEmail('lockout-test');
    
    // Note: This test requires a test user to be created first
    // You may need to create via Rails console or factory in beforeAll
    
    // Attempt login with wrong password multiple times
    for (let i = 0; i < 5; i++) {
      await page.goto('/users/sign_in');
      await page.fill('input[type="email"]', testEmail);
      await page.fill('input[type="password"]', 'WrongPassword123!');
      await page.click('button[type="submit"]');
      await page.waitForURL(/\/users\/sign_in/);
    }

    // Next attempt should show locked message
    await page.goto('/users/sign_in');
    await page.fill('input[type="email"]', testEmail);
    await page.fill('input[type="password"]', 'WrongPassword123!');
    await page.click('button[type="submit"]');

    await expect(page.locator('text=Your account is locked')).toBeVisible();
    
    // Cleanup
    await cleanupUsers(request, testEmail);
  });
});
```

#### 3.2 User Management Tests

**File:** `e2e/admin/user-create.spec.ts`

```typescript
import { test, expect } from '@playwright/test';
import { login } from '../helpers/auth';
import { generateUserData } from '../fixtures/users';
import { getUser, cleanupUsers, validateAuditTrail } from '../helpers/database';

test.describe('User Creation', () => {
  test.beforeEach(async ({ page }) => {
    await login(page, 'super_admin');
  });

  test('admin can create a new player', async ({ page, request }) => {
    const userData = generateUserData('player');
    
    await page.goto('/admin/users/new');
    
    // Fill form
    await page.fill('input[type="email"]', userData.email);
    await page.selectOption('select[name="role"]', userData.role);
    await page.fill('input[name="first_name"]', userData.first_name);
    await page.fill('input[name="last_name"]', userData.last_name);
    await page.fill('input[name="password"]', userData.password);
    await page.fill('input[name="password_confirmation"]', userData.password);
    
    // Intercept Inertia POST request
    const responsePromise = page.waitForResponse(
      response => response.url().includes('/admin/users') && 
                  response.request().method() === 'POST'
    );
    
    await page.click('button[type="submit"]');
    
    const response = await responsePromise;
    expect(response.status()).toBe(302); // Redirect after creation
    
    // Should redirect to user show page
    await page.waitForURL(/\/admin\/users\/\d+/);
    const userId = page.url().match(/\/admin\/users\/(\d+)/)?.[1];
    expect(userId).toBeDefined();
    
    // Validate user appears on page
    await expect(page.locator(`text=${userData.email}`)).toBeVisible();
    await expect(page.locator(`text=${userData.first_name} ${userData.last_name}`)).toBeVisible();
    
    // Validate in database
    const user = await getUser(request, userId!);
    expect(user.email).toBe(userData.email);
    expect(user.first_name).toBe(userData.first_name);
    expect(user.last_name).toBe(userData.last_name);
    expect(user.role).toBe(userData.role);
    expect(user.confirmed_at).toBeNull(); // Should require email confirmation
    
    // Validate audit trail
    validateAuditTrail(user, ['create']);
    
    // Cleanup
    await cleanupUsers(request, userData.email);
  });

  test('validation errors for missing required fields', async ({ page }) => {
    await page.goto('/admin/users/new');
    
    // Select player role (requires first/last name)
    await page.selectOption('select[name="role"]', 'player');
    
    // Submit without filling required fields
    await page.click('button[type="submit"]');
    
    // Should show validation errors
    await expect(page.locator('text=Email can\'t be blank')).toBeVisible();
    await expect(page.locator('text=First name can\'t be blank')).toBeVisible();
    await expect(page.locator('text=Last name can\'t be blank')).toBeVisible();
    await expect(page.locator('text=Password can\'t be blank')).toBeVisible();
  });

  test('validation errors for weak password', async ({ page }) => {
    const userData = generateUserData('player', { password: 'weak' });
    
    await page.goto('/admin/users/new');
    
    await page.fill('input[type="email"]', userData.email);
    await page.selectOption('select[name="role"]', 'player');
    await page.fill('input[name="first_name"]', userData.first_name);
    await page.fill('input[name="last_name"]', userData.last_name);
    await page.fill('input[name="password"]', 'weak');
    await page.fill('input[name="password_confirmation"]', 'weak');
    
    await page.click('button[type="submit"]');
    
    // Should show password validation errors
    await expect(page.locator('text=Password is too short')).toBeVisible();
  });

  test('validation errors for duplicate email', async ({ page, request }) => {
    const userData = generateUserData('player');
    
    // Create first user
    await page.goto('/admin/users/new');
    await page.fill('input[type="email"]', userData.email);
    await page.selectOption('select[name="role"]', 'player');
    await page.fill('input[name="first_name"]', userData.first_name);
    await page.fill('input[name="last_name"]', userData.last_name);
    await page.fill('input[name="password"]', userData.password);
    await page.fill('input[name="password_confirmation"]', userData.password);
    await page.click('button[type="submit"]');
    await page.waitForURL(/\/admin\/users\/\d+/);
    
    // Try to create duplicate
    await page.goto('/admin/users/new');
    await page.fill('input[type="email"]', userData.email);
    await page.selectOption('select[name="role"]', 'player');
    await page.fill('input[name="first_name"]', 'Different');
    await page.fill('input[name="last_name"]', 'Name');
    await page.fill('input[name="password"]', userData.password);
    await page.fill('input[name="password_confirmation"]', userData.password);
    await page.click('button[type="submit"]');
    
    // Should show duplicate email error
    await expect(page.locator('text=Email has already been taken')).toBeVisible();
    
    // Cleanup
    await cleanupUsers(request, userData.email);
  });
});
```

**File:** `e2e/admin/user-edit.spec.ts`

```typescript
import { test, expect } from '@playwright/test';
import { login } from '../helpers/auth';
import { generateUserData } from '../fixtures/users';
import { getUser, cleanupUsers, validateAuditTrail } from '../helpers/database';

test.describe('User Editing', () => {
  test.beforeEach(async ({ page }) => {
    await login(page, 'super_admin');
  });

  test('admin can edit existing user', async ({ page, request }) => {
    const userData = generateUserData('player');
    
    // Create user first
    await page.goto('/admin/users/new');
    await page.fill('input[type="email"]', userData.email);
    await page.selectOption('select[name="role"]', 'player');
    await page.fill('input[name="first_name"]', userData.first_name);
    await page.fill('input[name="last_name"]', userData.last_name);
    await page.fill('input[name="password"]', userData.password);
    await page.fill('input[name="password_confirmation"]', userData.password);
    await page.click('button[type="submit"]');
    await page.waitForURL(/\/admin\/users\/(\d+)/);
    const userId = page.url().match(/\/admin\/users\/(\d+)/)?.[1];
    
    // Navigate to edit page
    await page.click('text=Edit User');
    await expect(page).toHaveURL(`/admin/users/${userId}/edit`);
    
    // Modify user data
    await page.fill('input[name="first_name"]', 'Updated');
    await page.fill('input[name="last_name"]', 'Name');
    await page.fill('input[name="phone"]', '+1234567890');
    
    await page.click('button[type="submit"]');
    
    // Should redirect back to show page
    await page.waitForURL(`/admin/users/${userId}`);
    
    // Validate changes appear
    await expect(page.locator('text=Updated Name')).toBeVisible();
    await expect(page.locator('text=+1234567890')).toBeVisible();
    
    // Validate in database
    const user = await getUser(request, userId!);
    expect(user.first_name).toBe('Updated');
    expect(user.last_name).toBe('Name');
    expect(user.phone).toBe('+1234567890');
    
    // Validate audit trail shows update
    validateAuditTrail(user, ['create', 'update']);
    
    // Cleanup
    await cleanupUsers(request, userData.email);
  });

  test('admin can change user role', async ({ page, request }) => {
    const userData = generateUserData('player');
    
    // Create user
    await page.goto('/admin/users/new');
    await page.fill('input[type="email"]', userData.email);
    await page.selectOption('select[name="role"]', 'player');
    await page.fill('input[name="first_name"]', userData.first_name);
    await page.fill('input[name="last_name"]', userData.last_name);
    await page.fill('input[name="password"]', userData.password);
    await page.fill('input[name="password_confirmation"]', userData.password);
    await page.click('button[type="submit"]');
    await page.waitForURL(/\/admin\/users\/(\d+)/);
    const userId = page.url().match(/\/admin\/users\/(\d+)/)?.[1];
    
    // Edit role
    await page.click('text=Edit User');
    await page.selectOption('select[name="role"]', 'coach');
    await page.click('button[type="submit"]');
    
    // Validate role changed
    const user = await getUser(request, userId!);
    expect(user.role).toBe('coach');
    
    // Validate audit shows role change
    const lastVersion = user.versions?.[user.versions.length - 1];
    expect(lastVersion?.changeset.role).toBeDefined();
    expect(lastVersion?.changeset.role[1]).toBe('coach');
    
    // Cleanup
    await cleanupUsers(request, userData.email);
  });
});
```

**File:** `e2e/admin/user-delete.spec.ts`

```typescript
import { test, expect } from '@playwright/test';
import { login } from '../helpers/auth';
import { generateUserData } from '../fixtures/users';
import { getUser } from '../helpers/database';

test.describe('User Deletion', () => {
  test.beforeEach(async ({ page }) => {
    await login(page, 'super_admin');
  });

  test('admin can delete user', async ({ page, request }) => {
    const userData = generateUserData('player');
    
    // Create user
    await page.goto('/admin/users/new');
    await page.fill('input[type="email"]', userData.email);
    await page.selectOption('select[name="role"]', 'player');
    await page.fill('input[name="first_name"]', userData.first_name);
    await page.fill('input[name="last_name"]', userData.last_name);
    await page.fill('input[name="password"]', userData.password);
    await page.fill('input[name="password_confirmation"]', userData.password);
    await page.click('button[type="submit"]');
    await page.waitForURL(/\/admin\/users\/(\d+)/);
    const userId = page.url().match(/\/admin\/users\/(\d+)/)?.[1];
    
    // Setup dialog handler for confirmation
    page.on('dialog', dialog => dialog.accept());
    
    // Delete user
    await page.click('text=Delete, button:has-text("Delete")');
    
    // Should redirect to users index
    await page.waitForURL('/admin/users');
    
    // User should not appear in list
    await expect(page.locator(`text=${userData.email}`)).not.toBeVisible();
    
    // Verify user is deleted in database
    try {
      await getUser(request, userId!);
      throw new Error('User should have been deleted');
    } catch (error: any) {
      expect(error.message).toContain('not found');
    }
  });
});
```

**File:** `e2e/admin/user-list.spec.ts`

```typescript
import { test, expect } from '@playwright/test';
import { login, TEST_USERS } from '../helpers/auth';

test.describe('User List', () => {
  test.beforeEach(async ({ page }) => {
    await login(page, 'super_admin');
  });

  test('admin can view user list', async ({ page }) => {
    await page.goto('/admin/users');
    
    // Should show page title
    await expect(page.locator('h1:has-text("User Management")')).toBeVisible();
    
    // Should show seed users
    await expect(page.locator(`text=${TEST_USERS.super_admin.email}`)).toBeVisible();
    await expect(page.locator(`text=${TEST_USERS.player.email}`)).toBeVisible();
  });

  test('admin can search users', async ({ page }) => {
    await page.goto('/admin/users');
    
    // Search for specific user
    await page.fill('input[placeholder*="Search"]', TEST_USERS.player.email);
    
    // Should show only matching user
    await expect(page.locator(`text=${TEST_USERS.player.email}`)).toBeVisible();
    
    // Should not show other users
    await expect(page.locator(`text=${TEST_USERS.coach.email}`)).not.toBeVisible();
  });
});
```

#### 3.3 Authorization Tests

**File:** `e2e/authorization/role-restrictions.spec.ts`

```typescript
import { test, expect } from '@playwright/test';
import { login, TEST_USERS } from '../helpers/auth';

test.describe('Role-Based Authorization', () => {
  test('player cannot access user management', async ({ page }) => {
    await login(page, 'player');
    
    await page.goto('/admin/users');
    
    // Should be redirected or show unauthorized
    await expect(page.locator('text=not authorized, text=unauthorized')).toBeVisible();
  });

  test('coach cannot access user management', async ({ page }) => {
    await login(page, 'coach');
    
    await page.goto('/admin/users');
    
    await expect(page.locator('text=not authorized, text=unauthorized')).toBeVisible();
  });

  test('academy_admin can access user management', async ({ page }) => {
    await login(page, 'academy_admin');
    
    await page.goto('/admin/users');
    
    // Should see user list
    await expect(page.locator('h1:has-text("User Management")')).toBeVisible();
  });

  test('super_admin can access all features', async ({ page }) => {
    await login(page, 'super_admin');
    
    await page.goto('/admin/users');
    
    await expect(page.locator('h1:has-text("User Management")')).toBeVisible();
    await expect(page.locator('text=Create User')).toBeVisible();
  });
});
```

#### 3.4 Dashboard Tests

**File:** `e2e/dashboard/access.spec.ts`

```typescript
import { test, expect } from '@playwright/test';
import { login, TEST_USERS } from '../helpers/auth';

test.describe('Dashboard Access', () => {
  test('authenticated user can access dashboard', async ({ page }) => {
    await login(page, 'player');
    
    await page.goto('/dashboard');
    
    await expect(page.locator('h1, text=Dashboard')).toBeVisible();
  });

  test('unauthenticated user redirected to login', async ({ page }) => {
    await page.goto('/dashboard');
    
    // Should redirect to sign in
    await expect(page).toHaveURL(/\/users\/sign_in/);
  });

  test('different roles see appropriate dashboard content', async ({ page }) => {
    // Test super admin dashboard
    await login(page, 'super_admin');
    await page.goto('/dashboard');
    // Add assertions for super admin specific content
    
    // Test player dashboard
    await page.click('text=Logout');
    await login(page, 'player');
    await page.goto('/dashboard');
    // Add assertions for player specific content
  });
});
```

---

### Phase 4: Running Tests (Day 5)

#### 4.1 Test Execution Commands

```bash
# Run all E2E tests
npm run e2e

# Run specific test file
npm run e2e e2e/auth/login.spec.ts

# Run in headed mode (see browser)
npm run e2e:headed

# Run in debug mode (step through)
npm run e2e:debug

# Run in UI mode (interactive)
npm run e2e:ui

# View test report
npm run e2e:report
```

#### 4.2 CI/CD Integration

**File:** `.github/workflows/e2e.yml` (if using GitHub Actions)

```yaml
name: E2E Tests

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

jobs:
  test:
    runs-on: ubuntu-latest
    
    services:
      postgres:
        image: postgres:16
        env:
          POSTGRES_USER: postgres
          POSTGRES_PASSWORD: postgres
          POSTGRES_DB: diquis_test
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 5432:5432
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup Ruby
        uses: ruby/setup-ruby@v1
        with:
          ruby-version: 3.3.7
          bundler-cache: true
      
      - name: Setup Node
        uses: actions/setup-node@v4
        with:
          node-version: 20
          cache: 'npm'
      
      - name: Install dependencies
        run: |
          npm ci
          bundle install
      
      - name: Setup database
        env:
          RAILS_ENV: test
          DATABASE_URL: postgresql://postgres:postgres@localhost:5432/diquis_test
        run: |
          bin/rails db:create
          bin/rails db:schema:load
          bin/rails db:seed
      
      - name: Install Playwright browsers
        run: npx playwright install --with-deps chromium
      
      - name: Run E2E tests
        env:
          RAILS_ENV: test
          DATABASE_URL: postgresql://postgres:postgres@localhost:5432/diquis_test
          CI: true
        run: npm run e2e
      
      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: playwright-report
          path: playwright-report/
          retention-days: 30
```

---

## Important Notes for Implementation

### 1. Seed Data Requirements

Ensure `db/seeds.rb` creates test users with known credentials:

```ruby
# db/seeds.rb
unless Rails.env.production?
  default_password = ENV.fetch("SEED_DEFAULT_PASSWORD", "Dev3l0pment!2025")
  
  # Super Admin
  User.find_or_create_by!(email: "admin@diquis.com") do |user|
    user.password = default_password
    user.password_confirmation = default_password
    user.role = :super_admin
    user.confirmed_at = Time.current
  end
  
  # Academy Admin
  User.find_or_create_by!(email: "academy@diquis.com") do |user|
    user.password = default_password
    user.password_confirmation = default_password
    user.role = :academy_admin
    user.confirmed_at = Time.current
  end
  
  # Coach
  User.find_or_create_by!(email: "coach@diquis.com") do |user|
    user.password = default_password
    user.password_confirmation = default_password
    user.role = :coach
    user.confirmed_at = Time.current
  end
  
  # Player
  User.find_or_create_by!(email: "player@diquis.com") do |user|
    user.password = default_password
    user.password_confirmation = default_password
    user.role = :player
    user.first_name = "Test"
    user.last_name = "Player"
    user.confirmed_at = Time.current
  end
end
```

### 2. Test Environment Setup

Ensure test environment is configured:

**File:** `config/environments/test.rb`

```ruby
# Add or verify these settings
config.action_mailer.delivery_method = :test
config.action_mailer.default_url_options = { host: 'localhost', port: 3000 }

# Disable CSRF protection in test (for API endpoints)
config.action_controller.allow_forgery_protection = false
```

### 3. Database Cleanup

Before running tests:

```bash
RAILS_ENV=test bin/rails db:drop db:create db:schema:load db:seed
```

### 4. Selectors Best Practices

**Recommended selector strategies:**

1. Use `data-testid` attributes for stable selectors
2. Use text content for user-facing elements
3. Avoid CSS classes (they change with styling)
4. Use semantic role selectors when possible

**Example:**

```typescript
// Good
await page.click('[data-testid="submit-button"]');
await page.click('button:has-text("Create User")');
await page.click('role=button[name="Save"]');

// Avoid
await page.click('.btn.btn-primary.btn-lg');
```

### 5. Common Gotchas

**Issue:** Tests fail with "Element not found"  
**Solution:** Inertia navigation can be slow. Use proper waiting:

```typescript
await page.waitForURL('/expected-url');
await page.waitForSelector('text=Expected content');
```

**Issue:** Form submission doesn't work  
**Solution:** Ensure you're clicking the submit button, not using `page.submit()`:

```typescript
// Correct
await page.click('button[type="submit"]');

// May not work with Inertia
await page.locator('form').submit();
```

**Issue:** Logout test fails intermittently.
**Solution:** The Inertia.js `Link` component with `method="delete"` can be unreliable in tests. The `logout` helper in `e2e/helpers/auth.ts` has been modified to directly send a `DELETE` request to the backend, which is a more robust solution.

---

## Success Criteria

After implementation, you should have:

✅ 30+ E2E tests covering critical paths  
✅ All tests passing in local environment  
✅ CI/CD pipeline running E2E tests  
✅ Test report generated with screenshots/videos for failures  
✅ Database validation for all create/update/delete operations  
✅ Audit trail validation for security-critical operations  
✅ Role-based authorization tests preventing unauthorized access  

---

## Next Steps After Implementation

1. **Add Visual Regression Testing**

   ```typescript
   await expect(page).toHaveScreenshot('user-list.png');
   ```

2. **Add Performance Testing**

   ```typescript
   const startTime = Date.now();
   await page.goto('/admin/users');
   const loadTime = Date.now() - startTime;
   expect(loadTime).toBeLessThan(2000); // 2 seconds
   ```

3. **Add Accessibility Testing**

   ```bash
   npm install -D @axe-core/playwright
   ```

4. **Add Network Mocking for Edge Cases**

   ```typescript
   await page.route('**/admin/users', route => {
     route.fulfill({ status: 500 });
   });
   ```

---

## Maintenance

**Weekly:**

- Review test failures and update selectors if UI changed
- Add tests for new features

**Monthly:**

- Update Playwright: `npm update @playwright/test`
- Review test execution time and optimize slow tests
- Update test data fixtures if schema changed

**Per Release:**

- Run full E2E suite before deployment
- Review test coverage gaps
- Update documentation if workflows changed

---

## Contact & Support

For questions about this implementation plan:

- Check Playwright docs: https://playwright.dev
- Review Inertia.js testing guide: https://inertiajs.com/testing
- Rails testing guide: https://guides.rubyonrails.org/testing.html

This plan should be sufficient for any developer or AI agent to implement E2E testing without additional context.
