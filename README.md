# RaceDay – Event Management System

RaceDay is a full-stack, web-based event management system designed for the South African road running, walking, and cycling community. Event organisers can create and manage events, categories, enrolments, routes, and participant results. Participants can browse events, enter races, and track their performance history.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [User Roles](#user-roles)
- [Technology Stack](#technology-stack)
- [Repository Structure](#repository-structure)
- [Part 1 – System Planning and Database](#part-1--system-planning-and-database)
- [Part 2 – RESTful API](#part-2--restful-api)
- [API Endpoints](#api-endpoints)
- [Authentication and Authorisation](#authentication-and-authorisation)
- [Setup and Running the Project](#setup-and-running-the-project)
- [Testing](#testing)
- [CI/CD](#cicd)
- [Video Presentation](#video-presentation)
- [Submission Checklist](#submission-checklist)

## Overview

RaceDay addresses the challenges faced by event organisers who manage events using paper-based registration, spreadsheets, and disconnected communication channels. It brings the main event-management tasks together in one platform.

### Features

- Event creation and management
- Participant registration and enrolment
- Event category management
- Results tracking and certificates
- Event routes and course information
- Role-based access for organisers and participants
- API documentation through Swagger/OpenAPI
- Automated tests and GitHub Actions CI checks

## User Roles

### Organiser

- Create, edit, and delete their own events
- Manage categories for events they own
- Capture participant results for their events
- View enrolments for events they manage
- Access event statistics and reports

### Participant

- Create an account and log in
- Browse upcoming events
- Enter events by selecting a category
- View their own enrolments
- Track personal race results
- Access event details and routes

## Technology Stack

| Layer | Technology |
|---|---|
| Database | Microsoft SQL Server (SSMS) |
| ORM | Entity Framework Core 8 (Code First) |
| Backend | C# .NET 8 – ASP.NET Core Web API |
| Authentication | Server-side session using `.RaceDay.Session` cookie |
| API documentation | Swagger / OpenAPI |
| Testing | xUnit + `WebApplicationFactory` |
| CI/CD | GitHub Actions |
| Frontend | ASP.NET Core MVC (planned for Part 3) |
| Image upload | Azure Blob Storage (planned for Part 3) |
| Containerisation | Docker (planned for Part 3) |

## Repository Structure

```text
RaceDay/
├── README.md
├── docs/
│   ├── RaceDay_ERD.jpg
│   ├── API Endpoint Plan ST10479846.pdf
│   ├── RaceDay_Database.sql
│   └── ci-screenshot.png
├── RaceDay.Api/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProfileController.cs
│   │   ├── EventsController.cs
│   │   ├── CategoriesController.cs
│   │   ├── EnrolmentsController.cs
│   │   └── ResultsController.cs
│   ├── Data/
│   │   └── RaceDayDbContext.cs
│   ├── Filters/
│   │   ├── RequireLoginAttribute.cs
│   │   ├── PasswordHasher.cs
│   │   └── SessionExtensions.cs
│   ├── Models/
│   │   ├── AppUser.cs
│   │   ├── Event.cs
│   │   ├── Category.cs
│   │   ├── Enrolment.cs
│   │   ├── Result.cs
│   │   └── EventRoute.cs
│   ├── Program.cs
│   └── appsettings.json
├── RaceDay.Contracts/
│   └── Dtos.cs
├── RaceDay.Api.Tests/
│   ├── AuthTests.cs
│   └── CustomWebApplicationFactory.cs
└── .github/
    └── workflows/
        └── ci.yml
```

All SQL scripts have been consolidated into `docs/RaceDay_Database.sql`. The previous `/Database` folder has been removed so the database script is stored in the required `/docs` folder.

## Part 1 – System Planning and Database

### 1. Entity Relationship Diagram (ERD)

**File:** `docs/RaceDay_ERD.jpg`

The database design uses six core entities:

| Entity | Purpose |
|---|---|
| `Users` | Stores organisers and participants, identified by a role column. |
| `Events` | Road-running, walking, and cycling events created by an organiser. Includes `distance_km` and `event_type`. |
| `Categories` | Age or distance categories within an event, such as Open Men or Fun Ride. |
| `Enrolments` | Associative entity linking a participant, event, and category. |
| `Results` | Stores finish time and position; links only to an enrolment (one-to-one, optional result). |
| `EventRoutes` | Stores course information such as start/end point, distance, and altitude. |

#### Relationships and cardinality

- One organiser can create many events.
- One event can have many categories.
- One participant can have many enrolments.
- One event can have many enrolments.
- One category can have many enrolments.
- One enrolment can have zero or one result.
- One event can have one or more routes.

#### Design changes following Part 1 feedback

- Cardinality is explicitly labelled on every ERD relationship.
- `Results` links only to `Enrolments`; direct event and category foreign keys were removed.
- `Events` includes the required `distance_km` and `event_type` fields.
- `Payments` was removed because it is outside the six-entity scope for this part.

### 2. API Endpoint Plan

**File:** `docs/API Endpoint Plan ST10479846.pdf`

The endpoint plan covers the six resource groups and documents the method, route, description, required role, request body, and expected response.

Authentication uses server-side sessions rather than JWT tokens.

| Method | Route | Description | Role | Expected response |
|---|---|---|---|---|
| `POST` | `/api/auth/register` | Register an organiser or participant | Public | `201 Created`, `400 Bad Request`, or `409 Conflict` |
| `POST` | `/api/auth/login` | Authenticate and create a server-side session | Public | `200 OK` or `401 Unauthorized` |
| `POST` | `/api/auth/logout` | Clear the current session | Logged-in user | `200 OK` |

Event creation and update requests include `distanceKm` and `eventType`. Example request body:

```json
{
  "title": "Cape Town Marathon",
  "description": "42.2km city marathon",
  "startDate": "2026-03-08T06:00:00",
  "endDate": "2026-03-08T14:00:00",
  "location": "Cape Town, Western Cape",
  "distanceKm": 42.2,
  "eventType": "Road"
}
```

### 3. SQL Database Script

**File:** `docs/RaceDay_Database.sql`

The consolidated SQL script is intended to run from top to bottom on a clean SQL Server instance. It creates:

- `RaceDayDB`
- `Users`, with a role constraint for `Organiser` and `Participant`
- `Events`, including `distance_km` and `event_type`
- `Categories`
- `Enrolments`, with a unique constraint on `(user_id, event_id)`
- `Results`, linked to `Enrolments` only
- `EventRoutes`
- Indexes on foreign keys

The script includes sample data for two organisers, two participants, three events (Cape Town Cycle Tour, Comrades, and Two Oceans), event categories, sample enrolments, and sample results.

#### Database fixes following feedback

- Added the missing `CREATE TABLE Results` statement.
- Removed the `Payments` table and its orphan references because it is out of scope.
- Consolidated `SQLQuery1.sql` through `SQLQuery10.sql` into one script.

#### Run the SQL script

1. Open SQL Server Management Studio (SSMS).
2. Connect to your SQL Server instance, for example `.`\`SQLEXPRESS`.
3. Open `docs/RaceDay_Database.sql`.
4. Execute the script with **F5**.
5. Confirm that `RaceDayDB` appears in Object Explorer and that all six tables and the sample data are present.

## Part 2 – RESTful API

The solution contains three projects:

| Project | Purpose |
|---|---|
| `RaceDay.Api` | ASP.NET Core Web API, controllers, EF Core, and session authentication |
| `RaceDay.Contracts` | Shared DTOs used by the API and planned MVC application |
| `RaceDay.Api.Tests` | xUnit tests, including role-based failure scenarios |

## API Endpoints

| Method | Route | Role | Description |
|---|---|---|---|
| `POST` | `/api/auth/register` | Public | Register an organiser or participant |
| `POST` | `/api/auth/login` | Public | Log in and create a session |
| `POST` | `/api/auth/logout` | Logged-in user | Clear the session |
| `GET` | `/api/profile/me` | Any logged-in user | Get own profile |
| `PUT` | `/api/profile/me` | Any logged-in user | Update own profile |
| `GET` | `/api/events` | Any | List events |
| `GET` | `/api/events/{id}` | Any | View one event |
| `GET` | `/api/events/mine` | Organiser | List events owned by the organiser |
| `POST` | `/api/events` | Organiser | Create an event, including distance and event type |
| `PUT` | `/api/events/{id}` | Event owner | Update own event |
| `DELETE` | `/api/events/{id}` | Event owner | Delete own event |
| `GET` | `/api/events/{eventId}/categories` | Any | List categories for an event |
| `POST` | `/api/events/{eventId}/categories` | Event owner | Create a category |
| `PUT` | `/api/categories/{id}` | Event owner | Update a category |
| `DELETE` | `/api/categories/{id}` | Event owner | Delete a category |
| `POST` | `/api/enrolments` | Participant | Enter an event by selecting a category |
| `GET` | `/api/enrolments/mine` | Logged-in user | View own enrolments |
| `GET` | `/api/enrolments/event/{eventId}` | Event owner | View enrolments for own event |
| `PUT` | `/api/enrolments/{id}/status` | Event owner | Update enrolment status or bib number |
| `POST` | `/api/results` | Event owner | Capture a finish time and position |
| `GET` | `/api/results/mine` | Logged-in user | View own results |
| `GET` | `/api/results/event/{eventId}` | Event owner | View results for own event |

## Authentication and Authorisation

RaceDay uses server-side sessions as required by the project brief.

- Registration hashes the password using SHA-256 with a salt and stores the hash rather than the plain-text password.
- Login verifies the password hash and stores `UserId`, `Role`, and `FullName` in the session.
- Session cookie name: `.RaceDay.Session`.
- Session idle timeout: 30 minutes.
- Logout clears the session.
- A global `RequireLoginAttribute` filter rejects unauthenticated requests with `401 Unauthorized`.

Expected access and validation responses include:

- Participant calling an organiser-only endpoint: `403 Forbidden`.
- Organiser attempting to modify another organiser's event: `403 Forbidden`.
- Resource ID not found: `404 Not Found`.
- Invalid request body: `400 Bad Request`.
- Duplicate enrolment: `409 Conflict`.

## Setup and Running the Project

### Prerequisites

- Visual Studio 2022
- .NET 8 SDK
- SQL Server Express or another supported SQL Server instance
- SQL Server Management Studio (SSMS)

### 1. Clone and open the solution

Clone the repository, then open `RaceDay.sln` in Visual Studio 2022 and restore the NuGet packages.

### 2. Configure the database connection

Ensure SQL Server Express is running. If your SQL Server instance differs, update the connection string in `RaceDay.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=RaceDayDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Keep the structure of your existing `appsettings.json` and update the relevant connection-string value rather than replacing unrelated settings.

### 3. Apply the Entity Framework migration

In Visual Studio, open **Tools → NuGet Package Manager → Package Manager Console**. Set the default project to `RaceDay.Api`, then run:

```powershell
Add-Migration InitialRaceDayDatabase
Update-Database
```

Open SSMS and verify that `Users`, `Events`, `Categories`, `Enrolments`, `Results`, and `EventRoutes` exist.

### 4. Run the API

Press **F5** in Visual Studio. Swagger should open at:

```text
https://localhost:{port}/swagger
```

Replace `{port}` with the HTTPS port assigned by your local launch settings.

## Testing

Tests are located in `RaceDay.Api.Tests` and use an in-memory database through `CustomWebApplicationFactory`.

Scenarios covered include:

- Valid registration returns `201 Created`.
- Valid login returns `200 OK` and creates a session.
- Incorrect password returns `401 Unauthorized`.
- An unauthenticated request to a protected endpoint returns `401 Unauthorized`.
- A participant attempting to create an event receives `403 Forbidden`.
- An organiser creating an event receives `201 Created`.
- A participant enrolment returns `201 Created`, or `409 Conflict` for a duplicate enrolment.
- An organiser can view enrolments for their own event.

To run tests in Visual Studio:

1. Open **Test → Test Explorer**.
2. Select **Run All**.
3. Confirm that the tests pass before pushing changes.

## CI/CD

The GitHub Actions workflow is located at `.github/workflows/ci.yml`.

On each push or pull request to `main`, the workflow:

- Checks that the `/docs` folder exists.
- Verifies that the ERD, API Endpoint Plan, and SQL script are present.
- Checks the SQL script for the required `CREATE TABLE` statements.
- Confirms that `README.md` exists.

The workflow was moved from the incorrect `github/` location to `.github/workflows/`, and its file checks were updated to match the actual filenames in `/docs`.

### CI build screenshot

Screenshot file: `docs/ci-screenshot.png`

> If you view this README on GitHub, the screenshot can be embedded with `![Green CI build](docs/ci-screenshot.png)` once the file is present at that path.

## GitHub History

The repository contains 20+ meaningful commits across Parts 1 and 2, covering:

- Corrected ERD cardinalities, Results-to-Enrolments relationship, distance, and event type
- Consolidated SQL script and `CREATE TABLE Results` fix
- Session-based authentication plan replacing JWT
- Solution and project setup
- DTOs, models, and `DbContext`
- Entity Framework migration
- All six controllers
- Session filter and password hasher
- Unit tests
- CI workflow correction
- README updates

## Video Presentation

- **Part 1 video (unlisted):** https://youtu.be/09Jxw9ozpZ0
- **Part 2 video (unlisted):** Add the Part 2 YouTube link before submission.

The Part 2 video should demonstrate:

1. Repository structure and project layout
2. Models and their relationships
3. `RaceDayDbContext` and the SQL Server database
4. Registration and password hashing
5. Login and session management
6. Organiser functionality: create an event, add a category, and capture a result
7. Participant functionality: enrol in an event and view personal results
8. A rejected wrong-role request (participant trying to create an event, resulting in `403`)
9. Swagger endpoints
10. Test Explorer with tests passing
11. A successful GitHub Actions build
12. How Part 2 prepares the system for Part 3

## Submission Checklist

- [ ] `/docs` contains the ERD, API Endpoint Plan, and SQL script.
- [ ] `docs/RaceDay_Database.sql` runs end-to-end on a clean SSMS instance.
- [ ] ERD cardinalities are labelled.
- [ ] `Results` links to `Enrolments` only.
- [ ] `Events` includes `distance_km` and `event_type`.
- [ ] Authentication is session-based; JWT is not used.
- [ ] `.github/workflows/ci.yml` is in the correct location and checks the correct filenames.
- [ ] Green CI build screenshot is present at `docs/ci-screenshot.png`.
- [ ] Both Part 1 and Part 2 unlisted video links are included.
- [ ] Repository contains 20+ meaningful commits.
- [ ] README explains setup, roles, and CI/CD.

---

**Project:** RaceDay Event Management System  
**Backend:** ASP.NET Core Web API on .NET 8  
**Database:** Microsoft SQL Server
