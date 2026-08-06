using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class StudentCourseConfiguration : IEntityTypeConfiguration<StudentCourse>
{
    public void Configure(EntityTypeBuilder<StudentCourse> builder)
    {
        // Composite primary key
        builder.HasKey(sc => new
        {
            sc.StudentId,
            sc.CourseId
        });

        // StudentCourse -> User
        builder.HasOne(sc => sc.Student)
        .WithMany(u => u.StudentCourses)
        .HasForeignKey(sc => sc.StudentId)
        .OnDelete(DeleteBehavior.Restrict);

        // StudentCourse -> Course
        builder.HasOne(sc => sc.Course)
        .WithMany(c => c.StudentCourses)
        .HasForeignKey(sc => sc.CourseId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Property(sc => sc.EnrolledAt)
        .IsRequired();
    }
}