# Assignment & Submission Management System

A role-based school/college Assignment & Submission Management System built for the **OnnoRokom Projukti Limited Assistant Software Engineer Recruitment Project**.

The system allows administrators to manage users and academic setup, teachers to create and manage assignments, and students to view assignments, submit answers, update submissions when permitted, and view grading results.

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

## Prerequisites

Install the following before running the project locally:

- .NET 10 SDK
- PostgreSQL
- Node.js and npm compatible with the Next.js application
- Git

## Repository Setup

Clone the repository:

```bash
git clone https://github.com/emiya-77/assignment-management-system.git
cd .\assignment-management-system\
```

The repository contains both the backend and frontend source code.

## Backend Setup

1. Configure PostgreSQL

Make sure PostgreSQL is installed and running.

Update the PostgreSQL credentials in the backend configuration.

Example appsettings.json:

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


### 3. Restore and build

From the backend directory:

```bash
dotnet restore
dotnet build
```

### 4. Run the backend

From the API project directory:

```bash
dotnet run
```

The frontend is configured to use the local API URL:

```text
http://localhost:5251/api
```

If your local ASP.NET configuration starts the API on a different port, update the frontend environment variable accordingly.

### 5. Database migration and seed behavior

The application automatically performs migrations when it starts:

```csharp
await context.Database.MigrateAsync();
```

It then runs the database initializer:

```csharp
await DbInitializer.SeedAsync(context);
```

Therefore, after creating the empty PostgreSQL database and starting the backend:

1. EF Core applies all pending migrations.
2. The database schema is created/updated.
3. Seed data is inserted if the database has not already been seeded.

No manual table creation is required.

### Resetting the demo database

The seed initializer intentionally does not reseed a database that already contains users. To recreate the demo database from scratch, drop and recreate the PostgreSQL database, then start the backend again.

Example PostgreSQL commands:

```sql
DROP DATABASE "AssignmentManagementDb";
CREATE DATABASE "AssignmentManagementDb";
```

Then run the API again.

## Frontend Setup

### 1. Configure the API URL

Create:

```text
frontend/.env.local
```

with:

```env
NEXT_PUBLIC_API_URL=http://localhost:5251/api
```

An example environment file should also be included in the repository as:

```text
frontend/.env.example
```

with the same variable name but no private secrets.

### 2. Install dependencies

From the frontend directory:

```bash
npm install
```

### 3. Run the frontend

```bash
npm run dev
```

The frontend should be available at:

```text
http://localhost:3000
```

## CORS

The backend currently allows the local frontend origin:

```text
http://localhost:3000
```

If the frontend is run on another local port, update the backend CORS configuration accordingly.

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

## Demo Credentials

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

Additional seeded teachers use the password:

```text
Teacher123!
```

### Main Student

```text
Email: student@example.com
Password: Student123!
```

Additional seeded students use the password:

```text
Student123!
```

## Recommended Evaluation Flow

### Admin

1. Log in as Admin.
2. Open Users.
3. Review teachers and students.
4. Review Courses and Subjects.
5. Review Teacher Assignments.
6. Review Student Enrollments.
7. Review assignments/submissions available through the protected backend APIs.

### Teacher

1. Log in as the main Teacher.
2. Open My Assignments.
3. View the teacher's assigned course/subject combinations.
4. Create a new assignment or inspect existing assignments.
5. Publish a draft assignment.
6. Review existing student submission data through the backend/API workflow.
7. Grade submissions and provide feedback.
8. Change submission status when necessary.

### Student

1. Log in as the main Student.
2. View published assignments for enrolled courses.
3. Open an assignment.
4. Submit an answer to an assignment that has not already been submitted.
5. Update the submission where the assignment permits updates and the deadline has not passed.
6. View submission status, marks, and feedback.

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

- [ ] Repository is accessible.
- [ ] Frontend source code is included.
- [ ] Backend/API source code is included.
- [ ] Unit tests are included.
- [ ] EF Core migration files are included.
- [ ] Database can be recreated from an empty PostgreSQL database without manually creating tables.
- [ ] Seed/sample data is included.
- [ ] Admin demo account works.
- [ ] Teacher demo account works.
- [ ] Student demo account works.
- [ ] README explains setup and database configuration.
- [ ] README explains how to run backend and frontend.
- [ ] README explains how to run tests.
- [ ] Role-based authorization is enforced by the backend.
- [ ] Important business rules are implemented and tested.
- [ ] `.env.example` files are included where required.
- [ ] No real passwords, API keys, database passwords, or other secrets are committed.
- [ ] `node_modules`, `.next`, `bin`, and `obj` are excluded through `.gitignore`.
- [ ] Production frontend build succeeds.
- [ ] Backend build succeeds.
- [ ] `dotnet test` succeeds.

## Project Submission

After the repository is pushed and verified, submit the repository link through the assessment submission form provided by OnnoRokom Projukti Limited.

Submission page:

https://q-rp.com/c/4CIs

Project submission deadline:

**August 14, 2026 at 11:59 PM**

---

## Quick Start Summary

For an evaluator who wants the shortest setup path:

```text
1. Clone repository
2. Install .NET 10 + PostgreSQL + Node.js/npm
3. Create empty PostgreSQL database: AssignmentManagementDb
4. Configure backend connection string with local PostgreSQL credentials
5. Run backend: dotnet run
6. Backend applies migrations and seeds demo data automatically
7. Create frontend/.env.local with NEXT_PUBLIC_API_URL=http://localhost:5251/api
8. Run npm install
9. Run npm run dev
10. Open http://localhost:3000
11. Log in using the demo credentials above
12. Run dotnet test from the backend directory
```
