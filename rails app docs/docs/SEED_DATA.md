# Seed Data Guide

This document explains how to use the seed data in the Diquis application for development and testing.

## Overview

The seed data creates a complete set of test users representing all roles in the football academy system:

- Administrative users (Super Admin, Academy Owner, Academy Admin)
- Coaching staff
- Support staff  
- Parents/Guardians
- Players
- Player-Guardian relationships

## Password Configuration

For security, all seed passwords use a configurable default password instead of hardcoded values.

### Setting the Password

You can configure the default seed password in three ways (in order of precedence):

1. **Environment Variable** (Recommended for local development):

   ```bash
   export SEED_DEFAULT_PASSWORD="YourSecurePassword!123"
   ```

2. **Rails Credentials** (Recommended for team sharing):

   ```bash
   rails credentials:edit
   ```

   Add:

   ```yaml
   seed:
     default_password: "YourSecurePassword!123"
   ```

3. **Fallback Default**: If neither is set, uses `Dev3l0pment!2025`

### Using .env File

For local development, copy `.env.example` to `.env` and set your password:

```bash
cp .env.example .env
# Edit .env and set SEED_DEFAULT_PASSWORD
```

**Important**: The `.env` file is gitignored and should never be committed.

## Running Seeds

```bash
# Seed the database
rails db:seed

# Reset and seed (drops, creates, migrates, seeds)
rails db:reset

# In Docker
bin/docker/db seed
```

## Created Accounts

After seeding, you can log in with any of these accounts (all use the configured default password):

### Administrative

- **Super Admin**: admin@diquis.com
- **Academy Owner**: owner@diquis.com  
- **Academy Admin**: admin.academy@diquis.com

### Coaching Staff

- **Head Coach**: coach.main@diquis.com
- **Assistant Coach**: coach.assistant@diquis.com
- **Youth Coach**: coach.youth@diquis.com

### Support Staff

- **Fitness Trainer**: staff.fitness@diquis.com
- **Medical Staff**: staff.medical@diquis.com
- **Equipment Manager**: staff.equipment@diquis.com

### Parents

- parent1@example.com through parent5@example.com

### Players

- player1@example.com through player6@example.com

## Player-Guardian Relationships

The seed data creates realistic family structures:

- Players 1 & 2 (Carlos and Sofia Pérez) share parents 1 & 4 (siblings)
- Player 3 (Diego) has parent 2 (single parent)
- Player 4 (Valentina) has parent 3
- Player 5 (Mateo) has parent 5
- Player 6 (Isabella) has a pending invitation from parent 2

## Security Notes

⚠️ **Important Security Practices**:

1. **Development Only**: Seeds only run in non-production environments (protected by `Rails.env.production?` check)

2. **Never in Production**: Production accounts should ALWAYS be created manually through the admin interface with unique, secure passwords

3. **No Hardcoded Secrets**: The seed file uses environment variables or credentials, not hardcoded passwords in the repository

4. **GitGuardian**: The fallback default password includes `# ggignore` to prevent false-positive security alerts

5. **Strong Passwords**: Default passwords must meet the application's password requirements:
   - Minimum 12 characters
   - Contains uppercase letters
   - Contains lowercase letters
   - Contains digits
   - Contains special characters

## Idempotency

The seed script is idempotent - it can be run multiple times safely:

- Uses `find_or_initialize_by` to avoid duplicates
- Skips existing records and only creates new ones
- Safe to run after data modifications

## Customization

To add more seed data:

1. Edit `db/seeds.rb`
2. Follow the existing pattern using `create_user` helper
3. Use `DEFAULT_SEED_PASSWORD` for all passwords
4. Maintain idempotency with `find_or_initialize_by`

## Troubleshooting

### Password Requirements Not Met

If you see an error about password requirements:

```txt
Password must be at least 12 characters and include...
```

Update your `SEED_DEFAULT_PASSWORD` to meet the requirements listed above.

### Seeds Don't Run

- Verify you're not in production: `echo $RAILS_ENV`
- Check the environment variable is set: `echo $SEED_DEFAULT_PASSWORD`
- Try running with explicit environment: `RAILS_ENV=development rails db:seed`

### Duplicate Records

If you accidentally created duplicates:

```bash
rails db:reset  # This will drop and recreate everything
```
