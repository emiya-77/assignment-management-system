using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ISubmissionRepository
{
    Task<List<Submission>> GetAllAsync();

    Task<Submission?> GetByIdAsync(int id);

    Task<Submission?> GetByIdWithDetailsAsync(int id);

    Task<List<Submission>> GetByAssignmentIdAsync(
        int assignmentId
    );

    Task<List<Submission>> GetByStudentIdAsync(
        int studentId
    );

    Task<bool> ExistsAsync(
        int assignmentId,
        int studentId
    );

    Task<Submission> AddAsync(
        Submission submission
    );

    Task<Submission?> UpdateAsync(
        Submission submission
    );
}