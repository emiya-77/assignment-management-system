using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Assignments;

public class UpdateAssignmentRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime Deadline { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal MaximumMarks { get; set; }

    public bool AllowSubmissionUpdate { get; set; }
}