# Assignment & Submission Management System

A role-based school/college Assignment & Submission Management System built for the **OnnoRokom Projukti Limited Assistant Software Engineer Recruitment Project**.

The system allows administrators to manage users and academic setup, teachers to create and manage assignments, and students to view assignments, submit answers, update submissions when permitted, and view grading results.

# Quick Start (Recommended)

The easiest way to run the complete project is with Docker.

## Requirements

- Git
- Docker Desktop

## 1. Clone the repository

```bash
git clone https://github.com/emiya-77/assignment-management-system.git
cd assignment-management-system
```

## 2. Start the application

```bash
docker compose up --build
```

Docker will start PostgreSQL, the ASP.NET Core API, and the Next.js frontend. The backend automatically applies the EF Core migrations and initializes the seed data.

You do **not** need to manually create the PostgreSQL database, tables, relationships, indexes, or sample data when using Docker.

## 3. Open the application

Frontend:

```text
http://localhost:3000
```

Backend API:

```text
http://localhost:5251
```

## Demo credentials

### Admin

```text
Email: admin@example.com
Password: Admin123!
```

### Main Teacher

```text
Email: teacher@example.com
Password: Teacher123!
```

Additional seeded teachers use:

```text
Password: Teacher123!
```

### Main Student

```text
Email: student@example.com
Password: Student123!
```

Additional seeded students use:

```text
Password: Student123!
```

## Quick evaluation flow

### Admin

1. Log in as Admin.
2. Check Users, Courses, Subjects, Teacher Assignments, and Enrollments.

### Teacher

1. Log in as the main Teacher.
2. Open My Assignments.
3. Create or inspect an assignment.
4. Publish a draft assignment.
5. Review, grade, and update student submissions.

### Student

1. Log in as the main Student.
2. View published assignments.
3. Open an assignment and submit an answer.
4. Update the submission when allowed.
5. View status, marks, and feedback.

## Stop the project

Press `Ctrl + C` when `docker compose up` is running in the foreground, or use:

```bash
docker compose down
```

Check container status with:

```bash
docker compose ps
```

## Reset the seeded database

To remove the Docker database and recreate all sample data from scratch:

```bash
docker compose down -v
docker compose up --build
```

The next startup creates the database, applies migrations, and seeds the demo data again.

---

## Project Overview

The application is a full-stack web system with:

- **Admin** management for users, courses, subjects, teacher assignments, and student enrollments.
- **Teacher** management for assignments, publishing, submission review, grading, feedback, and submission status.
- **Student** workflows for viewing eligible assignments, submitting answers, updating submissions when allowed, and viewing submission results.
- JWT-based authentication and backend-enforced role-based authorization.
- PostgreSQL persistence through Entity Framework Core.
- Unit tests covering important business rules and authorization/workflow logic.

## Technology Stack

### Frontend

- Next.js
- React
- TypeScript
- Tailwind CSS
- shadcn/ui
- Lucide React

### Backend

- ASP.NET Core Web API
- C#
- .NET 10
- Entity Framework Core
- Npgsql
- JWT Bearer Authentication
- OpenAPI
- Custom exception-handling middleware
- `ILogger`-based logging

### Database

- PostgreSQL
- Entity Framework Core migrations

### Testing

- xUnit
- Moq

## Main Features

### Admin

- Manage users.
- Create and update users.
- Change user active/inactive status.
- Manage courses.
- Manage subjects.
- Assign teachers to course/subject combinations.
- Enroll students into courses.
- View assignments and submissions through the protected backend APIs.

### Teacher

- View only their own assignments.
- View their assigned course/subject combinations.
- Create assignments.
- Update assignments.
- Delete assignments.
- Keep assignments as drafts.
- Publish assignments.
- Set title, description, deadline, maximum marks, and submission-update permission.
- View submissions for their own assignments.
- Grade submissions.
- Provide feedback.
- Change submission status.

### Student

- View published assignments for courses they are enrolled in.
- View assignment details and deadlines.
- Submit an answer.
- Update a submission when updates are allowed and the deadline has not passed.
- View submission status.
- View marks and teacher feedback.

## Important Business Rules

The following rules are enforced primarily by the backend API and application services:

