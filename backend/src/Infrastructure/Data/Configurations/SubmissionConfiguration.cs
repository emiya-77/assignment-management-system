using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasIndex(s => new
        {
            s.AssignmentId,
            s.StudentId
        }).IsUnique();

        builder.Property(s => s.Answer)
        .IsRequired()
        .HasMaxLength(10000);

        builder.Property(s => s.Status)
        .IsRequired();

        builder.Property(s => s.Marks)
        .HasPrecision(10, 2);

        builder.Property(s => s.Feedback)
        .HasMaxLength(5000);

        builder.Property(s => s.SubmittedAt)
        .IsRequired();

        builder.HasOne(s => s.Assignment)
        .WithMany(a => a.Submissions)
        .HasForeignKey(s => s.AssignmentId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Student)
        .WithMany(s => s.Submissions)
        .HasForeignKey(s => s.StudentId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}