using Domain.Entities;
using Domain.Enums;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Seed;

public static class DbInitializer
{
    public static async Task SeedAsync(
        AppDbContext context
    )
    {
        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync())
        {
            return;
        }

        var passwordService = new PasswordService();


        // ==================================================
        // USERS
        // ==================================================

        var admin = new User
        {
            FirstName = "System",
            LastName = "Admin",
            Email = "admin@example.com",
            Role = UserRole.Admin,
            IsActive = true
        };

        admin.PasswordHash =
            passwordService.HashPassword(
                admin,
                "Admin123!"
            );


        var teacher1 = new User
        {
            FirstName = "Kib",
            LastName = "Rahman",
            Email = "teacher@example.com",
            Role = UserRole.Teacher,
            IsActive = true
        };

        teacher1.PasswordHash =
            passwordService.HashPassword(
                teacher1,
                "Teacher123!"
            );


        var teacher2 = new User
        {
            FirstName = "Sarah",
            LastName = "Ahmed",
            Email = "sarah.teacher@example.com",
            Role = UserRole.Teacher,
            IsActive = true
        };

        teacher2.PasswordHash =
            passwordService.HashPassword(
                teacher2,
                "Teacher123!"
            );


        var teacher3 = new User
        {
            FirstName = "Imran",
            LastName = "Khan",
            Email = "imran.teacher@example.com",
            Role = UserRole.Teacher,
            IsActive = true
        };

        teacher3.PasswordHash =
            passwordService.HashPassword(
                teacher3,
                "Teacher123!"
            );


        var teacher4 = new User
        {
            FirstName = "Nadia",
            LastName = "Islam",
            Email = "nadia.teacher@example.com",
            Role = UserRole.Teacher,
            IsActive = true
        };

        teacher4.PasswordHash =
            passwordService.HashPassword(
                teacher4,
                "Teacher123!"
            );


        // STUDENTS

        var student1 = new User
        {
            FirstName = "Ria",
            LastName = "Student",
            Email = "student@example.com",
            Role = UserRole.Student,
            IsActive = true
        };

        student1.PasswordHash =
            passwordService.HashPassword(
                student1,
                "Student123!"
            );


        var student2 = new User
        {
            FirstName = "Araf",
            LastName = "Hasan",
            Email = "araf.student@example.com",
            Role = UserRole.Student,
            IsActive = true
        };

        student2.PasswordHash =
            passwordService.HashPassword(
                student2,
                "Student123!"
            );


        var student3 = new User
        {
            FirstName = "Mim",
            LastName = "Akter",
            Email = "mim.student@example.com",
            Role = UserRole.Student,
            IsActive = true
        };

        student3.PasswordHash =
            passwordService.HashPassword(
                student3,
                "Student123!"
            );


        var student4 = new User
        {
            FirstName = "Tanvir",
            LastName = "Ahmed",
            Email = "tanvir.student@example.com",
            Role = UserRole.Student,
            IsActive = true
        };

        student4.PasswordHash =
            passwordService.HashPassword(
                student4,
                "Student123!"
            );


        var student5 = new User
        {
            FirstName = "Nusrat",
            LastName = "Jahan",
            Email = "nusrat.student@example.com",
            Role = UserRole.Student,
            IsActive = true
        };

        student5.PasswordHash =
            passwordService.HashPassword(
                student5,
                "Student123!"
            );


        var student6 = new User
        {
            FirstName = "Sakib",
            LastName = "Hossain",
            Email = "sakib.student@example.com",
            Role = UserRole.Student,
            IsActive = true
        };

        student6.PasswordHash =
            passwordService.HashPassword(
                student6,
                "Student123!"
            );


        var student7 = new User
        {
            FirstName = "Farzana",
            LastName = "Islam",
            Email = "farzana.student@example.com",
            Role = UserRole.Student,
            IsActive = true
        };