- Assignment codes are unique and normalized to uppercase.
- Teachers can only create, update, publish, or delete assignments belonging to their own teacher/course/subject assignment.
- Students can only see published assignments for courses in which they are enrolled.
- Students cannot submit to draft assignments.
- Students cannot submit after the deadline.
- A student can submit only once for an assignment.
- A database unique constraint also protects the one-submission-per-student-per-assignment rule.
- Submission updates require both permission from the assignment and an unexpired deadline.
- Students can only update their own submissions.
- Teachers can only review, grade, or change status for submissions belonging to their own assignments.
- Marks cannot be negative and cannot exceed the assignment maximum marks.
- Successful grading automatically sets the submission status to `Graded` and records `GradedAt`.
- Submission statuses are:
  - `Submitted = 0`
  - `UnderReview = 1`
  - `Graded = 2`
  - `Returned = 3`
- Timestamps are stored and compared using UTC in backend business logic.

## Architecture

The backend follows a layered / Clean Architecture-inspired structure:

```text
HTTP Request
    ↓
API Controller
    ↓
Application Service
    ↓
Repository Interface
    ↓
Infrastructure Repository
    ↓
Entity Framework Core
    ↓
PostgreSQL
```

### Backend Projects

```text
backend/
├── AssignmentManagementSystem.slnx
├── src/
│   ├── API/
│   ├── Application/
│   ├── Domain/
│   └── Infrastructure/
└── tests/
    └── Application.Tests/
```

### Layer Responsibilities

**Domain**

Contains core entities and enums.

**Application**

Contains DTOs, repository contracts, service contracts, and business logic.

**Infrastructure**

Contains Entity Framework Core, PostgreSQL access, repositories, authentication infrastructure, password hashing, migrations, and database seeding.

**API**

Contains controllers, authentication/authorization configuration, middleware, OpenAPI configuration, CORS, and application startup.

**Application.Tests**

Contains unit tests for important application business rules using xUnit and Moq.

## Database Model

The main entities are:

```text
User
Course
Subject
StudentCourse
TeacherAssignment
Assignment
Submission
```

Important relationships:

```text
User (Student)
    ↓
StudentCourse
    ↓
Course

User (Teacher)
    ↓
TeacherAssignment
    ├── Course
    └── Subject
          ↓
      Assignment
          ↓
      Submission
          ↓
      User (Student)
```

### Assignment ownership

An assignment belongs to a `TeacherAssignment`, which connects one teacher with one course and one subject. This provides a single relationship through which ownership and academic context are enforced.

### Submission uniqueness

A submission belongs to one assignment and one student. A unique composite database index on:

```text
AssignmentId + StudentId
```

prevents duplicate submissions at the database level.

## Authentication & Authorization

Authentication uses JWT Bearer tokens.

The JWT contains:

- User ID in `ClaimTypes.NameIdentifier`
- Email
- Role

Backend authorization is enforced using role-based authorization and service-level ownership checks.

The application supports these roles:

```text
Admin
Teacher
Student
```

## API Overview

### Authentication

```text
POST /api/Auth/login
```

### Users - Admin

```text
GET    /api/Users
GET    /api/Users/{id}
POST   /api/Users
PUT    /api/Users/{id}
PATCH  /api/Users/{id}/status
```

### Courses - Admin

```text
GET    /api/Courses
GET    /api/Courses/{id}
POST   /api/Courses
PUT    /api/Courses/{id}
DELETE /api/Courses/{id}
```

### Subjects - Admin

```text
GET    /api/Subjects
GET    /api/Subjects/{id}
POST   /api/Subjects
PUT    /api/Subjects/{id}
DELETE /api/Subjects/{id}
```

### Teacher Assignments

Admin:

```text
GET    /api/TeacherAssignments
GET    /api/TeacherAssignments/{id}
POST   /api/TeacherAssignments
DELETE /api/TeacherAssignments/{id}
```

Teacher:

```text
GET    /api/TeacherAssignments/my-assignments
```

### Enrollments - Admin

```text
GET    /api/Enrollments
GET    /api/Enrollments/{studentId}/{courseId}
POST   /api/Enrollments
DELETE /api/Enrollments/{studentId}/{courseId}
```

### Assignments

```text
GET    /api/Assignments
GET    /api/Assignments/{id}
POST   /api/Assignments
PUT    /api/Assignments/{id}
DELETE /api/Assignments/{id}
PATCH  /api/Assignments/{id}/publish
```

Access is role-aware. Admins can view all assignments, teachers see their own assignments, and students see published assignments for their enrolled courses.

### Submissions

```text
GET    /api/Submissions
GET    /api/Submissions/{id}
GET    /api/Submissions/assignment/{assignmentId}
POST   /api/Submissions/assignment/{assignmentId}
PUT    /api/Submissions/{id}
PATCH  /api/Submissions/{id}/grade
PATCH  /api/Submissions/{id}/status
```

