using Domain.Enums;

namespace Application.DTOs.Submissions;

public class SubmissionResponse
{
    public int Id { get; set; }

    public int AssignmentId { get; set; }
    public string AssignmentCode { get; set; } = string.Empty;
    public string AssignmentTitle { get; set; } = string.Empty;

    public int TeacherId { get; set; }

    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public SubmissionStatus Status { get; set; }

    public decimal? Marks { get; set; }
    public string? Feedback { get; set; }

    public DateTime SubmittedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? GradedAt { get; set; }
}