        student7.PasswordHash =
            passwordService.HashPassword(
                student7,
                "Student123!"
            );


        var student8 = new User
        {
            FirstName = "Rakib",
            LastName = "Hasan",
            Email = "rakib.student@example.com",
            Role = UserRole.Student,
            IsActive = true
        };

        student8.PasswordHash =
            passwordService.HashPassword(
                student8,
                "Student123!"
            );


        context.Users.AddRange(

            admin,

            teacher1,
            teacher2,
            teacher3,
            teacher4,

            student1,
            student2,
            student3,
            student4,
            student5,
            student6,
            student7,
            student8
        );

        await context.SaveChangesAsync();


        // ==================================================
        // COURSES
        // ==================================================

        var course1 = new Course
        {
            Code = "CSE-101",
            Name = "Computer Science Fundamentals",
            Description =
                "Introduction to fundamental concepts of computer science."
        };


        var course2 = new Course
        {
            Code = "CSE-201",
            Name = "Software Engineering",
            Description =
                "Fundamentals of software development and engineering practices."
        };


        var course3 = new Course
        {
            Code = "CSE-301",
            Name = "Web Development",
            Description =
                "Modern frontend and backend web development."
        };


        var course4 = new Course
        {
            Code = "CSE-401",
            Name = "Data Structures and Algorithms",
            Description =
                "Core data structures, algorithms, and problem solving."
        };


        var course5 = new Course
        {
            Code = "CSE-501",
            Name = "Database Systems",
            Description =
                "Relational databases, SQL, and database design."
        };


        context.Courses.AddRange(
            course1,
            course2,
            course3,
            course4,
            course5
        );

        await context.SaveChangesAsync();


        // ==================================================
        // SUBJECTS
        // ==================================================

        var subject1 = new Subject
        {
            Code = "CSE-101-DB",
            Name = "Database Fundamentals",
            Description =
                "Introduction to relational databases and SQL."
        };


        var subject2 = new Subject
        {
            Code = "CSE-101-PROG",
            Name = "Programming Fundamentals",
            Description =
                "Basic programming concepts using modern programming languages."
        };


        var subject3 = new Subject
        {
            Code = "CSE-201-SE",
            Name = "Software Engineering Principles",
            Description =
                "Software development lifecycle and engineering practices."
        };


        var subject4 = new Subject
        {
            Code = "CSE-301-FE",
            Name = "Frontend Development",
            Description =
                "Building responsive user interfaces with modern web technologies."
        };


        var subject5 = new Subject
        {
            Code = "CSE-301-BE",
            Name = "Backend Development",
            Description =
                "Building APIs, services, and backend applications."
        };


        var subject6 = new Subject
        {
            Code = "CSE-401-DSA",
            Name = "Data Structures",
            Description =
                "Arrays, linked lists, stacks, queues, trees, and graphs."
        };


        var subject7 = new Subject
        {
            Code = "CSE-401-ALG",
            Name = "Algorithms",
            Description =
                "Algorithm design, complexity, sorting, and searching."
        };


        var subject8 = new Subject
        {
            Code = "CSE-501-SQL",
            Name = "SQL and Database Design",
            Description =
                "SQL queries, normalization, and relational database design."
        };


        context.Subjects.AddRange(
            subject1,
            subject2,
            subject3,
            subject4,
            subject5,
            subject6,
            subject7,
            subject8
        );

        await context.SaveChangesAsync();


        // ==================================================
        // STUDENT ENROLLMENTS
        // ==================================================

