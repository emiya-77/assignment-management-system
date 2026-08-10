using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Submissions;

public class GradeSubmissionRequest
{
    [Range(0, double.MaxValue)]
    public decimal Marks { get; set; }

    [MaxLength(5000)]
    public string? Feedback { get; set; }
}