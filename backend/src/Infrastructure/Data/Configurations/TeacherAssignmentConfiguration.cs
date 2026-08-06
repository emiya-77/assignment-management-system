using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class TeacherAssignmentConfiguration : IEntityTypeConfiguration<TeacherAssignment>
{
    public void Configure(EntityTypeBuilder<TeacherAssignment> builder)
    {
        // Primary key
        builder.HasKey(ta => ta.Id);

        // Prevent duplicate
        builder.HasIndex(ta => new
        {
            ta.TeacherId,
            ta.CourseId,
            ta.SubjectId
        }).IsUnique();

        builder.HasOne(ta => ta.Teacher)
        .WithMany(u => u.TeacherAssignments)
        .HasForeignKey(ta => ta.TeacherId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ta => ta.Course)
        .WithMany(u => u.TeacherAssignments)
        .HasForeignKey(ta => ta.CourseId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ta => ta.Subject)
        .WithMany(u => u.TeacherAssignments)
        .HasForeignKey(ta => ta.SubjectId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Property(ta => ta.AssignedAt)
        .IsRequired();
    }
}