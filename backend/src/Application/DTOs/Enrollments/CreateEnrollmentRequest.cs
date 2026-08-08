using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Enrollments;

public class CreateEnrollmentRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int StudentId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }
}