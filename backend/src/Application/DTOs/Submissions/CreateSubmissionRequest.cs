using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Submissions;

public class CreateSubmissionRequest
{
    [Required]
    [MaxLength(10000)]
    public string Answer { get; set; } = string.Empty;
}