        context.StudentCourses.AddRange(

            // Ria
            new StudentCourse
            {
                StudentId = student1.Id,
                CourseId = course1.Id
            },
            new StudentCourse
            {
                StudentId = student1.Id,
                CourseId = course2.Id
            },
            new StudentCourse
            {
                StudentId = student1.Id,
                CourseId = course3.Id
            },

            // Araf
            new StudentCourse
            {
                StudentId = student2.Id,
                CourseId = course1.Id
            },
            new StudentCourse
            {
                StudentId = student2.Id,
                CourseId = course4.Id
            },

            // Mim
            new StudentCourse
            {
                StudentId = student3.Id,
                CourseId = course1.Id
            },
            new StudentCourse
            {
                StudentId = student3.Id,
                CourseId = course3.Id
            },

            // Tanvir
            new StudentCourse
            {
                StudentId = student4.Id,
                CourseId = course2.Id
            },
            new StudentCourse
            {
                StudentId = student4.Id,
                CourseId = course4.Id
            },

            // Nusrat
            new StudentCourse
            {
                StudentId = student5.Id,
                CourseId = course3.Id
            },
            new StudentCourse
            {
                StudentId = student5.Id,
                CourseId = course5.Id
            },

            // Sakib
            new StudentCourse
            {
                StudentId = student6.Id,
                CourseId = course1.Id
            },
            new StudentCourse
            {
                StudentId = student6.Id,
                CourseId = course5.Id
            },

            // Farzana
            new StudentCourse
            {
                StudentId = student7.Id,
                CourseId = course2.Id
            },
            new StudentCourse
            {
                StudentId = student7.Id,
                CourseId = course3.Id
            },

            // Rakib
            new StudentCourse
            {
                StudentId = student8.Id,
                CourseId = course4.Id
            },
            new StudentCourse
            {
                StudentId = student8.Id,
                CourseId = course5.Id
            }
        );

        await context.SaveChangesAsync();


        // ==================================================
        // TEACHER ASSIGNMENTS
        // ==================================================

        var teacherAssignment1 =
            new TeacherAssignment
            {
                TeacherId = teacher1.Id,
                CourseId = course1.Id,
                SubjectId = subject1.Id
            };


        var teacherAssignment2 =
            new TeacherAssignment
            {
                TeacherId = teacher1.Id,
                CourseId = course1.Id,
                SubjectId = subject2.Id
            };


        var teacherAssignment3 =
            new TeacherAssignment
            {
                TeacherId = teacher2.Id,
                CourseId = course2.Id,
                SubjectId = subject3.Id
            };


        var teacherAssignment4 =
            new TeacherAssignment
            {
                TeacherId = teacher2.Id,
                CourseId = course3.Id,
                SubjectId = subject4.Id
            };


        var teacherAssignment5 =
            new TeacherAssignment
            {
                TeacherId = teacher3.Id,
                CourseId = course3.Id,
                SubjectId = subject5.Id
            };


        var teacherAssignment6 =
            new TeacherAssignment
            {
                TeacherId = teacher3.Id,
                CourseId = course4.Id,
                SubjectId = subject6.Id
            };


        var teacherAssignment7 =
            new TeacherAssignment
            {
                TeacherId = teacher4.Id,
                CourseId = course4.Id,
                SubjectId = subject7.Id
            };


        var teacherAssignment8 =
            new TeacherAssignment
            {
                TeacherId = teacher4.Id,
                CourseId = course5.Id,
                SubjectId = subject8.Id
            };


        context.TeacherAssignments.AddRange(

            teacherAssignment1,
            teacherAssignment2,
            teacherAssignment3,
            teacherAssignment4,
            teacherAssignment5,
            teacherAssignment6,
            teacherAssignment7,
            teacherAssignment8
        );

        await context.SaveChangesAsync();


        // ==================================================
        // ASSIGNMENTS
        // ==================================================

        var assignment1 = new Assignment
        {
            Code = "DB-ASSIGN-001",
            Title = "Database Fundamentals Assignment",
            Description =
                "Explain the difference between primary keys and foreign keys. " +
                "Provide examples showing how they are used to create relationships between tables.",

            Deadline = DateTime.UtcNow.AddDays(7),

            MaximumMarks = 100,

            IsPublished = true,

            AllowSubmissionUpdate = true,

            TeacherAssignmentId =
                teacherAssignment1.Id
        };


