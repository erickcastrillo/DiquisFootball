# Diquis Roles and Permissions

This document outlines the roles and their corresponding permissions within the Diquis application.

## Role Definitions

### `super_user`
- **Description**: This is the highest-level administrator, equivalent to a root user.
- **Permissions**:
    - Can manage all aspects of the system across all tenants.
    - Has full access to all data.
    - Can create, edit, and delete tenants.
    - Can perform system-wide maintenance and configuration.

### `academy_owner`
- **Description**: This role is assigned to the individual who signs up and owns an academy.
- **Permissions**:
    - Can manage all settings and information scoped to their own academy (e.g., academy name, logos).
    - Can create, edit, and manage all users within their academy, including `academy_admin`, `director_of_football`, `coach`, `player`, and `parent`.
    - Cannot create `super_user` roles.
    - Has full access to all data within their academy's tenant.

### `academy_admin`
- **Description**: A role designed for administrative staff, like a secretary, to assist with daily operations.
- **Permissions**:
    - Can manage academy inventory.
    - Can process payments and manage financial records.
    - Can assist the `academy_owner` or `director_of_football` with administrative tasks.
    - Generally has read/write access to operational data but not sensitive configuration.

### `director_of_football`
- **Description**: The head of all football-related activities within an academy. This role reports to the `academy_owner`.
- **Permissions**:
    - Can create and manage `coach` users.
    - Can create and manage teams, players, and their associated data.
    - Can schedule and manage tournaments, matches, and training sessions.
    - Has oversight of all football operations.

### `coach`
- **Description**: A role for team coaches.
- **Permissions**:
    - Can interact with player information for their assigned teams (create, read, update, delete).
    - Can manage training session information.
    - Can view team rosters and player profiles.

### `parent`
- **Description**: This role is for the parents or guardians of players.
- **Permissions**:
    - Can only view data related to their own children (players).
    - Can edit personal information for their children (e.g., name, email, phone number).
    - **Cannot** edit sensitive football-related data like biometric or skill information.

### `player`
- **Description**: The base role for a player within the academy.
- **Permissions**:
    - Can view their own profile and team information.
    - Can interact with features made available to them, such as marking attendance.
    - This is the most restrictive role.
