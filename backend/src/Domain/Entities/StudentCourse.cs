namespace Domain.Entities;

public class StudentCourse
{
    public int StudentId {get; set;}
    public User Student {get; set;} = null!;

    public int CourseId {get; set;}
    public Course Course {get; set;} = null!;

    public DateTime EnrolledAt {get; set;} = DateTime.UtcNow;
}