## Running Tests

Unit tests are located in:

```text
backend/tests/Application.Tests
```

From the backend directory:

```bash
dotnet test
```

The unit test suite covers important business rules including:

- Assignment creation and uniqueness.
- Teacher ownership and authorization.
- Assignment update/delete/publish ownership.
- Submission deadline validation.
- Submission enrollment validation.
- Duplicate submission prevention.
- Submission ownership.
- Submission update restrictions.
- Teacher grading authorization.
- Maximum marks validation.
- Automatic grading status updates.
- Invalid submission status validation.

## Assumptions & Design Decisions

The assessment explicitly allows reasonable assumptions where requirements are not fully defined. The major decisions made in this implementation are:

1. **One User entity for all roles.** Admin, Teacher, and Student are represented by a single `User` entity with a role enum.
2. **TeacherAssignment is the ownership boundary for assignments.** A teacher is associated with a course and subject through `TeacherAssignment`; assignments reference this relationship rather than storing unrelated teacher/course/subject identifiers independently.
3. **Students must be enrolled in a course before submitting.** Enrollment is checked by the backend before accepting a submission.
4. **Only published assignments can be submitted.** Draft assignments are visible to their owning teacher but are not available for student submission.
5. **One submission per student per assignment.** The application checks this rule and PostgreSQL also enforces it with a unique composite index.
6. **Submission updates are controlled by the assignment.** Updates require `AllowSubmissionUpdate = true` and an unexpired deadline.
7. **Assignments begin as drafts.** Creating an assignment always creates it with `IsPublished = false`; publishing is a separate teacher action.
8. **Grading automatically sets the status to `Graded`.** Teachers can subsequently change the submission status when necessary.
9. **UTC timestamps are used by backend business logic.** The frontend converts timestamps for display.
10. **Assignment codes are unique and normalized.** Codes are trimmed and normalized to uppercase before persistence.
11. **No file uploads.** Assignment answers are text-based in the current implementation.
12. **No notification subsystem.** The assessment does not require email/push notifications, so user feedback is provided through API responses and frontend notifications/toasts.

## Known Limitations

The following are intentionally outside the core assessment scope:

- No file attachments for assignment submissions.
- No email or push notification system.
- No pagination or advanced server-side filtering.
- No password reset workflow.
- No production deployment configuration is required for local evaluation.
- The frontend currently focuses on the core role workflows required by the assessment rather than a large enterprise feature set.

## Environment Configuration

### Frontend `.env.example`

```env
NEXT_PUBLIC_API_URL=http://localhost:5251/api
```

### Backend configuration example

Do not commit real secrets. Use placeholders such as:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=AssignmentManagementDb;Username=postgres;Password=YOUR_POSTGRES_PASSWORD"
  },
  "Jwt": {
    "Key": "YOUR_DEVELOPMENT_JWT_SECRET",
    "Issuer": "AssignmentManagementSystem",
    "Audience": "AssignmentManagementSystem",
    "ExpiryInMinutes": 60
  }
}
```

## OpenAPI

The backend is configured with OpenAPI support through ASP.NET Core's OpenAPI services. In Development mode, the application maps the OpenAPI endpoint through `MapOpenApi()`.

Use the API project's generated OpenAPI endpoint or development tooling to inspect the available REST API contracts.

## Final Submission Checklist

Before submitting the repository, verify all of the following:

- [x] Repository is accessible.
- [x] Frontend source code is included.
- [x] Backend/API source code is included.
- [x] Unit tests are included.
- [x] EF Core migration files are included.
- [x] Database can be recreated from an empty PostgreSQL database without manually creating tables.
- [x] Seed/sample data is included.
- [x] Admin demo account works.
- [x] Teacher demo account works.
- [x] Student demo account works.
- [x] README explains setup and database configuration.
- [x] README explains how to run backend and frontend.
- [x] README explains how to run tests.
- [x] Role-based authorization is enforced by the backend.
- [x] Important business rules are implemented and tested.
- [x] `.env.example` files are included where required.
- [x] No real passwords, API keys, database passwords, or other secrets are committed.
- [x] `node_modules`, `.next`, `bin`, and `obj` are excluded through `.gitignore`.
- [x] Production frontend build succeeds.
- [x] Backend build succeeds.
- [x] `dotnet test` succeeds.

## Project Submission

After the repository is pushed and verified, submit the repository link through the assessment submission form provided by OnnoRokom Projukti Limited.

Submission page:

https://q-rp.com/c/4CIs

Project submission deadline:

**August 14, 2026 at 11:59 PM**

---
