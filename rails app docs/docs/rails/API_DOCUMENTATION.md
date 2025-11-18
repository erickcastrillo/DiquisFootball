# Diquis - API Documentation

## Table of Contents

1. [API Overview](#api-overview)
2. [Authentication](#authentication)
3. [Academy Management API](#academy-management-api)
4. [Player Management API](#player-management-api)
5. [Team Management API](#team-management-api)
6. [Training Management API](#training-management-api)
7. [Asset Management API](#asset-management-api)
8. [Reporting & Analytics API](#reporting--analytics-api)
9. [Communication API](#communication-api)
10. [Shared Resources API](#shared-resources-api)
11. [Error Handling](#error-handling)

---

## API Overview

### Base URL

```txt
Development: http://localhost:3000/api/v1
Production:  https://api.diquis.com/api/v1
```text

### Content Type

All requests and responses use JSON:

```txt
Content-Type: application/json
Accept: application/json
```text

### API Versioning

The API uses URL-based versioning:

- Current version: `v1`
- Future versions: `v2`, `v3`, etc.

### Response Format

All successful responses follow this structure:

```json
{
  "data": { ... },
  "meta": { ... }
}
```text

Collection responses include pagination:

```json
{
  "data": [ ... ],
  "meta": {
    "pagination": {
      "current_page": 1,
      "per_page": 25,
      "total_pages": 10,
      "total_count": 250,
      "has_next_page": true,
      "has_prev_page": false
    }
  }
}
```text

---

## Authentication

### Login

**Endpoint:** `POST /auth/sign_in`

**Request Body:**

```json
{
  "user": {
    "email": "user@example.com",
    "password": "password123"
  }
}
```text

**Response (200 OK):**

```json
{
  "data": {
    "id": 123,
    "email": "user@example.com",
    "created_at": "2025-10-13T10:00:00Z"
  },
  "token": "eyJhbGciOiJIUzI1NiJ9..."
}
```text

**Headers:**

```txt
Authorization: Bearer eyJhbGciOiJIUzI1NiJ9...
```text

### Register

**Endpoint:** `POST /auth/sign_up`

**Request Body:**

```json
{
  "user": {
    "email": "newuser@example.com",
    "password": "password123",
    "password_confirmation": "password123"
  }
}
```text

### Logout

**Endpoint:** `DELETE /auth/sign_out`

**Headers:**

```txt
Authorization: Bearer {token}
```text

---

## Academy Management API

### List Academies

**Endpoint:** `GET /api/v1/academies`

**Query Parameters:**

- `page` - Page number (default: 1)
- `per_page` - Items per page (default: 25, max: 100)
- `search` - Search by name or location
- `country` - Filter by country
- `is_active` - Filter by active status (true/false)

**Example Request:**

```bash
GET /api/v1/academies?search=barcelona&page=1&per_page=10
```text

**Response (200 OK):**

```json
{
  "data": [
    {
      "id": 1,
      "slug": "550e8400-e29b-41d4-a716-446655440000",
      "name": "FC Barcelona Academy",
      "description": "Professional football academy",
      "city": "Barcelona",
      "country": "Spain",
      "owner_name": "Joan Laporta",
      "owner_email": "owner@fcbarcelona.com",
      "is_active": true,
      "logo_url": "https://storage.example.com/logos/barcelona.jpg",
      "created_at": "2025-01-15T10:00:00Z",
      "updated_at": "2025-10-13T10:00:00Z"
    }
  ],
  "meta": {
    "pagination": {
      "current_page": 1,
      "per_page": 10,
      "total_pages": 1,
      "total_count": 1,
      "has_next_page": false,
      "has_prev_page": false
    }
  }
}
```text

### Get Academy Details

**Endpoint:** `GET /api/v1/academies/{slug}`

**Response (200 OK):**

```json
{
  "data": {
    "id": 1,
    "slug": "550e8400-e29b-41d4-a716-446655440000",
    "name": "FC Barcelona Academy",
    "description": "Professional football academy in Barcelona",
    "owner_name": "Joan Laporta",
    "owner_email": "owner@fcbarcelona.com",
    "owner_phone": "+34-123-456-789",
    "address_line_1": "Camp Nou, Av. d'Arístides Maillol",
    "address_line_2": null,
    "city": "Barcelona",
    "state_province": "Catalonia",
    "postal_code": "08028",
    "country": "Spain",
    "founded_date": "2010-06-15",
    "website": "https://www.fcbarcelona.com",
    "is_active": true,
    "logo_url": "https://storage.example.com/logos/barcelona.jpg",
    "created_at": "2025-01-15T10:00:00Z",
    "updated_at": "2025-10-13T10:00:00Z"
  },
  "meta": {
    "resource_type": "academy"
  }
}
```text

### Create Academy

**Endpoint:** `POST /api/v1/academies`

**Request Body:**

```json
{
  "academy": {
    "name": "New Football Academy",
    "description": "A new academy for young talents",
    "owner_name": "John Doe",
    "owner_email": "john@newacademy.com",
    "owner_phone": "+1-555-123-4567",
    "address_line_1": "123 Football Street",
    "city": "New York",
    "state_province": "NY",
    "postal_code": "10001",
    "country": "USA",
    "founded_date": "2025-10-13",
    "website": "https://newacademy.com"
  }
}
```text

**Response (201 Created):**

```json
{
  "data": {
    "id": 2,
    "slug": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "name": "New Football Academy",
    ...
  },
  "message": "Academy created successfully"
}
```text

### Update Academy

**Endpoint:** `PATCH /api/v1/academies/{slug}`

**Request Body:**

```json
{
  "academy": {
    "name": "Updated Academy Name",
    "website": "https://updated-website.com"
  }
}
```text

---

## Player Management API

### List Players

**Endpoint:** `GET /api/v1/{academy_slug}/players`

**Query Parameters:**

- `page` - Page number
- `per_page` - Items per page
- `search` - Search by name or parent name
- `position` - Filter by position slug
- `category` - Filter by category slug
- `age_min` - Minimum age
- `age_max` - Maximum age
- `gender` - Filter by gender (M/F/NB/PNTS)
- `is_active` - Filter by active status
- `include` - Include relationships (position,category,teams)

**Example Request:**

```bash
GET /api/v1/550e8400-e29b-41d4-a716-446655440000/players?age_min=10&age_max=15&include=position,category
```text

**Response (200 OK):**

```json
{
  "data": [
    {
      "id": 100,
      "slug": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "first_name": "Lionel",
      "last_name": "Messi",
      "full_name": "Lionel Messi",
      "age": 15,
      "gender": "M",
      "foot": "L",
      "parent_name": "Jorge Messi",
      "parent_email": "jorge@example.com",
      "phone_number": "+34-123-456-789",
      "is_active": true,
      "picture_url": "https://storage.example.com/players/messi.jpg",
      "registration_date": "2025-01-10T10:00:00Z",
      "position": {
        "id": 1,
        "slug": "pos-12345",
        "name": "Forward",
        "abbreviation": "FW"
      },
      "category": {
        "id": 5,
        "slug": "cat-67890",
        "name": "U-16",
        "language": "en"
      },
      "created_at": "2025-01-10T10:00:00Z",
      "updated_at": "2025-10-13T10:00:00Z"
    }
  ],
  "meta": {
    "pagination": { ... }
  }
}
```text

### Get Player Details

**Endpoint:** `GET /api/v1/{academy_slug}/players/{slug}`

**Query Parameters:**

- `include` - Include relationships (position,category,teams,skills)

**Response (200 OK):**

```json
{
  "data": {
    "id": 100,
    "slug": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "first_name": "Lionel",
    "last_name": "Messi",
    "full_name": "Lionel Messi",
    "age": 15,
    "gender": "M",
    "foot": "L",
    "parent_name": "Jorge Messi",
    "parent_email": "jorge@example.com",
    "phone_number": "+34-123-456-789",
    "is_active": true,
    "picture_url": "https://storage.example.com/players/messi.jpg",
    "registration_date": "2025-01-10T10:00:00Z",
    "position": { ... },
    "category": { ... },
    "teams": [ ... ],
    "player_skills": [
      {
        "skill_name": "Dribbling",
        "level": 5,
        "notes": "Exceptional dribbling skills",
        "assessed_at": "2025-10-01T10:00:00Z"
      }
    ],
    "created_at": "2025-01-10T10:00:00Z",
    "updated_at": "2025-10-13T10:00:00Z"
  },
  "meta": {
    "resource_type": "player"
  }
}
```text

### Register New Player

**Endpoint:** `POST /api/v1/{academy_slug}/players`

**Request Body (multipart/form-data for file upload):**

```json
{
  "player": {
    "first_name": "Cristiano",
    "last_name": "Ronaldo",
    "age": 16,
    "gender": "M",
    "foot": "R",
    "parent_name": "Maria Dolores",
    "parent_email": "maria@example.com",
    "phone_number": "+351-123-456-789",
    "position_slug": "pos-12345",
    "category_slug": "cat-67890",
    "picture": "(file upload)"
  }
}
```text

**Response (201 Created):**

```json
{
  "data": {
    "id": 101,
    "slug": "new-player-slug",
    "first_name": "Cristiano",
    "last_name": "Ronaldo",
    ...
  },
  "message": "Player created successfully"
}
```text

**Validation Errors (422 Unprocessable Entity):**

```json
{
  "error": "VALIDATION_ERROR",
  "message": "Validation failed",
  "details": [
    "Age must be greater than 4",
    "Parent email is invalid",
    "Player age (18) exceeds category maximum (16)"
  ],
  "field_errors": {
    "age": ["must be greater than 4"],
    "parent_email": ["is invalid"]
  }
}
```text

### Update Player

**Endpoint:** `PATCH /api/v1/{academy_slug}/players/{slug}`

**Request Body:**

```json
{
  "player": {
    "age": 16,
    "phone_number": "+351-999-888-777"
  }
}
```text

### Search Players (Advanced)

**Endpoint:** `GET /api/v1/{academy_slug}/players/search`

**Query Parameters:**

- `q` - Search query (name, parent name)
- `filters[position]` - Position slug
- `filters[category]` - Category slug
- `filters[age_range]` - e.g., "10-15"
- `filters[gender]` - Gender filter
- `sort` - Sort fields (e.g., "age:desc,last_name:asc")

---

## Team Management API

### List Teams

**Endpoint:** `GET /api/v1/{academy_slug}/teams`

**Query Parameters:**

- `page`, `per_page`
- `category` - Filter by category
- `is_active` - Filter by active status
- `include` - Include relationships (players,category,division)

**Response (200 OK):**

```json
{
  "data": [
    {
      "id": 10,
      "slug": "team-slug-123",
      "name": "U-16 Barcelona A",
      "coach": "Pep Guardiola",
      "is_active": true,
      "category": {
        "id": 5,
        "name": "U-16"
      },
      "division": {
        "id": 1,
        "name": "Primera"
      },
      "player_count": 22,
      "created_at": "2025-01-15T10:00:00Z",
      "updated_at": "2025-10-13T10:00:00Z"
    }
  ],
  "meta": { ... }
}
```text

### Create Team

**Endpoint:** `POST /api/v1/{academy_slug}/teams`

**Request Body:**

```json
{
  "team": {
    "name": "U-14 Team A",
    "category_slug": "cat-u14",
    "division_slug": "div-amateur",
    "coach": "John Smith"
  }
}
```text

### Add Player to Team

**Endpoint:** `POST /api/v1/{academy_slug}/teams/{slug}/add_player`

**Request Body:**

```json
{
  "player_slug": "player-slug-123"
}
```text

### Remove Player from Team

**Endpoint:** `DELETE /api/v1/{academy_slug}/teams/{slug}/players/{player_slug}`

---

## Training Management API

### List Team Trainings

**Endpoint:** `GET /api/v1/{academy_slug}/teams/{team_slug}/trainings`

**Query Parameters:**

- `date_from` - Filter from date (YYYY-MM-DD)
- `date_to` - Filter to date (YYYY-MM-DD)
- `training_type` - Filter by type (Technical, Tactical, Physical, Fitness, Scrimmage)

**Response (200 OK):**

```json
{
  "data": [
    {
      "id": 50,
      "slug": "training-slug-123",
      "place": "Camp Nou Training Ground",
      "date": "2025-10-15",
      "time": "16:00:00",
      "duration": "01:30:00",
      "training_type": "Technical",
      "coach": "Xavi Hernandez",
      "description": "Focus on ball control and passing",
      "attendance_count": 18,
      "total_players": 20,
      "created_at": "2025-10-10T10:00:00Z",
      "updated_at": "2025-10-13T10:00:00Z"
    }
  ],
  "meta": { ... }
}
```text

### Schedule Training

**Endpoint:** `POST /api/v1/{academy_slug}/teams/{team_slug}/trainings`

**Request Body:**

```json
{
  "training": {
    "place": "Main Training Field",
    "date": "2025-10-20",
    "time": "17:00",
    "duration_minutes": 90,
    "training_type": "Tactical",
    "coach": "Carlo Ancelotti",
    "description": "Team formation and tactics"
  }
}
```text

**Response (201 Created):**

```json
{
  "data": {
    "id": 51,
    "slug": "new-training-slug",
    ...
  },
  "message": "Training scheduled successfully"
}
```text

**Validation Errors (422):**

```json
{
  "error": "VALIDATION_ERROR",
  "message": "Validation failed",
  "details": [
    "Training date cannot be in the past",
    "Training time conflicts with existing training session"
  ],
  "field_errors": {
    "date": ["cannot be in the past"],
    "time": ["conflicts with existing training session"]
  }
}
```text

### Bulk Update Attendance

**Endpoint:** `POST /api/v1/{academy_slug}/teams/{team_slug}/trainings/{slug}/bulk_attendance`

**Request Body:**

```json
{
  "attendances": [
    {
      "player_slug": "player-1-slug",
      "status": "present",
      "notes": "Great performance"
    },
    {
      "player_slug": "player-2-slug",
      "status": "absent",
      "notes": "Injury"
    },
    {
      "player_slug": "player-3-slug",
      "status": "late",
      "notes": "Arrived 15 minutes late"
    }
  ]
}
```text

**Response (200 OK):**

```json
{
  "data": {
    "training_slug": "training-slug-123",
    "updated_count": 3,
    "attendances": [
      {
        "player_slug": "player-1-slug",
        "player_name": "Lionel Messi",
        "status": "present"
      },
      ...
    ]
  },
  "message": "Attendance updated successfully"
}
```text

### Get Attendance Report

**Endpoint:** `GET /api/v1/{academy_slug}/teams/{team_slug}/trainings/{slug}/attendance_report`

**Response (200 OK):**

```json
{
  "data": {
    "training": {
      "date": "2025-10-15",
      "time": "16:00:00",
      "place": "Main Field"
    },
    "summary": {
      "total_players": 20,
      "present": 18,
      "absent": 1,
      "late": 1,
      "attendance_rate": 90.0
    },
    "details": [
      {
        "player_slug": "player-1",
        "player_name": "Lionel Messi",
        "status": "present",
        "notes": null
      },
      ...
    ]
  }
}
```text

### Training Calendar

**Endpoint:** `GET /api/v1/{academy_slug}/teams/{team_slug}/trainings/calendar`

**Query Parameters:**

- `month` - Month (1-12)
- `year` - Year (YYYY)

**Response (200 OK):**

```json
{
  "data": {
    "month": 10,
    "year": 2025,
    "trainings": [
      {
        "date": "2025-10-15",
        "trainings": [
          {
            "slug": "training-1",
            "time": "16:00:00",
            "duration": "01:30:00",
            "type": "Technical",
            "place": "Main Field"
          }
        ]
      },
      ...
    ]
  }
}
```text

---

## Asset Management API

### List Assets

**Endpoint:** `GET /api/v1/{academy_slug}/assets`

**Query Parameters:**

- `category` - Filter by asset category
- `condition` - Filter by condition (new, good, fair, poor, needs_repair, retired)
- `location` - Filter by location
- `allocated` - Filter by allocation status (true/false)
- `search` - Search by name, description, brand, model
- `include` - Include relationships (category,allocations,maintenance_records)

### Create Asset

**Endpoint:** `POST /api/v1/{academy_slug}/assets`

**Request Body:**

```json
{
  "asset": {
    "name": "Training Soccer Ball",
    "description": "Size 5 soccer ball for training",
    "asset_category_slug": "equipment-balls",
    "brand": "Nike",
    "model": "Strike Team",
    "purchase_price": 25.99,
    "purchase_date": "2025-10-13",
    "condition": "new",
    "location": "Equipment Room A"
  }
}
```text

### Allocate Asset

**Endpoint:** `POST /api/v1/{academy_slug}/assets/{slug}/allocate`

**Request Body:**

```json
{
  "allocation": {
    "allocatable_type": "Player",
    "allocatable_slug": "player-slug-123",
    "expected_return_at": "2025-12-31T23:59:59Z",
    "checkout_notes": "Player uniform for season"
  }
}
```text

### Asset Maintenance

**Endpoint:** `POST /api/v1/{academy_slug}/assets/{slug}/maintenance`

**Request Body:**

```json
{
  "maintenance": {
    "maintenance_type": "corrective",
    "description": "Repair torn seam",
    "performed_at": "2025-10-13T14:30:00Z",
    "performed_by": "Equipment Manager",
    "cost": 15.50,
    "next_maintenance_due": "2026-01-15"
  }
}
```text

---

## Reporting & Analytics API

### Generate Financial Report

**Endpoint:** `POST /api/v1/{academy_slug}/reports/financial/generate`

**Request Body:**

```json
{
  "report": {
    "report_type": "profit_loss",
    "date_range_start": "2025-01-01",
    "date_range_end": "2025-10-13",
    "format": "pdf",
    "include_charts": true,
    "currency": "USD"
  }
}
```text

### Get Revenue Analytics

**Endpoint:** `GET /api/v1/{academy_slug}/analytics/revenue`

**Query Parameters:**

- `start_date` - Start date (YYYY-MM-DD)
- `end_date` - End date (YYYY-MM-DD)
- `category` - Filter by revenue category
- `compare_previous` - Include previous period comparison (true/false)

**Response:**

```json
{
  "data": {
    "total_revenue": 125000.00,
    "revenue_by_category": {
      "registration_fees": 80000.00,
      "merchandise": 15000.00,
      "camps": 30000.00
    },
    "growth_rate": 12.5,
    "previous_period_comparison": {
      "total_revenue": 111607.14,
      "growth_percentage": 12.0
    }
  }
}
```text

### Player Development Analytics

**Endpoint:** `GET /api/v1/{academy_slug}/analytics/player-development`

**Query Parameters:**

- `player_ids` - Specific players (comma-separated slugs)
- `start_date` - Analysis start date
- `end_date` - Analysis end date
- `metric_types` - Specific metrics (attendance_rate,skill_improvement,performance_score)

### Scheduled Reports

**Endpoint:** `GET /api/v1/{academy_slug}/reports/scheduled`

**Response:**

```json
{
  "data": [
    {
      "id": 1,
      "slug": "monthly-revenue-report",
      "name": "Monthly Revenue Report",
      "report_type": "financial",
      "schedule_frequency": "monthly",
      "next_generation_at": "2025-11-01T09:00:00Z",
      "last_generated_at": "2025-10-01T09:15:23Z"
    }
  ]
}
```text

---

## Communication API

### Send Message

**Endpoint:** `POST /api/v1/{academy_slug}/messages`

**Request Body:**

```json
{
  "message": {
    "subject": "Training Session Update",
    "content": "Tomorrow's training session has been moved to Field B due to maintenance.",
    "recipient_type": "Team",
    "recipient_slugs": ["team-u16-a"],
    "delivery_method": "email",
    "is_emergency": false,
    "scheduled_at": "2025-10-13T18:00:00Z"
  }
}
```text

### Parent Portal Access

**Endpoint:** `GET /api/v1/{academy_slug}/parent-portal/{player_slug}`

**Headers:**

```txt
Authorization: Bearer {parent_token}
```text

**Response:**

```json
{
  "data": {
    "player": {
      "slug": "player-123",
      "first_name": "John",
      "last_name": "Doe",
      "age": 12,
      "team": "U-12 Team A",
      "next_training": "2025-10-15T16:00:00Z"
    },
    "recent_attendance": {
      "attendance_rate": 85.5,
      "sessions_attended": 17,
      "total_sessions": 20
    },
    "upcoming_events": [
      {
        "type": "training",
        "date": "2025-10-15T16:00:00Z",
        "location": "Main Field"
      }
    ],
    "outstanding_payments": [],
    "ai_training_available": true,
    "latest_home_training": {
      "generated_at": "2025-10-13T10:00:00Z",
      "focus_areas": ["ball_control", "passing"],
      "difficulty_level": "intermediate",
      "estimated_duration": 30
    }
  }
}
```text

### Generate AI-Powered Home Training

**Endpoint:** `POST /api/v1/{academy_slug}/parent-portal/{player_slug}/generate-training`

**Headers:**

```txt
Authorization: Bearer {parent_token}
```text

**Request Body:**

```json
{
  "training_request": {
    "duration_minutes": 30,
    "focus_areas": ["ball_control", "shooting"],
    "equipment_available": ["ball", "cones"],
    "space_type": "backyard",
    "difficulty_preference": "adaptive",
    "parent_involvement": true
  }
}
```text

**Response:**

```json
{
  "data": {
    "training_session": {
      "id": "home-training-456",
      "slug": "ht-456-2025-10-13",
      "generated_at": "2025-10-13T14:30:00Z",
      "player_slug": "player-123",
      "duration_minutes": 30,
      "difficulty_level": "intermediate",
      "focus_areas": ["ball_control", "shooting"],
      "exercises": [
        {
          "id": 1,
          "name": "Cone Dribbling Circuit",
          "description": "Set up 5 cones in a straight line, dribble through using both feet",
          "duration_minutes": 8,
          "sets": 3,
          "rest_seconds": 30,
          "difficulty": "medium",
          "equipment": ["ball", "cones"],
          "video_url": "https://videos.diquis.com/cone-dribbling.mp4",
          "instructions": [
            "Place 5 cones 2 yards apart",
            "Dribble through using only right foot",
            "Return using only left foot",
            "Focus on close ball control"
          ],
          "coaching_tips": [
            "Keep head up while dribbling",
            "Use inside and outside of foot",
            "Accelerate between cones"
          ],
          "progression": {
            "easier": "Increase cone spacing to 3 yards",
            "harder": "Add time pressure - complete in under 15 seconds"
          }
        },
        {
          "id": 2,
          "name": "Wall Pass Practice",
          "description": "Practice accurate passing and first touch using a wall",
          "duration_minutes": 10,
          "sets": 4,
          "rest_seconds": 45,
          "difficulty": "medium",
          "equipment": ["ball", "wall"],
          "video_url": "https://videos.diquis.com/wall-passing.mp4",
          "instructions": [
            "Stand 3 yards from wall",
            "Pass ball to wall with inside foot",
            "Control return with first touch",
            "Alternate feet each set"
          ],
          "coaching_tips": [
            "Strike through center of ball",
            "Prepare receiving foot early",
            "Keep passes firm and accurate"
          ],
          "progression": {
            "easier": "Move closer to wall (2 yards)",
            "harder": "Add movement - side step between passes"
          }
        },
        {
          "id": 3,
          "name": "Target Shooting",
          "description": "Improve shooting accuracy with target practice",
          "duration_minutes": 12,
          "sets": 3,
          "rest_seconds": 60,
          "difficulty": "medium",
          "equipment": ["ball", "cones", "goal_or_target"],
          "video_url": "https://videos.diquis.com/target-shooting.mp4",
          "instructions": [
            "Set up targets in corners of goal (or marked area)",
            "Take 5 shots at each target",
            "Use both feet",
            "Focus on accuracy over power"
          ],
          "coaching_tips": [
            "Plant standing foot firmly",
            "Keep head over ball at contact",
            "Follow through toward target"
          ],
          "progression": {
            "easier": "Move closer to target",
            "harder": "Add time pressure or moving ball"
          }
        }
      ],
      "warm_up": {
        "name": "Dynamic Warm-up",
        "duration_minutes": 5,
        "exercises": [
          "Light jogging in place - 1 minute",
          "High knees - 30 seconds",
          "Butt kicks - 30 seconds",
          "Leg swings - 30 seconds each leg",
          "Light ball touches - 2 minutes"
        ]
      },
      "cool_down": {
        "name": "Stretching Routine",
        "duration_minutes": 5,
        "exercises": [
          "Quad stretch - 30 seconds each leg",
          "Hamstring stretch - 30 seconds each leg",
          "Calf stretch - 30 seconds each leg",
          "Hip flexor stretch - 30 seconds each leg",
          "Light walking - 2 minutes"
        ]
      },
      "ai_personalization": {
        "based_on": [
          "Player's position: Midfielder",
          "Recent skill assessments: Ball control (7/10), Shooting (6/10)",
          "Coach feedback: 'Needs work on first touch and shooting accuracy'",
          "Training attendance: 85% (good consistency)",
          "Age group: U-12 (technical development focus)",
          "Previous home training completion: 80%"
        ],
        "adaptations": [
          "Increased ball control exercises due to recent assessment",
          "Added shooting drills to address coach feedback",
          "Medium difficulty to match current skill level",
          "30-minute duration based on age and attention span"
        ]
      },
      "parent_guidance": {
        "supervision_required": true,
        "safety_notes": [
          "Ensure adequate space (minimum 10x10 yards)",
          "Check area for obstacles before starting",
          "Have water available for hydration breaks",
          "Stop if player shows signs of fatigue"
        ],
        "encouragement_tips": [
          "Focus on effort over perfection",
          "Celebrate small improvements",
          "Make it fun with music or challenges",
          "Record progress with photos/videos"
        ],
        "how_to_help": [
          "Act as goalkeeper for shooting practice",
          "Time exercises and provide rest reminders",
          "Give positive feedback on technique",
          "Help set up equipment before starting"
        ]
      },
      "progress_tracking": {
        "completion_url": "/api/v1/{academy_slug}/parent-portal/{player_slug}/training-sessions/ht-456-2025-10-13/complete",
        "feedback_url": "/api/v1/{academy_slug}/parent-portal/{player_slug}/training-sessions/ht-456-2025-10-13/feedback",
        "metrics_to_track": [
          "exercises_completed",
          "difficulty_rating",
          "enjoyment_level",
          "areas_of_struggle",
          "notable_improvements"
        ]
      }
    },
    "next_generation_available": "2025-10-15T14:30:00Z"
  }
}
```text

### Complete Home Training Session

**Endpoint:** `POST /api/v1/{academy_slug}/parent-portal/{player_slug}/training-sessions/{session_slug}/complete`

**Request Body:**

```json
{
  "completion": {
    "completed_at": "2025-10-13T15:30:00Z",
    "exercises_completed": [1, 2, 3],
    "exercises_skipped": [],
    "actual_duration_minutes": 35,
    "difficulty_rating": 3,
    "enjoyment_level": 4,
    "parent_notes": "Great improvement in shooting accuracy. Struggled initially with cone dribbling but improved by the end.",
    "player_feedback": "Fun session! Want to work more on shooting.",
    "areas_of_struggle": ["cone_dribbling_speed"],
    "notable_improvements": ["shooting_accuracy", "first_touch"]
  }
}
```text

### Get Home Training History

**Endpoint:** `GET /api/v1/{academy_slug}/parent-portal/{player_slug}/training-sessions`

**Query Parameters:**

- `limit` - Number of sessions to return (default: 10)
- `completed` - Filter by completion status (true/false)
- `date_from` - Filter sessions from date
- `date_to` - Filter sessions to date

**Response:**

```json
{
  "data": [
    {
      "slug": "ht-456-2025-10-13",
      "generated_at": "2025-10-13T14:30:00Z",
      "completed_at": "2025-10-13T15:30:00Z",
      "focus_areas": ["ball_control", "shooting"],
      "duration_minutes": 30,
      "difficulty_level": "intermediate",
      "completion_rate": 100,
      "enjoyment_level": 4,
      "progress_score": 8.5
    }
  ],
  "meta": {
    "total_sessions": 15,
    "completed_sessions": 12,
    "completion_rate": 80,
    "average_enjoyment": 4.2,
    "improvement_trend": "improving"
  }
}
```text

### Message Delivery Status

**Endpoint:** `GET /api/v1/{academy_slug}/messages/{slug}/delivery-status`

**Response:**

```json
{
  "data": {
    "message_slug": "msg-123",
    "total_recipients": 25,
    "delivered": 23,
    "failed": 1,
    "pending": 1,
    "delivery_details": [
      {
        "recipient": "parent@example.com",
        "delivery_method": "email",
        "status": "delivered",
        "delivered_at": "2025-10-13T18:05:12Z"
      }
    ]
  }
}
```text

### Emergency Alert

**Endpoint:** `POST /api/v1/{academy_slug}/messages/emergency`

**Request Body:**

```json
{
  "alert": {
    "subject": "Weather Alert - Training Cancelled",
    "content": "Due to severe weather conditions, all training sessions for today are cancelled. We will notify you when sessions resume.",
    "recipient_type": "All",
    "delivery_methods": ["email", "sms", "push_notification"],
    "requires_acknowledgment": true
  }
}
```text

---

## Shared Resources API

### List Positions

**Endpoint:** `GET /api/v1/positions` (global) or `GET /api/v1/{academy_slug}/positions` (academy-specific)

**Response (200 OK):**

```json
{
  "data": [
    {
      "id": 1,
      "slug": "pos-gk",
      "name": "Goalkeeper",
      "abbreviation": "GK",
      "category": "Defensive",
      "description": "The player who guards the goal",
      "is_academy_specific": false
    },
    {
      "id": 2,
      "slug": "pos-def",
      "name": "Defender",
      "abbreviation": "DEF",
      "category": "Defensive",
      "is_academy_specific": false
    }
  ]
}
```text

### List Categories

**Endpoint:** `GET /api/v1/categories`

**Response (200 OK):**

```json
{
  "data": [
    {
      "id": 1,
      "slug": "cat-u8",
      "name": "U-8",
      "language": "en",
      "player_count": 45
    },
    {
      "id": 2,
      "slug": "cat-u10",
      "name": "U-10",
      "language": "en",
      "player_count": 52
    }
  ]
}
```text

### List Skills

**Endpoint:** `GET /api/v1/{academy_slug}/skills`

**Response (200 OK):**

```json
{
  "data": [
    {
      "id": 1,
      "slug": "skill-passing",
      "name": "Passing",
      "description": "Ability to pass the ball accurately",
      "category": "Technical",
      "is_academy_specific": true
    }
  ]
}
```text

---

## Error Handling

### Error Response Format

```json
{
  "error": "ERROR_CODE",
  "message": "Human-readable error message",
  "details": ["Additional error details"],
  "context": {
    "additional": "context information"
  }
}
```text

### Common Error Codes

#### 400 Bad Request

```json
{
  "error": "BAD_REQUEST",
  "message": "Invalid request parameters",
  "details": ["Missing required parameter: academy_slug"]
}
```text

#### 401 Unauthorized

```json
{
  "error": "UNAUTHORIZED",
  "message": "Authentication required",
  "context": {
    "reason": "Missing or invalid authentication token"
  }
}
```text

#### 403 Forbidden

```json
{
  "error": "PERMISSION_DENIED",
  "message": "You don't have permission to perform this action",
  "context": {
    "action": "create",
    "resource": "Player"
  }
}
```text

#### 404 Not Found

```json
{
  "error": "RESOURCE_NOT_FOUND",
  "message": "Player not found",
  "context": {
    "resource_type": "Player",
    "slug": "invalid-slug-123"
  }
}
```text

#### 422 Unprocessable Entity

```json
{
  "error": "VALIDATION_ERROR",
  "message": "Validation failed",
  "details": [
    "Age must be greater than 4",
    "Parent email is invalid"
  ],
  "field_errors": {
    "age": ["must be greater than 4"],
    "parent_email": ["is invalid"]
  }
}
```text

#### 500 Internal Server Error

```json
{
  "error": "INTERNAL_SERVER_ERROR",
  "message": "An unexpected error occurred",
  "context": {
    "request_id": "req-123456"
  }
}
```text

---

## Rate Limiting

### Default Limits

- **Authenticated requests:** 1000 requests per hour
- **Unauthenticated requests:** 100 requests per hour

### Rate Limit Headers

```txt
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 998
X-RateLimit-Reset: 1634140800
```text

### Rate Limit Exceeded (429)

```json
{
  "error": "RATE_LIMIT_EXCEEDED",
  "message": "API rate limit exceeded",
  "context": {
    "limit": 1000,
    "reset_at": "2025-10-13T15:00:00Z"
  }
}
```text

---

## Webhooks (Future Feature)

### Available Events

- `player.created`
- `player.updated`
- `training.scheduled`
- `training.attendance_updated`
- `academy.created`

### Webhook Payload Format

```json
{
  "event": "player.created",
  "timestamp": "2025-10-13T10:30:00Z",
  "data": {
    "academy_slug": "academy-123",
    "player": { ... }
  }
}
```text

---

## Testing the API

### Using cURL

```bash
# Get access token
TOKEN=$(curl -X POST http://localhost:3000/auth/sign_in \
  -H "Content-Type: application/json" \
  -d '{"user":{"email":"user@example.com","password":"password"}}' \
  | jq -r '.token')

# List players
curl -X GET "http://localhost:3000/api/v1/academy-slug/players" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json"
```text

### Using Swagger UI

Visit `http://localhost:3000/api-docs` for interactive API documentation and testing.

---

For more information, see:

- [PROJECT_OVERVIEW.md](./PROJECT_OVERVIEW.md)
- [ARCHITECTURE.md](./ARCHITECTURE.md)
- [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md)