        var assignment2 = new Assignment
        {
            Code = "DB-ASSIGN-002",
            Title = "SQL Query Practice",
            Description =
                "Write SQL queries for selecting, filtering, joining, and aggregating data from relational tables.",

            Deadline = DateTime.UtcNow.AddDays(10),

            MaximumMarks = 50,

            IsPublished = true,

            AllowSubmissionUpdate = true,

            TeacherAssignmentId =
                teacherAssignment1.Id
        };


        var assignment3 = new Assignment
        {
            Code = "PROG-ASSIGN-001",
            Title = "Programming Concepts",
            Description =
                "Explain variables, functions, loops, and conditional statements with examples.",

            Deadline = DateTime.UtcNow.AddDays(5),

            MaximumMarks = 75,

            IsPublished = true,

            AllowSubmissionUpdate = false,

            TeacherAssignmentId =
                teacherAssignment2.Id
        };


        var assignment4 = new Assignment
        {
            Code = "SE-ASSIGN-001",
            Title = "Software Development Lifecycle",
            Description =
                "Explain the major phases of the Software Development Lifecycle and their importance.",

            Deadline = DateTime.UtcNow.AddDays(14),

            MaximumMarks = 100,

            IsPublished = true,

            AllowSubmissionUpdate = true,

            TeacherAssignmentId =
                teacherAssignment3.Id
        };


        var assignment5 = new Assignment
        {
            Code = "FE-ASSIGN-001",
            Title = "Responsive Web Page",
            Description =
                "Design and implement a responsive web page using modern HTML and CSS concepts.",

            Deadline = DateTime.UtcNow.AddDays(8),

            MaximumMarks = 80,

            IsPublished = true,

            AllowSubmissionUpdate = true,

            TeacherAssignmentId =
                teacherAssignment4.Id
        };


        var assignment6 = new Assignment
        {
            Code = "BE-ASSIGN-001",
            Title = "REST API Design",
            Description =
                "Design a RESTful API for a simple task management application and explain the available endpoints.",

            Deadline = DateTime.UtcNow.AddDays(12),

            MaximumMarks = 100,

            IsPublished = true,

            AllowSubmissionUpdate = true,

            TeacherAssignmentId =
                teacherAssignment5.Id
        };


        var assignment7 = new Assignment
        {
            Code = "DSA-ASSIGN-001",
            Title = "Data Structures Analysis",
            Description =
                "Compare arrays, linked lists, stacks, and queues. Explain their strengths and use cases.",

            Deadline = DateTime.UtcNow.AddDays(9),

            MaximumMarks = 100,

            IsPublished = true,

            AllowSubmissionUpdate = false,

            TeacherAssignmentId =
                teacherAssignment6.Id
        };


        var assignment8 = new Assignment
        {
            Code = "ALG-DRAFT-001",
            Title = "Sorting Algorithms",
            Description =
                "Compare common sorting algorithms and analyze their time complexity.",

            Deadline = DateTime.UtcNow.AddDays(15),

            MaximumMarks = 100,

            IsPublished = false,

            AllowSubmissionUpdate = true,

            TeacherAssignmentId =
                teacherAssignment7.Id
        };


        var assignment9 = new Assignment
        {
            Code = "SQL-PAST-001",
            Title = "Database Normalization",
            Description =
                "Normalize the provided database schema up to Third Normal Form.",

            Deadline = DateTime.UtcNow.AddDays(-2),

            MaximumMarks = 100,

            IsPublished = true,

            AllowSubmissionUpdate = false,

            TeacherAssignmentId =
                teacherAssignment8.Id
        };


        context.Assignments.AddRange(

            assignment1,
            assignment2,
            assignment3,
            assignment4,
            assignment5,
            assignment6,
            assignment7,
            assignment8,
            assignment9
        );

        await context.SaveChangesAsync();


