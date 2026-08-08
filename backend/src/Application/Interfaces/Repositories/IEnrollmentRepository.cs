using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IEnrollmentRepository
{
    Task<List<StudentCourse>> GetAllAsync();
    Task<StudentCourse?> GetByIdsAsync(
        int studentId,
        int courseId
    );
    Task<StudentCourse> AddAsync(
        StudentCourse enrollment
    );
    Task<bool> ExistsAsync(
        int studentId,
        int courseId
    );
    Task<bool> DeleteAsync(
        int studentId,
        int courseId
    );
}