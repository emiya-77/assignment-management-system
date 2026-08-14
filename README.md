````markdown
# Assignment & Submission Management System

A role-based school/college Assignment & Submission Management System built for the **OnnoRokom Projukti Limited Assistant Software Engineer Recruitment Project**.

The system allows administrators to manage users and academic setup, teachers to create and manage assignments and review submissions, and students to view assignments, submit answers, update submissions when permitted, and view grading results.

---

# Quick Start

The easiest way to run the project is with Docker.

## Requirements

You only need:

- Git
- Docker Desktop

## 1. Clone the repository

```bash
git clone https://github.com/emiya-77/assignment-management-system.git
cd assignment-management-system
````

## 2. Start the project

```bash
docker compose up --build
```

Docker will automatically:

1. Start PostgreSQL.
2. Build and start the ASP.NET Core API.
3. Build and start the Next.js frontend.
4. Create the PostgreSQL database.
5. Apply EF Core migrations.
6. Create the database schema, relationships, and indexes.
7. Seed the database with demo/sample data.

No manual database creation or table setup is required.

## 3. Open the application

Frontend:

```text
http://localhost:3000
```

Backend API:

```text
http://localhost:5251
```

The project is ready to test once the containers are running.

---

# Demo Credentials

The database comes with demo users and sample data.

## Admin

```text
Email: admin@example.com
Password: Admin123!
```

## Teacher

```text
Email: teacher@example.com
Password: Teacher123!
```

Additional seeded teachers use:

```text
Password: Teacher123!
```

## Student

```text
Email: student@example.com
Password: Student123!
```

Additional seeded students use:

```text
Password: Student123!
```

---

# Recommended Evaluation Flow

## Admin

1. Log in as the Admin.
2. Open Users.
3. Review the seeded teachers and students.
4. Review Courses and Subjects.
5. Review Teacher Assignments.
6. Review Student Enrollments.

## Teacher

1. Log in as the Teacher.
2. Open My Assignments.
3. Review the assigned course/subject combinations.
4. Create a new assignment or inspect the seeded assignments.
5. Publish a draft assignment.
6. Review student submissions.
7. Grade a submission.
8. Provide marks and feedback.
9. Change submission status when necessary.

## Student

1. Log in as the Student.
2. View published assignments for enrolled courses.
3. Open an assignment.
4. Submit an answer.
5. Update the submission when updates are allowed and the deadline has not passed.
6. View submission status, marks, and feedback.

---

# Stopping the Project

If Docker Compose is running in the current terminal, press:

```text
Ctrl + C
```

Or stop the containers with:

```bash
docker compose down
```

To check the container status:

```bash
docker compose ps
```

---

# Resetting the Database

The PostgreSQL data is stored in a Docker volume.

To completely reset the database and restore the original seed data:

```bash
docker compose down -v
docker compose up --build
```

The `-v` removes the PostgreSQL volume. The next startup creates a fresh database, applies the migrations, and seeds the demo data again.

---

# Project Overview

This is a full-stack role-based application with three main roles:

* **Admin** — manages users, courses, subjects, teacher assignments, and student enrollments.
* **Teacher** — creates and manages assignments, publishes assignments, reviews submissions, gives marks and feedback, and manages submission status.
* **Student** — views eligible assignments, submits answers, updates submissions when permitted, and views marks and feedback.

The backend handles authentication, authorization, validation, business rules, database access, error handling, and logging.

The frontend provides the user interface for the main role-based workflows.

---

# Technology Stack

## Frontend

* Next.js
* React
* TypeScript
* Tailwind CSS
* shadcn/ui
* Lucide React

## Backend

* ASP.NET Core Web API
* C#
* .NET 10
* Entity Framework Core
* Npgsql
* JWT Bearer Authentication
* OpenAPI
* Custom exception-handling middleware
* `ILogger`-based logging

## Database

* PostgreSQL
* Entity Framework Core migrations

## Testing

* xUnit
* Moq

## Containerization

* Docker
* Docker Compose

---

# Main Features

## Admin

* View and manage users.
* Create and update users.
* Change user active/inactive status.
* Manage courses.
* Manage subjects.
* Assign teachers to course/subject combinations.
* Enroll students into courses.
* Access protected assignment and submission APIs.

## Teacher

* View their own assignments.
* View assigned course/subject combinations.
* Create assignments.
* Update assignments.
* Delete assignments.
* Keep assignments as drafts.
* Publish assignments.
* Set title, description, deadline, maximum marks, and submission update permission.
* View submissions for their own assignments.
* Grade submissions.
* Provide feedback.
* Change submission status.

## Student

* View published assignments for courses they are enrolled in.
* View assignment details and deadlines.
* Submit answers.
* Update submissions when updates are allowed and the deadline has not passed.
* View submission status.
* View marks and teacher feedback.

---

# Important Business Rules

The following rules are enforced primarily by the backend:

* Assignment codes are unique and normalized to uppercase.
* Teachers can only create, update, publish, or delete assignments belonging to their own teacher/course/subject assignment.
* Students can only view published assignments for courses in which they are enrolled.
* Students cannot submit to draft assignments.
* Students cannot submit after the deadline.
* A student can submit only once for an assignment.
* A database unique constraint also protects the one-submission-per-student-per-assignment rule.
* Submission updates require `AllowSubmissionUpdate = true`.
* Submission updates also require the deadline to have not passed.
* Students can only update their own submissions.
* Teachers can only review, grade, or change the status of submissions belonging to their own assignments.
* Marks cannot be negative.
* Marks cannot exceed the assignment maximum marks.
* Successful grading automatically changes the submission status to `Graded`.
* Successful grading records `GradedAt`.
* Backend timestamps use UTC.

Submission statuses:

```text
Submitted = 0
UnderReview = 1
Graded = 2
Returned = 3
```

---

# Architecture

The backend follows a layered, Clean Architecture-inspired structure:

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

## Backend Structure

```text
backend/
├── AssignmentManagementSystem.slnx
│
├── src/
│   ├── API/
│   ├── Application/
│   ├── Domain/
│   └── Infrastructure/
│
└── tests/
    └── Application.Tests/