        // ==================================================
        // SUBMISSIONS
        // ==================================================

        context.Submissions.AddRange(

            // Ria - Graded
            new Submission
            {
                AssignmentId = assignment1.Id,
                StudentId = student1.Id,

                Answer =
                    "A primary key uniquely identifies each record in a table. " +
                    "A foreign key creates a relationship between tables by referencing a key in another table.",

                Status = SubmissionStatus.Graded,

                Marks = 85,

                Feedback =
                    "Good explanation. You clearly described the purpose of both keys.",

                SubmittedAt =
                    DateTime.UtcNow.AddDays(-2),

                UpdatedAt =
                    DateTime.UtcNow.AddDays(-1),

                GradedAt =
                    DateTime.UtcNow.AddHours(-12)
            },


            // Araf - Submitted
            new Submission
            {
                AssignmentId = assignment1.Id,
                StudentId = student2.Id,

                Answer =
                    "Primary keys identify rows uniquely while foreign keys connect related tables.",

                Status =
                    SubmissionStatus.Submitted,

                SubmittedAt =
                    DateTime.UtcNow.AddHours(-10)
            },


            // Mim - Under Review
            new Submission
            {
                AssignmentId = assignment1.Id,
                StudentId = student3.Id,

                Answer =
                    "Primary keys provide unique identification and foreign keys reference another table.",

                Status =
                    SubmissionStatus.UnderReview,

                SubmittedAt =
                    DateTime.UtcNow.AddDays(-1)
            },


            // Ria - Programming
            new Submission
            {
                AssignmentId = assignment3.Id,
                StudentId = student1.Id,

                Answer =
                    "Variables store data, functions organize reusable logic, loops repeat instructions, and conditions control program flow.",

                Status =
                    SubmissionStatus.Returned,

                Feedback =
                    "Please provide more practical examples for each concept.",

                SubmittedAt =
                    DateTime.UtcNow.AddDays(-2),

                GradedAt =
                    DateTime.UtcNow.AddDays(-1)
            },


            // Tanvir - Software Engineering
            new Submission
            {
                AssignmentId = assignment4.Id,
                StudentId = student4.Id,

                Answer =
                    "The SDLC includes planning, analysis, design, implementation, testing, deployment, and maintenance.",

                Status =
                    SubmissionStatus.Graded,

                Marks = 92,

                Feedback =
                    "Excellent overview of the SDLC phases.",

                SubmittedAt =
                    DateTime.UtcNow.AddDays(-1),

                GradedAt =
                    DateTime.UtcNow.AddHours(-8)
            },


            // Mim - Frontend
            new Submission
            {
                AssignmentId = assignment5.Id,
                StudentId = student3.Id,

                Answer =
                    "I created a responsive layout using flexible containers and media queries.",

                Status =
                    SubmissionStatus.Submitted,

                SubmittedAt =
                    DateTime.UtcNow.AddHours(-5)
            },


            // Nusrat - Frontend
            new Submission
            {
                AssignmentId = assignment5.Id,
                StudentId = student5.Id,

                Answer =
                    "The page adapts to desktop, tablet, and mobile screen sizes.",

                Status =
                    SubmissionStatus.UnderReview,

                SubmittedAt =
                    DateTime.UtcNow.AddHours(-20)
            },


            // Rakib - Past assignment
            new Submission
            {
                AssignmentId = assignment9.Id,
                StudentId = student8.Id,

                Answer =
                    "The schema was normalized by separating repeating groups and removing partial and transitive dependencies.",

                Status =
                    SubmissionStatus.Graded,

                Marks = 78,

                Feedback =
                    "Good work overall. The normalization process is correct but could use more detailed examples.",

                SubmittedAt =
                    DateTime.UtcNow.AddDays(-5),

                GradedAt =
                    DateTime.UtcNow.AddDays(-3)
            }
        );

        await context.SaveChangesAsync();
    }
}