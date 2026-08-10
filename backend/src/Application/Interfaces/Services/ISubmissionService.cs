using Application.DTOs.Submissions;

namespace Application.Interfaces.Services;

public interface ISubmissionService
{
    Task<List<SubmissionResponse>> GetAllAsync();

    Task<List<SubmissionResponse>> GetByStudentIdAsync(
        int studentId
    );

    Task<SubmissionResponse?> GetByIdAsync(int id);

    Task<List<SubmissionResponse>> GetByAssignmentIdAsync(
        int assignmentId,
        int teacherId
    );

    Task<SubmissionResponse> CreateAsync(
        int assignmentId,
        int studentId,
        CreateSubmissionRequest request
    );

    Task<SubmissionResponse?> UpdateAsync(
        int id,
        int studentId,
        UpdateSubmissionRequest request
    );

    Task<SubmissionResponse?> GradeAsync(
        int id,
        int teacherId,
        GradeSubmissionRequest request
    );

    Task<SubmissionResponse?> UpdateStatusAsync(
        int id,
        int teacherId,
        UpdateSubmissionStatusRequest request
    );
}