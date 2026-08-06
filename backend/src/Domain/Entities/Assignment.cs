namespace Domain.Entities;

public class Assignment
{
    public int Id {get; set;}
    public string Code {get; set;} = string.Empty;
    public string Title {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public DateTime Deadline {get; set;}
    public decimal MaximumMarks {get; set;}
    public bool IsPublished {get; set;} = false;
    public bool AllowSubmissionUpdate {get; set;} = true;

    public int TeacherAssignmentId {get; set;}
    public TeacherAssignment TeacherAssignment {get; set;} = null!;

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime? UpdatedAt {get; set;}

    public ICollection<Submission> Submissions {get; set;} = new List<Submission>();
}