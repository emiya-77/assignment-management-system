namespace Domain.Entities;

public class Course
{
    public int Id {get; set;}
    public string Code {get; set;} = string.Empty;
    public string Name {get; set;} = string.Empty;
    public string? Description {get; set;} = string.Empty;
    public bool IsActive {get; set;} = true;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public ICollection<StudentCourse> StudentCourses {get; set;} = new List<StudentCouse>();
    public ICollection<TeacherAssignment> TeacherAssignments {get; set;} = new List<TeacherAssignment>();
}