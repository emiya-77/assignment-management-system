using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AssignmentConfiguration
    : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(a => a.Code)
            .IsUnique();

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(a => a.Deadline)
            .IsRequired();

        builder.Property(a => a.MaximumMarks)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(a => a.IsPublished)
            .HasDefaultValue(false);

        builder.Property(a => a.AllowSubmissionUpdate)
            .HasDefaultValue(true);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.HasOne(a => a.TeacherAssignment)
        .WithMany(ta => ta.Assignments)
        .HasForeignKey(a => a.TeacherAssignmentId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}