namespace Domain.Entities;

public class TeacherAssignment
{
    public int Id {get; set;}
    
    public int TeacherId {get; set;}
    public User Teacher {get; set;} = null!;

    public int CourseId {get; set;}
    public Course Course {get; set;} = null!;

    public int SubjectId {get; set;}
    public Subject Subject {get; set;} = null!;

    public DateTime AssignedAt {get; set;} = DateTime.UtcNow;

    public ICollection<Assignment> Assignments {get; set;} = new List<Assignment>();
}