using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ITeacherAssignmentRepository
{
    Task<List<TeacherAssignment>> GetAllAsync();

    Task<TeacherAssignment?> GetByIdAsync(int id);

    Task<TeacherAssignment> AddAsync(
        TeacherAssignment teacherAssignment
    );

    Task<bool> ExistsAsync(
        int teacherId,
        int courseId,
        int subjectId
    );

    Task<bool> DeleteAsync(int id);
}