```

## Frontend Structure

```text
frontend/
├── src/
│   ├── app/
│   │   ├── dashboard/
│   │   │   ├── admin/
│   │   │   ├── teacher/
│   │   │   └── student/
│   │   └── login/
│   │
│   ├── components/
│   ├── lib/
│   └── types/
│
└── public/
```

## Layer Responsibilities

### Domain

Contains core entities and enums.

### Application

Contains DTOs, repository contracts, service contracts, and business logic.

### Infrastructure

Contains Entity Framework Core, PostgreSQL access, repositories, authentication infrastructure, password hashing, migrations, and database seeding.

### API

Contains controllers, authentication/authorization configuration, middleware, OpenAPI configuration, CORS, and application startup.

### Application.Tests

Contains unit tests for important application business rules using xUnit and Moq.

---

# Database Model

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

## Assignment Ownership

An assignment belongs to a `TeacherAssignment`.

A `TeacherAssignment` connects one teacher with one course and one subject. This is also used to enforce assignment ownership.

## Submission Uniqueness

A submission belongs to one assignment and one student.

A unique composite database index is used on:

```text
AssignmentId + StudentId
```

This prevents duplicate submissions at the database level.

---

# Authentication & Authorization

Authentication uses JWT Bearer tokens.

The JWT contains:

* User ID in `ClaimTypes.NameIdentifier`
* Email
* Role

Supported roles:

```text
Admin
Teacher
Student
```

Authorization is enforced by the backend using role-based authorization and service-level ownership checks.

For example:

* Teachers cannot modify assignments owned by other teachers.
* Students cannot modify another student's submission.
* Teachers cannot grade submissions belonging to another teacher's assignment.

---

# API Overview

## Authentication

```text
POST /api/Auth/login
```

## Users

```text
GET    /api/Users
GET    /api/Users/{id}
POST   /api/Users
PUT    /api/Users/{id}
PATCH  /api/Users/{id}/status
```

## Courses

```text
GET    /api/Courses
GET    /api/Courses/{id}
POST   /api/Courses
PUT    /api/Courses/{id}
DELETE /api/Courses/{id}
```

## Subjects

```text
GET    /api/Subjects
GET    /api/Subjects/{id}
POST   /api/Subjects
PUT    /api/Subjects/{id}
DELETE /api/Subjects/{id}
```

## Teacher Assignments

Admin:

```text
GET    /api/TeacherAssignments
GET    /api/TeacherAssignments/{id}
POST   /api/TeacherAssignments
DELETE /api/TeacherAssignments/{id}
```

Teacher:

```text
GET /api/TeacherAssignments/my-assignments
```

## Enrollments

```text
GET    /api/Enrollments
GET    /api/Enrollments/{studentId}/{courseId}
POST   /api/Enrollments
DELETE /api/Enrollments/{studentId}/{courseId}
```

## Assignments

```text
GET    /api/Assignments
GET    /api/Assignments/{id}
POST   /api/Assignments
PUT    /api/Assignments/{id}
DELETE /api/Assignments/{id}
PATCH  /api/Assignments/{id}/publish
```

Access is role-aware. Admins can view all assignments, teachers can view their own assignments, and students can view published assignments for their enrolled courses.

## Submissions

```text
GET    /api/Submissions
GET    /api/Submissions/{id}
GET    /api/Submissions/assignment/{assignmentId}
POST   /api/Submissions/assignment/{assignmentId}
PUT    /api/Submissions/{id}
PATCH  /api/Submissions/{id}/grade
PATCH  /api/Submissions/{id}/status
```

---

# Database and Seed Data

The project includes EF Core migration files in:

```text
backend/src/Infrastructure/Data/Migrations
```

Seed logic is located in:

```text
backend/src/Infrastructure/Data/Seed/DbInitializer.cs
```

When the backend starts, it applies pending migrations and initializes the sample data.

The seed data includes:

* 1 Admin
* Multiple Teachers
* Multiple Students
* Multiple Courses
* Multiple Subjects
* Student enrollments
* Teacher assignments
* Sample assignments
* Sample submissions

This means the main workflows can be tested without manually creating all the required data first.

---

# Running Tests

Unit tests are located in:

```text
backend/tests/Application.Tests
```

With the .NET 10 SDK installed, run:

```bash
dotnet test
```

The tests cover important business rules and workflows including:

* Assignment creation and uniqueness.
* Teacher ownership and authorization.
* Assignment update/delete/publish ownership.
* Submission deadline validation.
* Student enrollment validation.
* Duplicate submission prevention.
* Submission ownership.
* Submission update restrictions.
* Teacher grading authorization.
* Maximum marks validation.
* Automatic grading status updates.
* Invalid submission status validation.

---

# Manual Setup

Docker is the recommended setup method.

The project can also be run manually if needed.

## Backend

Requirements:

* .NET 10 SDK
* PostgreSQL

Configure the PostgreSQL connection in the backend configuration.

Then:

```bash
cd backend
dotnet restore
dotnet build
```

Run the API:

```bash
cd src/API
dotnet run
```

The backend automatically applies pending migrations and initializes the seed data.

## Frontend

Create:

```text
frontend/.env.local
```

with:

```env
NEXT_PUBLIC_API_URL=http://localhost:5251/api
```

Then:

```bash
cd frontend
npm install
npm run dev
```

Open:

```text
http://localhost:3000
```

---

# Environment Configuration

The frontend supports:

```env
NEXT_PUBLIC_API_URL=http://localhost:5251/api
```

An example environment file should be included in the repository as:

```text
frontend/.env.example
```

Do not commit real PostgreSQL passwords, JWT secrets, API keys, or other private deployment secrets.

For Docker, the application configuration is handled through the Docker Compose configuration.

---

# Assumptions & Design Decisions

The assessment allows reasonable assumptions where the requirements are not explicitly defined. These are the main decisions I made:

1. **One User entity for all roles**
   Admin, Teacher, and Student are represented by a single `User` entity with a role enum.

2. **TeacherAssignment is the ownership boundary for assignments**
   A teacher is connected to a course and subject through `TeacherAssignment`. Assignments reference this relationship.

3. **Students must be enrolled before submitting**
   The backend verifies course enrollment before accepting a submission.

4. **Only published assignments can be submitted**
   Draft assignments cannot be submitted by students.

5. **One submission per student per assignment**
   This is checked by the application and also enforced by a database unique constraint.

6. **Submission updates are controlled by the assignment**
   Updates require `AllowSubmissionUpdate = true` and an unexpired deadline.

7. **Assignments start as drafts**
   Creating an assignment starts with `IsPublished = false`.

8. **Grading automatically sets the status to `Graded`**
   Teachers can still change the submission status later when necessary.

9. **UTC timestamps are used by the backend**
   The frontend converts them for display.

10. **Assignment codes are normalized**
    Assignment codes are trimmed and converted to uppercase.

11. **No file uploads**
    Assignment answers are text-based in the current implementation.

12. **No notification system**
    Email/push notifications were outside the core scope of the assessment.

---

# Known Limitations

* No file attachments for assignment submissions.
* No email or push notification system.
* No pagination.
* No advanced server-side filtering.
* No password reset workflow.
* No production deployment configuration.
* The frontend focuses on the core workflows required by the assessment.

---

# Final Submission Checklist

* [x] Frontend source code included.
* [x] Backend/API source code included.
* [x] Unit tests included.
* [x] EF Core migration files included.
* [x] Database schema can be created automatically.
* [x] No manual table creation is required.
* [x] Seed/sample data included.
* [x] Admin demo account included.
* [x] Teacher demo account included.
* [x] Student demo account included.
* [x] Docker Compose configuration included.
* [x] README includes setup instructions.
* [x] README includes database setup behavior.
* [x] README includes frontend and backend instructions.
* [x] README includes test instructions.
* [x] Role-based authorization is enforced by the backend.
* [x] Important business rules are implemented and tested.
* [x] No real secrets should be committed to the repository.

---

# Project Submission

After pushing the final version of the project to GitHub, submit the repository link through the assessment submission form provided by OnnoRokom Projukti Limited.

Submission page:

[https://q-rp.com/c/4CIs](https://q-rp.com/c/4CIs)

```

This version puts **Docker + Quick Start + demo credentials + evaluation flow right at the top**, which is what the evaluator is most likely to need first.
```
