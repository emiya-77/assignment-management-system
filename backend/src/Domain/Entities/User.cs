using Domain.Enums;

namespace Domain.Entities;

public class User
{
    public int Id {get; set;}
    public string FirstName {get; set;} = string.Empty;
    public string LastName {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string PasswordHash {get; set;} = string.Empty;
    public UserRole Role {get; set;}
    public bool IsActive {get; set;} = true;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    
    public ICollection<StudentCourse> StudentCourses {get; set;} = new List<StudentCourse>();
    public ICollection<TeacherAssignment> TeacherAssignments {get; set;} = new List<TeacherAssignment>();
    public ICollection<Submission> Submissions {get; set;} = new List<Submissions>();
}