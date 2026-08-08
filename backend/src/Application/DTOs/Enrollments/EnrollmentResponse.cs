namespace Application.DTOs.Enrollments;

public class EnrollmentResponse
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
}