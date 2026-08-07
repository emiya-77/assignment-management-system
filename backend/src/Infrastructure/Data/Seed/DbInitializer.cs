using Domain.Entities;
using Domain.Enums;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Seed;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync())
        {
            return;
        }

        var passwordService = new PasswordService();


        // USERS
        var admin = new User
        {
            FirstName = "System",
            LastName = "Admin",
            Email = "admin@example.com",
            Role = UserRole.Admin,
            IsActive = true
        };

        admin.PasswordHash = passwordService.HashPassword(
            admin,
            "Admin123!"
        );


        var teacher = new User
        {
            FirstName = "Kib",
            LastName = "Teacher",
            Email = "teacher@example.com",
            Role = UserRole.Teacher,
            IsActive = true
        };

        teacher.PasswordHash = passwordService.HashPassword(
            teacher,
            "Teacher123!"
        );


        var student = new User
        {
            FirstName = "Ria",
            LastName = "Student",
            Email = "student@example.com",
            Role = UserRole.Student,
            IsActive = true
        };

        student.PasswordHash = passwordService.HashPassword(
            student,
            "Student123!"
        );

        context.Users.AddRange(
            admin,
            teacher,
            student
        );


        // COURSE
        var course = new Course
        {
            Code = "CSE-101",
            Name = "Computer Science Fundamentals",
            Description = "Introduction to computer science."
        };

        context.Courses.Add(course);


        // SUBJECT
        var subject = new Subject
        {
            Code = "CSE-101-DB",
            Name = "Database Fundamentals",
            Description = "Introduction to relational databases."
        };

        context.Subjects.Add(subject);

        await context.SaveChangesAsync();

        // STUDENT ENROLLMENT
        var studentCourse = new StudentCourse
        {
            StudentId = student.Id,
            CourseId = course.Id
        };

        context.StudentCourses.Add(studentCourse);


        // TEACHER ASSIGNMENT
        var teacherAssignment = new TeacherAssignment
        {
            TeacherId = teacher.Id,
            CourseId = course.Id,
            SubjectId = subject.Id
        };

        context.TeacherAssignments.Add(teacherAssignment);

        await context.SaveChangesAsync();
    }
}