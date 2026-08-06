using Domain.Enums;

namespace Domain.Entities;

public class Submission
{
    public int Id {get; set;}
    
    public int AssignmentId {get; set;}
    public Assignment Assignment {get; set;} = null!;

    public int StudentId {get; set;}
    public User Student {get; set;} = null!;

    public string Answer {get; set;} = string.Empty;

    public SubmissionStatus Status {get; set;} = SubmissionStatus.Submitted;

    public decimal? Marks {get; set;}
    public string? Feedback {get; set;}

    public DateTime SubmittedAt {get; set;} = DateTime.UtcNow;
    public DateTime? UpdatedAt {get; set;}
    public DateTime? GradedAt {get; set;}
}