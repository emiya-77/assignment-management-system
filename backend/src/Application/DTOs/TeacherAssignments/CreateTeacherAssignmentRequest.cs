using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.TeacherAssignments;

public class CreateTeacherAssignmentRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int TeacherId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int SubjectId { get; set; }
}